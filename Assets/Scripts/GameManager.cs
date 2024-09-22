using GwentEngine;
using GwentEngine.Phases;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Phases;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AsyncOperation = UnityEngine.AsyncOperation;

public class GameManager : MonoBehaviour, IManager
{
    private List<GamePhase> _gamePhases = new List<GamePhase>();
    private Dictionary<int, CardMetadata> _cardMetadata;
    private GameState _gameState;
    private bool _isCardPlayed;
    private bool[,] _points;
    private int _roundNumber;
    private bool _playerPassed;
    private bool _enemyPassed;
    private GameObject[] _cardsShown;


    [HideInInspector] public bool onClickCalled = false;
    [HideInInspector] public bool onFinishClicked = false;
    [HideInInspector] public bool isEmhyr1Active = false;
    
    public GamePhase CurrentGamePhase { get; set; }
    public PlayerKind CurrentPlayer
    {
        get => _gameState.CurrentPlayer;
        private set{}
    }
    
    private PlayerKind NotCurrentPlayer
    {
        get => CurrentPlayer == PlayerKind.Player ? PlayerKind.Opponent : PlayerKind.Player;
    }

    public bool IsChooseDeckPhase
    {
        get
        {
            if (CurrentGamePhase == null)
                return false;
            return CurrentGamePhase.GetType() == typeof(ChooseDeckPhase);
        }
    } 
    private bool IsTurnPhase
    {
        get
        {
            if (CurrentGamePhase == null)
                return false;
            return CurrentGamePhase.GetType() == typeof(TurnPhase);
        }
    }
    private bool IsOppositePlayerPassed
    {
        get => NotCurrentPlayer == PlayerKind.Player ? _playerPassed : _enemyPassed;
    }

    private Button FinishButtonActivePlayer
    {
        get => CurrentPlayer == PlayerKind.Player ? finishButtonEnemy : finishButton;
    }

    private ConcurrentDictionary<int, (GameObject gameObject, Card card, PlayerKind player, Location location, int
        position)> _cardGameObjects
    {
        get => CardsManager.CardGameObjects;
        set
        {
            CardsManager.CardGameObjects = value;
        }
    }

    private Zones _zones;

    [SerializeField] public GameObject card;
    [SerializeField] private TextMeshProUGUI youStart;
    [SerializeField] private TextMeshProUGUI opponentStarts;
    [SerializeField] private Button keepCardsButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private Button finishButtonEnemy;
    [SerializeField] private TextMeshProUGUI enemyPoints;
    [SerializeField] private TextMeshProUGUI playerPoints;
    [SerializeField] private GameObject showOpponentsCardsZone;
    [SerializeField] private GameObject showOpponentsCardsZoneEnemy;


    public class Zones
    {
        public Zones()
        {
            PlayerSword = GameObject.Find("DropZoneS");
            PlayerArc = GameObject.Find("DropZoneA");
            PlayerCatapult = GameObject.Find("DropZoneC");
            EnemySword = GameObject.Find("DropZoneEnemyS");
            EnemyArc = GameObject.Find("DropZoneEnemyA");
            EnemyCatapult = GameObject.Find("DropZoneEnemyC");
            Discard = GameObject.Find("Defausse");
            Card = GameObject.Find("CardZone");

            Canvas = GameObject.Find("Canvas");

            CommandersHornSword = GameObject.Find("CommandersHornSword");
            CommandersHornArc = GameObject.Find("CommandersHornArc");
            CommandersHornCat = GameObject.Find("CommandersHornCat");
            CommandersHornSwordEnemy = GameObject.Find("CommandersHornSwordEnemy");
            CommandersHornArcEnemy = GameObject.Find("CommandersHornArcEnemy");
            CommandersHornCatEnemy = GameObject.Find("CommandersHornCatEnemy");
            Weather = GameObject.Find("WeatherZone");
            Leader = GameObject.Find("LeaderZone");
            LeaderEnemy = GameObject.Find("LeaderZoneEnemy");
        }

