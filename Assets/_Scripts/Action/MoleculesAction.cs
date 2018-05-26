using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MoleculesAction : VRTK_InteractableObject
{

    public override void Grabbed(VRTK_InteractGrab grabbingObject)
    {
        base.Grabbed(grabbingObject);
		/*grabbingObject.gameObject.GetComponent<RotateController> ().SetMolecule (gameObject);
		gameObject.GetComponent<Rotator> ().enabled = true;
		gameObject.GetComponent<Rotator> ().reset ();*/
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);
		/*gameObject.GetComponent<Rotator> ().enabled = false;
		previousGrabbingObject.gameObject.GetComponent<RotateController> ().RemoveMolecule ();*/
        if (GameManager.MoleculeInBuildArea(gameObject))
        {
            print("in build area");
            GameManager.PutIntoBuildArea(gameObject);
        } else
        {
            print("out of build area");
            GameManager.RemoveMolecule(gameObject);
        }

    }

	/*void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e){
		gameObject.GetComponent<Rotator> ().SetTouchAxis (e.touchpadAxis);
	}*/

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("use molecule");
        base.StartUsing(usingObject);
        gameObject.GetComponent<Collider>().enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
            gameObject.transform.GetChild(i).gameObject.GetComponent<Collider>().enabled = true;

    }

}
