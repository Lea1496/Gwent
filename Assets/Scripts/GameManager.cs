using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GwentEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;


public class GameManager : MonoBehaviour
{
    //[SerializeField] private GameObject cardZone;
    public GameState _gameState = new GameState();
    private Dictionary<int, CardMetadata> _metadata;
    
    private static float offset;

    public bool isComUsedS = false;
    public bool isComUsedA = false;
    public bool isComUsedC = false;
    public List<bool> isComUsed;

    public bool isDecoyBeingUsed = false;
    public GameObject decoyCard;
    
    public bool isFrostUsed = false;
    public bool isFogUsed = false;
    public bool isRainUsed = false;
    public List<bool> isWeatherUsed;
    [SerializeField] private TextMeshProUGUI youStart;
    [SerializeField] private TextMeshProUGUI opponentStarts;
    
    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private static List<GameObject> cardsInDefausse = new List<GameObject>();

    [SerializeField] private GameObject dropZoneSword;
    [SerializeField] private GameObject dropZoneArc;
    [SerializeField] private GameObject dropZoneCatapult;
    [SerializeField] private GameObject dropZoneSwordEnemy;
    [SerializeField]private GameObject dropZoneArcEnemy;
    [SerializeField] private GameObject dropZoneCatapultEnemy;
    [SerializeField] public GameObject zoneCard;
    

    [SerializeField] private GameObject defausse;
    public static bool defausseOpen = false;
    private GameObject cardToKeep;
    private GameObject tempCard;

    public List<GameObject> physicalCards = new List<GameObject>();

    public bool isTimeToChangeCards = false;

    [SerializeField]  public GameObject card;
    [SerializeField] private GameObject keepCardsButton;
    [SerializeField] private GameObject canvas;
    private int setActiveCounter = 0;

    private int startingPlayer;
    public int nbCardChanged = 0;
    public int totPts = 0;
    public int totPtsEnemy = 0;
    private int index = 0;
    private void Start()
    {
        _gameState.NewGame(_metadata);
        
        ChooseWhoStarts();
        isComUsed = new List<bool>(3) { isComUsedS, isComUsedA, isComUsedC };
        isWeatherUsed = new List<bool>(3) { isFrostUsed, isFogUsed, isRainUsed };
    }

    private void Update()
    {
        ActivateAndDeactivateButton();
    }

