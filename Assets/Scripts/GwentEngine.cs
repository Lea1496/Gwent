using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Numerics;

namespace GwentEngine
{
    public static class Settings
    {
        public const int InitialCardCount = 10;
    }

    //TODO : Question: une abilité est composable ?
    public enum Ability
    {
        agile = 0,
        berserker = 1,
        mardroeme = 2,
        medic = 3,
        moralBoost = 4,
        muster = 5,
        spy = 6,
        tightBond = 7,
        none = 8,

        frost = 9,
        fog = 10,
        rain = 11,
        commandersHorn = 12,
        clearWeather = 13,

        //TODO : Question c'est des abileté? elle doivent aller la si oui
        decoy = 14,
        scorch = 15,
        leader = 16
    }

    public abstract class CardAbility
    {
        public abstract IEnumerable<CardInfo> Apply(CardMetadata cardMetadata, CardInPlay cardInPlay, GameState gameState);
    }

    public class DefaultAbility : CardAbility
    {
        public override IEnumerable<CardInfo> Apply(CardMetadata cardMetadata, CardInPlay cardInPlay, GameState gameState)
        {
            yield break;
        }
    }

    public class FrostAbility : CardAbility
    {
        public override IEnumerable<CardInfo> Apply(CardMetadata cardMetadata, CardInPlay cardInPlay, GameState gameState)
        {
            var cardsInOpponentSword = gameState.GetCards(PlayerKind.Opponent, Location.Sword);
            var cardsInPlayerSword = gameState.GetCards(PlayerKind.Player, Location.Sword);

            foreach (var cardInSwordLocation in cardsInOpponentSword.Union(cardsInPlayerSword))
            {
                if (!cardInSwordLocation.IsHero)
                {
                    cardInSwordLocation.ChangePower(1);
                    yield return new CardInfo() { Number = cardInSwordLocation.Number, Location = cardInSwordLocation.Location, Player = cardInSwordLocation.Player };
                }
            }

            yield break;
        }
    }

    public static class CardAbilityFactory
    {
        public static CardAbility Create(Ability ability)
        {
            switch (ability)
            {
                case Ability.agile: return new DefaultAbility();
                case Ability.berserker: return new DefaultAbility();
                case Ability.mardroeme: return new DefaultAbility();
                case Ability.medic: return new DefaultAbility();
                case Ability.moralBoost: return new DefaultAbility();
                case Ability.muster: return new DefaultAbility();
                case Ability.spy: return new DefaultAbility();
                case Ability.tightBond: return new DefaultAbility();
                case Ability.none: return new DefaultAbility();
                case Ability.frost: return new FrostAbility();
                case Ability.fog: return new DefaultAbility();
                case Ability.rain: return new DefaultAbility();
                case Ability.commandersHorn: return new DefaultAbility();
                case Ability.clearWeather: return new DefaultAbility();
                case Ability.decoy: return new DefaultAbility();
                case Ability.scorch: return new DefaultAbility();
                case Ability.leader: return new DefaultAbility();
                default: throw new Exception("Unkown ability");
            }
        }
    }


    public class CardMetadata
    {
        public string Name { get; set; }
        public Ability Ability { get; set; }
        public int DefaultPower { get; set; }
        public Location PossibleLocations { get; set; }
        public bool IsHero { get; set; }
        public int Number { get; set; }
        public int Faction { get; set; }
        public bool InOpponentZone { get; set; }
        public bool IsCommandersHorn { get; set; }

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
        Weather = 256
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

        public int Power { get; set; }

        public CardInPlay(int number, Location location, PlayerKind player, int power)
        {
            Number = number;
            Sequence = ++_sequence;
            Location = location;
            Player = player;
            Power = power;
        }

        public override string ToString() => $"#{Number} {Location} {Player}";
    }

    public class Card
    {
        private CardInPlay _cardInPlay;
        private CardMetadata _metadata;
        private int _powerMultiplier;

        public Card(CardInPlay cardInPlay, CardMetadata metadata, int powerMultiplier)
        {
            _cardInPlay = cardInPlay;
            _metadata = metadata;
            _powerMultiplier = powerMultiplier;
        }

