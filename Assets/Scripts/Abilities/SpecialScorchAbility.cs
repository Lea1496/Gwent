using System;
using System.Collections.Generic;
using Assets.Scripts.Phases;
using GwentEngine.Phases;
using System.Linq;


namespace GwentEngine.Abilities
{
    public class SpecialScorchAbility : CardAbility
    { 
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                PlayerKind opponentPlayer;

                if (gameManager.CurrentPlayer == PlayerKind.Opponent)
                    opponentPlayer = PlayerKind.Player;
                else
                    opponentPlayer = PlayerKind.Opponent;
                
                var cardsInRow = gameManager.GetCards(opponentPlayer, cardInPlay.Location).Where(card => !card.IsHero).ToArray();

                int powerSum = cardsInRow.Sum(card => card.EffectivePower); // I think that works...
                
                if (powerSum < 10)
                    gameManager.EndCurrentPhase();
                
                var highestPower = cardsInRow.Max(card => card.EffectivePower);

                // Filter cards that have the highest power
                var highestCards = cardsInRow.Where(card => card.EffectivePower == highestPower).ToList();

                // Remove cards from highestCards if a new higher power is found
                var cardsToRemove = highestCards.Where(card => card.EffectivePower < highestPower).ToList();
                
                highestCards = highestCards.Except(cardsToRemove).ToList();

                highestCards.ForEach(card => gameManager.RemoveCard(card.Number, false));

               gameManager.EndCurrentPhase();
            });
        }
    }
}
