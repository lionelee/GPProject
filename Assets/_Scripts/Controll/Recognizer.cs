using UnityEngine;
using System;
using System.Collections.Generic;

class Recognizer
{

    private static Dictionary<GameObject, string> atomInRing = new Dictionary<GameObject, string>();
    private static int ringN = 0;
    private static GameObject startAtom = null;

    #region /// generate SMILES for selected molecule ///
    private static void findRing(GameObject obj)
    {
        ++ringN;
        GameObject cur = obj;
        List<Bond> visitedBond = new List<Bond>();
        while(true)
        {
            foreach (GameObject bond in cur.GetComponent<Atom>().Bonds)
            {
                if (bond != null)
                {
                    Bond b = bond.GetComponent<Bond>();
                    if (b.InRing && !visitedBond.Contains(b))
                    {
                        visitedBond.Add(b);
                        GameObject adj = b.getAdjacent(cur);
                        atomInRing.Add(adj, adj.GetComponent<Atom>().Symbol + ringN);
                        cur = adj;
                        break;
                    }
                }
            }
            if (cur == obj) return;
        }
    }

    public static Dictionary<GameObject, List<GameObject>> getLinkedTable(GameObject mole)
    {
        Dictionary<GameObject, List<GameObject>> table = new Dictionary<GameObject, List<GameObject>>();

        //find the atom to start
        GameObject firstCarbon = null;
        bool flag = false;
        foreach (Transform child in mole.transform)
        {
            if (child.tag == "Component")
            {
                GameObject atom = child.gameObject;
                if (atom.GetComponent<Atom>().Symbol == "C")
                {
                    if (!flag)
                    {
                        firstCarbon = atom;
                        flag = true;
                    }

                    int cnt = 0;
                    foreach (GameObject bond in atom.GetComponent<Atom>().Bonds)
                    {
                        if (bond != null)
                        {
                            Bond b = bond.GetComponent<Bond>();
                            GameObject adj = b.getAdjacent(atom);
                            if (adj.GetComponent<Atom>().Symbol == "C")
                                ++cnt;
                        }
                    }
                    if (cnt == 1)
                        startAtom = atom;
                }
            }
        }
        if (startAtom == null)
            startAtom = firstCarbon;

        // generate spanning tree from startAtom using DFS
        Stack<GameObject> S = new Stack<GameObject>();
        S.Push(startAtom);
        List<GameObject> visited = new List<GameObject>();
        Dictionary<GameObject, GameObject> parent = new Dictionary<GameObject, GameObject>();
        while (S.Count > 0)
        {
            GameObject cur = S.Pop();
            if (visited.Contains(cur)) continue;
            visited.Add(cur);
            if (parent.ContainsKey(cur))
            {
                GameObject par = parent[cur];
                if (table.ContainsKey(par))
                    table[par].Add(par.GetComponent<Atom>().getBond(cur));
            }
            if (!table.ContainsKey(cur))
            {
                table.Add(cur, new List<GameObject>());
            }
            foreach (GameObject bond in cur.GetComponent<Atom>().Bonds)
            {
                if (bond != null)
                {
                    Bond b = bond.GetComponent<Bond>();
                    GameObject adj = b.getAdjacent(cur);
                    if (adj.GetComponent<Atom>().Symbol == "H")
                        continue;
                    if (visited.Contains(adj))
                    {
                        if (adj.GetComponent<Atom>().InRing && !atomInRing.ContainsKey(adj))
                            findRing(adj);
                    }
                    else
                    {
                        S.Push(adj);
                        if (parent.ContainsKey(adj))
                            parent[adj] = cur;
                        else
                            parent.Add(adj, cur);
                    }
                }
            }
        }


		Debug.Log ("startAtomn: "+startAtom.GetComponent<Atom>().Id);
		foreach (GameObject obj in table.Keys) {
			Debug.Log ("------------obj:" + obj.GetComponent<Atom> ().Id);
			foreach (GameObject bond in table[obj]) {
                if (bond != null)
                {
                    Bond b = bond.GetComponent<Bond>();
                    Debug.Log("       bond: " + b.A1.GetComponent<Atom>().Id + " " + b.A2.GetComponent<Atom>().Id);
                }
			}
		}

        return table;
    }

