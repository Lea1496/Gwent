using System;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    public class FrostAbility : CardAbility //SpecificLocationAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.RemoveClearWeatherCards();
                gameManager.SetRowAction(Location.Sword, PlayerKind.Opponent, ActionKind.Weather);
                gameManager.SetRowAction(Location.Sword, PlayerKind.Player, ActionKind.Weather);

                gameManager.EndCurrentPhase();
            });
        }
        //public FrostAbility() : base(Location.Sword) { }
    }
}
