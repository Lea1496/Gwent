using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if false

namespace Assets.Scripts
{
    internal class OldCode
    {

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


        private List<GameObject> cardsOnBoard = new List<GameObject>();
        private static List<GameObject> cardsInDefausse = new List<GameObject>();

        public static bool defausseOpen = false;;

        public List<GameObject> physicalCards = new List<GameObject>();

        public bool isTimeToChangeCards = false;
        private int setActiveCounter = 0;

        private int startingPlayer;
        public int nbCardChanged = 0;
        public int totPts = 0;
        public int totPtsEnemy = 0;
        private int index = 0;

        [SerializeField] private GameObject dropZoneSword;
        [SerializeField] private GameObject dropZoneArc;
        [SerializeField] private GameObject dropZoneCatapult;
        [SerializeField] private GameObject dropZoneSwordEnemy;
        [SerializeField] private GameObject dropZoneArcEnemy;
        [SerializeField] private GameObject dropZoneCatapultEnemy;
        [SerializeField] public GameObject zoneCard;
        [SerializeField] private GameObject defausse;
        private GameObject cardToKeep;
        private GameObject tempCard;

        [SerializeField] private GameObject keepCardsButton;
        [SerializeField] private GameObject canvas;
        //[SerializeField] private GameObject cardZone;

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
            else if (keepCardsButton.GameObject().activeInHierarchy)
            {
                keepCardsButton.SetActive(false);
            }
        }
        public void EndRound(GameObject carte)
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
                    carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.isHero, -1, card.no, card.faction);
                    // cardsInDefausse.Add(GameObject.Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation));
                    Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation).transform.SetParent(canvas.transform, true);

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


    }
}


    private void Test()
    {
        //Cards.CreateDeck();
         //List<(string, int, int, int, bool,int, int, int)> test = BoardManager.deck;
      //DefineDropZones();
      //  StartGame();
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
        float offset = _cardSize / nbCards;
        GameObject card = Instantiate(aCard, new Vector2(offset * index + zoneCard.transform.position.x - (_cardSize / 2), zoneCard.transform.position.y),
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
        // offset = _cardSize / (row.Count + 1);
        foreach (var card in row)
        {
            card.transform.position = new Vector2(dropZone.transform.position.x - _cardSize / 2 + offset * card.GetComponent<CardBehavior>().indice, dropZone.transform.position.y);
        }

        index++;
    }

    public void RemoveCard(int index, GameObject dropZone, List<GameObject> row)
    {
        CardBehavior cardB;
        offset = _cardSize / (row.Count);
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

            card.transform.position = new Vector2(dropZone.transform.position.x - _cardSize / 2 + offset * cardB.indice,
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

     private  void CreateDeck()
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



     }
     
public class CardBehavior : MonoBehaviour
{

    //public enum abilities
    //{
    //    agile,
    //    berserker,
    //    mardroeme,
    //    medic,
    //    moraleBoost,
    //    muster,
    //    spy,
    //    tightBond,
    //    none,
    //    frost,
    //    fog,
    //    rain,
    //    commandersHorn,
    //    clearWeather
    //}

    //public enum clickableAbilities
    //{
    //    decoy,
    //    scorch,
    //    leader
    //}

    //public string name;
    //public int ability;
    //public int power;
    //public int rank;

    //public bool isHero;
    //public int indice;
    //public int no;
    //public int faction;


    [SerializeField] public TextMeshProUGUI powerText;
    [SerializeField] public GameObject cover;

    //private int offset;

    //private List<GameObject> cardsOnBoard = new List<GameObject>();
    //private List<GameObject> cardsInDefausse = new List<GameObject>();

    //private GameObject dropZoneSword;
    //private GameObject dropZoneArc;
    //private GameObject dropZoneCatapult;
    //private GameObject dropZoneSwordEnemy;
    //private GameObject dropZoneArcEnemy;
    //private GameObject dropZoneCatapultEnemy;
    //private GameObject cardZone;
    //private GameObject canvas;

    //public bool isTightBondUsed = false;
    //public int ogPower = 0;

    //private GameObject defausse;
    //private bool defausseOpen = false;
    //private GameObject cardToKeep;

    //private List<Action> abilityFunctions;
    //private List<Action> clickableAbilityFunctions;

    //private List<GameObject> rangee = new List<GameObject>();

    //[SerializeField] private GameObject card;
    //[SerializeField] private GameManager gameManager;
    //private List<GameObject> dropZones;
    //private int oldPower = 0;


    private void Start()
    {
        //oldPower = power;
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //DefineDropZones();

        //dropZones = new List<GameObject>()
        //{
        //    dropZoneSword, dropZoneArc, dropZoneCatapult, dropZoneSwordEnemy, dropZoneArcEnemy, dropZoneCatapultEnemy
        //};
        ////       abilityFunctions.Add(UseMedic);
        //// abilityFunctions.Add(UseMoralBoost);

        //abilityFunctions = new List<Action>()
        //    { null, null, null, UseMedic, UseMoralBoost, UseMuster, UseSpy, UseTightBond, null, UseFrost, UseFog, UseRain, UseCommandersHorn, UseClearWeather};
        //clickableAbilityFunctions = new List<Action>() { UseDecoy, UseScorch };


    }

    //private void DefineDropZones()
    //{
    //    dropZoneSword = GameObject.Find("DropZoneS");
    //    dropZoneArc = GameObject.Find("DropZoneA");
    //    dropZoneCatapult = GameObject.Find("DropZoneC");
    //    dropZoneSwordEnemy = GameObject.Find("DropZoneEnemyS");
    //    dropZoneArcEnemy = GameObject.Find("DropZoneEnemyA");
    //    dropZoneCatapultEnemy = GameObject.Find("DropZoneEnemyC");
    //    defausse = GameObject.Find("Defausse");
    //    cardZone = GameObject.Find("CardZone");
    //    canvas = GameObject.Find("Canvas");
    //}

    //public CardBehavior(string _name, int _ability, int _power, int _rank, bool _isHero, int _indice, int _no)
    //{
    //    name = _name;
    //    ability = _ability;
    //    power = _power;
    //    rank = _rank;

    //    isHero = _isHero;
    //    indice = _indice;
    //    no = _no;
    //}

    private void Update()
    {
        //if (power != oldPower)
        //{
        //    ChangePower();
        //    oldPower = power;
        //}
    }

    //public void ChangePower()
    //{
    //    powerText.text = $"{power}";
    //}

    //public void AssociateCard()
    //{

    //    //CardObj cardObj = TransformIntoCardObj();
    //    //if (GameManager.defausseOpen && BoardManager.defausse.Contains(cardObj) && !gameObject.GetComponent<DragDrop>().isDraggable)
    //    //{
    //    //    indice = -2;
    //    //    Debug.Log(card.GetComponent<DragDrop>().isDraggable);
    //    //    gameManager.CacherDefausse(cardObj);
    //    //    BoardManager.defausse.Remove(cardObj); //Facon plus efficace de dire ca??
    //    //}
    //}

    public void ChooseThisCardToChange()
    {
       //gameManager._gameState.ChangeCard(no);

        /*int index = indice;
         * 
         * 
        public void ChangeCard(int cardNumber)
        {
            //_isTimeToChangeCards = _nbCardChanged >= 2 ? false : true;
            //if (_isTimeToChangeCards)
            //{
            //    _availableCards.Add(cardNumber);
            //    CurrentState.CardsInPlay.Remove(cardNumber);
            //    Draw(CurrentState.CardsInPlay[cardNumber].Player);
            //    _nbCardChanged++;
            //}
        }


        if (gameManager.isTimeToChangeCards)
        {
            BoardManager.cardsInHand.Remove(this);
            (string, int, int, int, bool, int, int, int) obj = gameManager.ChooseCardInHand();
            Debug.Log(obj.Item1);
            BoardManager.cardsAlreadyPicked.Remove(BoardManager.deck.IndexOf( (name, ability, power, rank, isHero, indice, no, faction)));
            card.GetComponent<CardBehavior>().Constructeur(obj.Item1, obj.Item2, obj.Item3, obj.Item4, obj.Item5, obj.Item6, obj.Item7, obj.Item8);
            gameManager.CreateCardDisplay(card);
            gameManager.InstanciateCards(index, BoardManager.cardsInHand.Count + 1, card);
            gameManager.physicalCards.Add(card);
            
            gameManager.physicalCards.Remove(gameObject);
            gameManager.nbCardChanged++;
            Destroy(gameObject);
        }*/
    }

    //private void Transfo((string, int, int, int, bool, int, int, int) obj)
    //{
    //    name = obj.Item1;
    //    ability = obj.Item2;
    //    power = obj.Item3;
    //    rank = obj.Item4;
    //    isHero = obj.Item5;
    //    indice = obj.Item6;
    //    no = obj.Item7;

    //}

    public void UseDecoy()
    {
        //gameManager.isDecoyBeingUsed = true;
        //gameManager.decoyCard = gameObject;
        ////BoardManager.cardsInHand.Remove(this);
        //gameManager.physicalCards.Remove(gameObject);
        //RectTransform rect = GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(100, 140);
    }


    public void SwapCards()
    {
        //if (gameManager.isDecoyBeingUsed && !gameManager.physicalCards.Contains(gameObject) && !isHero)
        //{
        //    gameObject.GetComponent<DragDrop>().isDraggable = true;
        //    gameManager.isDecoyBeingUsed = false;
        //    gameManager.decoyCard.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 129);
        //    GameObject dropZone = dropZones[rank - 7];
        //    float offset = 844f / BoardManager.wholeBoard[rank - 7].Count;
        //    Vector2 newPos = gameManager.decoyCard.transform.position;
        //    gameManager.decoyCard.transform.position = transform.position;
        //    transform.position = newPos;
        //    //gameManager.RemoveCard(indice, dropZones[rank - 7], BoardManager.wholeBoard[rank - 7]);
        //    //gameManager.AddCard(dropZones[rank - 7], gameManager.physicalCards, 844.0f / (gameManager.physicalCards.Count + 1));
        //    gameManager.physicalCards.Add(gameObject);
        //    BoardManager.wholeBoard[rank - 7].Remove(gameObject);
        //    BoardManager.board.Remove(gameObject);
        //    BoardManager.cardsInHand.Add(this);

        //}
    }
    public void UseFrost()
    {
        //CardBehavior cardB;
        //foreach (var card in BoardManager.wholeBoard[0])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (!cardB.isHero)
        //    {
        //        cardB.power = cardB.power - (cardB.ogPower - 1);
        //    }
        //}
        //foreach (var card in BoardManager.wholeBoard[3])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (!cardB.isHero)
        //    {
        //        cardB.power = cardB.power - (cardB.ogPower - 1);
        //    }
        //}
    }

    public void UseFog()
    {
        //CardBehavior cardB;
        //foreach (var card in BoardManager.wholeBoard[1])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (!cardB.isHero)
        //    {
        //        cardB.power = cardB.power - (cardB.ogPower - 1);
        //    }
        //}

        //foreach (var card in BoardManager.wholeBoard[4])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (!cardB.isHero)
        //    {
        //        cardB.power = cardB.power - (cardB.ogPower - 1);
        //    }
        //}
    }

    public void UseRain()
    {
        //CardBehavior cardB;
        //foreach (var card in BoardManager.wholeBoard[2])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (!cardB.isHero)
        //    {
        //        cardB.power = cardB.power - (cardB.ogPower - 1);
        //    }
        //}
        //foreach (var card in BoardManager.wholeBoard[5])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (!cardB.isHero)
        //    {
        //        cardB.power = cardB.power - (cardB.ogPower - 1);
        //    }
        //}
    }

    public void UseClearWeather()
    {
        //for (int i = 0; i < gameManager.isWeatherUsed.Count; i++)
        //{
        //    if (gameManager.isWeatherUsed[i])
        //    {
        //        foreach (var card in BoardManager.wholeBoard[i])
        //        {
        //            card.GetComponent<CardBehavior>().power += ogPower - 1;
        //        }

        //        foreach (var card in BoardManager.wholeBoard[i + 3])
        //        {
        //            card.GetComponent<CardBehavior>().power += ogPower - 1;
        //        }
        //    }
        //}

        //foreach (var card in BoardManager.board)
        //{
        //    if (card != gameObject && card.GetComponent<CardBehavior>().rank == 20)
        //    {
        //        Destroy(card);
        //    }
        //}
    }



    public void UseCommandersHorn()
    {
        //if (!gameManager.isComUsed[rank - 14])
        //{
        //    CardBehavior cardB;
        //    foreach (var card in BoardManager.wholeBoard[rank - 14])
        //    {
        //        cardB = card.GetComponent<CardBehavior>();
        //        if (!cardB.isHero)
        //        {
        //            cardB.power *= 2;
        //        }
        //    }

        //    gameManager.isComUsed[rank - 14] = true;

        //}
    }

    private void UseScorch()
    {

        //int power = 0;
        //RectTransform rect = GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(100, 139);
        //for (int i = 0; i < BoardManager.board.Count; i++)
        //{
        //    CardBehavior cardB = BoardManager.board[i].GetComponent<CardBehavior>();
        //    if (cardB.power > power)
        //    {
        //        power = cardB.power;
        //    }
        //}

        //foreach (var card in BoardManager.board)
        //{
        //    CardBehavior cardB = card.GetComponent<CardBehavior>();
        //    if (cardB.power == power && !cardB.isHero)
        //    {
        //        cardB.power = ogPower;
        //        BoardManager.wholeBoard[cardB.rank - 7].Remove(card);
        //        BoardManager.board.Remove(card);
        //        BoardManager.defausse.Add(cardB.TransformIntoCardObj());
        //        Destroy(card);
        //    }
        //}
        //Destroy(gameObject);

    }
    private void UseSpy()
    {
        //for (int i = 0; i < 2; i++)
        //{
        //    gameManager.CreateCards(gameManager.ChooseCardInHand(), BoardManager.cardsInHand.Count + 2, BoardManager.cardsInHand.Count + i);
        //}
        //gameManager.CreateCardDisplay(gameManager.physicalCards[gameManager.physicalCards.Count - 1]);
        //gameManager.CreateCardDisplay(gameManager.physicalCards[gameManager.physicalCards.Count - 2]);
    }

    private void UseTightBond()
    {
        //int nbCard = 0;
        //List<CardBehavior> tightBondCards = new List<CardBehavior>();
        //CardBehavior cardB;
        //foreach (var card in BoardManager.wholeBoard[rank - 7])
        //{
        //    cardB = card.GetComponent<CardBehavior>();
        //    if (cardB.name == name)
        //    {
        //        isTightBondUsed = true;
        //        tightBondCards.Add(cardB);
        //        nbCard++;
        //    }
        //}

        //foreach (var card in tightBondCards)
        //{
        //    isTightBondUsed = true;
        //    card.power *= nbCard;
        //}

        /* if (isTightBondUsed)
         {
             ogPower = power;
             power = (int)Mathf.Pow(power, nbCard);
         }*/

    }
    private void UseMedic()
    {
        //if (BoardManager.defausse.Count != 0)
        //{
        //    gameManager.MontrerDefausse(card);
        //}
    }


    private void UseMuster()
    {
        //int nbCardSpawned = 0;
        //int nbCardsInHandsToRemove = 0;
        //float offset;
        //Transform dropZone = dropZones[rank - 7].transform;
        //CardBehavior cardB;
        //CardBehavior cardBeh;
        //int index = 0;
        //List<(string, int, int, int, bool, int, int, int)> cardsToAdd =
        //    new List<(string, int, int, int, bool, int, int, int)>();
        //List<(string, int, int, int, bool, int, int, int)> cardsInHandsToMove =
        //    new List<(string, int, int, int, bool, int, int, int)>();
        //List<GameObject> row = BoardManager.wholeBoard[rank - 7];
        //foreach (var card in BoardManager.deck)
        //{

        //    if (card.Item1 == name && card.Item7 != no && !BoardManager.cardsAlreadyPicked.Contains(index))
        //    {
        //        Debug.Log(name + " nom");
        //        Debug.Log(card.Item1 + "nommm");
        //        /*BoardManager.board.Add(Instantiate(this.card,
        //            new Vector2(dropZone.position.x - 844 / 2.0f + offset * (indice + ++nbCardSpawned),
        //                dropZone.position.y), dropZone.rotation));
        //        cardB = BoardManager.board[BoardManager.board.Count - 1].GetComponent<CardBehavior>();
        //        cardB.Constructeur(card.Item1, card.Item2, card.Item3, card.Item4, card.Item5,  indice + nbCardSpawned, card.Item7, card.Item8);*/
        //        //BoardManager.wholeBoard[cardB.rank - 7].Add(BoardManager.board[BoardManager.board.Count - 1]);

        //        cardsToAdd.Add(card);
        //        BoardManager.cardsAlreadyPicked.Add(index);
        //        nbCardSpawned++;
        //    }
        //    else
        //    {
        //        if (card.Item1 == name && card.Item7 != no && BoardManager.cardsAlreadyPicked.Contains(index))
        //        {
        //            cardsInHandsToMove.Add(card);
        //            nbCardSpawned++;
        //        }
        //    }

        //    index++;
        //}

        //offset = 844.0f / (BoardManager.wholeBoard[rank - 7].Count + nbCardSpawned);
        //gameManager.AddCard(dropZone.gameObject, row, offset);


        //Debug.Log(cardsToAdd.Count + " cards to add");
        //int nbCardInRow = BoardManager.wholeBoard[rank - 7].Count;
        //gameManager.AddCard(dropZone.gameObject, BoardManager.wholeBoard[rank - 7], offset);
        //index = 0;
        //for (int i = 0; i < cardsToAdd.Count; i++)
        //{
        //    Debug.Log(nbCardInRow + " yass");
        //    GameObject card = Instantiate(this.card,
        //        new Vector2(dropZone.position.x - 844 / 2.0f + offset * (nbCardInRow),
        //            dropZone.position.y), dropZone.rotation);
        //    card.transform.SetParent(canvas.transform, true);
        //    cardB = card.GetComponent<CardBehavior>();
        //    cardB.indice = nbCardInRow++;

        //    cardB.Constructeur(cardsToAdd[i].Item1, cardsToAdd[i].Item2, cardsToAdd[i].Item3, cardsToAdd[i].Item4, cardsToAdd[i].Item5, cardsToAdd[i].Item6, cardsToAdd[i].Item7, cardsToAdd[i].Item8);
        //    BoardManager.wholeBoard[cardB.rank - 7].Add(card);
        //    BoardManager.board.Add(card);
        //}

        //List<CardBehavior> temp = new List<CardBehavior>();
        //foreach (var obj in cardsInHandsToMove)
        //{
        //    foreach (var card in BoardManager.cardsInHand)
        //    {
        //        if (card.no == obj.Item7)
        //        {
        //            card.transform.position = new Vector2(dropZone.position.x - 844 / 2.0f + offset * (nbCardInRow),
        //                dropZone.position.y);
        //            card.GetComponent<CardBehavior>().indice = nbCardInRow++;
        //            BoardManager.board.Add(card.gameObject);
        //            BoardManager.wholeBoard[rank - 7].Add(card.gameObject);
        //            gameManager.physicalCards.Remove(card.gameObject);
        //            temp.Add(card);

        //        }
        //    }
        //}


        //for (int i = 0; i < temp.Count; i++)
        //{
        //    gameManager.RemoveCard(temp[i].indice, cardZone, gameManager.physicalCards);
        //    BoardManager.cardsInHand.Remove(temp[i]);

        //}
        //temp.Clear();
        //cardsToAdd.Clear();
        //cardsInHandsToMove.Clear();
        /*foreach (var card in BoardManager.cardsInHand)
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
        }*/
    }
    private void UseMoralBoost()
    {

        //CardBehavior cardB;
        //rangee = BoardManager.wholeBoard[rank - 7]; //Jsp si ça marche de même
        //if (rangee.Count != 0 && name != "dandelion_card")
        //{
        //    foreach (var card in rangee)
        //    {
        //        cardB = card.GetComponent<CardBehavior>();
        //        if (!cardB.isHero)
        //        {
        //            cardB.power++;
        //        }
        //    }
        //}
        //else if (name == "dandelion_card")
        //{

        //    foreach (var card in BoardManager.wholeBoard[0])
        //    {
        //        cardB = card.GetComponent<CardBehavior>();
        //        if (!cardB.isHero)
        //        {
        //            cardB.power *= 2;
        //        }
        //    }

        //    gameManager.isComUsed[0] = true;
        //}
    }
    public void OnDrop()
    {
        //if ((int)abilities.none != ability)
        //{
        //    abilityFunctions[ability].Invoke();
        //}

    }
    public void OnClick()
    {
        //if (rank == -1)
        //{
        //    clickableAbilityFunctions[ability - 12].Invoke();
        //}
    }

}


#endif