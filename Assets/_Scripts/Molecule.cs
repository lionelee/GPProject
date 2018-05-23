using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule{


    List<Atom> atoms;
    List<Bond> bonds;
     
    public Molecule()
    {
        atoms = new List<Atom>();
        bonds = new List<Bond>();
    }

    public void addAtom(Atom a)
    {
        atoms.Add(a);
    }

    public void deleteAtomById(int id)
    {

    }
}
