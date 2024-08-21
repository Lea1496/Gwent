using System;
using Phases;
using UnityEngine;

namespace GwentEngine.Phases
{
    public class ChooseDeckPhase : GamePhase
    {
        public ChooseDeckPhase(GameState gameState, Action onActivatePhase, Action onEndPhase)
            : base(onActivatePhase, onEndPhase)
        {
        }
        public override void Activate()
        {
        }
        
        public override bool IsDraggable(Card card)
        {
            return false;
        }
        
        public override void OnClick(GameObject card)
        {
          
        }
        
       
    }
}
