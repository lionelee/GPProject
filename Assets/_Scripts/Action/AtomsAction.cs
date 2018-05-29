using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;



public class AtomsAction : VRTK_InteractableObject
{
    public override void StartUsing(VRTK_InteractUse usingObject)
    {

        base.StartUsing(usingObject);
        GameManager.SetSelectedComponent(gameObject);
        GameObject OpCanvas = GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ComponentOpCanvas;
        OpCanvas.GetComponentInChildren<Text>().text = "Atom " + gameObject.GetComponent<Atom>().Symbol;
        OpCanvas.SetActive(true);
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {

        base.StopUsing(usingObject);
        //usingObject.ForceStopUsing();
        //ForceStopInteracting();

        GameManager.CancelComponentSelected();
        GameObject OpCanvas = GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ComponentOpCanvas;
        OpCanvas.SetActive(false);

        disableWhenIdle = false;
        enabled = false;
        
        
        StartCoroutine(CountDown());
        
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponentInParent<MoleculesAction>().RemoveAtomsAction();
    }

    private void ShowComponentOperationCanvas()
    {
        
        print("atom id: " + gameObject.GetComponent<Atom>().Id);
    }
}
