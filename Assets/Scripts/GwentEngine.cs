using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace GwentEngine
{
    public static class Settings
    {
        public const int InitialCardCount = 10;
    }

    //TODO : Question: une abilité est composable ?
    public enum Ability
    {
        Agile = 0,
        Berserker = 1,
        Mardroeme = 2,
        Medic = 3,
        MoralBoost = 4,
        Muster = 5,
        Spy = 6,
        TightBond = 7,
        None = 8,

        Frost = 9,
        Fog = 10,
        Rain = 11,
        CommandersHorn = 12,
        ClearWeather = 13,

        //TODO : Question c'est des abileté? elle doivent aller la si oui
        Decoy = 14,
        Scorch = 15,
        Leader = 16
    }


    public abstract class CardAbility
    {
        public virtual void ApplyAbility(Card source, Card target)
        {
            if (target.IsHero)
            {
                //Not applicable to heroes
                return;
            }

            Apply(source, target);
        }

        protected virtual void Apply(Card source, Card target)
        {
        }
    }

    public abstract class SamePlayerCardAbility : CardAbility
    {
        public override void ApplyAbility(Card source, Card target)
        {
            if (source.EffectivePlayer != target.EffectivePlayer)
            {
                return;
            }

            if (source.Number == target.Number)
            {
                //Cannot apply to self
                return;
            }

            base.ApplyAbility(source, target);
        }
    }

    public class MoralBoostAbility : SamePlayerCardAbility
    {
        protected override void Apply(Card source, Card target)
        {
            if (source.Location != target.Location)
            {
                //Not in the same zone
                return;
            }

            //Applicable
            target.Power += 1;
        }
    }

    public class CommandersHornAbility : SamePlayerCardAbility
    {
        private static Dictionary<Location, Location> LocationWithCommandersHorn = new()
        {
            { Location.ComandersHornCatapult, Location.Catapult },
            { Location.ComandersHornSword, Location.Sword },
            { Location.ComandersHornArchery, Location.Archery },
            { Location.Sword, Location.Sword },
        };

        protected override void Apply(Card source, Card target)
        {
            if (!LocationWithCommandersHorn.TryGetValue(source.Location, out var targetLocation))
            {
                //This commanders horn is not in play
                return;
            }

            if (target.Location != targetLocation)
            {
                //This commanders horn is not in play
                return;
            }

            //Applicable
            target.PowerMultiplier = 2;
        }
    }

    public abstract class SpecificLocationAbility : CardAbility
    {
        private readonly Location _targetLocation;

        public SpecificLocationAbility(Location targetLocation)
        {
            _targetLocation = targetLocation;
        }

        public override void ApplyAbility(Card source, Card target)
        {
            if (target.Location != _targetLocation)
            {
                return;
            }

            base.ApplyAbility(source, target);
        }

        protected override void Apply(Card source, Card target)
        {
            target.Power = 1;
        }
    }

    public class FogAbility : SpecificLocationAbility
    {
        public FogAbility() : base(Location.Archery) { }
    }

    public class FrostAbility : SpecificLocationAbility
    {
        public FrostAbility() : base(Location.Sword) { }
    }

    public class RainAbility : SpecificLocationAbility
    {
        public RainAbility() : base(Location.Catapult) { }
    }

    public class DefaultAbility : CardAbility
    {
    }


    public static class CardAbilityFactory
    {
        public static CardAbility Create(Ability ability)
        {
            switch (ability)
            {
                case Ability.Agile: return new DefaultAbility();
                case Ability.Berserker: return new DefaultAbility();
                case Ability.Mardroeme: return new DefaultAbility();
                case Ability.Medic: return new DefaultAbility();
                case Ability.MoralBoost: return new MoralBoostAbility();
                case Ability.Muster: return new DefaultAbility();
                case Ability.Spy: return new DefaultAbility();
                case Ability.TightBond: return new DefaultAbility();
                case Ability.None: return new DefaultAbility();
                case Ability.Frost: return new FrostAbility();
                case Ability.Fog: return new FogAbility();
                case Ability.Rain: return new RainAbility();
                case Ability.CommandersHorn: return new CommandersHornAbility();
                case Ability.ClearWeather: return new DefaultAbility();
                case Ability.Decoy: return new DefaultAbility();
                case Ability.Scorch: return new DefaultAbility();
                case Ability.Leader: return new DefaultAbility();
                default: throw new Exception("Unkown ability");
            }
        }
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
        None = 0,
        Hand = 1,
        ComandersHornCatapult = 2,
        Catapult = 4,
        ComandersHornSword = 8,
        Sword = 16,
        ComandersHornArchery = 32,
        Archery = 64,
        Discard = 128,
        Weather = 256,
        Dead = 512
    }

    public enum PlayerKind
    {
        Player,
        Opponent,
    }

    public class CardInPlay
    {
        private static int _sequence = 0;

        public readonly int Number;

        public PlayerKind Player;
        public int Sequence;
        public Location Location;

        public CardInPlay(int number, Location location, PlayerKind player, int? sequence = null)
        {
            Number = number;
            Sequence = sequence.GetValueOrDefault(++_sequence);
            Location = location;
            Player = player;
        }

        public override string ToString() => $"#{Number} {Location} {Player}";
    }

    public class Card
    {
        private CardInPlay _cardInPlay;
        private CardMetadata _metadata;

        public Card(CardInPlay cardInPlay, CardMetadata metadata)
        {
            _cardInPlay = cardInPlay;
            _metadata = metadata;
            PowerMultiplier = 1;
            Power = metadata.DefaultPower;
        }

        public int PowerMultiplier { get; set; }
        public int Power { get; set; }
        public int Number => _cardInPlay.Number;
        public int Sequence => _cardInPlay.Sequence;
        public Location Location => _cardInPlay.Location;
        public int EffectivePower => Power * PowerMultiplier;

        public string Name => _metadata.Name;
        public Ability Ability => _metadata.Ability;
        public int DefaultPower => _metadata.DefaultPower;
        public Location PossibleLocations => _metadata.PossibleLocations;
        public bool IsHero => _metadata.IsHero;
        public int Faction => _metadata.Faction;
        public CardMetadata Metadata => _metadata;
        public override string ToString() => $"#{Number} {Location} {EffectivePlayer}";

        public PlayerKind EffectivePlayer
        {
            get
            {
                if (Location == Location.Hand)
                {
                    return _cardInPlay.Player;
                }

                return _metadata.InOpponentZone ? _cardInPlay.Player.Reverse() : _cardInPlay.Player;
            }
        }
    }

    public class CardInfo
    {
        public CardInfo(int number, CardMetadata metadata, PlayerKind player, Location location)
        {
            Number = number;
            Metadata = metadata;
            Player = player;
            Location = location;
        }

        public CardMetadata Metadata;
        public PlayerKind Player;
        public int Number;
        public Location Location;
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
            var cardInPlay = new CardInPlay(cardNumber, Location.Hand, player, sequence);
            _currentState.CardsInPlay[cardInPlay.Number] = cardInPlay;
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
            var allCards = _currentState.CardsInPlay.Values.Select(c => new Card(c, _metadata[c.Number])).ToArray();

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


    public class Renderer
    {
        private class CardGameObject
        {
            public Card card;
            public object obj;
            public int Power;
        }

        private Dictionary<int, CardGameObject> _cache = new();

        public void Render()
        {
            var gameState = new GameState();
            gameState.NewGame(null);

            foreach (var cardInPlay in gameState.GetCards(PlayerKind.Player, Location.Hand))
            {
                if (_cache.TryGetValue(cardInPlay.Number, out var cachedCard))
                {
                    cachedCard.Power = cardInPlay.EffectivePower != cachedCard.Power ? cardInPlay.EffectivePower : cachedCard.Power;
                }
                else
                {
                    //Create and cache new card
                }
            }




        }
    }

}