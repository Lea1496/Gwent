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

    private List<CardBehavior> rangee = new List<CardBehavior>();

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

            CardBehavior cardB = BoardManager.deck[indice];
            BoardManager.cardsAlreadyPicked.Add(indice);
            BoardManager.cardsInHand.Add(BoardManager.deck[indice]);
            BoardManager.cardsAlreadyPicked.Remove(BoardManager.deck.IndexOf(this));
            card.GetComponent<CardBehavior>().Constructeur(cardB.name, cardB.ability, cardB.power, cardB.rank, cardB.isHero, cardB.indice, cardB.no, cardB.faction);
            gameManager.InstanciateCards(index, BoardManager.cardsInHand.Count + 1, card);
            gameManager.AddCardInHands();
            gameManager.physicalCards.Add(card);
            
            gameManager.physicalCards.Remove(gameObject);
            gameManager.nbCardChanged++;
        }
    }

    
    public void UseFrost()
    {
        
    }

    public void UseCommandersHorn()
    {
        if (gameManager.isComUsed[rank - 14])
        {
            foreach (var card in BoardManager.wholeBoard[rank-14])
            {
                if (!card.isHero)
                {
                    card.power *= 2;
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
        foreach (var card in BoardManager.wholeBoard[rank - 7])
        {
            if (card.name == name)
            {
                isTightBondUsed = true;
                tightBondCards.Add(card);
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
        float offset = 506.0f / (BoardManager.wholeBoard[rank - 7].Count + 1);
        Transform dropZone = dropZones[rank - 7].transform;
        CardBehavior cardB;
        foreach (var card in BoardManager.deck)
        {
            if (card.name == name)
            {
                BoardManager.board.Add(Instantiate(this.card,
                    new Vector2(dropZone.position.x - 506 / 2.0f + offset * (indice + ++nbCardSpawned),
                        dropZone.position.y), dropZone.rotation));
                cardB = BoardManager.board[BoardManager.board.Count - 1].GetComponent<CardBehavior>();
                cardB.Constructeur(card.name, card.ability, card.power, card.rank, card.isHero,  indice + nbCardSpawned, card.no, card.faction);
                BoardManager.wholeBoard[cardB.rank - 7].Add(cardB);
            }
        }

        foreach (var card in BoardManager.cardsInHand)
        {
            if (card.name == name)
            {
                BoardManager.board.Add(Instantiate(this.card,
                    new Vector2(dropZone.position.x - 506 / 2.0f + offset * (indice + ++nbCardSpawned),
                        dropZone.position.y), dropZone.rotation));
                cardB = BoardManager.board[BoardManager.board.Count - 1].GetComponent<CardBehavior>();
                cardB.Constructeur(card.name, card.ability, card.power, card.rank, card.isHero,  indice + nbCardSpawned, card.no, card.faction);
                BoardManager.wholeBoard[cardB.rank - 7].Add(cardB);
                BoardManager.cardsInHand.Remove(cardB);
            }
        }
        foreach (var card in BoardManager.wholeBoard[rank - 7])
        {
            if (card.indice > indice)
            {
                card.indice += nbCardSpawned;
            }
        }
    }
    private void UseMoralBoost()
    {
        rangee = BoardManager.wholeBoard[rank]; //Jsp si ça marche de même
        if (rangee.Count != 0 && name != "dandelion_card")
        {
            foreach (var card in rangee)
            {
                if (!card.isHero)
                {
                    card.power++;
                }
            }
        }
        else if (name == "dandelion_name")
        {
            foreach (var card in BoardManager.wholeBoard[rank - 7])
            {
                if (!card.isHero)
                {
                    card.power *= 2;
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
