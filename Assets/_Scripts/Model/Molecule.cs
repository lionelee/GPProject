using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour
{    
    public int Id;
    public int CurrentAtomId;
    public int AtomNum;
    
    public Molecule(int id)
    {
        CurrentAtomId = 0;
        Id = id;
        AtomNum = 1;
    }
    
    public string toString()
    {
        return "Molecule " + AtomNum.ToString();
    }
}
