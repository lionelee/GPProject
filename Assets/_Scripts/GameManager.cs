using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public GameObject newComponentPos;

    static Collider buildArea;
    static public Molecule curMolecule;
    static public List<GameObject> bonds;
    static public List<GameObject> molecules;
    Atom selectedAtom;
    int currentMoleId;


    // Use this for initialization
    void Start()
    {
        Config.Init();
        buildArea = GameObject.FindGameObjectWithTag("BuildArea").GetComponent<Collider>();
        molecules = new List<GameObject>();
        currentMoleId = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static bool MoleculeInBuildArea(GameObject mole)
    {
        return buildArea.bounds.Contains(mole.transform.position);
    }

    public static void PutIntoBuildArea(GameObject mole)
    {
        //as new
        //buildArea.gameObject.
        mole.transform.parent = buildArea.gameObject.transform;
        //component.transform.SetParent(blob.transform, false);

        //or add to it    
    }

    public static void RemoveMolecule(GameObject mole)
    {
        Destroy(mole);
        molecules.Remove(mole);
        foreach (Transform child in mole.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Atom GetSelectedAtom()
    {
        return null;
    }

    public void GenerateAtom(string symbol, int valence)
    {
        // clear atoms in component area
        for (int i = 0; i < newComponentPos.transform.childCount; i++)
            DestroyObject(newComponentPos.transform.GetChild(i).gameObject);

        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
        GameObject mole = Instantiate(prefebMole);
        molecules.Add(mole);
        Molecule molecule = mole.AddComponent<Molecule>();
        molecule.Id = currentMoleId++;

        mole.transform.parent = newComponentPos.transform;
        mole.transform.Translate(newComponentPos.transform.position - mole.transform.position);

        GameObject prefebAtom;
        if (symbol == "C")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Carbon") as GameObject;
        }
        else if (symbol == "H")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Hydrogen") as GameObject;

        }
        else if (symbol == "O")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Oxygen") as GameObject;
        }
        else return;

        GameObject generatedAtom = Instantiate(prefebAtom);
        Atom atom = prefebAtom.AddComponent<Atom>();
        atom.Id = molecule.CurrentAtomId++;
        atom.Symbol = symbol;
        atom.Valence = valence;
        atom.vbonds = Config.BondAngleTable[symbol];
        generatedAtom.transform.parent = mole.transform;
        generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);

        //scale molecule's collider to fit the atom
        mole.GetComponent<SphereCollider>().radius *= generatedAtom.transform.lossyScale.x;
    }

    /* methods for operating bonds */
    public void ConnectAtoms(GameObject obj1, GameObject obj2)
    {
        Atom a1 = obj1.GetComponent<Atom>();
        Atom a2 = obj2.GetComponent<Atom>();

        /* connect atoms as a ring in one molecule */
        if(obj1.transform.parent == obj2.transform.parent)
        {
            
            return;
        }

        /* connect two atoms from different molecules */
        //calculate expected position
        string[] pair = new string[] { a1.Symbol, a2.Symbol };
        float length = Config.BondLengthTable[pair];
        Vector3 pos = obj1.transform.position;
        Vector3 angle = a1.getAngle();
        Vector3 expectedPos = angle * length + pos;

        //draw bond
        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        GameObject bond = Instantiate(prefebMole);
        bond.transform.position = Vector3.Lerp(pos, expectedPos, 0.5f);
        Vector3 scale = prefebMole.transform.lossyScale;
        scale.y = length;
        bond.transform.localScale = scale;
        bond.transform.rotation = Quaternion.LookRotation(pos, expectedPos);
        GameManager.bonds.Add(bond);

        //set abstract bond
        Bond b = bond.AddComponent<Bond>();
        b.A1 = a1.Id;
        b.A2 = a2.Id;
        b.Type = BondType.SINGLE;
    }


    /* methods for importing model from file */
    public GameObject GenerateMolecule()
    {
        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
        GameObject mole = Instantiate(prefebMole);
        molecules.Add(mole);
        Molecule molecule = mole.AddComponent<Molecule>();
        molecule.Id = currentMoleId++;
        return mole;
    }

    public void GenAtomForMole(string[] info, GameObject mole)
    {
        string symbol = info[0];
        GameObject prefebAtom;
        if (symbol == "C")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Carbon") as GameObject;
        }
        else if (symbol == "H")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Hydrogen") as GameObject;

        }
        else if (symbol == "O")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Oxygen") as GameObject;
        }
        else return;

        GameObject generatedAtom = Instantiate(prefebAtom);
        generatedAtom.transform.position = TypeConvert.StrToVec3(info[3]);
        Atom atom = prefebAtom.AddComponent<Atom>();
        atom.Id = int.Parse(info[1]);
        atom.Symbol = symbol;
        atom.Valence = int.Parse(info[2]);
        atom.vbonds = Config.BondAngleTable[symbol];

        generatedAtom.transform.parent = mole.transform;
        generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);
    }
}
