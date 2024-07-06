using System;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    public class RainAbility : CardAbility //SpecificLocationAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.RemoveClearWeatherCards();
                gameManager.SetRowAction(Location.Catapult, PlayerKind.Opponent, ActionKind.Weather);
                gameManager.SetRowAction(Location.Catapult, PlayerKind.Player, ActionKind.Weather);

                gameManager.EndCurrentPhase();
            });
        }
        //public RainAbility() : base(Location.Catapult) { }
    }
}
