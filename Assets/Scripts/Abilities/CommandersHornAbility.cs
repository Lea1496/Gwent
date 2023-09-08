using System;
using System.Collections.Generic;

namespace GwentEngine.Abilities
{
    public class CommandersHornAbility : SamePlayerCardAbility
    {
        private static Dictionary<Location, Location> LocationWithCommandersHorn = new()
        {
            { Location.ComandersHornCatapult, Location.Catapult },
            { Location.ComandersHornSword, Location.Sword },
            { Location.ComandersHornArchery, Location.Archery },
            { Location.Sword, Location.Sword },
        };

        protected override void Apply(Card source, Card target)
        {
            
            
            if (!LocationWithCommandersHorn.TryGetValue(source.Location, out var targetLocation))
            {
                //This commanders horn is not in play
                return;
            }

            if (target.Location != targetLocation)
            {
                //This commanders horn is not in play
                return;
            }

            //Applicable
            target.PowerMultiplier = 2;
        }
    }
}
