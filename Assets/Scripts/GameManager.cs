using GwentEngine;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float _cardSize = 844.0f;
    private Dictionary<int, CardMetadata> _cardMetadata;
    private bool _isTimeToChangeCards = false;
    public GameState _gameState;
    private int _instanciationCounter = 0;

    private ConcurrentDictionary<int, (GameObject gameObject, Card card, PlayerKind player, Location location, int
        position)> _cardGameObjects;

    private Zones _zones;

    [SerializeField] public GameObject card;
    [SerializeField] private TextMeshProUGUI youStart;
    [SerializeField] private TextMeshProUGUI opponentStarts;
    [SerializeField] private Button keepCardsButton;
    public bool IsTimeToChangeCards
    {
        get => _isTimeToChangeCards;
        set => _isTimeToChangeCards = IsTimeToChangeCards;
    }

    public int NbCardsChanged { get; set; }
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
                }
            }

            Debug.Log($"Cannot plate card at : {player}, {location}");
            return null;
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
            UpdateAllCards();
        }

        if (_isTimeToChangeCards && _instanciationCounter < 1)
        {
            keepCardsButton.gameObject.SetActive(true);
            _instanciationCounter++;
        }

        _isTimeToChangeCards = NbCardsChanged >= 2 ? false : true;
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

        UpdateAllCards();
        _isTimeToChangeCards = true;
        StartCoroutine();
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

    private void UpdateAllCards()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var cardsByPlayerAndLocation = _gameState.GetAllCards();

        var unknownCardNumbers = _cardGameObjects.Keys.ToList();

        foreach (var ((player, location), cards) in cardsByPlayerAndLocation)
        {
            var zone = _zones.GetZone(player, location);
            if (zone == null)
            {
                continue;
            }

            for (var i = 0; i < cards.Length; i++)
            {
                var card = cards[i];

                var offset = _cardSize / cards.Length;
                var x2 = (offset * i) + zone.transform.position.x - (_cardSize / 2);

                unknownCardNumbers.Remove(card.Number);

                _cardGameObjects.AddOrUpdate(card.Number, key =>
                {
                    var gameObject = Instantiate(this.card, new Vector2(x2, zone.transform.position.y),
                        zone.transform.rotation);
                    gameObject.transform.SetParent(_zones.Canvas.transform, true);
                    gameObject.GetComponent<Image>().sprite =
                        GameObject.Find(card.Metadata.Name).GetComponent<SpriteRenderer>().sprite;
                    gameObject.GetComponent<CardBehavior>().SetInfo(card, player);

                    return (gameObject, card, player, location, i);
                }, (key, existing) =>
                {
                    //TODO: also add the power in the tuple for changing the text
                    if (existing.player != player || existing.location != location || existing.position != i)
                    {
                        //TODO: update to new X / Y
                    }

                    return (existing.gameObject, existing.card, existing.player, existing.location, i);
                });
            }

            foreach (var cardNumber in unknownCardNumbers) 
            {
                if(_cardGameObjects.TryRemove(cardNumber, out var v))
                {
                    Destroy(v.gameObject);
                }
            }

            //TODO: update row scores
            var rowScore = cards.Sum(card => card.Power);
        }

//        Debug.Log($"Update phase: {sw.ElapsedMilliseconds} ms");
    }
}