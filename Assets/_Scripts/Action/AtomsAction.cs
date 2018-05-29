using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;



public class AtomsAction : VRTK_InteractableObject
{


        //touchHighlightColor = new Color(26, 160, 255, 0);
        //isUsable = true;


    public override void StartUsing(VRTK_InteractUse usingObject)
    {

        base.StartUsing(usingObject);
        DoSomething();
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {

        base.StopUsing(usingObject);
        //usingObject.ForceStopUsing();
        //ForceStopInteracting();
        

        disableWhenIdle = false;
        enabled = false;
        
        
        StartCoroutine(CountDown());
        
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponentInParent<MoleculesAction>().RemoveAtomsAction();
    }
    private void DoSomething()
    {
        print("Atom id: " + gameObject.GetComponent<ComponentInformation>().Id);
    }
}
