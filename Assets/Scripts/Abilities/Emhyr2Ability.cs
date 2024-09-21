using System.Linq;
using Assets.Scripts.Phases;
using GwentEngine;
using GwentEngine.Abilities;
using GwentEngine.Phases;

namespace Abilities
{
    public class Emhyr2Ability : CardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                // todo : quand il va avoir deux decks séparés ajouter une vérif pour le player
                var torrentialRain = gameManager.AllAvailableCards.First(card =>
                    card.Ability == Ability.Rain);
                

                if (torrentialRain == null)
                    return;
            
                gameManager.UseCard(torrentialRain.Number, gameManager.CurrentPlayer);

                gameManager.Play(torrentialRain.Number, Location.Weather);
                gameManager.EndCurrentPhase();
            });
        }
    }
}