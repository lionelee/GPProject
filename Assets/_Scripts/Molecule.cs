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

    public Atom getAtomById(int id)
    {
        foreach(Atom a in atoms)
        {
            if (a.Id == id)
            {
                return a;
            }
        }
        return null;
    }
    public void deleteAtomById(int id)
    {
        foreach (Atom a in atoms)
        {
            if (a.Id == id)
            {
                atoms.Remove(a);
                return;
            }
        }
        Debug.Log("id:" + id + "not found");
    }
}
