using GwentEngine.Phases;
using System;

namespace GwentEngine.Abilities
{
    public abstract class CardAbility
    {
        public virtual void ApplyAbility(Card source, Card target, GameState gameState)
        {
            if (target.IsHero)
            {
                //Not applicable to heroes
                return;
            }

            Apply(source, target, gameState);
        }

        protected virtual void Apply(Card source, Card target, GameState gameState)
        {
        }

        public virtual GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return null;
        }
    }
}
