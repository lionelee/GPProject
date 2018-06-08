using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;




public class MoleculesAction : VRTK_InteractableObject
{
    GameObject connectableAtom;

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
		GameManager.SetLinearMovableMole(gameObject);
        foreach (Assembler assembler in gameObject.GetComponentsInChildren<Assembler>())
        {
            assembler.SetGrabbed();
        }
		/*grabbingObject.gameObject.GetComponent<RotateController> ().SetMolecule (gameObject);
		gameObject.GetComponent<Rotator> ().enabled = true;
		gameObject.GetComponent<Rotator> ().reset ();*/
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        GameManager.CancelLinearMovable();
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
                ResetConnectable();
            }
        }
        else
        {
            GameManager.RemoveMolecule(gameObject);
        }

    }
     
    //切换到选择单个原子的模式，这个操作应当应用到所有场景中的分子上，并且此时应该禁止生成新原子
    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        GameManager.SetRotatableMole(gameObject);
        GameManager.SwitchMode(InteracteMode.SELECT);
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
            else if(child.GetComponent<Bond>() != null)
            {
                child.GetComponent<BondsAction>().disableWhenIdle = false;
                child.GetComponent<BondsAction>().enabled = false;
            }
        }

        StartCoroutine(CountDownToRemoveAction());
    }

    IEnumerator CountDownToRemoveAction()
    {
        yield return new WaitForSeconds(0.2f);
        RemoveComponentsAction();
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
            else if(child.GetComponent<Bond>() != null)
            {
                child.GetComponent<BondsAction>().ForceStopInteracting();
                DestroyObject(child.GetComponent<BondsAction>());
            }
        }

    }

    public Vector3 GetJointPosition()
    {
        Vector3 pos = new Vector3();
        foreach(Transform child in transform)
        {
            if(child.tag != "Component" && child.tag != "Bond")
            {
                pos = child.transform.position;
                break;
            }
        }
        return pos;
    }

}
