using System;
using GwentEngine;
using TMPro;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI powerText;

    private Card _card;
    private PlayerKind _player;

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

            gameManager._gameState.ChangeCard(_card.Number);
            gameManager.NbCardsChanged++;
        }
    }

    public void Update()
    {
        if (_card == null)
        {
            return;
        }

        var powerString = _card.EffectivePower == -1 ? "" : _card.EffectivePower.ToString();

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
