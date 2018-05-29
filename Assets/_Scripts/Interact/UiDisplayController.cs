using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDisplayController : MonoBehaviour {
    public GameObject ComponentOpCanvas;
    public GameObject SelectAtomCanvas;

    public void ShowComponentOpCanvas(bool stat)
    {
        if(stat == true)
        {
            ComponentOpCanvas.SetActive(true);
        } else
        {
            ComponentOpCanvas.SetActive(false);
        }
    }

    public void ShowSelectAtomCanvas(bool stat)
    {
        if (stat == true)
        {
            SelectAtomCanvas.SetActive(true);
        }
        else
        {
            SelectAtomCanvas.SetActive(false);
        }
    }

}
