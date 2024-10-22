
using GwentEngine.Phases;
using UnityEngine;

public interface IManager
{
    public GamePhase CurrentGamePhase { get; set; }
    public GameObject InstantiateCard();
    public void OnClick(int number, GameObject card);
}
