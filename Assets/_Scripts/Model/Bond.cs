using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BondType
{
    SINGLE, DOUBLE, TRIPLE
}

public class Bond : MonoBehaviour{
    public GameObject A1 { get; set; }
    public GameObject A2 { get; set; }
    public int A1Index { get; set; }
    public int A2Index { get; set; }
    public BondType Type { get; set; }

    public string toString()
    {
        return "Bond" + " " + Type.ToString() + " " + A1.ToString() + " " + A1.ToString();
    }
}


///
/*C ID VALENCE POS
BOND type 1 2*/
