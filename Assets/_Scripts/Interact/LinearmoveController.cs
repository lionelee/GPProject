using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LinearmoveController : MonoBehaviour {

    GameObject molecule;

    public void SetMolecule(GameObject mole)
    {
        molecule = mole;
    }

    public void RemoveMolecule()
    {
        molecule = null;
    }

    // Use this for initialization
    void Start()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
    }

    void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        if (molecule != null)
            molecule.GetComponent<LinearMover>().SetTouchAxis(e.touchpadAxis);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
