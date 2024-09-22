using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Phases;
using UnityEngine;

namespace GwentEngine.Phases
{
    public class ChooseDeckPhase : GamePhase
    {
        private  DeckBuilderManager _deckBuilderManager;

        
        private Dictionary<int, GameObject> _highlights;
        private List<int> _deckCards;
        private List<int> _unselectedCards;
        
        public ChooseDeckPhase(GameState gameState, Action onActivatePhase, Action onEndPhase)
            : base(onActivatePhase, onEndPhase)
        {
            _deckBuilderManager = _manager as DeckBuilderManager;
        }
        
        public override void Activate()
        {
        }

        public override void EndCurrentPhase()
        {
            Dictionary<int, CardMetadata> chosenDeck = new Dictionary<int, CardMetadata>();
            
            foreach (var keyValuePair in _deckBuilderManager.CurrentDeck.CardsInPlay)
            {
                if(keyValuePair.Value.IsSelected)
                    chosenDeck[keyValuePair.Key] = keyValuePair.Value.Metadata;
            }

            CardsManager.CardMetadata = chosenDeck;
            
            base.EndCurrentPhase();
        }
        
        public override bool IsDraggable(Card card)
        {
            return false;
        }
        
        public override void OnClick(int number, GameObject card)
        {
            _deckBuilderManager.ManageHighlights(number, card);
        }
        
        
        
       
    }
}
