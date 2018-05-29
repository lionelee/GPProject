using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    //public GameObject newComponentPos;

    static Collider buildArea;
    static public Molecule curMolecule;
    static public List<GameObject> objs;
    static List<Molecule> molecules;
    static GameObject selectedComponent;
    static int currentMoleId;


	// Use this for initialization
	void Start () {
        Config.Init();
        buildArea = GameObject.FindGameObjectWithTag("BuildArea").GetComponent<Collider>();
        molecules = new List<Molecule>();
        currentMoleId = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
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
        GameObject[] atoms = mole.GetComponentsInChildren<GameObject>();
       foreach(GameObject atom in atoms)
        {
            Destroy(atom);
        }
    }

    public static void RemoveAtom(GameObject atom)
    {
        
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

    /// <summary>
    /// Generate an atom model to the ray pointer,
    /// and new a Atom 
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="valence"></param>

    public static void GenerateAtom(string symbol, int valence)
    {
        GameObject newComponentPos = GameObject.FindGameObjectWithTag("NewComponentPos");
        // clear atoms in component area
        for (int i = 0; i < newComponentPos.transform.childCount; i++)
            DestroyObject(newComponentPos.transform.GetChild(i).gameObject);

        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
        GameObject mole = Instantiate(prefebMole);

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
        atom.vbonds  = Config.BondAngleTable[symbol];
        generatedAtom.transform.parent = mole.transform;
        generatedAtom.transform.Translate(mole.transform.position-generatedAtom.transform.position);
    }


}
