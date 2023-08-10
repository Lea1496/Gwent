using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public static class BoardManager
{
    public static List<GameObject> sword = new List<GameObject>();
    public static List<GameObject> arc = new List<GameObject>();
    public static List<GameObject> catapult = new List<GameObject>();
    public static List<GameObject> swordEnemy = new List<GameObject>();
    public static List<GameObject> arcEnemy = new List<GameObject>();
    public static List<GameObject> catapultEnemy = new List<GameObject>();
    
    public static List<int> cardsAlreadyPicked = new List<int>();
    
    public static List<List<GameObject>> wholeBoard = new List<List<GameObject>>() {sword, arc, catapult, swordEnemy, arcEnemy, catapultEnemy};

    
    public static bool playerHasPassed = false;
    public static bool enemyHasPassed = false;
  

    
    public static List<CardBehavior> defausse = new List<CardBehavior>()/* {c11,c22,c33,c44,c55}*/;
    public static List<GameObject> objDefausse = new List<GameObject>();
    public static List<GameObject> board = new List<GameObject>();

    public static List<(string, int, int, int, bool,int, int, int)> deck = new List<(string, int, int, int, bool,int, int, int)>() /*{c1,c2,c3,c4,c5,c6,c7,c8,c9,c0}*/;
    
    public static List<GameObject> deckEnemy = new List<GameObject>();

    

    public static List<CardBehavior> cardsInHand = new List<CardBehavior>();
    public static List<CardBehavior> cardsInHandEnemy = new List<CardBehavior>();

    public static int nbCardsInHand = 10;
    public static int nbCardsInHandEnemy = 10;
    private static int offset = 0;
    

    
    


}
