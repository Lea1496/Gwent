using System;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    public class TightBondAbility : CardAbility
    { 
        
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                Card[] cards = gameManager.GetCards(cardInPlay.Player, cardInPlay.Location);
                foreach (var card in cards)
                {
                    card.PowerMultiplier = (int)Math.Pow(2, cards.Length);
                }
                gameManager.EndCurrentPhase();
            });
        }
    }
}
