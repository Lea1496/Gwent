using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GwentEngine
{
    public class DeckBuilderManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI factionName;
        [SerializeField] private TextMeshProUGUI cardsDeck;
        [SerializeField] private TextMeshProUGUI unitCards;
        [SerializeField] private TextMeshProUGUI specialCards;
        [SerializeField] private TextMeshProUGUI cardsStrength;
        [SerializeField] private TextMeshProUGUI heroCards;
        [SerializeField] public GameObject CardHighlight;

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
        private int m_currentFaction;
        private int m_nFactions;

        private DeckState m_deckState;

        private bool m_bDeckModified;

        private GameManager m_gameManager;

        private GameObject[] m_vZones;
        private GameObject[] m_vZonesDeck;

        private void Awake()
        {
            factionNames = new List<string>() { "Northern Realms", "Nilfgaard", "Scotiatël", "Monster" };
            m_vZones = new GameObject[] { zone1, zone2, zone3, zone4, zone5, zone6, zone7, zone8, zone9, zone10, zone11, zone12 };
            m_vZonesDeck = new GameObject[] { zoneDeck1, zoneDeck2, zoneDeck3, zoneDeck4, zoneDeck5, zoneDeck6, zoneDeck7, zoneDeck8, zoneDeck9, zoneDeck10, zoneDeck11, zoneDeck12 };
            m_currentFaction = 0;
            m_nFactions = 4;
            m_bDeckModified = false;

            m_deckState = new();
            
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            BuildCards();
           // for(int i = 0; i < 4; i++)
           //     m_deckState.SaveDeck(i);
        }
       public void GenerateCards(Card[] allCards, GameObject[] zones)
       {
           var cardGameObjects = m_gameManager.GenerateCardGameObjects(allCards, PlayerKind.Player /* À REVOIR */, Location.None);
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
            
            var allCardsSelected = m_deckState.m_boardStates[m_currentFaction].CardsInPlay
                                           .Values
                                           .Where(card => card.IsSelected)
                                           .Select(c => new Card(c))
                                           .ToArray();
            var allCardsUnselected = m_deckState.m_boardStates[m_currentFaction].CardsInPlay
                .Values
                .Where(card => !card.IsSelected)
                .Select(c => new Card(c))
                .ToArray();

            if(allCardsSelected.Length != 0) GenerateCards(allCardsSelected, m_vZonesDeck);
            if(allCardsUnselected.Length != 0) GenerateCards(allCardsUnselected, m_vZones);
            
        }
        public void ChangeFaction(bool bNext)
        {
            m_currentFaction = bNext ? (m_currentFaction + 1) % m_nFactions : (m_currentFaction - 1) % m_nFactions;
            factionName.text = factionNames[m_currentFaction];

            m_bDeckModified = true;
        }
        public void Update()
        {
            if (m_bDeckModified)
            {
                CalculateTotalCards();
                CalculateTotalUnitCards();
                CalculateTotalSpecialCards();
                CalculateTotalStrength();
                CalculateTotalHeroCards();
            }
        }

        public void StartGame()
        {
            m_deckState.SaveDeck(m_currentFaction);

            m_gameManager.CurrentGamePhase.EndCurrentPhase();
        }

        private void CalculateTotalCards()
        {
            cardsDeck.text = m_deckState.m_boardStates[m_currentFaction].CardsInPlay.Count.ToString();
        }

        private void CalculateTotalUnitCards()
        {
            unitCards.text = m_deckState.m_boardStates[m_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.DefaultPower != -1).ToString();
        }

        private void CalculateTotalSpecialCards()
        {
            var value = m_deckState.m_boardStates[m_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.DefaultPower == -1);
            specialCards.text = $"{value}/10";
        }

        private void CalculateTotalStrength()
        {
            cardsStrength.text = heroCards.text = m_deckState.m_boardStates[m_currentFaction].CardsInPlay
                .Where(kv => kv.Value.Metadata.DefaultPower != -1).Sum(kv => kv.Value.Metadata.DefaultPower).ToString();
        }

        private void CalculateTotalHeroCards()
        {
            heroCards.text = m_deckState.m_boardStates[m_currentFaction].CardsInPlay
                .Count(kv => kv.Value.Metadata.IsHero ).ToString();
        }
    }
}