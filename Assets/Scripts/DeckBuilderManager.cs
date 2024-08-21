using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
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

        private List<string> factionNames;
        private int m_currentFaction;
        private int m_nFactions;

        private DeckState m_deckState;

        private bool m_bDeckModified;

        private GameManager m_gameManager;

        private void Awake()
        {
            factionNames = new List<string>() { "Northern Realms", "Nilfgaard", "Scotiatël", "Monster" };
            m_currentFaction = 0;
            m_nFactions = 4;
            m_bDeckModified = false;

            m_deckState = new();
            
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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