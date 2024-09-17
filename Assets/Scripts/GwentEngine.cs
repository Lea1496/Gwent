using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using GwentEngine.Abilities;
using Random = System.Random;
using System.Collections.Concurrent;
using GwentEngine.Phases;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

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
        None = 0b0000000000, //0
        Hand = 0b0000000001, //1
        ComandersHornCatapult = 0b0000000010, //2
        Catapult = 0b0000000100, //4
        ComandersHornSword = 0b0000001000, //8
        Sword = 0b0000010000, //16
        ComandersHornArchery = 0b0000100000, //32
        Archery = 0b0001000000, //64
        Discard = 0b0010000000, //128
        Weather = 0b0100000000, //256
        Dead = 0b1000000000, //512
        Leader = 0b10000000000 //1024
    }
    [Flags]
    public enum ActionKind
    {
        None = 0,
        CommandersHorn = 1,
        MoralBoost = 2,
        Weather = 4
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
        public bool IsSelected;

        public CardInPlay(CardMetadata metadata, Location location, PlayerKind player, int? sequence = null, bool isSelected = false)
        {
            Metadata = metadata;
            Sequence = sequence.GetValueOrDefault(++_sequence);
            Location = location;
            Player = player;
            IsSelected = isSelected;
        }

        public override string ToString() => $"#{Metadata} {Location} {Player}"; // pas certaine
    }

    public class Card
    {
        private CardInPlay _cardInPlay;

        private List<int> _powerMultiplierSources;

        public void SetAction(ActionKind action)
        {
            //if ((action & ActionKind.CommandersHorn) != 0 && (action & ActionKind.MoralBoost) != 0 && (action & ActionKind.Weather) != 0)
            //    Action = action;
            //else
            Action = action;
        }
        

        public Card(CardInPlay cardInPlay)
        {
            _cardInPlay = cardInPlay;
            _powerMultiplierSources = new();
            Power = _cardInPlay.Metadata.DefaultPower;
            Action = ActionKind.None;
        }

        public void SetPowerMultiplier(int value)
        { 
            _powerMultiplierSources.Add(value);
           // _powerMultiplierSources.AddOrUpdate(name, getNewValue(1), (key, currentValue) => getNewValue(currentValue)); 
        }

        public int Power { get; set; }
        public int Number => _cardInPlay.Metadata.Number;
        public int Sequence => _cardInPlay.Sequence;
        public Location Location => _cardInPlay.Location;
        public string Name => _cardInPlay.Metadata.Name;
        public Ability Ability => _cardInPlay.Metadata.Ability;
        public int DefaultPower => _cardInPlay.Metadata.DefaultPower;
        public ActionKind Action { get; private set; }
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

        public int PowerMultiplier
       {
            get
            {
                var multiplier = 1;
               // _powerMultiplierSources.ForEach(v => multiplier *= v);
                for (int i = 0; i < _powerMultiplierSources.Count(); i++)
                {
                    if (i == 0)
                        continue;
                    multiplier *= _powerMultiplierSources[i];
                }
                return multiplier;
            }
        }

        public int EffectivePower
        {
            get
            {
                if (Location == Location.Hand || IsHero)
                    return DefaultPower;
                var power = DefaultPower;
                if ((Action & ActionKind.Weather) == ActionKind.Weather)
                {
                    power = 1;
                }
                
                power *= PowerMultiplier;
                if (Action == ActionKind.None)
                    return power;
                
                if ((Action & ActionKind.MoralBoost) == ActionKind.MoralBoost)
                {
                    if(Ability != Ability.MoralBoost)
                        power++;
                }
                if ((Action & ActionKind.CommandersHorn) == ActionKind.CommandersHorn)
                {
                    if(Ability != Ability.CommandersHorn)
                        power *= 2;
                }

                Power = power;
                return power;
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
            (new[] { Ability.Decoy, Ability.Scorch/*hmm pas sure de sccorch*/, Ability.Leader }).Contains(ability);

        public static PlayerKind Reverse(this PlayerKind playerKind) =>
            playerKind == PlayerKind.Opponent ? PlayerKind.Player : PlayerKind.Opponent;
    }

    public class BoardState
    {
        public Dictionary<int, CardInPlay> CardsInPlay { get; set; } = new();
        
        public static Dictionary<int, CardInPlay> FromFile(string fullPath)
        {
            var json = File.ReadAllText(fullPath);
            return FromJson(json);
        }

        public static Dictionary<int, CardInPlay> FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<int, CardInPlay>>(json);
        }
    }

    public class GameState
    {
        private BoardState _currentState = new();
        private Dictionary<int, CardMetadata> _metadata = new();
        private List<int> _availableCards = new();
        private Random _random = new Random(DateTime.Now.Millisecond);
        private Card[] _allCards;
        private bool _isLeaderPlayed;
        private bool _isLeaderPlayedEnemy;

        public static Dictionary<int, ActionKind> ActionDict;
        public PlayerKind FirstPlayer { get; private set; }
        public PlayerKind CurrentPlayer { get; set; }

        public Dictionary<Tuple<Location, PlayerKind>, ActionKind> RowMultipliers { get; private set; }

        public void NewGame(Dictionary<int, CardMetadata> metadata, int initialCardCount = 0)
        {
            _metadata = metadata;
            _availableCards = _metadata.Keys.ToList();
            _currentState = new();

            ActionDict = new Dictionary<int, ActionKind>();

            Enumerable.Range(0, initialCardCount).ForEach(i =>
            {
                Draw(PlayerKind.Player);
                Draw(PlayerKind.Opponent);
            });

            FirstPlayer = _random.Next(0, 1) == 0 ? PlayerKind.Player : PlayerKind.Opponent;
            CurrentPlayer = FirstPlayer;

            RowMultipliers = new Dictionary<Tuple<Location, PlayerKind>, ActionKind>()
            {
                { new Tuple<Location, PlayerKind>(Location.Sword, PlayerKind.Player), ActionKind.None },
                { new Tuple<Location, PlayerKind>(Location.Archery, PlayerKind.Player), ActionKind.None },
                { new Tuple<Location, PlayerKind>(Location.Catapult, PlayerKind.Player), ActionKind.None },
                { new Tuple<Location, PlayerKind>(Location.Sword, PlayerKind.Opponent), ActionKind.None },
                { new Tuple<Location, PlayerKind>(Location.Archery, PlayerKind.Opponent), ActionKind.None },
                { new Tuple<Location, PlayerKind>(Location.Catapult, PlayerKind.Opponent), ActionKind.None }
            };
            
            _allCards = null;
            
            _isLeaderPlayedEnemy = false;
            _isLeaderPlayed = false;
        }

        private void Draw(PlayerKind player, int? sequence = null)
        {
            var index = _random.Next(0, _availableCards.Count - 1);
            var cardNumber = _availableCards[index];

            UseCard(cardNumber, player, sequence);
        }

        public void ExecuteSpy(PlayerKind player, int? sequence = null)
        {
            Draw(player, sequence);
            Draw(player, sequence);
        }

        public void ChangeCard(int cardNumber)
        {
            var currentCard = _currentState.CardsInPlay[cardNumber];
            Draw(currentCard.Player, currentCard.Sequence);
            UseCard(181, currentCard.Player);
            //UseCard(17, currentCard.Player);
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

        public void RemoveAllWeatherCards()
        {
            AllCards
                .Where(card => !_availableCards.Contains(card.Number) && card.Location == Location.Weather && card.Ability != Ability.ClearWeather)
                .ToList()
                .ForEach(card => RemoveCard(card.Number, false));
        }
        public void RemoveClearWeatherCards()
        {
            AllCards
                .Where(card => !_availableCards.Contains(card.Number) && card.Location == Location.Weather && card.Ability == Ability.ClearWeather)
                .ToList()
                .ForEach(card => RemoveCard(card.Number, false));
        }

        public bool SwitchCardFromBoard(int cardFromHand, int cardOnBoard, PlayerKind player, int? sequence = null)
        {
            if (_currentState.CardsInPlay[cardOnBoard].Location == Location.Hand ||
                _currentState.CardsInPlay[cardOnBoard].Player != player
                || _currentState.CardsInPlay[cardOnBoard].Metadata.IsHero
                || _currentState.CardsInPlay[cardOnBoard].Metadata.DefaultPower == -1)
                return false;
            
            _currentState.CardsInPlay[cardOnBoard].Location = Location.Hand;
            _availableCards.Remove(cardFromHand);
            RemoveCard(cardFromHand, false);
            return true;
        }

        public bool ReviveCard(int cardFromHand, int cardDiscard, PlayerKind player, int? sequence = null)
        {
            if (_currentState.CardsInPlay[cardDiscard].Location != Location.Discard ||
                _currentState.CardsInPlay[cardDiscard].Player != player)
                return false;

            _currentState.CardsInPlay[cardDiscard].Location =
                _currentState.CardsInPlay[cardDiscard].Metadata.PossibleLocations;
            _availableCards.Remove(cardFromHand);
            RemoveCard(cardFromHand, false);
            return true;
        }

        public Card[] ShowOponnentCards()
        {
            PlayerKind opPlayer = CurrentPlayer == PlayerKind.Opponent ? PlayerKind.Player : PlayerKind.Opponent;

            var opponentsCards = AllCards.Where(card => card.EffectivePlayer == opPlayer && card.Number <= 180).ToArray();

            Card[] cardsToShow = new Card[3];
            
            int index = 0;
            while(index < 3)
            {
                Random ran = new Random(DateTime.Now.Millisecond);

                var pos = ran.Next(0, opponentsCards.Length);
                if (cardsToShow.Contains(opponentsCards[pos]))
                    continue;

                cardsToShow[index++] = opponentsCards[pos];
            }

            return cardsToShow;
        }

        public bool IsCardDecoy(int number)
        {
            return _currentState.CardsInPlay[number].Metadata.Ability == Ability.Decoy;
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
            
            if (cardNumber > 280)
            {
                bool isCurrentLeaderPlayed = CurrentPlayer == PlayerKind.Opponent ? _isLeaderPlayedEnemy : _isLeaderPlayed;
                if (isCurrentLeaderPlayed)
                    return false;
            }

            return true;
        }


        public CardInPlay Play(int cardNumber, Location location)
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

            return cardInPlay;
        }

        public void RemoveCard(int cardNumber, bool isRecyclable)
        {
            var cardMetadata = _metadata[cardNumber];
            
            if (!_currentState.CardsInPlay.TryGetValue(cardNumber, out var cardInPlay))
            {
                throw new Exception($"Cannot play a card not drawn yet");
            }

            if (cardMetadata.Ability == Ability.Fog)
            {
                RemoveRowActionBothSides(Location.Archery, ActionKind.Weather);
            }
            else if (cardMetadata.Ability == Ability.Frost)
            {
                RemoveRowActionBothSides(Location.Sword, ActionKind.Weather);
            }
            else if (cardMetadata.Ability == Ability.Rain)
            {
                RemoveRowActionBothSides(Location.Catapult, ActionKind.Weather);
            }
            else if (cardMetadata.Ability == Ability.CommandersHorn)
            {
                RemoveRowAction(_currentState.CardsInPlay[cardMetadata.Number].Location, CurrentPlayer, ActionKind.CommandersHorn);
            }
            else if (cardMetadata.Ability == Ability.MoralBoost)
            {
                RemoveRowAction(_currentState.CardsInPlay[cardMetadata.Number].Location, CurrentPlayer, ActionKind.MoralBoost);
            }
            
            if (isRecyclable)
            {
                _currentState.CardsInPlay.Remove(cardNumber);
                _availableCards.Add(cardNumber);
            }
            else
            {
                if (!cardMetadata.IsHero && cardMetadata.DefaultPower != -1)
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

        public void SetRowAction(Location location, PlayerKind player, ActionKind action)
        {
            if (RowMultipliers[new Tuple<Location, PlayerKind>(location, player)] == ActionKind.None)
                RowMultipliers[new Tuple<Location, PlayerKind>(location, player)] = action;
            else
                RowMultipliers[new Tuple<Location, PlayerKind>(location, player)] |= action;
        }

        public void RemoveRowAction(Location location, PlayerKind player, ActionKind action)
        {
            RowMultipliers[new Tuple<Location, PlayerKind>(location, player)] &= ~action;
        }
        
        public void RemoveRowActionBothSides(Location location, ActionKind action)
        {
            RemoveRowAction(location, PlayerKind.Opponent, action);
            RemoveRowAction(location, PlayerKind.Player, action);
        }

        public CardMetadata[] AllAvailableCards
        {
            get
            {
                return _availableCards.Select(c => _metadata[c]).ToArray();
            }
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
            var abilityOrder = new[]
            {
                Ability.None, Ability.Muster, Ability.Medic, Ability.Spy, Ability.Fog, Ability.Rain, Ability.Frost,
                Ability.ClearWeather, Ability.TightBond, Ability.MoralBoost, Ability.CommandersHorn, Ability.Scorch
            };
            //var allCards = _currentState.CardsInPlay.Values.Select(c => new Card(c)).ToArray();
          
            var allCards = _currentState.CardsInPlay
                .Values
                .Select(c => new Card(c))
                .OrderBy(c => abilityOrder.IndexOf(a => a == c.Metadata.Ability))
                .ToArray();
            foreach (var cardX in allCards)
            {
                foreach (var cardY in allCards)
                {
                    cardX.Metadata.CardAbility.ApplyAbility(cardX, cardY, this);
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
        
        private static string ToJson(BoardState boardState) => JsonConvert.SerializeObject(boardState.CardsInPlay, Formatting.Indented);
        
        public static void WriteJsonToFile(string filePath, BoardState boardState)
        {
            string json = ToJson(boardState);
            
            File.WriteAllText(filePath, json);
        }

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

    public class DeckState
    {
        public List<BoardState> BoardStates { get; private set; }

        private string[] m_vPaths;

        public DeckState()
        {
            BoardState nrDeck = new();
            BoardState nilfgaardDeck = new();
            BoardState sociatelDeck = new();
            BoardState monsterDeck = new();
            BoardState skelligeDeck = new();

            m_vPaths = new[]
            {
                "Cards/NorthernRealmsDeck.json", "Cards/NilfgaardDeck.json",
                "Cards/ScotiatelDeck.json", "Cards/MonsterDeck.json"  /*" Cards/SkelligeDeck.json"*/
            };

            BoardStates = new List<BoardState>()
                { nrDeck, nilfgaardDeck, sociatelDeck, monsterDeck /*, skelligeDeck*/ };
            
            FillDecks();
        }
        
        private void FillDecks()
        {
            Action<int, string> fillDeck = (int index, string path) =>
            {
                BoardState bs = BoardStates[index];
                path = Path.Combine(Application.dataPath, path);
                
                //var deckFullPath = Path.Combine(Application.dataPath, "Cards", "Deck.json");

                Dictionary<int, CardInPlay> tempDict = BoardState.FromFile(path);
                
                
                foreach (var kvp in tempDict)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;

                    if (value.Metadata.Faction == index || value.Metadata.Faction == 4)
                        bs.CardsInPlay[key] = value;
                }
            };

            fillDeck(0, m_vPaths[0]);
            fillDeck(1, m_vPaths[1]);
            fillDeck(2, m_vPaths[2]);
            fillDeck(3, m_vPaths[3]);
           // fillDeck(4, "Assets/Cards/SkelligeDeck.json");
        }

        public void SaveDeck(int nDeckIndex)
        {
            string path = Path.Combine(Application.dataPath, m_vPaths[nDeckIndex]);
            GameState.WriteJsonToFile(path, BoardStates[nDeckIndex]);
        }
    }
}