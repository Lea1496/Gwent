
using Phases;
using UnityEngine;

public class OnButtonPressed : MonoBehaviour
{
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void EndCurrentPhase()
    {
        _gameManager.EndCurrentPhase();
        gameObject.SetActive(false);
    }

    public void PassTurn()
    {
        if (_gameManager.CurrentGamePhase.GetType() != typeof(TurnPhase))
            return;
        
        _gameManager.PassTurn();
        _gameManager.OnEndTurnPhase(true, (TurnPhase)_gameManager.CurrentGamePhase );
    }
    
}
