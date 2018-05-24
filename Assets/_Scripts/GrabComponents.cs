using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GrabComponents : VRTK_InteractableObject
{

    public override void Grabbed(VRTK_InteractGrab grabbingObject)
    {
        base.Grabbed(grabbingObject);
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);
        if (GameManager.ComponentInBuildArea(gameObject))
        {
            print("in build area");
            GameManager.PutIntoBuildArea(gameObject);
        } else
        {
            print("out of build area");
            GameManager.RemoveComponent(gameObject);
        }

    }
}
