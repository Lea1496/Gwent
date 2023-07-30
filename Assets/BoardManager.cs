using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardManager
{
    public static List<CardBehavior> sword = new List<CardBehavior>();
    public static List<CardBehavior> arc = new List<CardBehavior>();
    public static List<CardBehavior> catapult = new List<CardBehavior>();
    public static List<CardBehavior> swordEnemy = new List<CardBehavior>();
    public static List<CardBehavior> arcEnemy = new List<CardBehavior>();
    public static List<CardBehavior> catapultEnemy = new List<CardBehavior>();

    public static List<List<CardBehavior>> wholeBoard = new List<List<CardBehavior>>() {sword, arc, catapult};


    public static CardBehavior c1 = new CardBehavior("L", 8, 9, 7, "null", false);
    public static CardBehavior c2 = new CardBehavior("Le", 7, 9, 8, "null", false);
    public static CardBehavior c3 = new CardBehavior("Lea", 5, 9, 7, "null", true);
    public static CardBehavior c4 = new CardBehavior("Leaa", 8, 9, 9, "null", false);
    public static CardBehavior c5 = new CardBehavior("Leea", 8, 9, 9, "null", false);
    public static List<CardBehavior> defausse = new List<CardBehavior>();
    public static List<GameObject> objDefausse = new List<GameObject>();
    public static List<CardBehavior> board = new List<CardBehavior>();

    public static List<CardBehavior> deck = new List<CardBehavior>();
    private static int offset = 0;
    

    public static void EndGame(GameObject carte)
    {
        CompterNbPts();
        foreach (CardBehavior card in board)
        {
            card.GetComponent<DragDrop>().isDraggable = true;
            defausse.Add(card);
            
        }

        foreach (var card in GameObject.FindGameObjectsWithTag("Card"))
        {
            GameObject.Destroy(card);
        }
    }
    

    public static int totPts = 0;
    public static int totPtsEnemy = 0;

    public static void CompterNbPts()
    {
        foreach (CardBehavior card in sword)
        {
            totPts += card.power;
        }
        foreach (CardBehavior card in arc)
        {
            totPts += card.power;
        }
        foreach (CardBehavior card in catapult)
        {
            totPts += card.power;
        }
    }


}
