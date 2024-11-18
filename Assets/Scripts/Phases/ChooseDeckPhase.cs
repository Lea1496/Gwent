using System;
using System.Collections.Generic;
using Unity.Netcode;
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

            //var deck = new NetworkVariable<Dictionary<int, CardMetadata>>(chosenDeck);
            if (NetworkManager.Singleton.LocalClientId == 0)
                CardsManager.RequestHostCardChange(chosenDeck);
            else
                CardsManager.RequestClientCardChange(chosenDeck);
            
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
