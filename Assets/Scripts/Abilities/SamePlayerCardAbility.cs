using System;

namespace GwentEngine.Abilities
{
    public abstract class SamePlayerCardAbility : CardAbility
    {
        public override void ApplyAbility(Card source, Card target)
        {
            if (source.EffectivePlayer != target.EffectivePlayer)
            {
                return;
            }

            if (source.Number == target.Number)
            {
                //Cannot apply to self
                return;
            }

            base.ApplyAbility(source, target);
        }
    }

}
