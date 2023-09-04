using System;

namespace GwentEngine.Phases
{
    public class RoundPhase : GamePhase
    {
        public override bool IsDraggable(Card card)
        {
            return card.EffectivePlayer == PlayerKind.Player && card.Location == Location.Hand;
        }
    }
}
