using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class CardBehavior : MonoBehaviour
{
    
    public enum abilities {agile, berserker, mardroeme, medic, moralBoost, muster, spy, tightBond, none}
    public string name;
    public int ability;
    public int power;
    public int rank;
    public string image;
    public bool isHero;
    public int indice;
 
    private int offset;

    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private List<GameObject> cardsInDefausse = new List<GameObject>();

    private GameObject dropZoneSword;
    private GameObject dropZoneArc;
    private GameObject dropZoneCatapult;
    private GameObject dropZoneSwordEnemy;
    private GameObject dropZoneArcEnemy;
    private GameObject dropZoneCatapultEnemy;

    private GameObject defausse;
    private bool defausseOpen = false;
    private GameObject cardToKeep;

    private List<Action> abilityFunctions = new List<Action>();

    private List<CardObj> rangee = new List<CardObj>();

    [SerializeField] private GameObject card;
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        DefineDropZones();
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

        

    }

    public CardBehavior(string _name, int _ability, int _power, int _rank, string _image, bool _isHero, int _indice)
    {
        name = _name;
        ability = _ability;
        power = _power;
        rank = _rank;
        image = _image;
        isHero = _isHero;
        indice = _indice;
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
        CardObj cardObj = gameObject.GetComponent<CardBehavior>().TransformIntoCardObj();
        if (GameManager.defausseOpen && BoardManager.defausse.Contains(cardObj) && !gameObject.GetComponent<DragDrop>().isDraggable)
        {
            Debug.Log(card.GetComponent<DragDrop>().isDraggable);
            GameManager.CacherDefausse(card);
            BoardManager.defausse.Remove(cardObj); //Facon plus efficace de dire ca??
        }
    }

    public void ChooseThisCardToChange()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int index = gameManager.physicalCards.IndexOf(gameObject);
        if (gameManager.isTimeToChangeCards)
        {
            CardObj carbObj = this.TransformIntoCardObj();
            BoardManager.cardsInHand.Remove(carbObj);
            BoardManager.cardsAlreadyPicked.Remove(BoardManager.deck.IndexOf(carbObj));
            gameManager.InstanciateCards(index, BoardManager.cardsInHand.Count + 1);
            gameManager.nbCardChanged++;
        }
    }
    private void UseMedic()
    {
        if (BoardManager.defausse.Count != 0)
        {
            
            gameManager.MontrerDefausse(card);
        }
    }

    private void UseMoralBoost()
    {
        rangee = BoardManager.wholeBoard[rank]; //Jsp si ça marche de même
        if (rangee.Count != 0)
        {
            foreach (var card in rangee)
            {
                if (!card.isHero)
                {
                    card.power++;
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

    public void Constructeur(string _name, int _ability, int _power, int _rank, string _image, bool _isHero, int _indice)
    {
        name = _name;
        ability = _ability;
        power = _power;
        rank = _rank;
        image = _image;
        isHero = _isHero;
        indice = _indice;

    }

    public CardObj TransformIntoCardObj()
    {
        return new CardObj(name, ability, power, rank, image, isHero, indice);
    }
}
