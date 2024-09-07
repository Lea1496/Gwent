using System;
using System.Collections;
using GwentEngine.Abilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace GwentEngine.Phases
{
    public class UseDecoyPhase : GamePhase
    {
        private readonly GameState _gameState;

        private int _nbCardsChanged;

     //   public bool OnClickCalled = false;

     private int _decoyNumber;
        
        public UseDecoyPhase(GameState gameState, int decoyNumber, Action onActivatePhase = null, Action onEndPhase =  null)
            : base(onActivatePhase, onEndPhase)
        {
            _gameState = gameState;
            _decoyNumber = decoyNumber;
        }

        public override void Activate()
        {
            base.Activate();
            _gameManager.StartWaitForFunctionCoroutine();
        }

        public override void OnClick(int number)
        {
            if (number != _decoyNumber)
            {
                _gameState.SwitchCardFromBoard(_decoyNumber, number, _gameManager.CurrentPlayer);
            }
            
            _gameManager.onClickCalled = true;
            EndCurrentPhase();
        }
        
        public override bool IsDraggable(Card card)
        {
            return false;
        }
        
        public override void EndCurrentPhase()
        {
            _gameManager.OnEndPhase(this);
            base.EndCurrentPhase();
        }
    }
}
