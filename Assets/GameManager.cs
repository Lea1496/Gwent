using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static int offset;

    private List<GameObject> cardsOnBoard = new List<GameObject>();
    private static List<GameObject> cardsInDefausse = new List<GameObject>();

    private GameObject dropZoneSword;
    private GameObject dropZoneArc;
    private GameObject dropZoneCatapult;
    private GameObject dropZoneSwordEnemy;
    private GameObject dropZoneArcEnemy;
    private GameObject dropZoneCatapultEnemy;
    
   

    private static GameObject defausse;
    private static bool defausseOpen = false;
    private GameObject cardToKeep;
    
    [SerializeField] static private GameObject card;
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

    }
    public static void MontrerDefausse(GameObject carte)
    {
        defausseOpen = true;
        offset = (int)(38 / BoardManager.defausse.Count + 1);
        foreach (CardBehavior card in BoardManager.defausse)
        {
            if (!card.isHero)
            {
                carte.GetComponent<CardBehavior>().Constructeur(card.name, card.ability, card.power, card.rank, card.image, card.isHero);
                cardsInDefausse.Add(GameObject.Instantiate(carte, new Vector2(offset, defausse.transform.position.y), defausse.transform.rotation));
                offset += offset;
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
                GameObject.Destroy(card);
            }
        }
        cardsInDefausse.Clear();
    }
    

    
}
