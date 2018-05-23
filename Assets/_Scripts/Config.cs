using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config{

    static public Dictionary<string, List<int>> ValenceTable = new Dictionary<string, List<int>>();
    static public Dictionary<string, List<Vector3>> BondAngleTable = new Dictionary<string, List<Vector3>>();

    static public void init()
    {
        ValenceTable.Add("C", new List<int> { 0, 2, 4 });
        ValenceTable.Add("H", new List<int> { -1, 0, 1 });
        ValenceTable.Add("O", new List<int> { 0, -2 });
        ValenceTable.Add("N", new List<int> { 0, 1, 2, 3, 4, -3 });
        ValenceTable.Add("P", new List<int> { 0, 3, 5, -3 });
        ValenceTable.Add("S", new List<int> { -2, 0, 2, 4, 6 });
        ValenceTable.Add("Si", new List<int> { 0, 2, 4 });
        ValenceTable.Add("Cl", new List<int> { 0, -1, 1, 2, 3, 4, 5, 6, 7 });
        ValenceTable.Add("F", new List<int> { 0, -1, 1, 2, 3, 4, 5, 6, 7 });
        ValenceTable.Add("Br", new List<int> { 0, -1, 1, 2, 3, 4, 5, 6, 7 });

        BondAngleTable.Add("C", new List<Vector3> { new Vector3() });
        BondAngleTable.Add("H", new List<Vector3> { new Vector3() });
        BondAngleTable.Add("O", new List<Vector3> { new Vector3() });

    }
}

