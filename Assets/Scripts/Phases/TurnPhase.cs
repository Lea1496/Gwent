using System;
using GwentEngine;
using GwentEngine.Abilities;
using GwentEngine.Phases;
using Unity.VisualScripting;
using UnityEngine;

namespace Phases
{
    public class TurnPhase : GamePhase
    {
        private GameManager _gameManager;

        private GameState _gameState;

        private bool _isDecoyPhase;
        
        public TurnPhase(GameState gameState):
            base( )

        {
            _gameState = gameState;
        }
        public override void Activate()
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            base.Activate();
        }
        
        public override void OnClick(int number)
        {
            if (_gameState.IsCardDecoy(number))
            {
                var decoyPhase = new UseDecoyPhase(_gameState, number, null, () =>
                {
                    var turnPhase = new TurnPhase(_gameState);
                    _gameManager.ActivateGamePhase(turnPhase);
                });
                _gameManager.ActivateGamePhase(decoyPhase);
                _isDecoyPhase = true;
                EndCurrentPhase();
            }

            _gameManager.onClickCalled = false;
        }
        
        public override void EndCurrentPhase()
        {
            _gameManager.OnEndTurnPhase(!_isDecoyPhase, this);
        }
        
        public override bool IsDraggable(Card card)
        {
            return _gameState.CurrentPlayer == PlayerKind.Player && card.EffectivePlayer == PlayerKind.Player && card.Location == Location.Hand && card.Ability != Ability.Decoy;
        }
    }
}