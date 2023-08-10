using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = Unity.Mathematics.Random;

public class CardBehavior : MonoBehaviour
{

    public enum abilities
    {
        agile,
        berserker,
        mardroeme,
        medic,
        moraleBoost,
        muster,
        spy,
        tightBond,
        none,
        frost,
        fog,
        rain,
        decoy,
        scorch,
        commandersHorn,
        clearWeather
    }

    public string name;
    public int ability;
    public int power;
    public int rank;
  
    public bool isHero;
    public int indice;
    public int no;
    public int faction;
    
    
    private int offset;

    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private List<GameObject> cardsInDefausse = new List<GameObject>();

    private GameObject dropZoneSword;
    private GameObject dropZoneArc;
    private GameObject dropZoneCatapult;
    private GameObject dropZoneSwordEnemy;
    private GameObject dropZoneArcEnemy;
    private GameObject dropZoneCatapultEnemy;
    private GameObject cardZone;
    
    public bool isTightBondUsed = false;
    public int ogPower = 0;

    private GameObject defausse;
    private bool defausseOpen = false;
    private GameObject cardToKeep;

    private List<Action> abilityFunctions = new List<Action>();

    private List<GameObject> rangee = new List<GameObject>();

    [SerializeField] private GameObject card;
    private GameManager gameManager;
    private List<GameObject> dropZones;
    

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        DefineDropZones();

        dropZones = new List<GameObject>()
        {
            dropZoneSword, dropZoneArc, dropZoneCatapult, dropZoneSwordEnemy, dropZoneArcEnemy, dropZoneCatapultEnemy
        };
//        abilityFunctions.Add(UseMedic);
        // abilityFunctions.Add(UseMoralBoost);