        public int Number => _cardInPlay.Number;
        public int Sequence => _cardInPlay.Sequence;
        public Location Location => _cardInPlay.Location;
        public PlayerKind Player => _cardInPlay.Player;
        public int Power => _cardInPlay.Power * _powerMultiplier;

        public string Name => _metadata.Name;
        public Ability Ability => _metadata.Ability;
        public int DefaultPower => _metadata.DefaultPower;
        public Location PossibleLocations => _metadata.PossibleLocations;
        public bool IsHero => _metadata.IsHero;
        public int Faction => _metadata.Faction;

        public override string ToString() => $"#{Number} {Location} {Player}";

        public void ChangePower(int power) => _cardInPlay.Power = power;
    }

    public struct CardInfo
    {
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

        public static bool IsClickable(this Ability ability) => (new[] { Ability.decoy, Ability.scorch, Ability.leader }).Contains(ability);

        public static PlayerKind Reverse(this PlayerKind playerKind) => playerKind == PlayerKind.Opponent ? PlayerKind.Player : PlayerKind.Opponent;
    }

    public class BoardState
    {
        public Dictionary<int, CardInPlay> CardsInPlay { get; set; } = new();
    }

    public class GameState
    {
        private Stack<BoardState> _boardStates = new();
        private Dictionary<int, CardMetadata> _metadata = new();
        private List<int> _availableCards = new();
        private Random _random = new Random(DateTime.Now.Millisecond);
        private int _nbCardChanged = 0;
        private bool _isTimeToChangeCards = false;

        public bool IsTimeToChangeCards { get => _isTimeToChangeCards; }
        public BoardState CurrentState => _boardStates.Peek();

        public void NewGame(Dictionary<int, CardMetadata> metadata, int initialCardCount = 0)
        {
            _metadata = metadata;
            _availableCards = _metadata.Keys.ToList();
            _boardStates = new Stack<BoardState>();
            _boardStates.Push(new BoardState());

            Enumerable.Range(0, initialCardCount).ForEach(i =>
            {
                Draw(PlayerKind.Player);
                Draw(PlayerKind.Opponent);
            });
            _isTimeToChangeCards = true;
        }

        private void Draw(PlayerKind player)
        {
            var index = _random.Next(0, _availableCards.Count);
            var cardNumber = _availableCards[index];
            _availableCards.Remove(cardNumber);

            UseCard(cardNumber, player);
        }

        public void ChangeCard(int cardNumber)
        {
            _isTimeToChangeCards = _nbCardChanged >= 2 ? false : true;
            if (_isTimeToChangeCards)
            {
                _availableCards.Add(cardNumber);
                CurrentState.CardsInPlay.Remove(cardNumber);
                Draw(CurrentState.CardsInPlay[cardNumber].Player);
                _nbCardChanged++;
            }
        }


        public void UseCard(int cardNumber, PlayerKind player)
        {
            var cardMetadata = _metadata[cardNumber];
            var cardInPlay = new CardInPlay(cardNumber, Location.Hand, player, cardMetadata.DefaultPower);
            CurrentState.CardsInPlay[cardInPlay.Number] = cardInPlay;

            OnCardAdded(this, new CardInfo() { Number = cardNumber, Player = player, Location = Location.Hand });
        }

        public event EventHandler<CardInfo> OnCardAdded;
        public event EventHandler<CardInfo> OnCardChanged;

        public void Play(int cardNumber, Location location)
        {
            var cardMetadata = _metadata[cardNumber];

            if ((cardMetadata.PossibleLocations & location) == 0)
            {
                throw new Exception($"Location {location} is not playable");
            }

            if (!CurrentState.CardsInPlay.TryGetValue(cardNumber, out var cardInPlay))
            {
                throw new Exception($"Cannot play a card not drawn yet");
            }

            cardInPlay.Location = location;
            OnCardChanged(this, new CardInfo() { Number = cardNumber, Player = cardInPlay.Player, Location = location });

            ApplyCardAbility(cardMetadata, cardInPlay);
        }

