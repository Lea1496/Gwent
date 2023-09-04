using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using GwentEngine.Abilities;

namespace GwentEngine
{
    public static class Settings
    {
        public const int InitialCardCount = 10;
    }

    public class CardMetadata
    {
        private Lazy<CardAbility> _cardAbility;

        public string Name { get; set; }
        public Ability Ability { get; set; }
        public CardAbility CardAbility => _cardAbility.Value;
        public int DefaultPower { get; set; }
        public Location PossibleLocations { get; set; }
        public bool IsHero { get; set; }
        public int Number { get; set; }
        public int Faction { get; set; }
        public bool InOpponentZone { get; set; }

        public CardMetadata()
        {
            _cardAbility = new(() => CardAbilityFactory.Create(Ability));
        }

        public static Dictionary<int, CardMetadata> FromFile(string fullPath)
        {
            var json = File.ReadAllText(fullPath);
            return FromJson(json);
        }

        public static Dictionary<int, CardMetadata> FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CardMetadata[]>(json).ToDictionary(i => i.Number);
        }

        public override string ToString() => $"#{Number} {Name} {Ability}";
    }

    [Flags]
    public enum Location
    {
        None = 0b0000000000,
        Hand = 0b0000000001,
        ComandersHornCatapult = 0b0000000010,
        Catapult = 0b0000000100,
        ComandersHornSword = 0b0000001000,
        Sword = 0b0000010000,
        ComandersHornArchery = 0b0000100000,
        Archery = 0b0001000000,
        Discard = 0b0010000000,
        Weather = 0b0100000000,
        Dead = 0b1000000000
    }

    public enum PlayerKind
    {
        Player,
        Opponent,
    }

    public class CardInPlay
    {
        private static int _sequence = 0;

        public readonly CardMetadata Metadata;

        public PlayerKind Player;
        public int Sequence;
        public Location Location;

        public CardInPlay(CardMetadata metadata, Location location, PlayerKind player, int? sequence = null)
        {
            Metadata = metadata;
            Sequence = sequence.GetValueOrDefault(++_sequence);
            Location = location;
            Player = player;
        }

        public override string ToString() => $"#{Metadata} {Location} {Player}";
    }

    public class Card
    {
        private CardInPlay _cardInPlay;

        public Card(CardInPlay cardInPlay)
        {
            _cardInPlay = cardInPlay;
            PowerMultiplier = 1;
            Power = _cardInPlay.Metadata.DefaultPower;
        }

        public int PowerMultiplier { get; set; }
        public int Power { get; set; }
        public int Number => _cardInPlay.Metadata.Number;
        public int Sequence => _cardInPlay.Sequence;
        public Location Location => _cardInPlay.Location;
        public int EffectivePower => Power * PowerMultiplier;

        public string Name => _cardInPlay.Metadata.Name;
        public Ability Ability => _cardInPlay.Metadata.Ability;
        public int DefaultPower => _cardInPlay.Metadata.DefaultPower;
        public Location PossibleLocations => _cardInPlay.Metadata.PossibleLocations;
        public bool IsHero => _cardInPlay.Metadata.IsHero;
        public int Faction => _cardInPlay.Metadata.Faction;
        public CardMetadata Metadata => _cardInPlay.Metadata;
        public override string ToString() => $"#{Number} {Location} {EffectivePlayer}";

        public PlayerKind EffectivePlayer
        {
            get
            {
                if (Location == Location.Hand)
                {
                    return _cardInPlay.Player;
                }

                return _cardInPlay.Metadata.InOpponentZone ? _cardInPlay.Player.Reverse() : _cardInPlay.Player;
            }
        }
    }

    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate.Invoke(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static bool IsClickable(this Ability ability) =>
            (new[] { Ability.Decoy, Ability.Scorch, Ability.Leader }).Contains(ability);

        public static PlayerKind Reverse(this PlayerKind playerKind) =>
            playerKind == PlayerKind.Opponent ? PlayerKind.Player : PlayerKind.Opponent;
    }

    public class BoardState
    {
        public Dictionary<int, CardInPlay> CardsInPlay { get; set; } = new();
    }

    public class GameState
    {
        private BoardState _currentState = new();
        private Dictionary<int, CardMetadata> _metadata = new();
        private List<int> _availableCards = new();
        private Random _random = new Random(DateTime.Now.Millisecond);
        private Card[] _allCards;

        public PlayerKind FirstPlayer { get; private set; }

        public void NewGame(Dictionary<int, CardMetadata> metadata, int initialCardCount = 0)
        {
            _metadata = metadata;
            _availableCards = _metadata.Keys.ToList();
            _currentState = new();

            Enumerable.Range(0, initialCardCount).ForEach(i =>
            {
                Draw(PlayerKind.Player);
                Draw(PlayerKind.Opponent);
            });

            FirstPlayer = _random.Next(0, 1) == 0 ? PlayerKind.Player : PlayerKind.Opponent;

            _allCards = null;
        }

        private void Draw(PlayerKind player, int? sequence = null)
        {
            var index = _random.Next(0, _availableCards.Count - 1);
            var cardNumber = _availableCards[index];

            UseCard(cardNumber, player, sequence);
        }

        public void ChangeCard(int cardNumber)
        {
            var currentCard = _currentState.CardsInPlay[cardNumber];
            Draw(currentCard.Player, currentCard.Sequence);
            RemoveCard(cardNumber, true);
        }

        public void UseCard(int cardNumber, PlayerKind player, int? sequence = null)
        {
            var cardMetadata = _metadata[cardNumber];
            var cardInPlay = new CardInPlay(cardMetadata, Location.Hand, player, sequence);
            _currentState.CardsInPlay[cardNumber] = cardInPlay;
            _availableCards.Remove(cardNumber);

            _allCards = null;
        }

        public bool CanPlayAndAvailable(int cardNumber, Location location)
        {
            var cardMetadata = _metadata[cardNumber];

            if ((cardMetadata.PossibleLocations & location) == 0)
            {
                return false;
            }

            if (!_availableCards.Contains(cardNumber))
            {
                return false;
            }

            return true;
        }

        public bool CanPlay(int cardNumber, Location location)
        {
            var cardMetadata = _metadata[cardNumber];

            if ((cardMetadata.PossibleLocations & location) == 0)
            {
                return false;
            }


            return true;
        }


        public void Play(int cardNumber, Location location)
        {
            var cardMetadata = _metadata[cardNumber];

            if ((cardMetadata.PossibleLocations & location) == 0)
            {
                throw new Exception($"Location {location} is not playable");
            }

            if (!_currentState.CardsInPlay.TryGetValue(cardNumber, out var cardInPlay))
            {
                throw new Exception($"Cannot play a card not drawn yet");
            }

            cardInPlay.Location = location;

            _allCards = null;
        }

        public void RemoveCard(int cardNumber, bool isRecyclable)
        {
            var cardMetadata = _metadata[cardNumber];
            if (!_currentState.CardsInPlay.TryGetValue(cardNumber, out var cardInPlay))
            {
                throw new Exception($"Cannot play a card not drawn yet");
            }
            if (isRecyclable)
            {
                _currentState.CardsInPlay.Remove(cardNumber);
                _availableCards.Add(cardNumber);
            }
            else
            {
                if (!cardMetadata.IsHero || cardMetadata.DefaultPower != -1)
                {
                    cardInPlay.Location = Location.Discard;
                }
                else
                {
                    cardInPlay.Location = Location.Dead;
                }
            }

            _allCards = null;
        }

        public Card[] AllCards
        {
            get
            {
                if (_allCards == null)
                {
                    _allCards = BuildAllCards();
                }

                return _allCards;
            }
        }

        public Card[] GetCards(PlayerKind player, Location location)
        {
            return AllCards
                .Where(c => c.Location == location && c.EffectivePlayer == player)
                .OrderBy(c => c.Sequence)
                .ToArray();
        }

        private Card[] BuildAllCards()
        {
            var allCards = _currentState.CardsInPlay.Values.Select(c => new Card(c)).ToArray();

            foreach (var cardX in allCards)
            {
                foreach (var cardY in allCards)
                {
                    cardX.Metadata.CardAbility.ApplyAbility(cardX, cardY);
                }
            }

            return allCards;
        }

        public Dictionary<(PlayerKind, Location), Card[]> GetAllCards()
        {
            return AllCards
                .GroupBy(c => (c.EffectivePlayer, c.Location))
                .ToDictionary(g => g.Key, g => g.OrderBy(c => c.Sequence).ToArray());
        }

        public string ToJson() => JsonConvert.SerializeObject(_currentState);

        public static GameState FromFile(Dictionary<int, CardMetadata> metadata, string fullPath)
        {
            return FromJson(metadata, File.ReadAllText(fullPath));
        }

        public static GameState FromJson(Dictionary<int, CardMetadata> metadata, string json)
        {
            return new GameState()
            {
                _currentState = JsonConvert.DeserializeObject<BoardState>(json),
                _metadata = metadata
            };
        }

        public string PrettyPrint()
        {
            return $"{_currentState}";
        }
    }

    namespace Phases
    {

        public class GamePhase
        {
            private readonly Action _onActivatePhase;
            private readonly Action _onEndPhase;

            public virtual bool Done { get; private set; }

            public GamePhase(Action onActivatePhase = null, Action onEndPhase = null)
            {
                _onActivatePhase = onActivatePhase;
                _onEndPhase = onEndPhase;
            }

            public virtual void OnClick(int number) { }

            public virtual void Activate()
            {
                _onActivatePhase?.Invoke();
            }
            public void EndCurrentPhase()
            {
                _onEndPhase?.Invoke();
                Done = true;
            }

            public virtual bool IsDraggable(Card card)
            {
                return false;
            }
        }

        public class ChooseFaction : GamePhase
        {
            public override void Activate()
            {
                EndCurrentPhase(); //This phase is not implemented yet, just simulate end right away}
            }
        }

        public class ChooseDeck : GamePhase
        {
            public override void Activate()
            {
                EndCurrentPhase(); //This phase is not implemented yet, just simulate end right away}
            }
        }

        public class ChangeInitialCards : GamePhase
        {
            private readonly GameState _gameState;

            private int _nbCardsChanged;

            public ChangeInitialCards(GameState gameState, Action onActivatePhase, Action onEndPhase)
                : base(onActivatePhase, onEndPhase)
            {
                _gameState = gameState;
                _nbCardsChanged = 0;
            }

            public override void OnClick(int number)
            {
                _gameState.ChangeCard(number);
                _nbCardsChanged++;

                if (_nbCardsChanged == 2)
                {
                    EndCurrentPhase();
                }
            }
        }

        public class Round : GamePhase
        {
            public override bool IsDraggable(Card card)
            {
                return card.EffectivePlayer == PlayerKind.Player && card.Location == Location.Hand;
            }
        }
    }
}