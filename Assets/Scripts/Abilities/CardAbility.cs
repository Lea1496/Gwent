using System;

namespace GwentEngine.Abilities
{
    public abstract class CardAbility
    {
        public virtual void ApplyAbility(Card source, Card target)
        {
            if (target.IsHero)
            {
                //Not applicable to heroes
                return;
            }

            Apply(source, target);
        }

        protected virtual void Apply(Card source, Card target)
        {
        }
    }
}
