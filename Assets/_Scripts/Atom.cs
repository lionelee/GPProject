using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom {

    const float scale = 1.0f;

    public int Id { set; get; }

    public string Symbol { set; get; }
    public float Radius { set; get; }
    public int Valence { set; get; }
    public Vector3 Pos { set; get; }
    public Vector3 Color { set; get; }
    public List<Vector4> vbonds;

    public static int CurrentId = 0;

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
        Radius = 32;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }
}

public class Oxygen : Atom
{
    public Oxygen(int _Valence, int _Id)
    {
        Symbol = "O";
        Radius = 66;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }
}

public class Carbon : Atom
{
    public Carbon(int _Valence, int _Id)
    {
        Symbol = "C";
        Radius = 77;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }
}

public class Nitrogen : Atom
{
    public Nitrogen(int _Valence, int _Id)
    {
        Symbol = "N";
        Radius = 70;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }
}

public class Sulfur : Atom
{
    public Sulfur(int _Valence, int _Id)
    {
        Symbol = "S";
        Radius = 104;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }
}

public class Phosphorus : Atom
{
    public Phosphorus(int _Valence, int _Id)
    {
        Symbol = "P";
        Radius = 110;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }
}

public class Silicon : Atom
{
    public Silicon(int _Valence, int _Id)
    {
        Symbol = "Si";
        Radius = 117;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }

}

public class Chlorine : Atom
{
    public Chlorine(int _Valence, int _Id)
    {
        Symbol = "Cl";
        Radius = 99;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }

}

public class Fluorine : Atom
{
    public Fluorine(int _Valence, int _Id)
    {
        Symbol = "F";
        Radius = 64;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }

}

public class Bromine : Atom
{
    public Bromine(int _Valence, int _Id)
    {
        Symbol = "Br";
        Radius = 114;
        Color = new Vector3(1.0f, 0, 0);
        Valence = _Valence;
        Id = _Id;
    }

}

public class AtomFactory
{
    public static Atom GetAtom(string atomSymbol, int valence)
    {
        
        int id = Atom.CurrentId++;
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