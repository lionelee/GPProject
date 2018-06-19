using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;



public class AtomsAction : VRTK_InteractableObject
{
    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("atom Id: " + gameObject.GetComponent<Atom>().Id);
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

    /// <summary>
    /// 根据原子当前持有的化学键调整原子的vbond并重整结构
    /// </summary>
    public void Rebuild()
    {
        Atom atomInfo = gameObject.GetComponent<Atom>();
        Dictionary<GameObject, BondType> oppositeAtomWithBondType = new Dictionary<GameObject, BondType>();
        Dictionary<GameObject, int> oppositeAtomWithVbondIdx = new Dictionary<GameObject, int>();

        atomInfo.UpdateMaxBond();

        //排除在环上的情况？

        // break all
        for (int i = atomInfo.Bonds.Count - 1; i >= 0; i--)
        {
            Bond bondInfo = atomInfo.Bonds[i].GetComponent<Bond>();

            BondType type = bondInfo.Type;
            int oppositeVbondIdx = (bondInfo.A1 == gameObject) ? bondInfo.A2Index : bondInfo.A1Index;
            GameObject oppositeAtom = bondInfo.getAdjacent(gameObject);
            atomInfo.Bonds[i].GetComponent<BondsAction>().TmpBreak();
            oppositeAtomWithBondType.Add(oppositeAtom, type);
            oppositeAtomWithVbondIdx.Add(oppositeAtom, oppositeVbondIdx);

        }

        GameObject adjacentAtom = null;
        int adjacentIdx = 0;

        switch (atomInfo.MaxBomdType)
        {
            case BondType.SINGLE:
                atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C"]);
                break;
            case BondType.DOUBLE:
                List<GameObject> atomsOnDouble = new List<GameObject>();
                foreach (var v in oppositeAtomWithBondType)
                {
                    if (v.Value == BondType.DOUBLE)
                    {
                        atomsOnDouble.Add(v.Key);
                    }
                }
                if (atomsOnDouble.Count == 1)
                {
                    atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C="]);
                } else
                {
                    atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C#"]);
                }

                //connect that one
                int doubleBondIdx = 0;
                foreach (var v in atomsOnDouble)
                {
                    gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, doubleBondIdx++, v, oppositeAtomWithVbondIdx[v], BondType.DOUBLE, false);
                    oppositeAtomWithBondType.Remove(v);
                    oppositeAtomWithVbondIdx.Remove(v);
                }
                break;
            case BondType.TRIPLE:
                atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C#"]);
                //三键上的另一个原子
                foreach (var v in oppositeAtomWithBondType)
                {
                    if (v.Value == BondType.TRIPLE)
                    {
                        adjacentAtom = v.Key;
                        adjacentIdx = oppositeAtomWithVbondIdx[v.Key];
                        break;
                    }
                }
                //connect that one
                gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, 0, adjacentAtom, adjacentIdx, BondType.TRIPLE, false);

