using System;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

namespace GwentEngine.Abilities
{
    /// <summary>
    /// See: https://www.ign.com/wikis/the-witcher-3-wild-hunt/Gwent_Card_Game
    /// 
    /// Doubles the strength of both cards when placed next to a unit of the same name.
    /// </summary>
    public class TightBondAbility : SamePlayerCardAbility
    {
        public override void ApplyAbility(Card source, Card target, GameState gameState)
        {
            if(target.Location == source.Location && target.Name == source.Name)
            {
<<<<<<< HEAD
                Card[] cards = gameManager.GetCards(cardInPlay.Player, cardInPlay.Location);
                foreach (var card in cards)
                {
                    card.PowerMultiplier = (int)Math.Pow(2, cards.Length - 1);
                }
                gameManager.EndCurrentPhase();
            });
=======
                target.SetPowerMultiplier(nameof(TightBondAbility), currentValue => 2 * currentValue);
            }
>>>>>>> eb373a59fbbcbb6d1036fb5ec3f7561bdfa997f8
        }
    }
}
