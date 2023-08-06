using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObj
{
    public enum abilities {agile, berserker, mardroeme, medic, moralBoost, muster, spy, tightBond, none}
    public string name;
    public int ability;
    public int power;
    public int rank;
    
    public bool isHero;
    public int indice;

    public CardObj(string _name, int _ability, int _power, int _rank, bool _isHero, int _indice)
    {
        name = _name;
        ability = _ability;
        power = _power;
        rank = _rank;
        
        isHero = _isHero;
        indice = _indice;
    }
}