        public void RemoveCard(int cardNumber, Location location)
        {
            var cardMetadata = _metadata[cardNumber];
            if (!CurrentState.CardsInPlay.TryGetValue(cardNumber, out var cardInPlay))
            {
                throw new Exception($"Cannot play a card not drawn yet");
            }

            if (!cardMetadata.IsHero || cardMetadata.DefaultPower == -1)
            {
                cardInPlay.Location = Location.Discard;
                OnCardChanged(this, new CardInfo() { Number = cardNumber, Player = cardInPlay.Player, Location = location });
            }
        }

        private void ApplyCardAbility(CardMetadata cardMetadata, CardInPlay cardInPlay)
        {
            var cardAbility = CardAbilityFactory.Create(cardMetadata.Ability);

            foreach (var cardInfo in cardAbility.Apply(cardMetadata, cardInPlay, this))
            {
                OnCardChanged(this, cardInfo);
            }
        }

        private static Dictionary<Location, Location> LocationWithCommandersHorn = new()
        {
            { Location.Catapult, Location.ComandersHornCatapult } ,
            { Location.Sword, Location.ComandersHornSword },
            { Location.Archery, Location.ComandersHornArchery}
        };
        private static Dictionary<Location, Location> LocationWithWeather = new()
        {
            { Location.Sword, Location.ComandersHornCatapult } ,
            { Location.Archery, Location.ComandersHornSword },
            { Location.Catapult, Location.ComandersHornArchery}
        };

        public Card[] GetCards(PlayerKind player, Location location)
        {
            var cardsInLocation = CurrentState.CardsInPlay.Values
                .Where(c => c.Location == location)
                .Select(c => Tuple.Create(_metadata[c.Number], c))
                .Where(c =>
                {
                    if (location == Location.Hand)
                    {
                        return c.Item2.Player == player;
                    }

                    var affectedPlayer = c.Item1.InOpponentZone ? c.Item2.Player.Reverse() : c.Item2.Player;
                    return affectedPlayer == player;

                })
                .ToArray();

            return cardsInLocation
                .Select(c => new Card(c.Item2, c.Item1, GetPowerMultiplier(cardsInLocation, player, location, c.Item1)))
                .OrderBy(c => c.Sequence)
                .ToArray();
        }

        private int GetPowerMultiplier(Tuple<CardMetadata, CardInPlay>[] cardsInLocation, PlayerKind player, Location location, CardMetadata metadata)
        {
            var isRowCommanderHornPresent = LocationWithCommandersHorn.TryGetValue(location, out var commandersHornLocation) && GetCards(player, commandersHornLocation).Length > 0;

            if (metadata.IsCommandersHorn)
            {
                return isRowCommanderHornPresent ? 2 : 1;
            }

            if (metadata.IsHero)
            {
                return 1;
            }

            var isCardWithCommandersHornPresentInLocation = cardsInLocation.Any(c => c.Item1.IsCommandersHorn);

            return isCardWithCommandersHornPresentInLocation ? 2 : 1;
        }

        public void Undo()
        {
            if (_boardStates.Count == 0)
            {
                throw new Exception("Cannot undo an empty board");
            }

            _boardStates.Pop();
        }

        public string ToJson() => JsonConvert.SerializeObject(_boardStates);

        public static GameState FromFile(Dictionary<int, CardMetadata> metadata, string fullPath) => FromJson(metadata, File.ReadAllText(fullPath));
        public static GameState FromJson(Dictionary<int, CardMetadata> metadata, string json) => new GameState() { _boardStates = JsonConvert.DeserializeObject<Stack<BoardState>>(json), _metadata = metadata };

        public string PrettyPrint()
        {
            return $"{CurrentState}";
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
                    cachedCard.Power = cardInPlay.Power != cachedCard.Power ? cardInPlay.Power : cachedCard.Power;
                }
                else
                {
                    //Create and cache new card
                }
            }




        }
    }

    public class UseAbility
    {
        public void UseFrost()
        {

        }

        public void UseFog()
        {

        }
        public void UseRain()
        {

        }
        public void UseMedic()
        {

        }
        public void UseMoralBoost()
        {

        }

        public void UseMuster()
        {

        }

        public void UseSpy()
        {

        }

        public void UseTightBond()
        {

        }

        public void UseDecoy()
        {

        }

        public void UseScorch()
        {

        }

        public void UseClearWeather()
        {

        }
    }
}


