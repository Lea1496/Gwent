using System;

namespace GwentEngine.Abilities
{
    public class MoralBoostAbility : SamePlayerCardAbility
    {
        protected override void Apply(Card source, Card target)
        {
            if (source.Location != target.Location)
            {
                //Not in the same zone
                return;
            }

            //Applicable
            target.Power += 1;
        }
    }
}