    private static string getSmiles(GameObject atom, Dictionary<GameObject, List<GameObject>> table)
    {
        List<GameObject> visited = new List<GameObject>();
        visited.Add(atom);
        Queue<GameObject> Q = new Queue<GameObject>();
        
		string symbol = atom.GetComponent<Atom>().Symbol;
		if (atomInRing.ContainsKey(atom))
			symbol = atomInRing[atom];
		string smiles = symbol;

        while (table[atom].Count > 0)
        {
            int cnt = table[atom].Count;
            for (int i = 0; i < cnt - 1; ++i)
            {
                GameObject obj = table[atom][i].GetComponent<Bond>().getAdjacent(atom);
                smiles += "(" + getSmiles(obj, table) + ")";
            }
            Bond b = table[atom][cnt - 1].GetComponent<Bond>();
            switch (b.Type)
            {
                case BondType.DOUBLE:
                    smiles += "=";
                    break;
                case BondType.TRIPLE:
                    smiles += "#";
                    break;
            }
            atom = b.getAdjacent(atom);
            symbol = atom.GetComponent<Atom>().Symbol;
            if (atomInRing.ContainsKey(atom))
            {
                symbol = atomInRing[atom];
                // find bond break in ring
                foreach (GameObject vbond in atom.GetComponent<Atom>().Bonds)
                {
                    if (vbond != null && !table[atom].Contains(vbond))
                    {
                        Bond vb = vbond.GetComponent<Bond>();
                        GameObject par = vb.getAdjacent(atom);
                        if(vb.InRing && !table[par].Contains(vbond))
                            switch (vb.Type)
                            {
                                case BondType.DOUBLE:
                                    symbol = symbol[0] + "=" + symbol.Substring(1);
                                    break;
                                case BondType.TRIPLE:
                                    symbol = symbol[0] + "#" + symbol.Substring(1);
                                    break;
                            }
                    }
                }
			}
			smiles += symbol;
        }

        // remove reduant ringN
        char flag = '0';
        int m = 0, n = smiles.Length - 1;
        while(m < n)
        {
            while (m < smiles.Length)
            {
                char c = smiles[m];
                if (c > '0' && c <= '9')
                {
                    flag = c;
                    break;
                }
                ++m;
            }
            if (n <= m)
                break;
            while (n > 0)
            {
                char c = smiles[n];
                if (c == flag)
                    break;
                --n;
            }
            if (n <= m)
                break;
            string tmp = smiles.Substring(m + 1, n - m);
            int f = flag - '0';
            string end = smiles.Substring(n);
            smiles = smiles.Substring(0, m + 1) + tmp.Replace(f.ToString(), "");
            m = smiles.Length - 1;
            smiles += end;
            n = smiles.Length - 1;

            m++;
        }


        return smiles;
    }

    public static string generateSMILES(GameObject mole)
    {
        ringN = 0;
        startAtom = null;
        atomInRing.Clear();
        Dictionary<GameObject, List<GameObject>> table = getLinkedTable(mole);

        string smiles = getSmiles(startAtom, table);

        return smiles;
    }

    #endregion


    #region /// load molecule from SMILES ///
    public static GameObject genAtom(char symbol)
    {
        GameObject atom = null;
        switch (symbol)
        {
            case 'C':
                atom = GameManager.GenerateAtom("C", 4);
                break;
            case 'O':
                atom = GameManager.GenerateAtom("O", -2);
                break;
            case 'N':
                atom = GameManager.GenerateAtom("N", -3);
                break;
            case 'P':
                atom = GameManager.GenerateAtom("P", -5);
                break;
            default: break;
        }
        return atom;
    }

    public static void loadSMILES(string smiles)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        Dictionary<int, GameObject> ring = new Dictionary<int, GameObject>();
        int length = smiles.Length, i = 0;
        GameObject atom = null;
        for(; i < length; ++i)
        {
            char c = smiles[i];
            switch (c)
            {
                case 'C':case 'O':
                case 'N':case 'P':
                    atom = genAtom(c);
                    break;
                case '=':
                    GameObject ne = genAtom(smiles[i+1]);
                    if(ne != null)
                    {
                        // connect atom and ne
                    }
                    break;
                case '#':
                    GameObject nex = genAtom(smiles[i+1]);
                    if (nex != null)
                    {
                        // connect atom and nex
                    }
                    break;
                case '(':
                    break;
                case ')':
                    break;
                default: break;
            }
            if (c > '0' && c <= '9')
            {
                int idx = c - '0';
                if (!ring.ContainsKey(idx))
                    ring.Add(idx, atom);
                else
                {
                    //connect exist atom to become a ring

                }
            }
        }
    }

    #endregion
}