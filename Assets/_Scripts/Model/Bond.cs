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
	
	public Bond(int a1, int a2)
    {
        A1 = a1;
        A2 = a2;
    }
}


///
/*C 1 POS VALENCE ID  
H 2
BOND type 1 2*/
