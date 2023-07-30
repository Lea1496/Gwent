using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static float offset;

    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private static List<GameObject> cardsInDefausse = new List<GameObject>();

    private GameObject dropZoneSword;
    private GameObject dropZoneArc;
    private GameObject dropZoneCatapult;
    private GameObject dropZoneSwordEnemy;
    private GameObject dropZoneArcEnemy;
    private GameObject dropZoneCatapultEnemy;
    
   

    private static GameObject defausse;
    public static bool defausseOpen = false;
    private GameObject cardToKeep;
    private GameObject tempCard;
    [SerializeField] static private GameObject card;

    private GameObject canvas;
    private void Start()
    {
        DefineDropZones();
    }

    private void DefineDropZones()
    {
        dropZoneSword = GameObject.Find("DropZoneS");
        dropZoneArc = GameObject.Find("DropZoneA");
        dropZoneCatapult = GameObject.Find("DropZoneC");
        dropZoneSwordEnemy = GameObject.Find("DropZoneEnemyS");
        dropZoneArcEnemy = GameObject.Find("DropZoneEnemyA");
        dropZoneCatapultEnemy = GameObject.Find("DropZoneEnemyC");
        defausse = GameObject.Find("Defausse");
        canvas = GameObject.Find("Canvas");

    }
    public void MontrerDefausse(GameObject carte)
    {
        defausseOpen = true;
        offset = 276.0f / (BoardManager.defausse.Count + 1);
        float offsetStart = offset;
        Debug.Log(BoardManager.defausse.Count);
        foreach (CardBehavior card in BoardManager.defausse)
        {
            if (!card.isHero)
            {
                Debug.Log("testttt");
                carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.image, card.isHero);
               // cardsInDefausse.Add(GameObject.Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation));
                Instantiate(carte, new Vector2(offset + 10, defausse.transform.position.y), defausse.transform.rotation).transform.SetParent(canvas.transform, true);
                
                //tempCard.transform.SetParent(canvas.transform, true);
                //cardsInDefausse.Add(tempCard);
                offset += offsetStart;
            }
            
        }

        offset = 0;
    }

    public static void CacherDefausse(GameObject cardToKeep)
    {
        foreach (GameObject card in cardsInDefausse)
        {
            if (card != cardToKeep)
            {
                Destroy(card);
            }
        }
        cardsInDefausse.Clear();
    }
    

    
}
