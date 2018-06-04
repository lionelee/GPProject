using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;




public class MoleculesAction : VRTK_InteractableObject
{
    GameObject connectableAtom;
    private Color selectedColor = new Color(26 / 255.0f, 160 / 255.0f, 1, 0);

    public void SetConnectableAtom(GameObject atom)
    {
        connectableAtom = atom;
    }

    public void ResetConnectable()
    {
        connectableAtom = null;
    }

    public override void Grabbed(VRTK_InteractGrab grabbingObject)
    {
        base.Grabbed(grabbingObject);
        foreach(Assembler assembler in gameObject.GetComponentsInChildren<Assembler>())
        {
            assembler.SetGrabbed();
        }
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
            GameManager.PutIntoBuildArea(gameObject);
            foreach (Assembler assembler in gameObject.GetComponentsInChildren<Assembler>())
            {
                assembler.ResetGrabbed();
            }
            if (connectableAtom != null)
            {
                Assembler assembler = connectableAtom.GetComponent<Assembler>();
                assembler.Connect();
                //Destroy(assembler);
                ResetConnectable();
            }
        }
        else
        {
            GameManager.RemoveMolecule(gameObject);
        }

    }

	/*void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e){
		gameObject.GetComponent<Rotator> ().SetTouchAxis (e.touchpadAxis);
	}*/

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        List<GameObject> list = GetTouchingObjects();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.GetComponent<Atom>() != null)
            {
                AtomsAction aa = child.AddComponent<AtomsAction>();
                aa.touchHighlightColor = selectedColor;
                aa.isUsable = true;
                aa.holdButtonToUse = false;
            } else
            {
                BondsAction ba = child.AddComponent<BondsAction>();
                ba.touchHighlightColor = selectedColor;
                ba.isUsable = true;
                ba.holdButtonToUse = false;
            }
        }
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {
        base.StopUsing(usingObject);
        usingObject.ForceStopUsing();
        ForceStopInteracting();
    }

    public void DisableAllComponent()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.GetComponent<Atom>() != null)
            {
                child.GetComponent<AtomsAction>().disableWhenIdle = false;
                child.GetComponent<AtomsAction>().enabled = false;

            }
            else
            {
                child.GetComponent<BondsAction>().disableWhenIdle = false;
                child.GetComponent<BondsAction>().enabled = false;
            }
        }
    }

    public void RemoveComponentsAction()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.GetComponent<Atom>() != null)
            {
                child.GetComponent<AtomsAction>().ForceStopInteracting();
                DestroyObject(child.GetComponent<AtomsAction>());
            }
            else
            {
                child.GetComponent<BondsAction>().ForceStopInteracting();
                DestroyObject(child.GetComponent<BondsAction>());
            }
        }

    }

}
