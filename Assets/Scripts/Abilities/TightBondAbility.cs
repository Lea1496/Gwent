using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine.Phases;
using UnityEngine.Playables;

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
            if (target.Location == source.Location && target.Name == source.Name)
            {
                target.SetPowerMultiplier(2);
            }
        }
    }
}