        public GameObject PlayerSword { get; }
        public GameObject PlayerArc { get; }
        public GameObject PlayerCatapult { get; }

        public GameObject EnemySword { get; }
        public GameObject EnemyArc { get; }
        public GameObject EnemyCatapult { get; }

        public GameObject Discard { get; }
        public GameObject Card { get; }

        public GameObject Canvas { get; }

        public GameObject CommandersHornSword { get; }
        public GameObject CommandersHornArc { get; }
        public GameObject CommandersHornCat { get; }
        public GameObject CommandersHornSwordEnemy { get; }
        public GameObject CommandersHornArcEnemy { get; }
        public GameObject CommandersHornCatEnemy { get; }
        public GameObject Weather { get; }
        public GameObject Leader { get; }
        public GameObject LeaderEnemy { get; }

        public GameObject GetZone(PlayerKind player, Location location)
        {
            if (location == Location.Discard)
            {
                return Discard;
            }

            if (player == PlayerKind.Player)
            {
                switch (location)
                {
                    case Location.Hand: return Card;
                    case Location.Sword: return PlayerSword;
                    case Location.Archery: return PlayerArc;
                    case Location.Catapult: return PlayerCatapult;
                    case Location.ComandersHornSword: return CommandersHornSword;
                    case Location.ComandersHornArchery: return CommandersHornArc;
                    case Location.ComandersHornCatapult: return CommandersHornCat;
                    case Location.Weather: return Weather;
                    case Location.Leader: return Leader;
                }
            }
            else
            {
                switch (location)
                {
                    case Location.Hand: return null; //No hand display for opponent
                    case Location.Sword: return EnemySword;
                    case Location.Archery: return EnemyArc;
                    case Location.Catapult: return EnemyCatapult;
                    case Location.ComandersHornSword: return CommandersHornSwordEnemy;
                    case Location.ComandersHornArchery: return CommandersHornArcEnemy;
                    case Location.ComandersHornCatapult: return CommandersHornCatEnemy;
                    case Location.Weather: return Weather;
                    case Location.Leader: return LeaderEnemy;
                }
            }

            //Debug.Log($"Cannot place card at : {player}, {location}");
            return null;
        }

        public Location GetLocation(PlayerKind player, GameObject zone)
        {
            Location location = Location.None;

            if (zone == Discard)
            {
                return Location.Discard;
            }
            if (player == PlayerKind.Player)
            {
                location = zone == Card ? Location.Hand : location;
                location = zone == PlayerSword ? Location.Sword : location;
                location = zone == PlayerArc ? Location.Archery : location;
                location = zone == PlayerCatapult ? Location.Catapult : location;
                location = zone == CommandersHornSword ? Location.ComandersHornSword : location;
                location = zone == CommandersHornArc ? Location.ComandersHornArchery : location;
                location = zone == CommandersHornCat ? Location.ComandersHornCatapult : location;
                location = zone == Weather ? Location.Weather : location;
                location = zone == Leader ? Location.Leader : location;
            }
            else
            {
                //location = zone == Card ? Location.none : location;
                location = zone == EnemySword ? Location.Sword : location;
                location = zone == EnemyArc ? Location.Archery : location;
                location = zone == EnemyCatapult ? Location.Catapult : location;
                location = zone == CommandersHornSwordEnemy ? Location.ComandersHornSword : location;
                location = zone == CommandersHornArcEnemy ? Location.ComandersHornArchery : location;
                location = zone == CommandersHornCatEnemy ? Location.ComandersHornCatapult : location;
                location = zone == Weather ? Location.Weather : location;
                location = zone == LeaderEnemy ? Location.Leader : location;
            }

            //            Debug.Log($"Cannot plate card at : {player}, {location}");
            return location;
        }
    }


