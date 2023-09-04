using System;
using System.Security.Cryptography.X509Certificates;
using GwentEngine;
using TMPro;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI powerText;

    private Card _card;
    
    public Card Card
    {
        get => _card;
        
    }

    public void SetInfo(Card card)
    {
        _card = card;
    }

    private void Awake()
    {

    }

    public void OnClick()
    {
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.OnClick(_card.Number);
    }

    public void Update()
    {
        if (_card == null )
        {
            return;
        }

        var powerString = _card.EffectivePower == -1 || _card.IsHero ? "" : _card.EffectivePower.ToString();

        if (powerText.text != powerString)
        {
            powerText.text = powerString;
        }
    }
}
