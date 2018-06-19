using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using System;

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
    static bool connectable;
    static int currentMoleId;

    static public GameObject prefebMole;
    static public GameObject prefebCarbon;
    static public GameObject prefebHydrogen;
    static public GameObject prefebOxygen;
    static public GameObject prefebSingleBond;
    static public GameObject prefebDoubleBond;
    static public GameObject prefebTrippleBond;


    // Use this for initialization
    void Start()
    {
        Config.Init();
        buildArea = GameObject.FindGameObjectWithTag("BuildArea").GetComponent<Collider>();
        molecules = new List<GameObject>();
        interacteMode = InteracteMode.GRAB;
        currentMoleId = 0;
        //load prefeb
        prefebMole = (GameObject)Resources.Load("_Prefebs/Molecule") as GameObject;
        prefebCarbon = (GameObject)Resources.Load("_Prefebs/Carbon") as GameObject;
        prefebHydrogen = (GameObject)Resources.Load("_Prefebs/Hydrogen") as GameObject;
        prefebOxygen = (GameObject)Resources.Load("_Prefebs/Oxygen") as GameObject;
        prefebSingleBond = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        prefebDoubleBond = (GameObject)Resources.Load("_Prefebs/DoubleBond") as GameObject;
        prefebTrippleBond = (GameObject)Resources.Load("_Prefebs/TrippleBond") as GameObject;
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
		mole.transform.SetParent(buildArea.transform, true);
    }
    

    #region /*state-related operation*/
    public static GameObject GetSelectedComponent()
    {
        return selectedComponent;
    }

    public static void SetSelectedComponent(GameObject component)
    {
        selectedComponent = component;
    }

    public static bool IsConnectable()
    {
        return connectable;
    }
    public static void SetConnectable(bool stat)
    {
        connectable = stat;
    }

    public static void CancelComponentSelected()
    {
        selectedComponent = null;
    }

    public static void SetRotatableMole(GameObject mole)
    {
        VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<RotateController>().SetMolecule(mole);
    }

    public static void CancelRotatable()
    {
        VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<RotateController>().RemoveMolecule();
    }

    public static void SetLinearMovableMole(GameObject mole)
    {
        VRTK_DeviceFinder.GetControllerRightHand().GetComponent<LinearmoveController>().SetMolecule(mole);
    }

    public static void CancelLinearMovable()
    {
        VRTK_DeviceFinder.GetControllerRightHand().GetComponent<LinearmoveController>().RemoveMolecule();
    }
    #endregion

    #region /*methods for interaction*/
    public static void RemoveMolecule(GameObject mole)
    {
        molecules.Remove(mole);
        Destroy(mole);
    }

    public static GameObject NewMolecule()
    {
        GameObject mole = Instantiate(prefebMole);

        molecules.Add(mole);
        Molecule molecule = mole.AddComponent<Molecule>();
        molecule.Id = currentMoleId++;
        molecule.CurrentAtomId = 0;
        molecule.AtomNum = 1;

        return mole;
    }

    public static void RemoveAtom(GameObject atom)
    {
        //last atom;
        if (atom.transform.parent.childCount == 1)
        {
            Destroy(atom.transform.parent.gameObject);
        }
    }

    public static void GenerateAtom(string symbol, int valence)
    {
        GameObject newComponentPos = GameObject.FindGameObjectWithTag("NewComponentPos");
        // clear atoms in component area
        for (int i = 0; i < newComponentPos.transform.childCount; i++)
            DestroyObject(newComponentPos.transform.GetChild(i).gameObject);

        GameObject mole = NewMolecule();

        mole.transform.parent = newComponentPos.transform;
        mole.transform.Translate(newComponentPos.transform.position - mole.transform.position);
        
        GameObject generatedAtom = null;
        if (symbol == "C")
        {
            generatedAtom = Instantiate(prefebCarbon);
        }
        else if (symbol == "H")
        {
            generatedAtom = Instantiate(prefebHydrogen);
        }
        else if (symbol == "O")
        {
            generatedAtom = Instantiate(prefebOxygen);
        }
        else return;

        Atom atom = generatedAtom.AddComponent<Atom>();
        atom.Id = mole.GetComponent<Molecule>().CurrentAtomId++;
        atom.Symbol = symbol;
        atom.Valence = valence;
        atom.vbonds = new List<Vector4>(Config.BondAngleTable[symbol]);
        generatedAtom.transform.parent = mole.transform;
        generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);
    }
    #endregion

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

    public GameObject GenerateMolecule(int num)
    {
        GameObject mole = Instantiate(prefebMole);
        molecules.Add(mole);
        mole.transform.SetParent(buildArea.transform, true);
        Molecule molecule = mole.AddComponent<Molecule>();
        molecule.Id = currentMoleId++;
        molecule.CurrentAtomId = 0;
        molecule.AtomNum = num;
        return mole;
    }

    public void GenAtomForMole(string[] info, GameObject mole)
    {
        if (mole == null) return;
        string symbol = info[0];
        GameObject generatedAtom = null;
        switch (symbol)
        {
            case "C":
                generatedAtom = Instantiate(prefebCarbon);
                break;
            case "H":
                generatedAtom = Instantiate(prefebHydrogen);
                break;
            case "O":
                generatedAtom = Instantiate(prefebOxygen);
                break;
            default: return;
        }
        
        generatedAtom.transform.position = TypeConvert.StrToVec3(info[3]);
        Atom atom = generatedAtom.AddComponent<Atom>();
        atom.Id = int.Parse(info[1]);
        atom.Symbol = symbol;
        atom.Connected = 0;
        atom.Valence = int.Parse(info[2]);
        atom.vbonds = new List<Vector4>(Config.BondAngleTable[symbol]);

        generatedAtom.transform.parent = mole.transform;
        mole.GetComponent<Molecule>().CurrentAtomId++;
        //generatedAtom.transform.Translate(mole.transform.position - generatedAtom.transform.position);
    }

    public void GenBondForMole(string[] info, GameObject mole)
    {
        if (mole == null) return;
        GameObject generatedBond = null;
        BondType btype = BondType.SINGLE;

        string type = info[1];
        switch (type)
        {
            case "SINGLE":
                generatedBond = Instantiate(prefebSingleBond);
                btype = BondType.SINGLE;
                break;
            case "DOUBLE":
                generatedBond = Instantiate(prefebDoubleBond);
                btype = BondType.DOUBLE;
                break;
            case "TRIPPLE":
                generatedBond = Instantiate(prefebTrippleBond);
                btype = BondType.TRIPLE;
                break;
            default:
                break;
        }

        // find atoms connected by this bond
        int a1 = int.Parse(info[2]), a2 = int.Parse(info[4]);
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
        Vector3 scale = generatedBond.transform.lossyScale;
        scale.y = Vector3.Distance(pos1, pos2) * 0.5f;
        generatedBond.transform.localScale = scale;
        generatedBond.transform.LookAt(pos2);
        generatedBond.transform.Rotate(new Vector3(90, 0, 0));
        generatedBond.transform.parent = mole.transform;

        // set abstract bond
        Bond b = generatedBond.AddComponent<Bond>();
        b.A1 = atom1;
        b.A2 = atom2;
        b.Type = btype;
        Atom at1 = atom1.GetComponent<Atom>();
        at1.addBond(generatedBond);
        Atom at2 = atom2.GetComponent<Atom>();
        at2.addBond(generatedBond);

        // mark atom's vbond
        at1.setVbondUsed(int.Parse(info[3]));
        at2.setVbondUsed(int.Parse(info[5]));
    }
    #endregion

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

    public static void ChangeBondType(GameObject oldBond, BondType newType)
    {
        Bond bondInfo = oldBond.GetComponent<Bond>();
        GameObject Atom1 = bondInfo.A1,
            Atom2 = bondInfo.A2;
        Atom Atom1Info = bondInfo.A1.GetComponent<Atom>(),
            Atom2Info = bondInfo.A2.GetComponent<Atom>();
        AtomsAction Atom1Action = bondInfo.A1.GetComponent<AtomsAction>(),
            Atom2Action = bondInfo.A2.GetComponent<AtomsAction>();
        //可能需要单独处理在环上时的情况, 现在未特殊处理，在处理 环上双键到单键 这一操作时要经历，断键-》将环还原为碳链->重新连接成环
        bool newBondOnRing;
        int a1Idx, a2Idx;
        switch (newType)
        {
            case BondType.SINGLE:
                if (bondInfo.Type == BondType.SINGLE)
                    return;
                oldBond.GetComponent<BondsAction>().Break();
                Atom1.GetComponent<Assembler>().SelectionConnect(Atom2);
                break;
            case BondType.DOUBLE:
                if (bondInfo.Type == BondType.DOUBLE)
                {
                    print("already double bond");
                    return;
                }
                //upgrade, check free bond
                else if (bondInfo.Type == BondType.SINGLE)
                {

                    if (Mathf.Abs(Atom1Info.Valence) - Atom1Info.Connected < 1 ||
                        Mathf.Abs(Atom2Info.Valence) - Atom2Info.Connected < 1)
                    {
                        return;
                    }

                    newBondOnRing = bondInfo.InRing;
                    oldBond.GetComponent<BondsAction>().TmpBreak();
                    //}
                    a1Idx = Atom1.GetComponent<AtomsAction>().VbondSwitchWithNewBond(newType, newBondOnRing);
                    a2Idx = Atom2.GetComponent<AtomsAction>().VbondSwitchWithNewBond(newType, newBondOnRing);

                    Atom1.GetComponent<Assembler>().AccurateConnect(Atom1, a1Idx, Atom2, a2Idx, BondType.DOUBLE, newBondOnRing);

                }
                //degrade
                else if (bondInfo.Type == BondType.TRIPLE)
                {
                    newBondOnRing = bondInfo.InRing;
                    oldBond.GetComponent<BondsAction>().TmpBreak();
                    a1Idx = Atom1.GetComponent<AtomsAction>().VbondSwitchWithNewBond(newType, newBondOnRing);
                    a2Idx = Atom2.GetComponent<AtomsAction>().VbondSwitchWithNewBond(newType, newBondOnRing);

                    Atom1.GetComponent<Assembler>().AccurateConnect(Atom1, a1Idx, Atom2, a2Idx, BondType.DOUBLE, newBondOnRing);
                }
                break;
            case BondType.TRIPLE:
                if (bondInfo.Type == BondType.TRIPLE)
                    return;

                if (bondInfo.Type == BondType.SINGLE && (Mathf.Abs(Atom1Info.Valence) - Atom1Info.Connected < 2 ||
                    Mathf.Abs(Atom2Info.Valence) - Atom2Info.Connected < 2))
                {
                    return;
                }

                if (bondInfo.Type == BondType.DOUBLE && (Mathf.Abs(Atom1Info.Valence) - Atom1Info.Connected < 1 ||
                    Mathf.Abs(Atom2Info.Valence) - Atom2Info.Connected < 1))
                {
                    return;
                }

                newBondOnRing = bondInfo.InRing;
                oldBond.GetComponent<BondsAction>().TmpBreak();
                a1Idx = Atom1.GetComponent<AtomsAction>().VbondSwitchWithNewBond(newType, newBondOnRing);
                a2Idx = Atom2.GetComponent<AtomsAction>().VbondSwitchWithNewBond(newType, newBondOnRing);

                Atom1.GetComponent<Assembler>().AccurateConnect(Atom1, a1Idx, Atom2, a2Idx, BondType.TRIPLE, newBondOnRing);
                break;
        }

        Atom1Info.UpdateMaxBond();
        Atom2Info.UpdateMaxBond();

    }
}
