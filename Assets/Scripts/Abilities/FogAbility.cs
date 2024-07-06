using System;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    public class FogAbility : CardAbility //SpecificLocationAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.RemoveClearWeatherCards();
                gameManager.SetRowAction(Location.Archery, PlayerKind.Opponent, ActionKind.Weather);
                gameManager.SetRowAction(Location.Archery, PlayerKind.Player, ActionKind.Weather);

                gameManager.EndCurrentPhase();
            });
        }
        //public FogAbility() : base(Location.Archery) { }
    }
}