    private void StartCoroutine()
    {
        IEnumerator HideText(TextMeshProUGUI text)
        {
            yield return (2f);
            text.gameObject.SetActive(false);
        }

        IEnumerator coroutine;

        if (CurrentPlayer == PlayerKind.Player)
        {
            youStart.gameObject.SetActive(true);
            coroutine = HideText(youStart);
        }
        else
        {
            opponentStarts.gameObject.SetActive(true);
            coroutine = HideText(opponentStarts);
        }

        StartCoroutine(coroutine);
    }

    private void Update()
    {
        if (_gameState != null)
        {
            UpdateAll();
        }
    }
    
    private void Awake()
    {
        var deckFullPath = Path.Combine(Application.dataPath, "Cards", "Deck.json");

        _cardGameObjects = new();
        _gameState = new();
        _zones = new();
        _isCardPlayed = false;

        CreateGameState(CardsManager.CardMetadata);
       // _cardMetadata = CardMetadata.FromFile(deckFullPath);
       // _gameState.NewGame(_cardMetadata, Settings.InitialCardCount);

        _cardsShown = new GameObject[3];

        _points = new bool[2, 2];
        _points[0, 0] = true;
        _points[0, 1] = true;
        _points[1, 0] = true;
        _points[1, 1] = true;

        _roundNumber = 1;
        _playerPassed = false;
        _enemyPassed = false;

        //Simulate(_gameState, PlayerKind.Player);
        //Simulate(_gameState, PlayerKind.Opponent);

        CurrentPlayer = _gameState.CurrentPlayer;

        CurrentGamePhase = null;
        _gamePhases = new List<GamePhase>();
        _gamePhases.Add(new ChangeInitialCardsPhase(_gameState, () =>
        {
            StartCoroutine();
            keepCardsButton.gameObject.SetActive(true);
        }, () => keepCardsButton.gameObject.SetActive(false)));
       // _gamePhases.Add(new RoundPhase());
        _gamePhases.Add(new TurnPhase(_gameState));
        
        UpdateAll();
        //StartCoroutine();
    }

    public void CreateGameState(Dictionary<int, CardMetadata> cardMetadata)
    {
        _cardMetadata = cardMetadata;
        _gameState.NewGame(_cardMetadata, Settings.InitialCardCount);
    }

    private void ActivateGamePhase()
    {
        GamePhase GetCurrentGamePhase() => _gamePhases.First(gp => !gp.Done);
        
        var currentGamePhase = GetCurrentGamePhase();

        while (currentGamePhase != CurrentGamePhase)
        {
            CurrentGamePhase = currentGamePhase;
            CurrentGamePhase.Activate();

            currentGamePhase = GetCurrentGamePhase();
        }
    }
    
    public void ActivateGamePhase(GamePhase phase)
    {
        CurrentGamePhase = phase;
        _gamePhases.Add(phase);
        CurrentGamePhase.Activate(); //peut-être à changer
    }

