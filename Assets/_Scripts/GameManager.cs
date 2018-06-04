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
    static public GameObject prefebMole;
    static GameObject selectedComponent;
    static int currentMoleId;


    // Use this for initialization
    void Start()
    {
        Config.Init();
        buildArea = GameObject.FindGameObjectWithTag("BuildArea").GetComponent<Collider>();
        molecules = new List<GameObject>();
        currentMoleId = 0;
        prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
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
        //mole.transform.parent = buildArea.gameObject.transform;
		mole.transform.SetParent(buildArea.transform, true);
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

    public static void RemoveAtom(GameObject atom)
    {
        //last atom;
        if(atom.transform.parent.childCount == 1)
        {
            Destroy(atom.transform.parent.gameObject);
        }
    }

    public static GameObject GetSelectedComponent()
    {
        return selectedComponent;
    }

    public static void SetSelectedComponent(GameObject component)
    {
        selectedComponent = component;
    }

    public static void CancelComponentSelected()
    {
        selectedComponent = null;
    }
    

    public static void GenerateAtom(string symbol, int valence)
    {
        GameObject newComponentPos = GameObject.FindGameObjectWithTag("NewComponentPos");
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

        Atom atom = generatedAtom.AddComponent<Atom>();
        atom.Id = molecule.CurrentAtomId++;
        atom.Symbol = symbol;
        atom.Valence = valence;
        atom.vbonds = new List<Vector4>(Config.BondAngleTable[symbol]);
        generatedAtom.transform.parent = mole.transform;
        generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);
    }

    #region /* methods for operating bonds */
    //connect two atoms by selection
    public void ConnectAtoms(GameObject obj1, GameObject obj2)
    { }

    //disconnect two atoms by selection
    public void DisonnectAtoms(GameObject obj1, GameObject obj2)
    {

    }
    #endregion

    #region /* methods for importing model from file */

    public GameObject GenerateMolecule()
    {
        GameObject mole = Instantiate(prefebMole);
        molecules.Add(mole);
        Molecule molecule = mole.AddComponent<Molecule>();
        molecule.Id = currentMoleId++;
        return mole;
    }

    public void GenAtomForMole(string[] info, GameObject mole)
    {
        if (mole == null) return;
        string symbol = info[0];
        GameObject prefebAtom = null;
        switch (symbol)
        {
            case "C":
                prefebAtom = (GameObject)Resources.Load("_Prefebs/Carbon") as GameObject;
                break;
            case "H":
                prefebAtom = (GameObject)Resources.Load("_Prefebs/Hydrogen") as GameObject;
                break;
            case "O":
                prefebAtom = (GameObject)Resources.Load("_Prefebs/Oxygen") as GameObject;
                break;
            default: return;
        }

        GameObject generatedAtom = Instantiate(prefebAtom);
        generatedAtom.transform.position = TypeConvert.StrToVec3(info[3]);
        Atom atom = prefebAtom.AddComponent<Atom>();
        atom.Id = int.Parse(info[1]);
        atom.Symbol = symbol;
        atom.Valence = int.Parse(info[2]);
        atom.vbonds = Config.BondAngleTable[symbol];

        generatedAtom.transform.parent = mole.transform;
        //generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);
    }

    public void GenBondForMole(int type, int a1, int a2, GameObject mole)
    {
        if (mole == null) return;
        GameObject prefebBond = null;

        switch (type)
        {
            case 0:
                prefebBond = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
                break;
            case 1:
                prefebBond = (GameObject)Resources.Load("_Prefebs/DoubleBond") as GameObject;
                break;
            case 2:
                prefebBond = (GameObject)Resources.Load("_Prefebs/TrippleBond") as GameObject;
                break;
            default:
                break;
        }
        GameObject generatedBond = Instantiate(prefebBond);

        // find atoms connected by this bond
        GameObject atom1 = null, atom2 = null;
        foreach(Transform child in mole.transform)
        {
            if (child.tag != "Bond" && child.tag != "Component")
                continue;
            if (child.tag != "Bond")
            {
                if (child.GetComponent<Atom>().Id == a1)
                    atom1 = child.gameObject;
                if (child.GetComponent<Atom>().Id == a2)
                    atom2 = child.gameObject;
            }
        }
        if (atom1 == null || atom2 == null)
            return;

        // locate the bond
        Vector3 pos1 = atom1.transform.position, pos2 = atom2.transform.position;
        generatedBond.transform.position = Vector3.Lerp(pos1, pos2, 0.5f);
        Vector3 scale = prefebBond.transform.lossyScale;
        scale.y = Vector3.Distance(pos1, pos2) * 0.5f;
        generatedBond.transform.localScale = scale;
        generatedBond.transform.LookAt(pos2);
        generatedBond.transform.Rotate(new Vector3(90, 0, 0));

        // set abstract bond
        Bond b = generatedBond.AddComponent<Bond>();
        b.A1 = atom1;
        b.A2 = atom2;
        b.Type = BondType.SINGLE;
        atom1.GetComponent<Atom>().addBond(generatedBond);
        atom2.GetComponent<Atom>().addBond(generatedBond);
    }
    #endregion
}
