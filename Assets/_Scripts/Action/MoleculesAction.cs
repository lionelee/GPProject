using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;




public class MoleculesAction : VRTK_InteractableObject
{

    private Color selectedColor = new Color(26 / 255.0f, 160 / 255.0f, 1, 0);

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
        List<GameObject> list = GetTouchingObjects();
        print(list[0].GetComponent<VRTK_InteractTouch>().GetTouchedObject());
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            AtomsAction aa = child.AddComponent<AtomsAction>();
            aa.touchHighlightColor = selectedColor;
            aa.isUsable = true;
            aa.holdButtonToUse = false;
        }
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {
        base.StopUsing(usingObject);
        usingObject.ForceStopUsing();
        ForceStopInteracting();
    }

    public void RemoveAtomsAction()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            DestroyObject(child.GetComponent<AtomsAction>());
        }
    }

}
