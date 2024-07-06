using System;
using System.Collections.Generic;
using Assets.Scripts.Phases;
using GwentEngine.Phases;

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

        protected override void Apply(Card source, Card target, GameState gameState)
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
           // target.SetAction(ActionKind.CommandersHorn);
           
            gameState.SetRowAction(target.Location, target.EffectivePlayer, ActionKind.CommandersHorn); // hmm
           
        }
    }
}
