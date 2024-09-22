using System;
using GwentEngine.Phases;
using Phases;
using UnityEngine;

namespace GwentEngine.Abilities
{
    public class MedicAbility : CardAbility
    {
        /*public override void ApplyAbility(Card source, Card target, GameState gameState)
        {
            var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            var medicPhase = new UseMedicPhase(gameState, null, null);
            gameManager.ActivateGamePhase(medicPhase);
            base.ApplyAbility(source, target, gameState);
        }*/
        
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            if (!gameManager.ExistsCardsInDiscard())
                return null;
            
            return new UseMedicPhase(null, null);
        }

        
    }
}
