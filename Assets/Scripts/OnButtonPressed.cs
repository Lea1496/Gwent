using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonPressed : MonoBehaviour
{
    public void EndCurrentPhase()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().EndCurrentPhase();
    }
}
