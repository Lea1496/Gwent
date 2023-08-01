using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public static class BoardManager
{
    public static List<CardObj> sword = new List<CardObj>();
    public static List<CardObj> arc = new List<CardObj>();
    public static List<CardObj> catapult = new List<CardObj>();
    public static List<CardObj> swordEnemy = new List<CardObj>();
    public static List<CardObj> arcEnemy = new List<CardObj>();
    public static List<CardObj> catapultEnemy = new List<CardObj>();
    
    public static List<int> cardsAlreadyPicked = new List<int>();
    
    public static List<List<CardObj>> wholeBoard = new List<List<CardObj>>() {sword, arc, catapult};

    public static bool playerHasPassed = false;
    public static bool enemyHasPassed = false;
  

    
    public static List<CardObj> defausse = new List<CardObj>();
    public static List<GameObject> objDefausse = new List<GameObject>();
    public static List<GameObject> board = new List<GameObject>();

    public static List<CardObj> deck = new List<CardObj>();
    public static List<CardBehavior> deckEnemy = new List<CardBehavior>();

    public static List<CardObj> cardsInHand = new List<CardObj>();
    public static List<CardObj> cardsInHandEnemy = new List<CardObj>();

    public static int nbCardsInHand = 10;
    public static int nbCardsInHandEnemy = 10;
    private static int offset = 0;
    

    public static void EndGame(GameObject carte)
    {
        CompterNbPts();
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in wholeBoard[i])
            {
                defausse.Add(card);
            }
        }
        foreach (GameObject card in board)
        {
            //card.GetComponent<DragDrop>().isDraggable = true;
            GameObject.Destroy(card);
        }
        
        /*foreach (var card in GameObject.FindGameObjectsWithTag("Card"))
        {
            GameObject.Destroy(card);
        }*/
    }
    

    public static int totPts = 0;
    public static int totPtsEnemy = 0;

    public static void CompterNbPts()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in wholeBoard[i])
            {
                totPts += card.power;
            }
        }
    }
    public static CardObj ChooseCardInHand()
    {
        int index = 0;
        Random gen = new Random();
        index = gen.Next(0, deck.Count);
        while (!cardsAlreadyPicked.Contains(index))
        {
            index = gen.Next(0, deck.Count);
        }
        cardsInHand.Add(deck[index]);
        cardsAlreadyPicked.Add(index);
        return deck[index];
    }


}
