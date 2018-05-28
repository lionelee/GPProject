using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {



    public GameObject newComponentPos;

    static Collider buildArea;
    public static Molecule curMolecule;
    static List<Molecule> molecules;
    Atom selectedAtom;
    int currentMoleId;


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
        int moleId = mole.GetComponent<ComponentInformation>().Id;
        foreach(Molecule m in molecules)
        {
            if(m.Id == moleId)
            {
                molecules.Remove(m);
            }
        }

        Destroy(mole);
        /*molecule.deleteAtomById(int.Parse(component.GetComponent<Text>().text));
        if(component.transform.parent.gameObject.name == "blob" && component.transform.parent.childCount == 1)
        {
            Destroy(component.transform.parent.gameObject);
        } else
        {
            Destroy(component);
        }*/

    }

    public Atom GetSelectedAtom()
    {
        return null;
    }


    /// <summary>
    /// Generate an atom model to the ray pointer,
    /// and new a Atom 
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="valence"></param>

    public void GenerateAtom(string symbol, int valence)
    {
        //Atom and Molecule
        Molecule molecule = new Molecule(currentMoleId++);
        Atom atom = AtomFactory.GetAtom(symbol, valence, molecule.CurrentAtomId++);
        molecule.addAtom(atom);
        molecules.Add(molecule);
        //model
        for (int i = 0; i < newComponentPos.transform.childCount; i++)
            DestroyObject(newComponentPos.transform.GetChild(i).gameObject);

        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
        GameObject mole = Instantiate(prefebMole);
        mole.GetComponent<ComponentInformation>().Id = molecule.Id;
        //mole.transform.parent = newComponentPos.transform;
		mole.transform.SetParent (newComponentPos.transform, true);
        mole.transform.Translate(newComponentPos.transform.position - mole.transform.position);

        GameObject prefebAtom;
        if (symbol == "C")
        {
            Debug.Log("here");
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Carbon") as GameObject;
            Debug.Log(prefebAtom == null);
        }
        else if (symbol == "H")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Hydrogen") as GameObject;

        }
        else if(symbol == "O")
        {
            prefebAtom = (GameObject)Resources.Load("_Prefebs/Oxygen") as GameObject;
        }
        else
        {
            prefebAtom = null;
        }

        GameObject generatedAtom = Instantiate(prefebAtom);
        //generatedAtom.transform.parent = mole.transform;
		generatedAtom.transform.SetParent(mole.transform, true);
        generatedAtom.transform.Translate(mole.transform.position-generatedAtom.transform.position);

        generatedAtom.GetComponent<ComponentInformation>().Id = atom.Id;

        //scale molecule's collider to fit the atom
        //mole.GetComponent<SphereCollider>().radius *= generatedAtom.transform.lossyScale.x;
        //CopyAtomCollider(generatedAtom, mole);
    }

}
