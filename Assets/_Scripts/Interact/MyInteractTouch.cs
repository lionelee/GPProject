using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MyInteractTouch : VRTK_InteractTouch
{

    protected override GameObject GetColliderInteractableObject(Collider collider)
    {
        if (collider.isTrigger)
            return null;
        return base.GetColliderInteractableObject(collider);
    }
}
