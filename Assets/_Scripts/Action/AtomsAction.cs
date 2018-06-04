using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;



public class AtomsAction : VRTK_InteractableObject
{
    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("start use atom");
        base.StartUsing(usingObject);
        if (GameManager.IsConnectable())
        {
            StopUsing(usingObject);
            return;
        }
        GameManager.SetSelectedComponent(gameObject);
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowComponentOpCanvas(true, gameObject);
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {
        print("stop use atom");
        base.StopUsing(usingObject);

        //selection connect
        if (GameManager.IsConnectable())
        {
            GameManager.SetConnectable(false);
            gameObject.GetComponent<Assembler>().SelectionConnect(GameManager.GetSelectedComponent());
        }

        GameManager.CancelComponentSelected();
        GameManager.CancelRotatable();
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowComponentOpCanvas(false, gameObject);
        GameManager.SwitchMode(InteracteMode.GRAB);

    }


    private void ShowComponentOperationCanvas()
    {
        print("atom id: " + gameObject.GetComponent<Atom>().Id);
    }

    public void Detach()
    {
        List<GameObject> connectedBond = new List<GameObject>();
        connectedBond = gameObject.GetComponent<Atom>().Bonds;
        //倒序遍历删除...
        for(int i = connectedBond.Count - 1; i >=0 ; i--)
        {
            connectedBond[i].GetComponent<BondsAction>().Break();
        }
    }
}
