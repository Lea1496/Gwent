using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GwentEngine.Phases
{
    public class GamePhase  
    {
        private readonly Action _onActivatePhase;
        private readonly Action _onEndPhase;
        protected GameManager _gameManager;
        protected IManager _manager;

        public virtual bool Done { get; private set; }
        private GameObject FindManager<T>() where T : class
        {
            var allManagers = GameObject.FindObjectsOfType<MonoBehaviour>(true);
            return allManagers.FirstOrDefault(m => m.GetComponent(typeof(T)))?.gameObject;
        }


        public GamePhase(Action onActivatePhase = null, Action onEndPhase = null)
        {
            _onActivatePhase = onActivatePhase;
            _onEndPhase = onEndPhase;
            
            _manager = FindManager<IManager>().GetComponent<IManager>();
            _gameManager = _manager as GameManager;
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
