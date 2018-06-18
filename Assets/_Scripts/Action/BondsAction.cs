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
		GameManager.CancelRotatable();
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowComponentOpCanvas(false, gameObject);

        GameManager.SwitchMode(InteracteMode.GRAB);

    }

    private void ShowComponentOperationCanvas()
    {

        print("atom id: " + gameObject.GetComponent<Atom>().Id);
    }


    //TODO: change vbonds when break double bond or triple bond
    public void Break()
    {
        print("breaking!");
        //准备DFS参数
        List<GameObject> objectToDetach = new List<GameObject>();
        GameObject startAtom = gameObject.GetComponent<Bond>().A2;
        GameObject oppositeAtom = gameObject.GetComponent<Bond>().A1;
        Atom startAtomInfo = startAtom.GetComponent<Atom>();
        Atom oppositeAtomInfo = oppositeAtom.GetComponent<Atom>();

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
        startAtomInfo.removeBond(gameObject);
        oppositeAtomInfo.removeBond(gameObject);

        //TODO: break 时分子中的原子id的调整，因为这个问题break时会出问题
        
        //调整键角
        if (gameObject.GetComponent<Bond>().InRing)
        {
            //调整环上所有原子的位置
            startAtom.GetComponent<Assembler>().BreakRingToChain(startAtom, oppositeAtom);
        }
        else
        {
            startAtom.GetComponent<AtomsAction>().Rebuild();
            oppositeAtom.GetComponent<AtomsAction>().Rebuild();
        }

        Destroy(gameObject);
    }
    
    //暂时断开，不做断开后的键角调整, 当断开的键为双键或三键时，或者断开的是环上的键时需要调整键角
    public void TmpBreak()
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
