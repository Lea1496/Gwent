using Assets.Scripts.Phases;
using GwentEngine.Phases;
using System.Linq;

namespace GwentEngine.Abilities
{
    public class MusterAbility : CardAbility
    {
        public override GamePhase CreateInitialPhase(CardInPlay cardInPlay, GameManager gameManager)
        {
            return new CustomInitialPhasePhase(() =>
            {
                var sameCardsAvailable = gameManager.AllAvailableCards.Where(availableCard => availableCard.Name == cardInPlay.Metadata.Name).Select(c => c.Number);
                var sameCardsInHand = gameManager.GetCards(cardInPlay.Player, Location.Hand).Where(availableCard => availableCard.Name == cardInPlay.Metadata.Name).Select(c => c.Number);

                var sameCards = sameCardsAvailable.Union(sameCardsInHand).ToArray();

                foreach (var sameCard in sameCards)
                {
                    gameManager.UseCard(sameCard, cardInPlay.Player);
                }

                foreach (var sameCard in sameCards)
                {
                    gameManager.Play(sameCard, cardInPlay.Location);
                }

                gameManager.EndCurrentPhase();
            });
        }
    }
}
