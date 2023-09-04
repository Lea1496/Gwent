using System;

namespace GwentEngine.Abilities
{

    public abstract class SpecificLocationAbility : CardAbility
    {
        private readonly Location _targetLocation;

        public SpecificLocationAbility(Location targetLocation)
        {
            _targetLocation = targetLocation;
        }

        public override void ApplyAbility(Card source, Card target)
        {
            if (target.Location != _targetLocation)
            {
                return;
            }

            base.ApplyAbility(source, target);
        }

        protected override void Apply(Card source, Card target)
        {
            target.Power = 1;
        }
    }
}
