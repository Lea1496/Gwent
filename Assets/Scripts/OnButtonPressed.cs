using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonPressed : MonoBehaviour
{
    public void StopChangeCards()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().NbCardsChanged = 2;
        gameObject.SetActive(false);
    }

    public void Pass()
    {
        //BoardManager.playerHasPassed = true;
    }
}
