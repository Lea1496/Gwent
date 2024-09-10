using System;
using System.Collections.Generic;
using UnityEngine;

namespace GwentEngine.Phases
{
    public class GamePhase  
    {
        private readonly Action _onActivatePhase;
        private readonly Action _onEndPhase;
        protected GameManager _gameManager;

        public virtual bool Done { get; private set; }

        public GamePhase(Action onActivatePhase = null, Action onEndPhase = null)
        {
            _onActivatePhase = onActivatePhase;
            _onEndPhase = onEndPhase;
            
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        public virtual void OnClick(int number)
        {
            
        }
        
        public virtual void OnClick(int number, GameObject card)
        {
            OnClick(number);
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