                oppositeAtomWithBondType.Remove(adjacentAtom);
                oppositeAtomWithVbondIdx.Remove(adjacentAtom);
                break;
            default:
                print("Rebuild error: no such bond.");
                break;
        }
        
        //全是单键

        foreach(var v in oppositeAtomWithBondType)
        {
            gameObject.GetComponent<Assembler>().SelectionConnect(v.Key);
        }

    }

    /// <summary>
    /// 仅用在改变化学键类型时调整vbond,化学键类型改变的过程中，除开被改变的那个化学键(断开状态），剩下的至多只有一个双键，
    /// specialAtom 用于记录那个特殊的双键
    /// </summary>
    /// <param name="newBondType"></param>
    /// <returns>对应 newBondType 的一个vbond index </returns>
    public int VbondSwitchWithNewBond(BondType newBondType, bool newBondOnRing)
    {
        int newBondIdx = -1;
        Atom atomInfo = gameObject.GetComponent<Atom>();
        Dictionary<GameObject, BondType> oppositeAtomWithBondType = new Dictionary<GameObject, BondType>();
        //Dictionary<GameObject, int> oppositeAtomWithVbondIdx = new Dictionary<GameObject, int>();

        GameObject specialAtom = null;
        int specialAtomIdx = 0;

        if (newBondType == BondType.DOUBLE) {
            //TODO: debug on here
            if (!newBondOnRing)
            {
                //原子上仍然连接有双键，只可能有一个，记录在specialBond, 只有四键元素有可能
                if (atomInfo.MaxBomdType == BondType.DOUBLE)
                {
                    Bond specialBond = atomInfo.Bonds[0].GetComponent<Bond>();
                    specialAtom = specialBond.getAdjacent(gameObject);
                    specialAtomIdx = (specialBond.A1 == gameObject) ? specialBond.A2Index : specialBond.A1Index;
                    atomInfo.Bonds[0].GetComponent<BondsAction>().TmpBreak();

                    atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C#"]);
                    gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, 0, specialAtom, specialAtomIdx, BondType.DOUBLE, false);
                    newBondIdx = 1;
                }

                else if (atomInfo.InRing)
                {
                    // -C- -> -C=
                    // 只可能是两个单键在环上,只有四个键的才可能发生这个情况
                    Bond bond0 = atomInfo.Bonds[0].GetComponent<Bond>();
                    Bond bond1 = atomInfo.Bonds[1].GetComponent<Bond>();

                    List<Vector4> oldVbonds = atomInfo.vbonds;
                    atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["-C="]);

                    atomInfo.vbonds[1] = oldVbonds[2];
                    atomInfo.vbonds[2] = oldVbonds[3];

                    if (bond0.A1 == gameObject)
                    {
                        if (bond0.A1Index == 2)
                            bond0.A1Index = 1;
                        else
                            bond0.A1Index = 2;
                    }
                    else
                    {
                        if (bond0.A2Index == 2)
                            bond0.A2Index = 1;
                        else
                            bond0.A2Index = 2;
                    }

                    if (bond1.A1 == gameObject)
                    {
                        if (bond1.A1Index == 2)
                            bond1.A1Index = 1;
                        else
                            bond1.A1Index = 2;
                    }
                    else
                    {
                        if (bond1.A2Index == 2)
                            bond1.A2Index = 1;
                        else
                            bond1.A2Index = 2;
                    }

                    newBondIdx = 0;
                }
                else if (atomInfo.MaxBomdType == BondType.SINGLE)
                {
                    //全是单键，无所谓新键位置处原来是什么键
                    for (int i = atomInfo.Bonds.Count - 1; i >= 0; i--)
                    {
                        Bond bondInfo = atomInfo.Bonds[i].GetComponent<Bond>();

                        BondType type = bondInfo.Type;
                        int oppositeVbondIdx = (bondInfo.A1 == gameObject) ? bondInfo.A2Index : bondInfo.A1Index;
                        GameObject oppositeAtom = bondInfo.getAdjacent(gameObject);
                        atomInfo.Bonds[i].GetComponent<BondsAction>().TmpBreak();
                        oppositeAtomWithBondType.Add(oppositeAtom, type);
                    }
                    //考虑两个键和三个键的元素
                    switch (Mathf.Abs(atomInfo.Valence))
                    {
                        case 2:
                            atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["H"]);
                            break;
                        case 3:
                            atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C#"]);
                            break;
                        case 4:
                            atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C="]);
                            break;
                    }
                    int singleBindIdx = 1;
                    foreach (var v in oppositeAtomWithBondType)
                    {
                        //gameObject.GetComponent<Assembler>().SelectionConnect(v.Key);
                        int oppositeIdx = v.Key.GetComponent<Atom>().getFreeVbondIdx();

                        gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, singleBindIdx++, v.Key, oppositeIdx, BondType.SINGLE, false);
                    }
                    newBondIdx = 0;
                }
            }
            //new bond on ring
            else
            {
                //只有一个双键, 不可能有三键，四键元素才有可能
                if (atomInfo.MaxBomdType == BondType.DOUBLE)
                {
                    Bond specialBond = atomInfo.Bonds[0].GetComponent<Bond>();

                    List<Vector4> oldVbonds = atomInfo.vbonds; // "-c=" 
                    atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["=C="]);

                    atomInfo.vbonds[0] = oldVbonds[1];
                    atomInfo.vbonds[1] = oldVbonds[2];

                    if (specialBond.A1 == gameObject)
                    {
                        if (specialBond.A1Index == 1)
                        {

                            specialBond.A1Index = 0;
                            newBondIdx = 1;
                        }
                        else
                        {
                            specialBond.A1Index = 1;
                            newBondIdx = 0;
                        }
                    }
                    else
                    {
                        if (specialBond.A2Index == 1)
                        {
                            specialBond.A2Index = 0;
                            newBondIdx = 1;
                        }
                        else
                        {
                            specialBond.A2Index = 1;
                            newBondIdx = 0;
                        }
                    }
                }
                //新键处原来最大可能是单键或三键，是单键的情况下有可能是三价元素
                else if (atomInfo.MaxBomdType == BondType.SINGLE)
                {
                    Bond specialBond = null;//用于记录另一个连在环上的键

                    for (int i = atomInfo.Bonds.Count - 1; i >= 0; i--)
                    {
                        Bond bondInfo = atomInfo.Bonds[i].GetComponent<Bond>();

                        if (bondInfo.InRing)
                        {
                            specialBond = bondInfo;
                            continue;
                        }

                        BondType type = bondInfo.Type;
                        //int oppositeVbondIdx = (bondInfo.A1 == gameObject) ? bondInfo.A2Index : bondInfo.A1Index;
                        GameObject oppositeAtom = bondInfo.getAdjacent(gameObject);
                        atomInfo.Bonds[i].GetComponent<BondsAction>().TmpBreak();
                        oppositeAtomWithBondType.Add(oppositeAtom, type);
                    }

                    List<Vector4> oldVbonds = atomInfo.vbonds; 

                    int oldRingIdx0 = -1, oldRingIdx1 = -1;
                    //三价元素
                    if (oldVbonds.Count == 3)
                    {
                        atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["=C="]);
                        oldRingIdx0 = 1;
                        oldRingIdx1 = 2;

                        atomInfo.vbonds[0] = oldVbonds[oldRingIdx0];
                        atomInfo.vbonds[1] = oldVbonds[oldRingIdx1];

                        if (specialBond.A1 == gameObject)
                        {
                            if (specialBond.A1Index == oldRingIdx0)
                            {

                                specialBond.A1Index = 0;
                                newBondIdx = 1;
                            }
                            else
                            {
                                specialBond.A1Index = 1;
                                newBondIdx = 0;
                            }
                        }
                        else
                        {
                            if (specialBond.A2Index == oldRingIdx0)
                            {
                                specialBond.A2Index = 0;
                                newBondIdx = 1;
                            }
                            else
                            {
                                specialBond.A2Index = 1;
                                newBondIdx = 0;
                            }
                        }
                    }
                    //四价元素
                    else
                    {
                        atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["-C="]);

                        //原来全是单键
                        if (oldVbonds.Count == 4)
                        {
                            oldRingIdx0 = 2;
                            oldRingIdx1 = 3;
                        }
                        //原来有三键
                        else if (oldVbonds.Count == 2)
                        {
                            oldRingIdx0 = 0;
                            oldRingIdx1 = 1;
                        }

                        atomInfo.vbonds[1] = oldVbonds[oldRingIdx0];
                        atomInfo.vbonds[2] = oldVbonds[oldRingIdx1];

                        if (specialBond.A1 == gameObject)
                        {
                            if (specialBond.A1Index == oldRingIdx0)
                            {

                                specialBond.A1Index = 1;
                                newBondIdx = 2;
                            }
                            else
                            {
                                specialBond.A1Index = 2;
                                newBondIdx = 1;
                            }
                        }
                        else
                        {
                            if (specialBond.A2Index == oldRingIdx0)
                            {
                                specialBond.A2Index = 1;
                                newBondIdx = 2;
                            }
                            else
                            {
                                specialBond.A2Index = 2;
                                newBondIdx = 1;
                            }
                        }
                    }

                    foreach (var v in oppositeAtomWithBondType)
                    {
                        int oppositeIdx = v.Key.GetComponent<Atom>().getFreeVbondIdx();

                        gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, 0, v.Key, oppositeIdx, BondType.SINGLE, false);
                    }

                }
            }
        }

        else if(newBondType == BondType.TRIPLE)
        {

            if (!newBondOnRing)
            {
                //此时原子不可能在环上
                //只剩一个单键(四价元素), 或没有键(三键）
                for (int i = atomInfo.Bonds.Count - 1; i >= 0; i--)
                {
                    Bond bondInfo = atomInfo.Bonds[i].GetComponent<Bond>();

                    BondType type = bondInfo.Type;
                    int oppositeVbondIdx = (bondInfo.A1 == gameObject) ? bondInfo.A2Index : bondInfo.A1Index;
                    GameObject oppositeAtom = bondInfo.getAdjacent(gameObject);
                    atomInfo.Bonds[i].GetComponent<BondsAction>().TmpBreak();
                    oppositeAtomWithBondType.Add(oppositeAtom, type);
                }

                switch (atomInfo.vbonds.Count)
                {
                    case 3:
                        atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["H"]);
                        break;
                    case 4:
                        atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["C#"]);
                        break;
                }
                int singleBindIdx = 1;
                foreach (var v in oppositeAtomWithBondType)
                {
                    int oppositeIdx = v.Key.GetComponent<Atom>().getFreeVbondIdx();
                    
                    gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, singleBindIdx++, v.Key, oppositeIdx, BondType.SINGLE, false);
                }
                newBondIdx = 0;
            }
            // 三键在环上，只有可能是四价元素，三键原来可能是双键或单键
            else
            {
                //单键
                Bond specialBond = null;//which on ring

                for (int i = atomInfo.Bonds.Count - 1; i >= 0; i--)
                {
                    Bond bondInfo = atomInfo.Bonds[i].GetComponent<Bond>();

                    if (bondInfo.InRing)
                    {
                        specialBond = bondInfo;
                        continue;
                    }

                    BondType type = bondInfo.Type;
                    //int oppositeVbondIdx = (bondInfo.A1 == gameObject) ? bondInfo.A2Index : bondInfo.A1Index;
                    GameObject oppositeAtom = bondInfo.getAdjacent(gameObject);
                    atomInfo.Bonds[i].GetComponent<BondsAction>().TmpBreak();
                    oppositeAtomWithBondType.Add(oppositeAtom, type);
                }

                List<Vector4> oldVbonds = atomInfo.vbonds; // "-c-"
                atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["=C="]);

                int oldRingIdx0 = -1, oldRingIdx1 = -1;
                //原来全是单键
                if (oldVbonds.Count == 4)
                {
                    oldRingIdx0 = 2;
                    oldRingIdx1 = 3;
                }
                //原来有双键
                else if (oldVbonds.Count == 3)
                {
                    oldRingIdx0 = 1;
                    oldRingIdx1 = 2;
                }

                atomInfo.vbonds[0] = oldVbonds[oldRingIdx0];
                atomInfo.vbonds[1] = oldVbonds[oldRingIdx1];

                if (specialBond.A1 == gameObject)
                {
                    if (specialBond.A1Index == oldRingIdx0)
                    {

                        specialBond.A1Index = 0;
                        newBondIdx = 1;
                    }
                    else
                    {
                        specialBond.A1Index = 1;
                        newBondIdx = 0;
                    }
                }
                else
                {
                    if (specialBond.A2Index == oldRingIdx0)
                    {
                        specialBond.A2Index = 0;
                        newBondIdx = 1;
                    }
                    else
                    {
                        specialBond.A2Index = 1;
                        newBondIdx = 0;
                    }
                }

                //其实这里没必要，oppositeAtomWithBondType应该为空
                //int singleBindIdx = 1;
                foreach (var v in oppositeAtomWithBondType)
                {
                    int oppositeIdx = v.Key.GetComponent<Atom>().getFreeVbondIdx();

                    gameObject.GetComponent<Assembler>().AccurateConnect(gameObject, 0, v.Key, oppositeIdx, BondType.SINGLE, false);
                }
            }
        }
        if(newBondIdx == -1)
        {
            print("VbondSwitchWithNewBond error.");
        }

        return newBondIdx;
    }

}
