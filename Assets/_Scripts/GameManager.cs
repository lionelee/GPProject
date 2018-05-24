using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    //max size is 117 scale 0.6
    //h is 
    static Collider buildArea;

    public GameObject newAtomPos;

    static Molecule molecule;
    Atom selectedAtom;


	// Use this for initialization
	void Start () {
        Config.Init();
        buildArea = GameObject.FindGameObjectWithTag("BuildArea").GetComponent<Collider>();
        molecule = new Molecule();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static bool ComponentInBuildArea(GameObject component)
    {
        return buildArea.bounds.Contains(component.transform.position);
    }

    public static void PutIntoBuildArea(GameObject component)
    {
        //as new
        //buildArea.gameObject.
        GameObject blob = new GameObject("blob");
        blob.transform.parent = buildArea.gameObject.transform;
        component.transform.parent = blob.transform;
        //component.transform.SetParent(blob.transform, false);

        //or add to it    
    }

    public static void RemoveComponent(GameObject component)
    {
        molecule.deleteAtomById(int.Parse(component.GetComponent<Text>().text));
        if(component.transform.parent.gameObject.name == "blob" && component.transform.parent.childCount == 1)
        {
            Destroy(component.transform.parent.gameObject);
        } else
        {
            Destroy(component);
        }
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
        //Atom
        Atom atom = AtomFactory.GetAtom(symbol, valence);
        molecule.addAtom(atom);
        //model
        for (int i = 0; i < newAtomPos.transform.childCount; i++)
            DestroyObject(newAtomPos.transform.GetChild(i).gameObject);
        Debug.Log(symbol);
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
        generatedAtom.transform.parent = newAtomPos.transform;
        generatedAtom.transform.Translate(newAtomPos.transform.position-generatedAtom.transform.position);

        generatedAtom.GetComponent<Text>().text = atom.Id.ToString();

    }


}
