using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDisplayController : MonoBehaviour {
    public GameObject ComponentOpCanvas;
    public GameObject SelectAtomCanvas;

    public void ShowComponentOpCanvas(bool stat, GameObject compnonet)
    {
        if(stat == true)
        {
            if(compnonet.GetComponent<Atom>() != null)
            {
                ComponentOpCanvas.GetComponentInChildren<Text>().text = "Atom " + compnonet.GetComponent<Atom>().Symbol;
            }
            else
            {
                ComponentOpCanvas.GetComponentInChildren<Text>().text = "Bond ";
                switch (compnonet.GetComponent<Bond>().Type)
                {
                    case BondType.SINGLE:
                        ComponentOpCanvas.GetComponentInChildren<Text>().text += "Single";
                        break;
                    case BondType.DOUBLE:
                        ComponentOpCanvas.GetComponentInChildren<Text>().text += "Double";
                        break;
                    case BondType.TRIPLE:
                        ComponentOpCanvas.GetComponentInChildren<Text>().text += "Triple";
                        break;
                    default:
                        break;
                }
            }
            GetComponent<ButtonEvents>().SetComponentOpButton(compnonet);
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
