using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BondType
{
    SINGLE, DOUBLE, TRIPLE
}

public class Bond{
    public int A1 { get; set; }
    public int A2 { get; set; }

    public BondType Type { get; set; }
}


///
/*C 1 POS VALENCE ID  
H 2
BOND type 1 2*/
