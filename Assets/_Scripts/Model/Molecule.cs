using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule{

    
    public int Id { set; get; }
    public int CurrentAtomId { set; get; }

    List<Atom> atoms;
    List<Bond> bonds;
     
    public Molecule(int id)
    {
        atoms = new List<Atom>();
        bonds = new List<Bond>();
        CurrentAtomId = 0;
        Id = id;
    }

    public void addAtom(Atom a)
    {
        atoms.Add(a);
    }
	
	public void addBond(Bond b)
	{
		bonds.Add (b);
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
