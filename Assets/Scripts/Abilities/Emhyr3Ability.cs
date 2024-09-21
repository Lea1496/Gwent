using System;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine;
using GwentEngine.Abilities;
using GwentEngine.Phases;

namespace Abilities
{
    public class Emhyr3Ability : CardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.ReviveCards();
                gameManager.EndCurrentPhase();
            });
        }
    }
}