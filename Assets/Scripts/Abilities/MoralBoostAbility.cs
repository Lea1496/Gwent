using System;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    public class MoralBoostAbility : SamePlayerCardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.SetRowAction(cardInPlay.Location, ActionKind.MoralBoost);

                gameManager.EndCurrentPhase();
            });
        }
    }
}
