﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config{

    static public Dictionary<string, List<int>> ValenceTable = new Dictionary<string, List<int>>();
    static public Dictionary<string, List<Vector4>> BondAngleTable = new Dictionary<string, List<Vector4>>();
    static public Dictionary<string[], float> BondLengthTable = new Dictionary<string[], float>();

    static public void Init()
    {
        // initialize valence table
        ValenceTable.Add("C", new List<int> { 0, 2, 4 });
        ValenceTable.Add("H", new List<int> { -1, 0, 1 });
        ValenceTable.Add("O", new List<int> { -2, 0 });
        ValenceTable.Add("N", new List<int> { -3, 0, 1, 2, 3, 4 });
        ValenceTable.Add("P", new List<int> { -3, 0, 3, 5});
        ValenceTable.Add("S", new List<int> { -2, 0, 2, 4, 6 });
        ValenceTable.Add("Si", new List<int> { 0, 2, 4 });
        ValenceTable.Add("Cl", new List<int> { -1, 0, 1, 2, 3, 4, 5, 6, 7 });
        ValenceTable.Add("F", new List<int> { -1, 0, 1, 2, 3, 4, 5, 6, 7 });
        ValenceTable.Add("Br", new List<int> { -1, 0, 1, 2, 3, 4, 5, 6, 7 });

        // initialize bond angle table
        BondAngleTable.Add("C", new List<Vector4> { new Vector4(1f,0f,0f,0), new Vector4(-0.334f,0.943f,0f,0), 
            new Vector4(-0.334f,-0.472f,0.817f,0), new Vector4(-0.334f,-0.472f,-0.817f,0)});
        BondAngleTable.Add("H", new List<Vector4> { new Vector4(1f,0f,0f,0)});
        BondAngleTable.Add("O", new List<Vector4> { new Vector4(1f,0f,0f,0), new Vector4(-0.259f,0f,0.966f,0)});

        // initialize bond length table
		BondLengthTable.Add(new string[]{"C","C"}, 1.512f);
		BondLengthTable.Add(new string[]{"C","H"}, 1.096f);
		BondLengthTable.Add(new string[]{"C","O"}, 1.427f);
		BondLengthTable.Add(new string[]{"O","O"}, 1.516f);
		BondLengthTable.Add(new string[]{"O","H"}, 0.956f);

    }
}

