using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InteracteMode
{
    GRAB, SELECT
}

public class GameManager : MonoBehaviour
{
    static Collider buildArea;
    static public Molecule curMolecule;
    static public List<GameObject> bonds;
    static public List<GameObject> molecules;
    static public InteracteMode interacteMode;
    static GameObject selectedComponent;
    static int currentMoleId;


    // Use this for initialization
    void Start()
    {
        Config.Init();
        buildArea = GameObject.FindGameObjectWithTag("BuildArea").GetComponent<Collider>();
        molecules = new List<GameObject>();
        interacteMode = InteracteMode.GRAB;
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
        //mole.transform.parent = buildArea.gameObject.transform;
		mole.transform.SetParent(buildArea.transform, true);
        //component.transform.SetParent(blob.transform, false);

        //or add to it    
    }

    public static void RemoveMolecule(GameObject mole)
    {
        molecules.Remove(mole);
        Destroy(mole);
    }

    public static GameObject NewMolecule()
    {
        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
        GameObject mole = Instantiate(prefebMole);

        molecules.Add(mole);
        Molecule molecule = mole.AddComponent<Molecule>();
        molecule.Id = currentMoleId++;

        return mole;
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

    public static void SetRotatableMole(GameObject mole)
    {
        mole.GetComponent<Rotator>().ResetRotation();
        GameObject.FindGameObjectWithTag("LeftController").GetComponent<RotateController>().SetMolecule(mole);
    }

    public static void CancelRotatable()
    {
        GameObject.FindGameObjectWithTag("LeftController").GetComponent<RotateController>().RemoveMolecule();
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

        GameObject mole = NewMolecule();

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
        atom.Id = mole.GetComponent<Molecule>().CurrentAtomId++;
        atom.Symbol = symbol;
        atom.Valence = valence;
        atom.vbonds = new List<Vector4>(Config.BondAngleTable[symbol]);
        generatedAtom.transform.parent = mole.transform;
        generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);
    }

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

    public static void SwitchMode(InteracteMode mode)
    {
        interacteMode = mode;
        if (mode == InteracteMode.SELECT)
        {
            
            foreach (GameObject molecule in molecules)
            {

                for (int i = 0; i < molecule.transform.childCount; i++)
                {
                    GameObject child = molecule.transform.GetChild(i).gameObject;
                    if (child.GetComponent<Atom>() != null)
                    {
                        AtomsAction aa = child.AddComponent<AtomsAction>();
                        aa.touchHighlightColor = Config.UsingSelectedColor;
                        aa.isUsable = true;
                        aa.holdButtonToUse = false;
                    }
                    else
                    {
                        BondsAction ba = child.AddComponent<BondsAction>();
                        ba.touchHighlightColor = Config.UsingSelectedColor;
                        ba.isUsable = true;
                        ba.holdButtonToUse = false;
                    }
                }
            }

            GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowSelectAtomCanvas(false);
        } else
        {
            foreach(GameObject molecule in molecules)
            {
                molecule.GetComponent<MoleculesAction>().DisableAllComponent();
            }

        }
    }

}
