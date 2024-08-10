using System;
using System.Collections.Generic;
using Assets.Scripts.Phases;
using GwentEngine.Phases;
using System.Linq;


namespace GwentEngine.Abilities
{
    public class ScorchAbility : CardAbility
    { 
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                List<Card> highestCards = new List<Card>();
                int highestPower = 0;
                
               foreach (Location location in Enum.GetValues(typeof(Location)))
               {
                   if (!(location == Location.Archery || location == Location.Catapult || location == Location.Sword))
                       continue;
                   
                   var localCards = gameManager.GetCards(PlayerKind.Player, location).Where(card => !card.IsHero).ToArray();
                   var opponentCards = gameManager.GetCards(PlayerKind.Opponent, location).Where(card => !card.IsHero).ToArray();
                    
                   
                   // local cards
                   if (localCards.Length != 0 && localCards.Max(card => card.EffectivePower) >= highestPower)
                   {
                       highestPower = localCards.Max(card => card.EffectivePower);

                       // Filter cards that have the highest power
                       highestCards = localCards.Where(card => card.EffectivePower == highestPower).ToList();

                       // Remove cards from highestCards if a new higher power is found
                       var cardsToRemove = highestCards.Where(card => card.EffectivePower < highestPower).ToList();
                       highestCards = highestCards.Except(cardsToRemove).ToList();
                   }
                   
                   //  opponent cards
                   if (opponentCards.Length != 0 && opponentCards.Max(card => card.EffectivePower) >= highestPower)
                   {
                       highestPower = opponentCards.Max(card => card.EffectivePower);

                       // Filter cards that have the highest power
                       highestCards = opponentCards.Where(card => card.EffectivePower == highestPower).ToList();

                       // Remove cards from highestCards if a new higher power is found
                       var cardsToRemove = highestCards.Where(card => card.EffectivePower < highestPower).ToList();
                       highestCards = highestCards.Except(cardsToRemove).ToList();
                   }
               }
               
               highestCards.ForEach(card => gameManager.RemoveCard(card.Number, false));

               gameManager.RemoveCard(cardInPlay.Metadata.Number, false);
               gameManager.EndCurrentPhase();
            });
        }
    }
}
