using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardZone;
    
    private static float offset;

    
    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private static List<GameObject> cardsInDefausse = new List<GameObject>();

    private GameObject dropZoneSword;
    private GameObject dropZoneArc;
    private GameObject dropZoneCatapult;
    private GameObject dropZoneSwordEnemy;
    private GameObject dropZoneArcEnemy;
    private GameObject dropZoneCatapultEnemy;
    
   

    private static GameObject defausse;
    public static bool defausseOpen = false;
    private GameObject cardToKeep;
    private GameObject tempCard;

    public List<GameObject> physicalCards = new List<GameObject>();

    public bool isTimeToChangeCards = false;

    [SerializeField]  private GameObject card;
    [SerializeField] private GameObject keepCardsButton;
    private GameObject canvas;
    private int setActiveCounter = 0;

    public int nbCardChanged = 0;
    private void Start()
    {
        DefineDropZones();
    }

    private void Update()
    {
        if (isTimeToChangeCards)
        {
            if (setActiveCounter < 1)
            {
                keepCardsButton.SetActive(true);
                setActiveCounter++;
            }
            if (nbCardChanged == 2)
            {
                isTimeToChangeCards = false;
                keepCardsButton.SetActive(false);
            }
            
        }
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
        canvas = GameObject.Find("Canvas");

    }
    public void MontrerDefausse(GameObject carte)
    {
        defausseOpen = true;
        offset = 276.0f / (BoardManager.defausse.Count + 1);
        float offsetStart = offset;
        Debug.Log(BoardManager.defausse.Count);
        foreach (CardObj card in BoardManager.defausse)
        {
            if (!card.isHero)
            {
                Debug.Log("testttt");
                carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.image, card.isHero, -1);
               // cardsInDefausse.Add(GameObject.Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation));
                Instantiate(carte, new Vector2(offset + 10, defausse.transform.position.y), defausse.transform.rotation).transform.SetParent(canvas.transform, true);
                
                //tempCard.transform.SetParent(canvas.transform, true);
                //cardsInDefausse.Add(tempCard);
                offset += offsetStart;
            }
            
        }

        offset = 0;
    }

    public static void CacherDefausse(GameObject cardToKeep)
    {
        foreach (GameObject card in cardsInDefausse)
        {
            if (card != cardToKeep)
            {
                Destroy(card);
            }
        }
        cardsInDefausse.Clear();
    }

    private void Awake()
    {
        StartGame();
    }

    private void StartGame()
    {
        while (BoardManager.cardsInHand.Count != 10)
        {
            BoardManager.ChooseCardInHand();
        }
        CreateCards();
        int i = 0;
        foreach (var card in physicalCards)
        {
            InstanciateCards(i++, 10);
        }
        
        isTimeToChangeCards = true;

    }

    private void CreateCards()
    {
        GameObject carte;
        int index = 0;
        foreach (var card in BoardManager.cardsInHand)
        {
            carte = this.card;
            carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.image, card.isHero, index++);
            physicalCards.Add(carte);
            
        }
    }
    public void InstanciateCards(int index, int nbCards)
    {
        float offset = 506.0f / nbCards;
        Instantiate(card, new Vector2(offset * index + cardZone.transform.position.x, cardZone.transform.position.y),
            cardZone.transform.rotation).transform.SetParent(canvas.transform, true);
    }

    

    
}
