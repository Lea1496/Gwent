using System;
using System.Collections;
using GwentEngine.Abilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace GwentEngine.Phases
{
    public class UseMedicPhase : GamePhase
    {
        private readonly GameState _gameState;

        private int _nbCardsChanged;

        //   public bool OnClickCalled = false;

        private GameManager _gameManager;

        public UseMedicPhase(Action onActivatePhase = null, Action onEndPhase =  null)
            : base(onActivatePhase, onEndPhase)
        {
            //_gameState = gameState;
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            Debug.Log("ici");
        }
        
        public override bool IsDraggable(Card card)
        {
            return card.Location == Location.Discard && card.EffectivePlayer == _gameManager.CurrentPlayer;
        }
        
        public override void EndCurrentPhase()
        {
            _gameManager.OnEndPhase(this);
            base.EndCurrentPhase();
        }
    }
}