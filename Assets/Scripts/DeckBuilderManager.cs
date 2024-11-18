using System;
using System.Collections.Generic;
using System.Linq;
using GwentEngine.Phases;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace GwentEngine
{
    public class DeckBuilderManager : NetworkBehaviour, IManager
    {
        [SerializeField] private TextMeshProUGUI factionName;
        [SerializeField] private TextMeshProUGUI cardsDeck;
        [SerializeField] private TextMeshProUGUI unitCards;
        [SerializeField] private TextMeshProUGUI specialCards;
        [SerializeField] private TextMeshProUGUI cardsStrength;
        [SerializeField] private TextMeshProUGUI heroCards;
        [SerializeField] public GameObject cardHighlight;
        [SerializeField] public GameObject card;
        [SerializeField] private GameObject leaderDescPanel;

        [SerializeField] private GameObject zone1;
        [SerializeField] private GameObject zone2;
        [SerializeField] private GameObject zone3;
        [SerializeField] private GameObject zone4;
        [SerializeField] private GameObject zone5;
        [SerializeField] private GameObject zone6;
        [SerializeField] private GameObject zone7;
        [SerializeField] private GameObject zone8;
        [SerializeField] private GameObject zone9;
        [SerializeField] private GameObject zone10;
        [SerializeField] private GameObject zone11;
        [SerializeField] private GameObject zone12;
        
        [SerializeField] private GameObject zoneDeck1;
        [SerializeField] private GameObject zoneDeck2;
        [SerializeField] private GameObject zoneDeck3;
        [SerializeField] private GameObject zoneDeck4;
        [SerializeField] private GameObject zoneDeck5;
        [SerializeField] private GameObject zoneDeck6;
        [SerializeField] private GameObject zoneDeck7;
        [SerializeField] private GameObject zoneDeck8;
        [SerializeField] private GameObject zoneDeck9;
        [SerializeField] private GameObject zoneDeck10;
        [SerializeField] private GameObject zoneDeck11;
        [SerializeField] private GameObject zoneDeck12;

        [SerializeField] private GameObject leaderCard;
        [SerializeField] private GameObject waiting;

        private List<string> factionNames;
        private int _currentFaction;
        private int m_nFactions;

        private DeckState _deckState;

        private bool m_bDeckModified;

        private GameObject[] m_vZones;
        private GameObject[] m_vZonesDeck;
        
        private Dictionary<int, GameObject> _highlights;

        private const int MINIMUM_CARDS = 22;
        private const int MAXIMUM_SPECIAL_CARDS = 10;

        private List<Card> _leaders;
        private int _selectedLeader;

        private CustSceneManager _sceneManager;
        

        private Dictionary<string, string> _leaderDesc = new Dictionary<string, string>
        {
            {"emhyr_var_emreis_emperor_of_nilfgaard_card", "Emperor of Nilfgaard: Look at 3 random cards from your opponent's hand."},
            {"emhyr_var_emreis_his_imperial_majesty_card","His Imperial Majesty: Pick a Torrential Rain card from your deck and play it instantly."},
            {"emhyr_var_emreis_invader_of_the_north_card","Invader of the North: Abilities that restore a unit to the battlefield restore a randomly-chosen unit. Affects both players."},
            {"emhyr_var_emreis_the_relentless_card","The Relentless: Draw a card from your opponent's discard pile."},
            {"emhyr_var_emreis_the_white_flame_card", "The White Flame: Cancel your opponent's Leader Ability."},
            {"foltest_lord_commander_of_the_north_card","Lord Commander of the North: Clear any weather effects (resulting from Biting Frost, Torrential Rain or Impenetrable Fog cards) in play."},
            {"foltest_king_of_temeria_card", "King of Temeria: Pick an Impenetrable Fog card from your deck and play it instantly."},
            {"foltest_the_siegemaster_card", "The Siegemaster: Doubles the strength of all your Siege units (unless a Commander's Horn is also present on that row)."},
            {"foltest_the_steel-forged_card", "The Steel Forged: Destroy your enemy's strongest Siege unit(s) if the combined strength of all his or her Siege units is 10 or more."},
            {"francesca_findabair_card", "The Beautiful: Doubles the strength of all your Ranged Combat units (unless a Commander's Horn is also present on that row)."},
            {"francesca_findabair_daisy_of_the_valley_card", "Daisy of the Valley: Draw an extra card at the beginning of the battle."},
            {"francesca_findabair_pureblood_elf_card", "Pureblood Elf: Pick a Biting Frost card from your deck and play it instantly."},
            {"francesca_findabair_queen_of_dol_blathanna_card", "Queen of Dol Blathanna: Destroy your enemy's strongest Close Combat unit(s) if the combined strength of all his or her Close Combat units is 10 or more."},
            {"eredin_bringer_of_death_card", "Bringer of Death: Restore a card from your discard pile to your hand."},
            {"eredin_destroyer_of_worlds_card", "Destroyer of Worlds: Discard 2 card and draw 1 card of your choice from your deck."},
            {"eredin_king_of_wild_hunt_card","King of the Wild Hunt: Pick any weather card from your deck and play it instantly."},
            {"eredin_commander_of_the_red_riders_card", "Commander of the Red Riders: Double the strength of all your Close Combat units (unless a Commander's horn is also present on that row)."}
        };
        

        public GamePhase CurrentGamePhase { get; set; }
        
        public BoardState CurrentDeck
        {
            get => _deckState.BoardStates[_currentFaction];
        }

        private List<int> DeckCards
        {
            get => CurrentDeck.CardsInPlay.Where(kv => kv.Value.IsSelected).Select(kv => kv.Key).ToList();
        }
        
        private List<int> UnselectedCards
        {
            get => CurrentDeck.CardsInPlay.Where(kv => !kv.Value.IsSelected).Select(kv => kv.Key).ToList();
        }
        

        public void MyAwake()
        {
            factionNames = new List<string>() { "Northern Realms", "Nilfgaard", "Scotiatël", "Monster" };
            m_vZones = new GameObject[] { zone1, zone2, zone3, zone4, zone5, zone6, zone7, zone8, zone9, zone10, zone11, zone12 };
            m_vZonesDeck = new GameObject[] { zoneDeck1, zoneDeck2, zoneDeck3, zoneDeck4, zoneDeck5, zoneDeck6, zoneDeck7, zoneDeck8, zoneDeck9, zoneDeck10, zoneDeck11, zoneDeck12 };
            _currentFaction = 0;
            m_nFactions = 4;
            m_bDeckModified = false;

            _deckState = new DeckState();
            _deckState.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DeckState.BoardStates))
                {
                    Debug.Break();
                    Console.WriteLine($"MyValue changed to: {_deckState.BoardStates}");
                }
            };

            _sceneManager = FindObjectOfType<CustSceneManager>();
            _highlights = new();

            _leaders = new List<Card>();
            _selectedLeader = 0;
            
            CurrentGamePhase = new ChooseDeckPhase(null, null, () =>
            {
                CardsManager.CardGameObjects.Clear();
                SceneManager.LoadScene(1);
            });
            
            CurrentGamePhase.Activate();

            BuildCards();
           //for(int i = 0; i < 4; i++)
            //    m_deckState.SaveDeck(i);
            int test = 0;
            
        }

        public void GenerateCards(Card[] allCards, GameObject[] zones)
       {
           var cardGameObjects = CardsManager.GenerateCardGameObjects(allCards, PlayerKind.Player /* À REVOIR */, Location.None, this);
            int index = 0;
            int cardNum = 0;
            bool isStart = true;
            GameObject[] cardsGO = new GameObject[6];

            foreach(var cardGameObject in cardGameObjects)
            {
                if (cardNum % 6 == 0 && !isStart)
                {
                    GameObjectsDisposition.DistributeCenter(zones[index++], cardsGO, 5);
                }

                cardsGO[cardNum++ % 6] = cardGameObject.gameObject;
                
                isStart = false;
                
                if (index == 12)
                    return;

                var zone = zones[index].transform;
                
                cardGameObject.gameObject.GetComponent<CardBehavior>().SetInfo(cardGameObject.card);
                
                if (cardGameObject.gameObject.transform.parent != zone)
                {
                    cardGameObject.gameObject.transform.localPosition = Vector3.zero;
                    cardGameObject.gameObject.transform.SetParent(zone, true);
                }
            }

            int numCards = cardNum % 6;
            if(numCards != 0)
                Array.Resize(ref cardsGO, numCards);
            if(cardGameObjects.Length != 0)
                GameObjectsDisposition.DistributeCenter(zones[index], cardsGO, 5);
            
       }

       private void BuildCards()
        {
            ClearRows();
            
            var allCardsSelected = _deckState.BoardStates[_currentFaction].CardsInPlay
                                           .Values
                                           .Where(card => card.IsSelected)
                                           .Select(c => new Card(c)).OrderBy(c => c.Ability)
                                           .ToArray();
            var allCardsUnselected = _deckState.BoardStates[_currentFaction].CardsInPlay
                .Values.Where(card => !card.IsSelected)
                .Select(c => new Card(c)).OrderBy(c => c.Ability)
                .ToArray();
            
            if(allCardsSelected.Length != 0) GenerateCards(allCardsSelected.Where(card => card.Number <= 180).ToArray(), m_vZonesDeck);
            if(allCardsUnselected.Length != 0) GenerateCards(allCardsUnselected.Where(card => card.Number <= 180).ToArray(), m_vZones);

            if (_selectedLeader == 0)
            {
                _leaders = allCardsSelected.Length != 0 ? allCardsSelected.Where(card => card.Number > 180).ToList() : new List<Card>();
                
                var leaders = allCardsUnselected.Where(card => card.Number > 180).ToArray();
                foreach (var card in leaders)
                {
                    _leaders.Add(card);
                }

                CurrentDeck.CardsInPlay[_leaders[0].Number].IsSelected = true;
                CardsManager.CreateNewCard(_leaders[_selectedLeader], leaderCard);
            }
            

            m_bDeckModified = true;
        }

       public void ChangeLeader(bool goBack)
       {
           CurrentDeck.CardsInPlay[_leaders[_selectedLeader].Number].IsSelected = false;
           _selectedLeader = goBack
               ? (_selectedLeader - 1 < 0 ? _leaders.Count - 1 : _selectedLeader - 1)
               : (_selectedLeader + 1) % _leaders.Count;
           
           CurrentDeck.CardsInPlay[_leaders[_selectedLeader].Number].IsSelected = true;
           CardsManager.CreateNewCard(_leaders[_selectedLeader], leaderCard);
       }

       public void ManageHighlights(int number, GameObject card)
       {
           if (_highlights.ContainsKey(number) && _highlights[number] != null)
           {
               Destroy(_highlights[number].gameObject);
               _highlights.Remove(number);
           }
           else if (_highlights.Count == 0 ||
                    DeckCards.Contains(number) &&
                    DeckCards.Contains(_highlights.First().Key)
               || UnselectedCards.Contains(number) &&
                    UnselectedCards.Contains(_highlights.First().Key))
           {
               _highlights.Add(number, Instantiate(cardHighlight, card.transform));
           }
       }
        public void ChangeFaction(bool bNext)
        {
            _currentFaction = bNext ? (_currentFaction + 1) % m_nFactions : ((_currentFaction - 1) < 0 ? _currentFaction - 1 + m_nFactions : _currentFaction - 1) ;
            factionName.text = factionNames[_currentFaction];
            
            m_bDeckModified = true;

            _selectedLeader = 0;
            
            BuildCards();
        }
        
        private void Update()
        {

            if (IsClient && _deckState.BoardStates[0].CardsInPlay.Count == 0)
                _deckState = new DeckState();

            if (m_bDeckModified)
            {
                CalculateTotalCards();
                CalculateTotalUnitCards();
                CalculateTotalSpecialCards();
                CalculateTotalStrength();
                CalculateTotalHeroCards();
                
                m_bDeckModified = false;
            }
        }

        public void StartGame()
        {
            if (CanStartGame())
            {
                _sceneManager.OnClientReadyStateChanged(true);
                
                _deckState.SaveDeck(_currentFaction);
                CurrentGamePhase.EndCurrentPhase();
                
                if (_sceneManager.ArePlayersReady())
                {
                    _sceneManager.LoadSceneSingleOnDemandServerRpc();
                }
                else
                {
                    waiting.SetActive(true);
                }
                
            }
            else
            {
                // Todo : ajouter un message or smtg
            }
           
        }

        private bool CanStartGame()
        {
            Debug.Log(CalculateTotalCards());
            Debug.Log(CalculateTotalUnitCards());
            Debug.Log(CalculateTotalSpecialCards());
            
            return CalculateTotalCards() && CalculateTotalUnitCards() && CalculateTotalSpecialCards();
        }
        private bool CalculateTotalCards()
        {
            var value = _deckState.BoardStates[_currentFaction].CardsInPlay.Values.Count(card => card.IsSelected) - 1;
            cardsDeck.text = value.ToString();
            return value >= MINIMUM_CARDS;
        }

        private bool CalculateTotalUnitCards()
        {
            var value = _deckState.BoardStates[_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.DefaultPower != -1 && kv.Value.IsSelected);
            unitCards.text = value.ToString();

            return value >= MINIMUM_CARDS;
        }

        private bool CalculateTotalSpecialCards()
        {
            var value = _deckState.BoardStates[_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.DefaultPower == -1 && kv.Value.IsSelected) -1;
            specialCards.text = $"{value}/10";

            if (value > MAXIMUM_SPECIAL_CARDS)
            {
                specialCards.color = Color.red;
                return false;
            }
            else
                specialCards.color = Color.white;
            
            return true;
        }

        private void CalculateTotalStrength()
        {
            var value = _deckState.BoardStates[_currentFaction].CardsInPlay
                .Where(kv => kv.Value.Metadata.DefaultPower != -1 && kv.Value.IsSelected).Sum(kv => kv.Value.Metadata.DefaultPower);
            cardsStrength.text = value.ToString();
        }

        private void CalculateTotalHeroCards()
        {
            heroCards.text = _deckState.BoardStates[_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.IsHero && kv.Value.IsSelected).ToString();
        }

        public void TransferCards()
        {
            if (_highlights.Count != 0)
            {
                m_bDeckModified = true;

                foreach (var card in _highlights)
                {
                    var cardNumber = card.Key;
                    Destroy(card.Value);

                    CurrentDeck.CardsInPlay[cardNumber].IsSelected = !CurrentDeck.CardsInPlay[cardNumber].IsSelected;

                    
                }
                _highlights.Clear();
                
                BuildCards();
            }
            
        }

        private void ClearRows()
        {
            foreach (var row in m_vZones)
            {
                foreach (Transform children in row.transform)
                {
                    var cb = children.gameObject.GetComponent<CardBehavior>();
                    CardsManager.UdateCardGameObjects(cb.Card.Number);

                    Destroy(children.gameObject);
                }
            }
            
            foreach (var row in m_vZonesDeck)
            {
                foreach (Transform children in row.transform)
                {
                    var cb = children.gameObject.GetComponent<CardBehavior>();
                    CardsManager.UdateCardGameObjects(cb.Card.Number);
                    
                    Destroy(children.gameObject);
                }
            }
        }

        public void OnClick(int number, GameObject card)
        {
            CurrentGamePhase.OnClick(number, card);
        }

        public GameObject InstantiateCard()
        {
            return Instantiate(card);
        }

        public void DisplayLeaderDesc()
        {
            string desc = _leaders != null ? _leaderDesc[_leaders[_selectedLeader].Name] : "";
            leaderDescPanel.SetActive(true);
            leaderDescPanel.GetComponentInChildren<TextMeshProUGUI>().text = desc;
        }

        public void HideLeaderDesc()
        {
            leaderDescPanel.SetActive(false);
        }
    }
    
    
}