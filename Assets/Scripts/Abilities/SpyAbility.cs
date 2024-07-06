using System;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    public class SpyAbility : CardAbility
    { 
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.ExecuteSpy();
                gameManager.EndCurrentPhase();
            });
        }
    }
}
