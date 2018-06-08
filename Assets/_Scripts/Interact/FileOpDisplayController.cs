using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class FileOpDisplayController : VRTK_InteractableObject
{

    public GameObject FileOpCanvas;

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        FileOpCanvas.SetActive(true);
    }
}
