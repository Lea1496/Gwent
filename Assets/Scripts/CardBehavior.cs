using System;
using System.Security.Cryptography.X509Certificates;
using GwentEngine;
using TMPro;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI powerText;

    private Card _card;
    private PlayerKind _player;
    
    public Card Card
    {
        get => _card;
        
    }
    
    public PlayerKind Player
    {
        get => _player;
    }

    public void SetInfo(Card card, PlayerKind player)
    {
        _card = card;
        _player = player;
    }

    private void Awake()
    {

    }

    public void ChoseThisCardToChange()
    {
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.IsTimeToChangeCards)
        {
            gameManager.GameState.ChangeCard(_card.Number);
            gameManager.NbCardsChanged++;
        }
    }

    public void Update()
    {
        if (_card == null )
        {
            return;
        }

        var powerString = _card.Power == -1 || _card.IsHero ? "" : _card.Power.ToString();

        if (powerText.text != powerString)
        {
            powerText.text = powerString;
        }
    }

    public void OnDrop()
    {
    }

    public void OnClick()
    {
    }
}
