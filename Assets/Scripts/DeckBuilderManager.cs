using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GwentEngine.Phases;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GwentEngine
{
    public class DeckBuilderManager : MonoBehaviour, IManager
    {
        [SerializeField] private TextMeshProUGUI factionName;
        [SerializeField] private TextMeshProUGUI cardsDeck;
        [SerializeField] private TextMeshProUGUI unitCards;
        [SerializeField] private TextMeshProUGUI specialCards;
        [SerializeField] private TextMeshProUGUI cardsStrength;
        [SerializeField] private TextMeshProUGUI heroCards;
        [SerializeField] public GameObject CardHighlight;
        [SerializeField] public GameObject Card;

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

        private List<string> factionNames;
        private int _currentFaction;
        private int m_nFactions;

        private DeckState m_deckState;

        private bool m_bDeckModified;

        private GameObject[] m_vZones;
        private GameObject[] m_vZonesDeck;
        
        private Dictionary<int, GameObject> _highlights;

        private const int MINIMUM_CARDS = 22;
        private const int MAXIMUM_SPECIAL_CARDS = 10;

        public GamePhase CurrentGamePhase { get; set; }
        
        public BoardState CurrentDeck
        {
            get => m_deckState.BoardStates[_currentFaction];
        }

        private List<int> DeckCards
        {
            get => CurrentDeck.CardsInPlay.Where(kv => kv.Value.IsSelected).Select(kv => kv.Key).ToList();
        }
        
        private List<int> UnselectedCards
        {
            get => CurrentDeck.CardsInPlay.Where(kv => !kv.Value.IsSelected).Select(kv => kv.Key).ToList();
        }
        

        private void Awake()
        {
            factionNames = new List<string>() { "Northern Realms", "Nilfgaard", "Scotiatël", "Monster" };
            m_vZones = new GameObject[] { zone1, zone2, zone3, zone4, zone5, zone6, zone7, zone8, zone9, zone10, zone11, zone12 };
            m_vZonesDeck = new GameObject[] { zoneDeck1, zoneDeck2, zoneDeck3, zoneDeck4, zoneDeck5, zoneDeck6, zoneDeck7, zoneDeck8, zoneDeck9, zoneDeck10, zoneDeck11, zoneDeck12 };
            _currentFaction = 0;
            m_nFactions = 4;
            m_bDeckModified = false;

            m_deckState = new();

            _highlights = new();

            CurrentGamePhase = new ChooseDeckPhase(null, null, () =>
            {
                CardsManager.CardGameObjects.Clear();
                SceneManager.LoadScene(1);
            });
            
            CurrentGamePhase.Activate();

            BuildCards();
           // for(int i = 0; i < 4; i++)
           //     m_deckState.SaveDeck(i);
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
            
            GameObjectsDisposition.DistributeCenter(zones[index], cardsGO, 5);
            
       }

       private void BuildCards()
        {
            ClearRows();
            
            var allCardsSelected = m_deckState.BoardStates[_currentFaction].CardsInPlay
                                           .Values
                                           .Where(card => card.IsSelected)
                                           .Select(c => new Card(c)).OrderBy(c => c.Ability)
                                           .ToArray();
            var allCardsUnselected = m_deckState.BoardStates[_currentFaction].CardsInPlay
                .Values.Where(card => !card.IsSelected)
                .Select(c => new Card(c)).OrderBy(c => c.Ability)
                .ToArray();

            if(allCardsSelected.Length != 0) GenerateCards(allCardsSelected, m_vZonesDeck);
            if(allCardsUnselected.Length != 0) GenerateCards(allCardsUnselected, m_vZones);

            m_bDeckModified = true;
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
               _highlights.Add(number, Instantiate(CardHighlight, card.transform));
           }
       }
        public void ChangeFaction(bool bNext)
        {
            _currentFaction = bNext ? (_currentFaction + 1) % m_nFactions : (_currentFaction - 1) % m_nFactions;
            factionName.text = factionNames[_currentFaction];
            
            m_bDeckModified = true;
            
            BuildCards();
        }
        private void Update()
        {
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
                m_deckState.SaveDeck(_currentFaction);
                
                CurrentGamePhase.EndCurrentPhase();
              //  _gameManager.CurrentGamePhase.EndCurrentPhase();
            }
            else
            {
                // Todo : ajouter un message or smtg
            }
           
        }

        private bool CanStartGame()
        {
            return CalculateTotalCards() && CalculateTotalUnitCards() && CalculateTotalSpecialCards();
        }
        private bool CalculateTotalCards()
        {
            var value = m_deckState.BoardStates[_currentFaction].CardsInPlay.Values.Count(card => card.IsSelected);
            cardsDeck.text = value.ToString();
            return value >= MINIMUM_CARDS;
        }

        private bool CalculateTotalUnitCards()
        {
            var value = m_deckState.BoardStates[_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.DefaultPower != -1 && kv.Value.IsSelected);
            unitCards.text = value.ToString();

            return value >= MINIMUM_CARDS;
        }

        private bool CalculateTotalSpecialCards()
        {
            var value = m_deckState.BoardStates[_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.DefaultPower == -1 && kv.Value.IsSelected);
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
            var value = m_deckState.BoardStates[_currentFaction].CardsInPlay
                .Where(kv => kv.Value.Metadata.DefaultPower != -1 && kv.Value.IsSelected).Sum(kv => kv.Value.Metadata.DefaultPower);
            cardsStrength.text = value.ToString();
        }

        private void CalculateTotalHeroCards()
        {
            heroCards.text = m_deckState.BoardStates[_currentFaction].CardsInPlay
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
                
                //ClearRows();
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
            return Instantiate(Card);
        }
    }
    
    
}