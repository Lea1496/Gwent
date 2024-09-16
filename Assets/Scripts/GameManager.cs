using GwentEngine;
using GwentEngine.Phases;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GwentEngine.Abilities;
using Phases;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GamePhase CurrentGamePhase;
    private List<GamePhase> _gamePhases = new List<GamePhase>();
    private Dictionary<int, CardMetadata> _cardMetadata;
    private GameState _gameState;
    private bool _isCardPlayed;
    private bool[,] _points;
    private int _roundNumber;
    private bool _playerPassed;
    private bool _enemyPassed;

   
    public bool onClickCalled = false;

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

    private ConcurrentDictionary<int, (GameObject gameObject, Card card, PlayerKind player, Location location, int
        position)> _cardGameObjects;

    private Zones _zones;

    [SerializeField] public GameObject card;
    [SerializeField] private TextMeshProUGUI youStart;
    [SerializeField] private TextMeshProUGUI opponentStarts;
    [SerializeField] private Button keepCardsButton;
    [SerializeField] private TextMeshProUGUI _ennemyPoints;
    [SerializeField] private TextMeshProUGUI _playerPoints;


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
            }
            else
            {
                //location = zone == Card ? Location.none : location;
                location = zone == EnemySword ? Location.Sword : location;
                location = zone == EnemyArc ? Location.Archery : location;
                location = zone == EnemyCatapult ? Location.Catapult : location;
                location = zone == CommandersHornSword ? Location.ComandersHornSword : location;
                location = zone == CommandersHornArc ? Location.ComandersHornArchery : location;
                location = zone == CommandersHornCat ? Location.ComandersHornCatapult : location;
                location = zone == Weather ? Location.Weather : location;
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
      //  DontDestroyOnLoad(gameObject);
        
        var deckFullPath = Path.Combine(Application.dataPath, "Cards", "Deck.json");

        _cardGameObjects = new();
        _gameState = new();
        _zones = new();
        _isCardPlayed = false;

        _cardMetadata = CardMetadata.FromFile(deckFullPath);

        _gameState.NewGame(_cardMetadata, Settings.InitialCardCount);

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
        _gamePhases.Add(new ChooseFactionPhase());
        _gamePhases.Add(new ChooseDeckPhase(_gameState, null, () =>
        {
            SceneManager.LoadScene(1);
            _cardGameObjects.Clear();
        }));
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
        
        if (!IsChooseDeckPhase)
        {
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
        }
        

        //        Debug.Log($"Update phase: {sw.ElapsedMilliseconds} ms");
    }

    private void ManageCards(Card[] cards, GameObject zone, PlayerKind player, Location location, ref List<int> unknownCardNumbers)
    {
        var cardGameObjects = GenerateCardGameObjects(cards, player, location);

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
        
        int totalPlayerScore = 0;
        int totalOpponentScore = 0;

        if(player == PlayerKind.Opponent)
            totalOpponentScore += rowScore;
        else 
            totalPlayerScore += rowScore;

        _ennemyPoints.text = totalOpponentScore.ToString();
        _playerPoints.text = totalPlayerScore.ToString();
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
    public GameObject CreateNewCard(Card card)
    {
        var gameObject = Instantiate(this.card);
        var cardImage = GameObject.Find(card.Metadata.Name);
        var image = gameObject.GetComponent<Image>();
        if (card.IsHero)
        {
            gameObject.GetComponentInChildren<RawImage>().enabled = false;
        }
        image.sprite = cardImage.GetComponent<SpriteRenderer>().sprite;
        return gameObject;
    }
    
    public (GameObject gameObject, Card card, PlayerKind player, Location location, int index)[] GenerateCardGameObjects(
        IEnumerable<Card> cards, PlayerKind player, Location location)
    {
        var cardGameObjects = cards.Select((card, index) =>
            _cardGameObjects.AddOrUpdate(card.Number,
                key => (CreateNewCard(card), card, player, location, index),
                (key, existing) => (existing.gameObject, existing.card, player, location, index)
            )
        ).ToArray();

        return cardGameObjects;
    }

    public void UdateCardGameObjects(int card)
    {
        _cardGameObjects.TryRemove(card, out var removedValue);
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
        OnEndPhase(CurrentGamePhase);
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
    }

    

    public void OnEndTurnPhase(bool shouldChangeTurn, TurnPhase phase)
    {
        if (_playerPassed && _enemyPassed)
        {
            EndRound(int.Parse(_ennemyPoints.text) > int.Parse(_playerPoints.text) ? PlayerKind.Player : PlayerKind.Opponent);
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
}