    private void Simulate(GameState gameState, PlayerKind player)
    {
        var items = new Dictionary<Location, int>()
        {
            { Location.ComandersHornCatapult, 1 },
            { Location.Catapult, 5 },
            { Location.ComandersHornSword, 1 },
            { Location.Sword, 6 },
            { Location.ComandersHornArchery, 1 },
            { Location.Archery, 2 }
        };

        foreach (var (location, count) in items)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (var metadata in _cardMetadata.Values)
                {
                    if (gameState.CanPlayAndAvailable(metadata.Number, location))
                    {
                        gameState.UseCard(metadata.Number, player);
                        Play(metadata.Number, location);
                        break;
                    }
                }

            }
        }
    }

    private void UpdateAll()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        if (CurrentGamePhase != null && IsTurnPhase)
        {
            if (!_isCardPlayed)
            {
                ActivateGamePhase();
            }
            else
            {
                _isCardPlayed = false;
                EndCurrentPhase();
                _gamePhases.Add(new TurnPhase(_gameState));
                ActivateGamePhase();
            }
        }
        else
        {
            ActivateGamePhase();
        }

        enemyPoints.text = "0";
        playerPoints.text = "0";
        
        var initialRowMultipliers = _gameState.RowMultipliers;
            
        var cardsByPlayerAndLocation = _gameState.GetAllCards();

        var unknownCardNumbers = _cardGameObjects.Keys.ToList();

        var finalRowMultipliers = _gameState.RowMultipliers;
        
        UpdateEffectivePowers(initialRowMultipliers, finalRowMultipliers);

        //Est-ce qu'on a vraiment besoin de generate à chaque tick?
    
        foreach (var ((player, location), cards) in cardsByPlayerAndLocation)
        {
            var zone = _zones.GetZone(player, location);
            if (!zone)
            {
                continue;
            }

            ManageCards(cards, zone, player, location, ref unknownCardNumbers);
        
        }
        foreach (var cardNumber in unknownCardNumbers)
        {
            if (_cardGameObjects.TryRemove(cardNumber, out var v))
            {
                Destroy(v.gameObject);
            }
        }
        
        

        //        Debug.Log($"Update phase: {sw.ElapsedMilliseconds} ms");
    }

    private void ManageCards(Card[] cards, GameObject zone, PlayerKind player, Location location, ref List<int> unknownCardNumbers)
    {
        var cardGameObjects = CardsManager.GenerateCardGameObjects(cards, player, location, this);

        foreach (var cardGameObject in cardGameObjects)
        {
            unknownCardNumbers.Remove(cardGameObject.card.Number);
            cardGameObject.gameObject.GetComponent<CardBehavior>().SetInfo(cardGameObject.card);
            if (cardGameObject.gameObject.transform.parent != zone.transform)
            {
                cardGameObject.gameObject.transform.localPosition = Vector3.zero;
                cardGameObject.gameObject.transform.SetParent(zone.transform, true);
            }
        }

        GameObjectsDisposition.DistributeCenter(zone, cardGameObjects.Select(c => c.gameObject).ToArray(), 5);
            
        UpdateScores(cards, player);
    }

    private void UpdateScores(Card[] cards, PlayerKind player)
    {
        var rowScore = cards
            .Where(rCard => rCard.Location != Location.None && 
                            rCard.Location != Location.Dead &&
                            rCard.Location != Location.Discard &&
                            rCard.Location != Location.Hand &&
                            rCard.EffectivePower != -1)
            .Sum(rCard => rCard.EffectivePower);
        
        int totalPlayerScore = int.Parse(playerPoints.text);
        int totalOpponentScore = int.Parse(enemyPoints.text) ;

        if(player == PlayerKind.Opponent)
            totalOpponentScore += rowScore;
        else 
            totalPlayerScore += rowScore;

        enemyPoints.text = totalOpponentScore.ToString();
        playerPoints.text = totalPlayerScore.ToString();
    }
    private void UpdateEffectivePowers(Dictionary<Tuple<Location, PlayerKind>, ActionKind> initialRowMult,
        Dictionary<Tuple<Location, PlayerKind>, ActionKind> finalRowMult)
    {
        var swordPlayer = new Tuple<Location, PlayerKind>(Location.Sword, PlayerKind.Player);
       // if(initialRowMult[swordPlayer] != finalRowMult[swordPlayer])
            GetCards(PlayerKind.Player, Location.Sword).ForEach(rCard => rCard.SetAction(finalRowMult[swordPlayer]));
        var archeryPlayer = new Tuple<Location, PlayerKind>(Location.Archery, PlayerKind.Player);
        //if(initialRowMult[archeryPlayer] != finalRowMult[archeryPlayer])
            GetCards(PlayerKind.Player, Location.Archery).ForEach(rCard => rCard.SetAction(finalRowMult[archeryPlayer]));
        var catapultPlayer = new Tuple<Location, PlayerKind>(Location.Catapult, PlayerKind.Player);
       // if(initialRowMult[catapultPlayer] != finalRowMult[catapultPlayer])
            GetCards(PlayerKind.Player, Location.Catapult).ForEach(rCard => rCard.SetAction(finalRowMult[catapultPlayer]));
        var swordOpponent = new Tuple<Location, PlayerKind>(Location.Sword, PlayerKind.Opponent);
       // if(initialRowMult[swordOpponent] != finalRowMult[swordOpponent])
            GetCards(PlayerKind.Opponent, Location.Sword).ForEach(rCard => rCard.SetAction(finalRowMult[swordOpponent]));
        var archeryOpponent = new Tuple<Location, PlayerKind>(Location.Archery, PlayerKind.Opponent);
        //if(initialRowMult[archeryOpponent] != finalRowMult[archeryOpponent])
            GetCards(PlayerKind.Opponent, Location.Archery).ForEach(rCard => rCard.SetAction(finalRowMult[archeryOpponent]));
        var catapultOpponent = new Tuple<Location, PlayerKind>(Location.Catapult, PlayerKind.Opponent);
        //sif(initialRowMult[catapultOpponent] != finalRowMult[catapultOpponent])
            GetCards(PlayerKind.Opponent, Location.Catapult).ForEach(rCard => rCard.SetAction(finalRowMult[catapultOpponent]));

    }

    public Card GetCard(int cardNumber, PlayerKind player, Location location)
    {
        var cards = _gameState.GetCards(player, location);
        var rCard = cards.First(rCard => rCard.Number == cardNumber);
        return rCard;
    }

    public Card GetCard(int cardNumber)
    {
        var cards = _gameState.AllCards;
        return cards.First(rCard => rCard.Number == cardNumber);
    }
    
    
    

    public Location GetLocation(PlayerKind player, GameObject zone)
    {
        return _zones.GetLocation(player, zone);
    }

    public bool CanPlay(int number, Location location)
    {
        return _gameState.CanPlay(number, location);
    }

    public void UseCard(int number, PlayerKind player)
    {
        _gameState.UseCard(number, player);
    }

    public void Play(int number, Location location)
    {
        _isCardPlayed = true;
        
        var cardInPlay = _gameState.Play(number, location);

        if (CurrentGamePhase.GetType() == typeof(UseMedicPhase))
        {
            EndCurrentPhase();
        }
       
        var initialPhase = cardInPlay.Metadata.CardAbility.CreateInitialPhase(cardInPlay, this);
        if (initialPhase != null)
        {
            _gamePhases.Insert(0, initialPhase);
        }

        cardInPlay.Location = location;
    }

    public CardMetadata[] AllAvailableCards
    {
        get
        {
            return _gameState.AllAvailableCards;
        }
    }

    public void RemoveAllWeatherCards()
    {
        _gameState.RemoveAllWeatherCards();
    }
    
    public void RemoveClearWeatherCards()
    {
        _gameState.RemoveClearWeatherCards();
    }
    
    public Card[] GetCards(PlayerKind player, Location location)
    {
        return _gameState.GetCards(player, location);
    }

    public void OnClick(int number, GameObject card)
    {
        CurrentGamePhase.OnClick(number, card);
    }

    public void EndCurrentPhase()
    {
        CurrentGamePhase.EndCurrentPhase();
     //   OnEndPhase(CurrentGamePhase);
    }

    public bool IsDraggable(Card card)
    {
        return CurrentGamePhase.IsDraggable(card);
    }

    public void ChangeEffectivePlayer()
    {
        if (_gameState.CurrentPlayer == PlayerKind.Opponent)  // pas certaine de cette implémentation
        {
            _gameState.CurrentPlayer = PlayerKind.Player;
        }
        else
        {
            _gameState.CurrentPlayer = PlayerKind.Opponent;
        }
    }
    
    public void StartWaitForFunctionCoroutine()
    {
        // Wait until functionCalled becomes true
        
        StartCoroutine(new WaitUntil(() => onClickCalled));
        onClickCalled = false;
    }

    

    public void OnEndTurnPhase(bool shouldChangeTurn, TurnPhase phase)
    {
        if (_playerPassed && _enemyPassed)
        {
            EndRound(int.Parse(enemyPoints.text) > int.Parse(playerPoints.text) ? PlayerKind.Player : PlayerKind.Opponent);
            return;
        }
        
        if (shouldChangeTurn && !IsOppositePlayerPassed)
        {
            ChangeEffectivePlayer();
        }
       /* else
        {
            _gamePhases.Remove(phase);
        }*/
    }

    public void OnEndPhase(GamePhase phase)
    {
        _gamePhases.Remove(phase);
    }

    public void RemoveCard(int cardNumber, bool isRecyclable)
    {
        _gameState.RemoveCard(cardNumber,isRecyclable);
    }

    public void ExecuteSpy(int? sequence = null)
    {
        _gameState.ExecuteSpy(CurrentPlayer, sequence);
    }

    public void SetRowAction(Location location, ActionKind action)
    {
        _gameState.SetRowAction(location, CurrentPlayer, action);
    }
    public void SetRowAction(Location location, PlayerKind player, ActionKind action)
    {
        _gameState.SetRowAction(location, player, action);
    }

    public void EndRound(PlayerKind winningPlayer)
    {
        int nPlayer = winningPlayer == PlayerKind.Player ? 1 : 0;

        _points[nPlayer, _roundNumber - 1] = false;
        
        // Todo : Implémenter les pouvoirs de factions selon la faction choisie et la round

        if (!(_points[nPlayer, 0] && _points[nPlayer, 1]))
        {
            EndGame();
        }
        else
        {
            _roundNumber++;
            _playerPassed = false;
            _enemyPassed = false;
        }
    }

    private void EndGame()
    {
        // Todo : faire un endgame phase ?
    }

    public void PassTurn()
    {
        if (CurrentPlayer == PlayerKind.Player)
        {
            _playerPassed = true;
        }
        else
        {
            _enemyPassed = true;
        }
    }

    public void ShowOpponentCards()
    {
        isEmhyr1Active = true;
        var cardsToShow = _gameState.ShowOponnentCards();

        var opponentCardZone =
            CurrentPlayer == PlayerKind.Opponent ? showOpponentsCardsZone : showOpponentsCardsZoneEnemy;

        FinishButtonActivePlayer.gameObject.SetActive(true);

        for(int i = 0; i < 3; i++)
        {
            var cardToInit = _cardGameObjects[cardsToShow[i].Number].gameObject;
            var boxCollider = cardToInit.GetComponent<BoxCollider2D>();
            Destroy(boxCollider);
            _cardsShown[i] = Instantiate(_cardGameObjects[cardsToShow[i].Number].gameObject, opponentCardZone.transform);
            _cardsShown[i].transform.localScale.Scale(new Vector3(2f, 2f, 2f));

            //pas certaine
            var newCollider = cardToInit.AddComponent<BoxCollider2D>();
            
            newCollider.size = boxCollider.size;
            newCollider.offset = boxCollider.offset;
            newCollider.isTrigger = boxCollider.isTrigger;

        }
        GameObjectsDisposition.DistributeCenter(opponentCardZone, _cardsShown, 5);

        StartWaitForFinishCoroutine();

    }
    public void StartWaitForFinishCoroutine()
    {       
        StartCoroutine(new WaitUntil(() => onFinishClicked));
        onFinishClicked = false;
    }
    public void OnHideOpponentCards()
    {
        for(int i = 0; i < _cardsShown.Length; i++)
        {
            Destroy(_cardsShown[i]);
            _cardsShown[i] = null;
        }
        
        FinishButtonActivePlayer.gameObject.SetActive(false);

        onFinishClicked = true;
        isEmhyr1Active = false;
    }
    

    public void ReviveCards()
    {
        _gameState.ReviveCards();
    }

    public GameObject InstantiateCard()
    {
        return Instantiate(card);
    }
    
    public bool ExistsCardsInDiscard()
    {
        return _gameState.GetCards(CurrentPlayer, Location.Discard).Length != 0;
    }
}