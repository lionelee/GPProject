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
		GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

	}

    void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        if (molecule != null)
            molecule.GetComponent<LinearMover>().SetTouchAxis(e.touchpadAxis);
    }

	private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
	{
		if(molecule != null)
			molecule.GetComponent<LinearMover> ().SetTouchAxis (Vector2.zero);
	}


    // Update is called once per frame
    void Update()
    {
        if (molecule != null)
        {
            if (Input.GetKey(KeyCode.I))
            {
                molecule.GetComponent<LinearMover>().SetTouchAxis(new Vector2(0, 1));
            }
            else if (Input.GetKey(KeyCode.K))
            {
                molecule.GetComponent<LinearMover>().SetTouchAxis(new Vector2(0, -1));
            }
            else
            {
                molecule.GetComponent<LinearMover>().SetTouchAxis(Vector2.zero);
            }
        }
    }
}
