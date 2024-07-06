using System;

namespace GwentEngine.Phases
{
    public class GamePhase  
    {
        private readonly Action _onActivatePhase;
        private readonly Action _onEndPhase;

        public virtual bool Done { get; private set; }

        public GamePhase(Action onActivatePhase = null, Action onEndPhase = null)
        {
            _onActivatePhase = onActivatePhase;
            _onEndPhase = onEndPhase;
        }

        public virtual void OnClick(int number)
        {
            
        }

        public virtual void Activate()
        {
            _onActivatePhase?.Invoke();
        }
        public virtual void EndCurrentPhase()
        {
            _onEndPhase?.Invoke();
            Done = true;
        }

        public virtual bool IsDraggable(Card card)
        {
            return false;
        }
    }
}
