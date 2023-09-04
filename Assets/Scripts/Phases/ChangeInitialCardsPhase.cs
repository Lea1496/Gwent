using System;

namespace GwentEngine.Phases
{
    public class ChangeInitialCardsPhase : GamePhase
    {
        private readonly GameState _gameState;

        private int _nbCardsChanged;

        public ChangeInitialCardsPhase(GameState gameState, Action onActivatePhase, Action onEndPhase)
            : base(onActivatePhase, onEndPhase)
        {
            _gameState = gameState;
            _nbCardsChanged = 0;
        }

        public override void OnClick(int number)
        {
            _gameState.ChangeCard(number);
            _nbCardsChanged++;

            if (_nbCardsChanged == 2)
            {
                EndCurrentPhase();
            }
        }
    }
}
