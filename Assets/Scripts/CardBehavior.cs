using System;
using GwentEngine;
using TMPro;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI powerText;

    private int _number;
    private Card _card;
    private GameManager gameManager;
    private PlayerKind _player;

    public void SetInfo(Card card, PlayerKind player)
    {
        _card = card;
        _player = player;
    }

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ChoseThisCardToChange()
    {
        if (gameManager.IsTimeToChangeCards)
        {
            
            gameManager._gameState.ChangeCard(_card.Number);
            gameManager.NbCardsChanged++;

        }
    }

    public void Update()
    {
        //if (gameManager._gameState.GetCards(_player, _card.Location))
        {
            
        }
    }


    public void OnDrop()
    {
    }

    public void OnClick()
    {
    }
}
