using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;



public class AtomsAction : VRTK_InteractableObject
{

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("use molecule");
        base.StartUsing(usingObject);
        print("atom id: " + gameObject.GetComponent<ComponentInformation>().Id);

        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.transform.parent.gameObject.GetComponent<SphereCollider>().enabled = true;

    }
}
