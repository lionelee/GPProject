using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MoleculesAction : VRTK_InteractableObject
{

    public override void Grabbed(VRTK_InteractGrab grabbingObject)
    {
        base.Grabbed(grabbingObject);
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);
        if (GameManager.MoleculeInBuildArea(gameObject))
        {
            print("in build area");
            GameManager.PutIntoBuildArea(gameObject);
            Assembler assembler = gameObject.GetComponentInChildren<Assembler>();
            assembler.Connect();
            Destroy(gameObject.GetComponentInChildren<Assembler>());
        }
        else
        {
            print("out of build area");
            GameManager.RemoveMolecule(gameObject);
        }

    }
    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("use molecule");
        base.StartUsing(usingObject);
        gameObject.GetComponent<Collider>().enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
            gameObject.transform.GetChild(i).gameObject.GetComponent<Collider>().enabled = true;
    }

}
