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

        public override void ApplyAbility(Card source, Card target, GameState gameState)
        {
            if (target.Location != _targetLocation)
            {
                return;
            }

            base.ApplyAbility(source, target, gameState);
        }

        protected override void Apply(Card source, Card target, GameState gameState)
        {
            target.Power = 1;
        }
    }
}
