using System;
using System.Collections.Generic;
using System.Linq;
using Phases;
using UnityEngine;

namespace GwentEngine.Phases
{
    public class ChooseDeckPhase : GamePhase
    {
        private DeckBuilderManager _deckBuilderManager;

        
        private Dictionary<int, GameObject> _highlights;
        private List<int> _deckCards;
        private List<int> _unselectedCards;
        
        public ChooseDeckPhase(GameState gameState, Action onActivatePhase, Action onEndPhase)
            : base(onActivatePhase, onEndPhase)
        {
            _deckBuilderManager = GameObject.Find("DeckBuilderManager").GetComponent<DeckBuilderManager>();
        }
        
        public override void Activate()
        {
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
