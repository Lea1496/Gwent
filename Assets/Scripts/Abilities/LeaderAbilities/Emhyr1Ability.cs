using Assets.Scripts.Phases;
using GwentEngine;
using GwentEngine.Abilities;
using GwentEngine.Phases;

namespace Abilities.LeaderAbilities
{
    public class Emhyr1Ability : CardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.ShowOpponentCards();
                gameManager.EndCurrentPhase();
            });
        }
    }
}