using GwentEngine.Phases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Phases
{
    public class CustomInitialPhasePhase : GamePhase
    {
        public CustomInitialPhasePhase(Action onActivatePhase = null, Action onEndPhase = null)
            : base(onActivatePhase, onEndPhase)
        {
        }
    }
}
