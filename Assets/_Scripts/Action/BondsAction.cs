using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class BondsAction : VRTK_InteractableObject
{

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("start bond using");
        base.StartUsing(usingObject);
        GameManager.SetSelectedComponent(gameObject);       
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowComponentOpCanvas(true, gameObject);
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {
        print("stop bond using");
        base.StopUsing(usingObject);

        GameManager.CancelComponentSelected();
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowComponentOpCanvas(false, gameObject);

        GameManager.SwitchMode(InteracteMode.GRAB);

    }

    private void ShowComponentOperationCanvas()
    {

        print("atom id: " + gameObject.GetComponent<Atom>().Id);
    }

    public void Break()
    {
        print("breaking!");
        //准备DFS参数
        List<GameObject> objectToDetach = new List<GameObject>();
        GameObject startAtom = gameObject.GetComponent<Bond>().A2;
        GameObject oppositeAtom = gameObject.GetComponent<Bond>().A1;
        List<GameObject> ignoreComponent = new List<GameObject>();
        ignoreComponent.Add(gameObject);
        //DFS
        StructureUtil.DfsMolecule(startAtom, ignoreComponent, objectToDetach);

        //只有在断开键后两边不属于同一个分子时，才生成新分子，否则只用删除bond即可
        if (!objectToDetach.Contains(gameObject.GetComponent<Bond>().A1))
        {
            //merge to new molecule
            GameObject mole = GameManager.NewMolecule();
            GameManager.PutIntoBuildArea(mole);

            mole.transform.position = startAtom.transform.position;
            foreach (GameObject component in objectToDetach)
            {
                component.transform.parent = mole.transform;
            }
        }

        //Destroy bond
        startAtom.GetComponent<Atom>().removeBond(gameObject);
        oppositeAtom.GetComponent<Atom>().removeBond(gameObject);
        Destroy(gameObject);
    }
}
