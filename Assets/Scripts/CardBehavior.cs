
using System.Linq;
using GwentEngine;
using TMPro;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI powerText;

    private Card _card;
    private IManager _manager;
    
    public Card Card
    {
        get => _card;
        
    }

    public void SetInfo(Card card)
    {
        _card = card;
    }

    private GameObject FindManager<T>() where T : class
    {
        var allManagers = GameObject.FindObjectsOfType<MonoBehaviour>(true);
        return allManagers.FirstOrDefault(m => m.GetComponent(typeof(T)))?.gameObject;
    }
    
        
    private void Awake()
    {
        _manager = FindManager<IManager>().GetComponent<IManager>();
    }

    public void OnClick()
    {
        GameManager gm = _manager as GameManager;
        if (gm != null && gm.isEmhyr1Active)
            return;
        _manager.OnClick(_card.Number, gameObject);
    }

    public void Update()
    {
        if (_card == null )
        {
            return;
        }
        GameManager gm = _manager as GameManager;
        
        if (gm != null && _card.Location != Location.Hand)
            _card = gm.GetCard(_card.Number, _card.EffectivePlayer, _card.Location); // pt qu'il faut donner le contraire si c'est un spy ?

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
