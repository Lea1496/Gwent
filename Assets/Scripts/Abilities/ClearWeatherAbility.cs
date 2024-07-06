using System;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine.Phases;
using UnityEngine;

namespace GwentEngine.Abilities
{
    public class ClearWeatherAbility : CardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                gameManager.RemoveAllWeatherCards();

                foreach (Location location in Enum.GetValues(typeof(Location)))
               {
                   if (!(location == Location.Archery || location == Location.Catapult || location == Location.Sword))
                       continue;
                   
                   var localCards = gameManager.GetCards(PlayerKind.Player, location).Where(card => !card.IsHero);
                   if (localCards != null)
                   {
                       //localCards = (Card[])localCards;
                       localCards.ForEach(card => card.Power = card.DefaultPower);
                   }
                   var opponentCards = gameManager.GetCards(PlayerKind.Opponent, location).Where(card => !card.IsHero);
                   if (opponentCards != null)
                   {
                       //opponentCards = (Card[])opponentCards;
                       opponentCards.ForEach(card => card.Power = card.DefaultPower);
                   }
                
               }
                gameManager.EndCurrentPhase();
            });
        }
    }
}
