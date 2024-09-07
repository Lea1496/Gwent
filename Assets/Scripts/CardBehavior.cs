using System;
using System.Security.Cryptography.X509Certificates;
using GwentEngine;
using TMPro;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI powerText;

    private Card _card;
    private GameManager _gameManager;
    
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
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClick()
    {
        _gameManager.OnClick(_card.Number, gameObject);
    }

    public void Update()
    {
        if (_card == null )
        {
            return;
        }
        
        if (_card.Location != Location.Hand && !_gameManager.IsChooseDeckPhase)
            _card = _gameManager.GetCard(_card.Number, _card.EffectivePlayer, _card.Location); // pt qu'il faut donner le contraire si c'est un spy ?

        var powerString = _card.EffectivePower == -1 || _card.IsHero ? "" : _card.EffectivePower.ToString();

        if (powerText.text != powerString)
        {
            powerText.text = powerString;
        }

        if (_card.Location == Location.Hand) // à enlever
            return;
        
        powerString = _card.EffectivePower == -1 || _card.IsHero ? "" : _card.EffectivePower.ToString();
        
    }
}