        /*BoardManager.defausse.Add(card.GetComponent<CardBehavior>());
        BoardManager.defausse.Add(card.GetComponent<CardBehavior>());
        BoardManager.defausse.Add(card.GetComponent<CardBehavior>());
        BoardManager.defausse.Add(card.GetComponent<CardBehavior>());
        BoardManager.defausse.Add(card.GetComponent<CardBehavior>());
        BoardManager.defausse.Add(card.GetComponent<CardBehavior>());*/

    }

    private void DefineDropZones()
    {
        dropZoneSword = GameObject.Find("DropZoneS");
        dropZoneArc = GameObject.Find("DropZoneA");
        dropZoneCatapult = GameObject.Find("DropZoneC");
        dropZoneSwordEnemy = GameObject.Find("DropZoneEnemyS");
        dropZoneArcEnemy = GameObject.Find("DropZoneEnemyA");
        dropZoneCatapultEnemy = GameObject.Find("DropZoneEnemyC");
        defausse = GameObject.Find("Defausse");
        cardZone = GameObject.Find("CardZone");

    }

    public CardBehavior(string _name, int _ability, int _power, int _rank, bool _isHero, int _indice, int _no)
    {
        name = _name;
        ability = _ability;
        power = _power;
        rank = _rank;
        
        isHero = _isHero;
        indice = _indice;
        no = _no;
    }
    /*private void MontrerDefausse(GameObject carte)
    {
        defausseOpen = true;
        offset = (int)(38 / BoardManager.defausse.Count + 1);
        foreach (CardBehavior card in BoardManager.defausse)
        {
            if (!card.isHero)
            {
                carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.image, card.isHero);
                cardsInDefausse.Add(Instantiate(carte, new Vector2(offset, defausse.transform.position.y), transform.rotation));
                offset += offset;
            }
            
        }

        offset = 0;
    }

    private void CacherDefausse(GameObject cardToKeep)
    {
        foreach (GameObject card in cardsInDefausse)
        {
            if (card != cardToKeep)
            {
                Destroy(card);
            }
        }
    }

    public void AssociateCard(GameObject card)
    {
        if (defausseOpen)
        {
            cardToKeep = card;
        }
    }*/

    
    //AJOUTER indice à CardObj?????
    public void AssociateCard()
    {
        indice = -2;
        CardBehavior cardBeh = gameObject.GetComponent<CardBehavior>();
        if (GameManager.defausseOpen && BoardManager.defausse.Contains(cardBeh) && !gameObject.GetComponent<DragDrop>().isDraggable)
        {
            Debug.Log(card.GetComponent<DragDrop>().isDraggable);
            GameManager.CacherDefausse(cardBeh);
            BoardManager.defausse.Remove(cardBeh); //Facon plus efficace de dire ca??
        }
    }

    public void ChooseThisCardToChange()
    {
        //GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int index = gameManager.physicalCards.IndexOf(gameObject);
        if (gameManager.isTimeToChangeCards)
        {
            
            BoardManager.cardsInHand.Remove(this);
            int indice = gameManager.ChooseRandomCard();

            //CardBehavior cardB = BoardManager.deck[indice].GetComponent<CardBehavior>();
            (string, int, int, int, bool, int, int, int) obj = BoardManager.deck[indice];
            BoardManager.cardsAlreadyPicked.Add(indice);
            //BoardManager.cardsInHand.Add(BoardManager.deck[indice].GetComponent<CardBehavior>()); //
            BoardManager.cardsAlreadyPicked.Remove(BoardManager.deck.IndexOf( (name, ability, power, rank, isHero, indice, no, faction)));
            //card.GetComponent<CardBehavior>().Constructeur(cardB.name, cardB.ability, cardB.power, cardB.rank, cardB.isHero, cardB.indice, cardB.no, cardB.faction);
            card.GetComponent<CardBehavior>().Constructeur(obj.Item1, obj.Item2, obj.Item3, obj.Item4, obj.Item5, obj.Item6, obj.Item7, obj.Item8);
            gameManager.InstanciateCards(index, BoardManager.cardsInHand.Count + 1, card);
            
            gameManager.physicalCards.Add(card);
            
            gameManager.physicalCards.Remove(gameObject);
            gameManager.nbCardChanged++;
        }
    }

    private void Transfo((string, int, int, int, bool, int, int, int) obj)
    {
        name = obj.Item1;
        ability = obj.Item2;
        power = obj.Item3;
        rank = obj.Item4;
        isHero = obj.Item5;
        indice = obj.Item6;
        no = obj.Item7;
        
    }
    public void UseFrost()
    {
        
    }

    public void UseCommandersHorn()
    {
        if (gameManager.isComUsed[rank - 14])
        {
            CardBehavior cardB;
            foreach (var card in BoardManager.wholeBoard[rank-14])
            {
                cardB = card.GetComponent<CardBehavior>();
                if (!cardB.isHero)
                {
                    cardB.power *= 2;
                }
            }
        }
    }
    private void UseSpy()
    {
        
    }

    private void UseTightBond()
    {
        int nbCard = 0;
        List<CardBehavior> tightBondCards = new List<CardBehavior>();
        CardBehavior cardB;
        foreach (var card in BoardManager.wholeBoard[rank - 7])
        {
            cardB = card.GetComponent<CardBehavior>();
            if (cardB.name == name)
            {
                isTightBondUsed = true;
                tightBondCards.Add(cardB);
                nbCard++;
            }
        }

        foreach (var card in tightBondCards)
        {
            isTightBondUsed = true;
            card.power = (int)Mathf.Pow(card.power, nbCard);
        }

        if (isTightBondUsed)
        {
            ogPower = power;
            power = (int)Mathf.Pow(power, nbCard);
        }
        
    }
    private void UseMedic()
    {
        if (BoardManager.defausse.Count != 0)
        {
            gameManager.MontrerDefausse(card);
        }
    }

    
    private void UseMuster()
    {
        int nbCardSpawned = 0;
        float offset = 844.0f / (BoardManager.wholeBoard[rank - 7].Count + 1);
        Transform dropZone = dropZones[rank - 7].transform;
        CardBehavior cardB;
        CardBehavior cardBeh;
        foreach (var card in BoardManager.deck)
        {
            
            if (card.Item1 == name)
            {
                BoardManager.board.Add(Instantiate(this.card,
                    new Vector2(dropZone.position.x - 844 / 2.0f + offset * (indice + ++nbCardSpawned),
                        dropZone.position.y), dropZone.rotation));
                cardB = BoardManager.board[BoardManager.board.Count - 1].GetComponent<CardBehavior>();
                cardB.Constructeur(card.Item1, card.Item2, card.Item3, card.Item4, card.Item5,  indice + nbCardSpawned, card.Item7, card.Item8);
                BoardManager.wholeBoard[cardB.rank - 7].Add(BoardManager.board[BoardManager.board.Count - 1]);
            }
        }

        foreach (var card in BoardManager.cardsInHand)
        {
           
            cardBeh = card.GetComponent<CardBehavior>();
            if (cardBeh.name == name)
            {
                BoardManager.board.Add(Instantiate(this.card,
                    new Vector2(dropZone.position.x - 844 / 2.0f + offset * (indice + ++nbCardSpawned),
                        dropZone.position.y), dropZone.rotation));
                cardB = BoardManager.board[BoardManager.board.Count - 1].GetComponent<CardBehavior>();
                cardB.Constructeur(card.name, card.ability, card.power, card.rank, card.isHero,  indice + nbCardSpawned, card.no, card.faction);
                BoardManager.wholeBoard[cardB.rank - 7].Add(BoardManager.board[BoardManager.board.Count - 1]);
            }
        }
        foreach (var card in BoardManager.wholeBoard[rank - 7])
        {
            cardBeh = card.GetComponent<CardBehavior>();
            if (cardBeh.indice > indice)
            {
                cardBeh.indice += nbCardSpawned;
            }
        }
    }
    private void UseMoralBoost()
    {

        CardBehavior cardB;
        rangee = BoardManager.wholeBoard[rank - 7]; //Jsp si ça marche de même
        if (rangee.Count != 0 && name != "dandelion_card")
        {
            foreach (var card in rangee)
            {
                cardB = card.GetComponent<CardBehavior>();
                if (!cardB.isHero)
                {
                    cardB.power++;
                }
            }
        }
        else if (name == "dandelion_name")
        {
            
            foreach (var card in BoardManager.wholeBoard[rank - 7])
            {
                cardB = card.GetComponent<CardBehavior>();
                if (!cardB.isHero)
                {
                    cardB.power *= 2;
                }
            }
        }
    }
    public void OnDrop()
    {
        if ((int)abilities.none != ability)
        {
            if (ability == (int)abilities.medic)
            {
                UseMedic();
            }
        }   
    
    }

    public void Constructeur(string _name, int _ability, int _power, int _rank, bool _isHero, int _indice, int _no, int _faction)
    {
        name = _name;
        ability = _ability;
        power = _power;
        rank = _rank;
        
        isHero = _isHero;
        indice = _indice;
        no = _no;
        ogPower = power;
        faction = _faction;
    }

   /* public CardObj TransformIntoCardObj()
    {
        return new CardObj(name, ability, power, rank, isHero, indice);
    }*/
    
}
