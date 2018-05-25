﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom {

    //max size is 117 scale 0.6
    //const float scale = 1.0f;

    public int Id { set; get; }

    public string Symbol { set; get; }
    public int Valence { set; get; }
    public Vector3 Pos { set; get; }
    public List<Vector4> vbonds;

    //public static int CurrentId = 0;

    public Atom()
    {
        vbonds = new List<Vector4>();
    }

    
}

public class Hydrogen : Atom
{
    public Hydrogen(int _Valence, int _Id)
    {
        Symbol = "H";
        Valence = _Valence;
        Id = _Id;
    }
}

public class Oxygen : Atom
{
    public Oxygen(int _Valence, int _Id)
    {
        Symbol = "O";
        Valence = _Valence;
        Id = _Id;
    }
}

public class Carbon : Atom
{
    public Carbon(int _Valence, int _Id)
    {
        Symbol = "C";
        Valence = _Valence;
        Id = _Id;
    }
}

public class Nitrogen : Atom
{
    public Nitrogen(int _Valence, int _Id)
    {
        Symbol = "N";
        Valence = _Valence;
        Id = _Id;
    }
}

public class Sulfur : Atom
{
    public Sulfur(int _Valence, int _Id)
    {
        Symbol = "S";
        Valence = _Valence;
        Id = _Id;
    }
}

public class Phosphorus : Atom
{
    public Phosphorus(int _Valence, int _Id)
    {
        Symbol = "P";
        Valence = _Valence;
        Id = _Id;
    }
}

public class Silicon : Atom
{
    public Silicon(int _Valence, int _Id)
    {
        Symbol = "Si";
        Valence = _Valence;
        Id = _Id;
    }

}

public class Chlorine : Atom
{
    public Chlorine(int _Valence, int _Id)
    {
        Symbol = "Cl";
        Valence = _Valence;
        Id = _Id;
    }

}

public class Fluorine : Atom
{
    public Fluorine(int _Valence, int _Id)
    {
        Symbol = "F";
        Valence = _Valence;
        Id = _Id;
    }

}

public class Bromine : Atom
{
    public Bromine(int _Valence, int _Id)
    {
        Symbol = "Br";
        Valence = _Valence;
        Id = _Id;
    }

}

public class AtomFactory
{
    public static Atom GetAtom(string atomSymbol, int valence, int id)
    {
        
        if (atomSymbol == "C")
        {
            
            return new Carbon(valence, id);
        }
        else if (atomSymbol == "H")
        {
            return new Hydrogen(valence, id);
        }
        else if (atomSymbol == "O")
        {
            return new Oxygen(valence, id);
        }

        return null;
    }
}