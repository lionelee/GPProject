using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BondType
{
    SINGLE = 0, DOUBLE, TRIPLE
}

public class Bond : MonoBehaviour{
    public GameObject A1 { get; set; }
    public GameObject A2 { get; set; }

    public int A1Index { get; set; }
    public int A2Index { get; set; }
    
    public BondType Type { get; set; }
    public bool InRing { get; set; }

    public GameObject getAdjacent(GameObject from)
    {
        if (A1== from)
            return A2;
        if (A2 == from)
            return A1;
        return null;
    }

    public string toSymbol()
    {
        switch (Type)
        {
            case BondType.SINGLE:
                return "-";
            case BondType.DOUBLE:
                return "=";
            case BondType.TRIPLE:
                return "#";
        }
        return null;
    }

    public string toString()
    {
        return "Bond" + " " + Type.ToString() + " " + A1.GetComponent<Atom>().Id.ToString() + " " 
            + A1Index.ToString()+ " " + A2.GetComponent<Atom>().Id.ToString() + " " + A2Index.ToString();
    }
}


///
/*C ID VALENCE POS
BOND type 1 2*/
