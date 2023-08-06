using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardZone;
    
    private static float offset;

    public bool isComUsedS = false;
    public bool isComUsedA = false;
    public bool isComUsedC = false;
    public List<bool> isComUsed;
    [SerializeField] private TextMeshProUGUI youStart;
    [SerializeField] private TextMeshProUGUI opponentStarts;
    
    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private static List<GameObject> cardsInDefausse = new List<GameObject>();

    private GameObject dropZoneSword;
    private GameObject dropZoneArc;
    private GameObject dropZoneCatapult;
    private GameObject dropZoneSwordEnemy;
    private GameObject dropZoneArcEnemy;
    private GameObject dropZoneCatapultEnemy;
    private GameObject zoneCard;
   

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

    private int startingPlayer;
    public int nbCardChanged = 0;
    public int totPts = 0;
    public int totPtsEnemy = 0;
    private void Start()
    {
        ChooseWhoStarts();
        isComUsed = new List<bool>(3) { isComUsedS, isComUsedA, isComUsedC };
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

    public  void EndRound(GameObject carte)
    {
        CompterNbPts();
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in BoardManager.wholeBoard[i])
            {
                card.power = card.ogPower;
                if (card.isTightBondUsed)
                {
                    card.isTightBondUsed = false;
                }
                BoardManager.defausse.Add(card);
                
            }
        }
        foreach (GameObject card in BoardManager.board)
        {
            //card.GetComponent<DragDrop>().isDraggable = true;
            Destroy(card);
        }

        for (int i = 0; i < isComUsed.Count; i++)
        {
            isComUsed[i] = false;
        }
        
        /*foreach (var card in GameObject.FindGameObjectsWithTag("Card"))
        {
            GameObject.Destroy(card);
        }*/
    }
    

    

    public void CompterNbPts()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in BoardManager.wholeBoard[i])
            {
                totPts += card.power;
            }
        }
    }
    private void ChooseWhoStarts()
    {
        Random gen = new Random();
        int index = gen.NextInt(1, 3);
        TextMeshProUGUI text;
        if (index % 2 == 0)
        {
            text = youStart;
            startingPlayer = 0;
        }
        else
        {
            startingPlayer = 1;
            text = opponentStarts;
        }
        text.gameObject.SetActive(true);
        StartCoroutine(HideText(text));

    }

    private IEnumerator HideText(TextMeshProUGUI text)
    {
        yield return (1f);
        text.gameObject.SetActive(false);
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
        zoneCard = GameObject.Find("CardZone");
    }
    public void MontrerDefausse(GameObject carte)
    {
        defausseOpen = true;
        offset = 276.0f / (BoardManager.defausse.Count + 1);
        float offsetStart = offset;
        Debug.Log(BoardManager.defausse.Count);
        foreach (CardBehavior card in BoardManager.defausse)
        {
            if (!card.isHero && card.ability < 9)
            {
                Debug.Log("testttt");
                carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.isHero, -1, card.no,card.faction);
               // cardsInDefausse.Add(GameObject.Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation));
                Instantiate(carte, new Vector2(offset + 10, defausse.transform.position.y), defausse.transform.rotation).transform.SetParent(canvas.transform, true);
                
                //tempCard.transform.SetParent(canvas.transform, true);
                //cardsInDefausse.Add(tempCard);
                offset += offsetStart;
            }
            
        }

        offset = 0;
    }

    public static void CacherDefausse(CardBehavior cardToKeep)
    {
        foreach (GameObject card in cardsInDefausse)
        {
            if (card.GetComponent<CardBehavior>().no != cardToKeep.no)
            {
                Destroy(card);
            }
        }
        cardsInDefausse.Clear();
    }

    private void Awake()
    {
        /*BoardManager.deck.Add(c1);
        BoardManager.deck.Add(c2);
        BoardManager.deck.Add(c3);
        BoardManager.deck.Add(c4);
        BoardManager.deck.Add(c5);
        BoardManager.deck.Add(c6);
        BoardManager.deck.Add(c7);
        BoardManager.deck.Add(c8);
        BoardManager.deck.Add(c9);
        BoardManager.deck.Add(c0);
        
        BoardManager.defausse.Add(c11);
        BoardManager.defausse.Add(c22);
        BoardManager.defausse.Add(c33);
        BoardManager.defausse.Add(c44);
        BoardManager.defausse.Add(c55);*/
        DefineDropZones();
        StartGame();
    }

    private void StartGame()
    {
        while (BoardManager.cardsInHand.Count != 10)
        {
            ChooseCardInHand();
        }
        CreateCards();
       /* int i = 0;
        foreach (var card in physicalCards)
        {
            InstanciateCards(i++, 10);
        }*/
        
        isTimeToChangeCards = true;

    }

    private void CreateCards()
    {
        GameObject carte;
        int index = 0;
        foreach (var card in BoardManager.cardsInHand)
        {
            carte = this.card;
            carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank,  card.isHero, index, card.no, card.faction);
            InstanciateCards(index++, 10, carte);
            
        }
    }
    public void InstanciateCards(int index, int nbCards, GameObject aCard)
    {
        float offset = 844.0f / nbCards;
        GameObject card = Instantiate(aCard, new Vector2(offset * index + cardZone.transform.position.x - (844f / 2), cardZone.transform.position.y),
            cardZone.transform.rotation);
        card.transform.SetParent(canvas.transform, true);
        
        physicalCards.Add(card);
    }

    public void ChooseCardInHand()
    {
        int index = ChooseRandomCard();
        BoardManager.cardsInHand.Add(BoardManager.deck[index]);
        BoardManager.cardsAlreadyPicked.Add(index);
        
    }

    public  int ChooseRandomCard()
    {
        int index = 0;
        Random gen = new Random();
        index = gen.NextInt(0, BoardManager.deck.Count);
        while (BoardManager.cardsAlreadyPicked.Contains(index))
        {
            index = gen.NextInt(0, BoardManager.deck.Count);
        }

        return index;
    }

    public void AddCardInHands()
    {
        offset = 844f / (physicalCards.Count + 1);
        foreach (var card in physicalCards)
        {
            card.transform.position = new Vector2(zoneCard.transform.position.x - 844.0f/2 + offset * card.GetComponent<CardBehavior>().indice, zoneCard.transform.position.y);
        }
    }

    public void RemoveCardInHands(int index)
    {
        CardBehavior cardB;
        offset = 844f / (physicalCards.Count);
        foreach (var card in physicalCards)
        {
            cardB = card.GetComponent<CardBehavior>();
            if (cardB.indice > index)
            {
                cardB.indice--;
            }

            card.transform.position = new Vector2(zoneCard.transform.position.x - 844.0f / 2 + offset * cardB.indice,
                zoneCard.transform.position.y);
        }
    }

    
}
