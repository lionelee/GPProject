using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BondType
{
    SINGLE, DOUBLE, TRIPLE
}

public class Bond : MonoBehaviour{
    public int A1 { get; set; }
    public int A2 { get; set; }
    public BondType Type { get; set; }

    public string toString()
    {
        return "Bond" + " " + Type.ToString() + " " + A1.ToString() + " " + A1.ToString();
    }
}


///
/*C ID VALENCE POS
BOND type 1 2*/