    public void ActivateAndDeactivateButton()
    {
        if (_gameState.IsTimeToChangeCards)
        {
            if (setActiveCounter < 1)
            {
                keepCardsButton.SetActive(true);
                setActiveCounter++;
            }

        }
        else if(keepCardsButton.GameObject().activeInHierarchy)
        {
            keepCardsButton.SetActive(false);
        }
    }
    public  void EndRound(GameObject carte)
    {
        CompterNbPts();
        CardBehavior cardB;
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in BoardManager.wholeBoard[i])
            {
                cardB = card.GetComponent<CardBehavior>();
                cardB.power = cardB.ogPower;
                if (cardB.isTightBondUsed)
                {
                    cardB.isTightBondUsed = false;
                }
                BoardManager.defausse.Add(cardB.TransformIntoCardObj());
                
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


    public void CreateCardDisplay(GameObject card)
    {
        Debug.Log(card.GetComponent<CardBehavior>().name);
        Debug.Log(GameObject.Find(card.GetComponent<CardBehavior>().name).name);
        card.GetComponent<Image>().sprite = GameObject.Find(card.GetComponent<CardBehavior>().name).GetComponent<SpriteRenderer>().sprite;
        CardBehavior cardB = card.GetComponent<CardBehavior>();
        if (!cardB.isHero && cardB.rank < 13)
        {
            cardB.ChangePower();
        }
        else
        {
            cardB.cover.gameObject.SetActive(false);
        }
        
    }
    

    public void CompterNbPts()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in BoardManager.wholeBoard[i])
            {
                totPts += card.GetComponent<CardBehavior>().power;
            }
        }
    }
    private void ChooseWhoStarts()
    {
        Random gen = new Random(2);
        int index = gen.NextInt(1, 3);
        TextMeshProUGUI text;
        if (index == 2)
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
        yield return (2f);
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
        CardBehavior cardB;
        foreach (var card in BoardManager.defausse)
        {
            //cardB = card.GetComponent<CardBehavior>();
            if (!card.isHero && card.ability < 13)
            {
                Debug.Log("testttt");
                carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.isHero, -1, card.no,card.faction);
               // cardsInDefausse.Add(GameObject.Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation));
                Instantiate(carte, new Vector2(offset , defausse.transform.position.y), defausse.transform.rotation).transform.SetParent(canvas.transform, true);
                
                //tempCard.transform.SetParent(canvas.transform, true);
                //cardsInDefausse.Add(tempCard);
                offset += offsetStart;
            }
            
        }

        offset = 0;
    }

    public void CacherDefausse(CardObj cardToKeep)
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
        //CreateDeck();
        
        //Cards.CreateDeck();
        //DefineDropZones();
        //Refaire();
        StartGame();
        foreach (var card in physicalCards)
        {
            CreateCardDisplay(card);
        }

        //StartGame();
    }

    private void Test()
    {
        //Cards.CreateDeck();
         //List<(string, int, int, int, bool,int, int, int)> test = BoardManager.deck;
      //DefineDropZones();
        StartGame();
    }
    private void StartGame()
    {
        int index = 0;
        while (BoardManager.cardsInHand.Count != 10)
        {
            
            CreateCards(ChooseCardInHand(), 10, index++);
        }
        
       /* int i = 0;
        foreach (var card in physicalCards)
        {
            InstanciateCards(i++, 10);
        }*/
        
        isTimeToChangeCards = true;

    }

    public void CreateCards((string, int, int, int, bool,int, int, int) obj, int nbCards, int index)
    {
        GameObject carte;
        
        //foreach (var card in BoardManager.cardsInHand)
        {
            carte = this.card;
            //carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank,  card.isHero, index, card.no, card.faction);
            carte.GetComponent<CardBehavior>().Constructeur(obj.Item1, obj.Item2,  obj.Item3, obj.Item4, obj.Item5, obj.Item6,  obj.Item7, obj.Item8);
            InstanciateCards(index, nbCards, carte);
            
        }
    }
    public void InstanciateCards(int index, int nbCards, GameObject aCard)
    {
        float offset = 844.0f / nbCards;
        GameObject card = Instantiate(aCard, new Vector2(offset * index + zoneCard.transform.position.x - (844f / 2), zoneCard.transform.position.y),
            zoneCard.transform.rotation);
        card.transform.SetParent(canvas.transform, true);
        card.GetComponent<CardBehavior>().indice = index;
        BoardManager.cardsInHand.Add(card.GetComponent<CardBehavior>()); //
        physicalCards.Add(card);
    }

    public (string, int, int, int, bool,int, int, int) ChooseCardInHand()
    {
        int index = ChooseRandomCard();
        //BoardManager.cardsInHand.Add(BoardManager.deck[index].GetComponent<CardBehavior>());
        
        return BoardManager.deck[index];
    }

    public  int ChooseRandomCard()
    {
        int index = 0;
        Random gen = new Random((uint)DateTime.Now.Millisecond);
        index = gen.NextInt(0, BoardManager.deck.Count);
        while (BoardManager.cardsAlreadyPicked.Contains(index))
        {
            index = gen.NextInt(0, BoardManager.deck.Count);
        }
        BoardManager.cardsAlreadyPicked.Add(index);
        return index;
    }

    public void AddCard(GameObject dropZone, List<GameObject> row, float offset)
    {
       // offset = 844f / (row.Count + 1);
        foreach (var card in row)
        {
            card.transform.position = new Vector2(dropZone.transform.position.x - 844.0f/2 + offset * card.GetComponent<CardBehavior>().indice, dropZone.transform.position.y);
        }

        index++;
    }

    public void RemoveCard(int index, GameObject dropZone, List<GameObject> row)
    {
        CardBehavior cardB;
        offset = 844f / (row.Count);
        int i = 0;
        foreach (var card in row)
        {
            Debug.Log(i++);
            //Debug.Log(card.name);
            cardB = card.GetComponent<CardBehavior>();
            if (cardB.indice > index)
            {
                cardB.indice--;
            }

            card.transform.position = new Vector2(dropZone.transform.position.x - 844.0f / 2 + offset * cardB.indice,
                dropZone.transform.position.y);
        }

        this.index--;
    }
    private void WorkThreadFunction()
    {
        try
        {
            Refaire();
        }
        catch 
        {
            Refaire();
        }
    }
    private void Refaire()
    {
        Thread thread = new Thread(new ThreadStart(WorkThreadFunction)) ;
       
        thread.Start();
        
        Test();
        if (!thread.Join(new TimeSpan(0, 0, 1)) )
        {
            thread.Abort();
            Debug.Log("Ça a pas marché");
            Refaire();
            
        }
    }
   /* private  void CreateDeck()
    {
        GameObject card = this.card;
        List<(string, int, int, int, bool,int, int, int)> cards = Cards.cards;
        for (int i = 0; i < cards.Count; i++)
        {
            Debug.Log(cards[i].Item1);
            BoardManager.deck.Add(card);
            BoardManager.deck[BoardManager.deck.Count - 1].GetComponent<CardBehavior>().Constructeur(cards[i].Item1, cards[i].Item2, cards[i].Item3, cards[i].Item4, cards[i].Item5, cards[i].Item6, cards[i].Item7,  cards[i].Item8);
            if (i != 0)
            {
                Debug.Log(BoardManager.deck[BoardManager.deck.Count - 2].GetComponent<CardBehavior>()
                    .name);
            } //BoardManager.deck.Add(card);
        }
        
        
        
    }*/

    
}
