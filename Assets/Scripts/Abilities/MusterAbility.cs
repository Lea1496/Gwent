using Assets.Scripts.Phases;
using GwentEngine.Phases;
using System.Linq;

namespace GwentEngine.Abilities
{
    public class MusterAbility : CardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() => {
                var sameCards = gameManager.AllAvailableCards.Where(availableCard => availableCard.Name == cardInPlay.Metadata.Name);

                foreach (var sameCard in sameCards)
                {
                    gameManager.UseCard(sameCard.Number, cardInPlay.Player);
                }

                foreach (var sameCard in sameCards)
                {
                    gameManager.Play(sameCard.Number, cardInPlay.Location);
                }

                gameManager.EndCurrentPhase();
            });
        }
    }
}
