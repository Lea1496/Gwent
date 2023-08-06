using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public static class BoardManager
{
    public static List<CardBehavior> sword = new List<CardBehavior>();
    public static List<CardBehavior> arc = new List<CardBehavior>();
    public static List<CardBehavior> catapult = new List<CardBehavior>();
    public static List<CardBehavior> swordEnemy = new List<CardBehavior>();
    public static List<CardBehavior> arcEnemy = new List<CardBehavior>();
    public static List<CardBehavior> catapultEnemy = new List<CardBehavior>();
    
    public static List<int> cardsAlreadyPicked = new List<int>();
    
    public static List<List<CardBehavior>> wholeBoard = new List<List<CardBehavior>>() {sword, arc, catapult, swordEnemy, arcEnemy, catapultEnemy};

    
    public static bool playerHasPassed = false;
    public static bool enemyHasPassed = false;
  

    
    public static List<CardBehavior> defausse = new List<CardBehavior>()/* {c11,c22,c33,c44,c55}*/;
    public static List<GameObject> objDefausse = new List<GameObject>();
    public static List<GameObject> board = new List<GameObject>();

    public static List<CardBehavior> deck = new List<CardBehavior>() /*{c1,c2,c3,c4,c5,c6,c7,c8,c9,c0}*/;
    
    public static List<CardBehavior> deckEnemy = new List<CardBehavior>();

    

    public static List<CardBehavior> cardsInHand = new List<CardBehavior>();
    public static List<CardObj> cardsInHandEnemy = new List<CardObj>();

    public static int nbCardsInHand = 10;
    public static int nbCardsInHandEnemy = 10;
    private static int offset = 0;
    

    
    


}
