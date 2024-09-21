using System;
using System.Globalization;
using GwentEngine;
using GwentEngine.Abilities;
using GwentEngine.Phases;
using Unity.VisualScripting;
using UnityEngine;

namespace Phases
{
    public class TurnPhase : GamePhase
    {
        private GameState _gameState;

        private bool _isDecoyPhase;
        
        public TurnPhase(GameState gameState)
        {
            _gameState = gameState;
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
            
            if (number > 180)
            {
                _gameManager.Play(number, Location.Leader);
            }
        }
        
        public override void EndCurrentPhase()
        {
            _gameManager.OnEndTurnPhase(!_isDecoyPhase, this);
            base.EndCurrentPhase();
        }
        
        public override bool IsDraggable(Card card)
        {
            return _gameState.CurrentPlayer == PlayerKind.Player && card.EffectivePlayer == PlayerKind.Player && card.Location == Location.Hand && card.Ability != Ability.Decoy && !_gameManager.isEmhyr1Active;
        }
    }
}