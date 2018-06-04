using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour
{    
    public int Id { set; get; }
    public int CurrentAtomId { set; get; }
    
    public Molecule(int id)
    {
        CurrentAtomId = 0;
        Id = id;
    }
    
    public string toString()
    {
        return "Molecule";
    }
}
