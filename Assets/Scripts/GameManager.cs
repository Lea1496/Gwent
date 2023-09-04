using GwentEngine;
using GwentEngine.Phases;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GamePhase _currentGamePhase;
    private List<GamePhase> _gamePhases = new List<GamePhase>();
    private Dictionary<int, CardMetadata> _cardMetadata;
    private GameState _gameState;

    private ConcurrentDictionary<int, (GameObject gameObject, Card card, PlayerKind player, Location location, int
        position)> _cardGameObjects;

    private Zones _zones;

    [SerializeField] public GameObject card;
    [SerializeField] private TextMeshProUGUI youStart;
    [SerializeField] private TextMeshProUGUI opponentStarts;
    [SerializeField] private Button keepCardsButton;


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

            Debug.Log($"Cannot place card at : {player}, {location}");
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

        if (_gameState.FirstPlayer == PlayerKind.Player)
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

        _cardMetadata = CardMetadata.FromFile(deckFullPath);

        _gameState.NewGame(_cardMetadata, Settings.InitialCardCount);

        //Simulate(_gameState, PlayerKind.Player);
        //Simulate(_gameState, PlayerKind.Opponent);

        _currentGamePhase = null;
        _gamePhases = new List<GamePhase>();
        _gamePhases.Add(new ChooseFactionPhase());
        _gamePhases.Add(new ChooseDeckPhase());
        _gamePhases.Add(new ChangeInitialCardsPhase(_gameState, () => keepCardsButton.gameObject.SetActive(true), () => keepCardsButton.gameObject.SetActive(false)));
        _gamePhases.Add(new RoundPhase());

        UpdateAll();
        StartCoroutine();
    }

    private void ActivateGamePhase() 
    {
        GamePhase GetCurrentGamePhase() => _gamePhases.First(gp => !gp.Done);

        var currentGamePhase = GetCurrentGamePhase();

        while(currentGamePhase != _currentGamePhase)
        {
            _currentGamePhase = currentGamePhase;
            _currentGamePhase.Activate();
            
            currentGamePhase = GetCurrentGamePhase();
        }
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
                        gameState.Play(metadata.Number, location);
                        break;
                    }
                }

            }
        }
    }

    private void UpdateAll()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        ActivateGamePhase();

        var cardsByPlayerAndLocation = _gameState.GetAllCards();

        var unknownCardNumbers = _cardGameObjects.Keys.ToList();

        foreach (var ((player, location), cards) in cardsByPlayerAndLocation)
        {
            var zone = _zones.GetZone(player, location);
            if (zone == null)
            {
                continue;
            }

            var cardGameObjects = cards.Select((card, index) => _cardGameObjects.AddOrUpdate(card.Number,
                    (key) => (CreateNewCard(card), card, player, location, index),
                    (key, existing) => (existing.gameObject, existing.card, player, location, index)
                    )).
                    ToArray();

            cardGameObjects.ForEach(cardGameObject =>
            {
                unknownCardNumbers.Remove(cardGameObject.card.Number);
                cardGameObject.gameObject.GetComponent<CardBehavior>().SetInfo(cardGameObject.card);
                if (cardGameObject.gameObject.transform.parent != zone.transform)
                {
                    cardGameObject.gameObject.transform.localPosition = Vector3.zero;
                    cardGameObject.gameObject.transform.SetParent(zone.transform, true);
                }
            });

            GameObjectsDisposition.DistributeCenter(zone, cardGameObjects.Select(c => c.gameObject).ToArray(), 5);


            //TODO: update row scores
            var rowScore = cards.Sum(card => card.EffectivePower);
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

    private GameObject CreateNewCard(Card card)
    {
        var gameObject = Instantiate(this.card);
        var cardImage = GameObject.Find(card.Metadata.Name);
        var image = gameObject.GetComponent<Image>();
        image.sprite = cardImage.GetComponent<SpriteRenderer>().sprite;
        return gameObject;
    }

    public Location GetLocation(PlayerKind player, GameObject zone)
    {
        return _zones.GetLocation(player, zone);
    }

    public bool CanPlay(int number, Location location)
    {
        return _gameState.CanPlay(number, location);
    }

    public void Play(int number, Location location)
    {
        _gameState.Play(number, location);
    }

    public void OnClick(int cardNumber)
    {
        _currentGamePhase.OnClick(cardNumber);
    }

    public void EndCurrentPhase()
    {
        _currentGamePhase.EndCurrentPhase();
    }

    public bool IsDraggable(Card card)
    {
        return _currentGamePhase.IsDraggable(card);
    }
}