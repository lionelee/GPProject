using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour
{
    GameObject sbond;
    GameObject satom;
    Material AtomHalo;
    Material BondHalo;
    Material DefaultMat;

    GameObject catom;
    int catomBondIndex;
    Vector3 expectedPos;
    int selfBondIndex;
    //Dictionary<GameObject, GameObject> sbonds;
    Dictionary<GameObject, bool> ContainObject;
    bool grabbed;

	void Start()
	{
        //sbonds = new Dictionary<GameObject, GameObject>();
        ContainObject = new Dictionary<GameObject, bool>();
        catom = null;
        catomBondIndex = -1;

        AtomHalo = Resources.Load("_Materials/AtomHalo") as Material;
        BondHalo = Resources.Load("_Materials/BondHalo") as Material;
        DefaultMat = Resources.Load("_Materials/Default") as Material;
    }

    public void SetGrabbed()
    {
        grabbed = true;
    }

    public void ResetGrabbed()
    {
        grabbed = false;
    }

    public void BreakRingToChain(GameObject startAtom, GameObject endAtom)
    {
        List<GameObject> path = DFSPath(startAtom, endAtom);

        for(int i = 0;i < path.Count; i++)
        {
            path[i].GetComponent<AtomsAction>().Rebuild();
        }

        //set atom out of ring, binds has been set in Rebuild()
        for (int i = 0; i < path.Count; i++)
        {
            path[i].GetComponent<Atom>().InRing = false;
            foreach(GameObject bond in path[i].GetComponent<Atom>().Bonds)
            {
                Bond bondInfo = bond.GetComponent<Bond>();
                if (bondInfo.InRing)
                {
                    path[i].GetComponent<Atom>().InRing = true;
                    break;
                }
            }
        }
    }

    #region ///connect two atoms via selection/// 

    public void SelectionConnect(GameObject otherAtom)
    {
        //先判断两个原子是否都有多余键
        Atom a1 = otherAtom.GetComponent<Atom>();
        Atom a2 = gameObject.GetComponent<Atom>();
        uint otherValence = (uint)Mathf.Abs(a1.Valence);
        otherValence = otherValence == 0 ? 1 : otherValence;
        uint thisValence = (uint)Mathf.Abs(a2.Valence);
        thisValence = thisValence == 0 ? 1 : thisValence;

        if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
        {
            Debug.Log("can not be connected");
            return;
        }

        //判断是否可连键
        if (a1.Symbol != a2.Symbol)
        {
            int bondValence = a1.Valence / (int)otherValence + a2.Valence / (int)thisValence;
            if (bondValence != 0) return;
        }

        GameObject selfMole = transform.parent.gameObject;
        GameObject otherMole = otherAtom.transform.parent.gameObject;
        if(selfMole == otherMole)
        {
            //两原子已经通过单键连接，直接返回不做操作
            if (StructureUtil.IsAdjacent(gameObject, otherAtom))
                return;
            else
                ConnectAsRing(gameObject, otherAtom);
        }
        else
        {
            ConnectAsChain(gameObject, otherAtom);
        }
    }

    private List<GameObject> DFSPath(GameObject atom1, GameObject atom2)
    {
        
        List<GameObject> path = new List<GameObject>();
        Stack<GameObject> st = new Stack<GameObject>();
        Atom a1 = atom1.GetComponent<Atom>();
        Atom a2 = atom2.GetComponent<Atom>();
        st.Push(atom1);

        int atomN = atom1.transform.parent.GetComponent<Molecule>().AtomNum;
        List<GameObject> visited = new List<GameObject>();
        Dictionary<GameObject, GameObject> parent = new Dictionary<GameObject, GameObject>();

        visited.Add(atom1);

        bool found = false;
       
        while (st.Count > 0)
        {
            GameObject par = st.Pop();
            foreach(GameObject bond in par.GetComponent<Atom>().Bonds)
            {
                if (bond != null)
                {
                    Bond b = bond.GetComponent<Bond>();
                    GameObject adj = b.getAdjacent(par);
                    if (visited.Contains(adj)) continue;
                    parent.Add(adj, par);
                    if (adj == atom2)

                    {
                        found = true;
                        goto end;
                    }
                    visited.Add(adj);
                    st.Push(adj);
                }
            }
        }
        end:
        if (found)
        {
            //path.Add(atom1);
            path.Add(atom2);
            GameObject cur = atom2;
            while (parent.ContainsKey(cur))
            {
                path.Add(parent[cur]);
                cur = parent[cur];
            }
            
        }
        return path;
    }

    #region ConnectAsRing
    private void ConnectAsRing(GameObject atom1, GameObject atom2)
    {
        //分单路径成环和多路径成环
        List<GameObject> path = DFSPath(atom1, atom2);
        
        int num = path.Count;
        if (num == 0) return;

        float angle = Mathf.PI * (0.5f - 1.0f / num);

        GameObject anchorAtom = path[0];
        GameObject anchorBond = path[0].GetComponent<Atom>().getBond(path[1]);

        BondType anchorBondType = anchorBond.GetComponent<Bond>().Type;
        anchorBond.GetComponent<BondsAction>().TmpBreak();

        
        SetRingAnchor(anchorAtom, angle);
        
        Vector3 anchorZ = anchorAtom.transform.TransformDirection(new Vector3(0, 0, 1));
        Vector3 anchorY = anchorAtom.transform.TransformDirection(new Vector3(0, 1, 0));
        Vector3 LookAtPoint;

        GameObject prevAnchor = anchorAtom;
        BondType prevBondType = anchorBondType;
        int i, prevIndex, curIndex;
        for (i = 1; i < path.Count - 1; ++i)
        {
            //break with next then connect to prev
            anchorAtom = path[i];
            anchorBond = path[i].GetComponent<Atom>().getBond(path[i+1]);
            anchorBondType = anchorBond.GetComponent<Bond>().Type;
            anchorBond.GetComponent<BondsAction>().TmpBreak();

            anchorAtom.transform.forward = anchorZ;
            LookAtPoint = anchorAtom.transform.TransformPoint(0, 0, 1);

            anchorY = Quaternion.AngleAxis(360.0f/num, anchorAtom.transform.forward) * anchorY;
            anchorAtom.transform.LookAt(LookAtPoint, anchorY);

            SetRingAnchor(anchorAtom, angle);

            //connect to prev
            prevIndex = prevAnchor.GetComponent<Atom>().vbonds.Count - 1;
            curIndex = anchorAtom.GetComponent<Atom>().vbonds.Count - 2;
            AccurateConnect(prevAnchor, prevIndex, anchorAtom, curIndex, prevBondType);

            //update
            prevAnchor = anchorAtom;
            prevBondType = anchorBondType;
            //prevAnchorY = anchorY;
        }
        //last one, no need to break
        anchorAtom = path[i];

        anchorAtom.transform.forward = anchorZ;
        LookAtPoint = anchorAtom.transform.TransformPoint(0, 0, 1);
           

        anchorY = Quaternion.AngleAxis(2 * angle * Mathf.Rad2Deg, anchorAtom.transform.forward) * anchorY;
        anchorAtom.transform.LookAt(LookAtPoint, anchorY);

        SetRingAnchor(anchorAtom, angle);

        //connect to prev
        prevIndex = prevAnchor.GetComponent<Atom>().vbonds.Count - 1;
        curIndex = anchorAtom.GetComponent<Atom>().vbonds.Count - 2;
        AccurateConnect(prevAnchor, prevIndex, anchorAtom, curIndex, prevBondType);

        //connect to first, connect without change position, just add bond
        #region Yet Another Connect Function

        Atom lastAtomInfo = anchorAtom.GetComponent<Atom>();
        Atom firstAtomInfo = path[0].GetComponent<Atom>();
        int firstIdx = firstAtomInfo.vbonds.Count - 2;
        int lastIdx = lastAtomInfo.vbonds.Count - 1;
        Vector3 firstDir = firstAtomInfo.getVbondByIndex(firstIdx);
        Vector3 lastDir = lastAtomInfo.getVbondByIndex(lastIdx);

        GameObject lastBond = Instantiate(GameManager.prefebSingleBond);
        lastBond.GetComponent<Renderer>().material = DefaultMat;

        Vector3 transformedDirection = path[0].transform.TransformDirection(new Vector3(firstDir.x, firstDir.y, firstDir.z));

        float length = Config.BondLengthTable["CC"];
        lastBond.transform.position = Vector3.Lerp(path[0].transform.position, path[0].transform.position + transformedDirection * length, 0.5f);
        lastBond.transform.parent = path[0].transform.parent;

        //bond

        Vector3 scale = lastBond.transform.lossyScale;
        scale.y = length * 0.5f;
        lastBond.transform.localScale = scale;
        lastBond.transform.LookAt(atom1.transform.position);
        lastBond.transform.Rotate(new Vector3(90, 0, 0));

        //set abstract bond
        Bond b = lastBond.AddComponent<Bond>();
        lastBond.AddComponent<BondsAction>();
        b.A1 = firstAtomInfo.gameObject;
        b.A2 = lastAtomInfo.gameObject;
        b.A1Index = firstIdx;
        b.A2Index = lastIdx;

        

        b.Type = BondType.SINGLE;
        firstAtomInfo.addBond(lastBond);
        lastAtomInfo.addBond(lastBond);
        #endregion
        //set all atoms and bonds inring
        for (i = 0; i < path.Count - 1; ++i)
        {
            path[i].GetComponent<Atom>().InRing = true;
            path[i].GetComponent<Atom>().getBond(path[i + 1]).GetComponent<Bond>().InRing = true;
        }
        path[path.Count - 1].GetComponent<Atom>().InRing = true;
        path[path.Count - 1].GetComponent<Atom>().getBond(path[0]).GetComponent<Bond>().InRing = true;
    }

    //only used for connect as ring, on a special condition that 四价元素的支路是环 
    //此时，原子的vbond 3、4位连在同一环上，1、2位为空
    private void AdjustVbondIdxOnAtom(GameObject atom)
    {
        Atom atomInfo = atom.GetComponent<Atom>();

        if(atomInfo.Bonds.Count != 2)
        {
            print("AdjustVbondIdx error");
        }

        atom.transform.LookAt(atom.transform.TransformPoint(1, 0, 0), atom.transform.TransformPoint(0, -1, 0));


        //float ringAngle = Vector3.Angle(atomInfo.vbonds[2], atomInfo.vbonds[3]);

        Vector4 oldVbond2 = atomInfo.vbonds[2];
        Vector4 oldVbond3 = atomInfo.vbonds[3];

        atomInfo.vbonds[0] = new Vector4(oldVbond2.z, -oldVbond2.y, oldVbond2.x, 1);
        atomInfo.vbonds[1] = new Vector4(oldVbond3.z, -oldVbond3.y, oldVbond3.x, 1);

        Bond bond0 = atomInfo.Bonds[0].GetComponent<Bond>();
        Bond bond1 = atomInfo.Bonds[1].GetComponent<Bond>();

        if(bond0.A1 == atom)
        {
            bond0.A1Index = 0;
        }
        else
        {
            bond0.A2Index = 0;
        }

        if(bond1.A1 = atom)
        {
            bond1.A1Index = 1;
        }
        else
        {
            bond1.A2Index = 1;
        }
    }

    /// <summary>
    /// 用于在连环时调整原子的键角
    /// </summary>
    /// <param name="anchorAtom"> 当前调整的环上原子 </param>
    /// <param name="angle"> 环的内角度数/2 </param>
    private void SetRingAnchor(GameObject anchorAtom, float angle)
    {
        Atom atomInfo = anchorAtom.GetComponent<Atom>();


        int vbondCount = atomInfo.getVbondCount();

        Dictionary<GameObject, BondType> oppositeAtomWithBondType = new Dictionary<GameObject, BondType>();
        Dictionary<GameObject, int> oppositeAtomWithVbondIdx = new Dictionary<GameObject, int>();

        //断开支路，并记录

        for(int i = atomInfo.Bonds.Count - 1;i >= 0;i--)
        {
            Bond bondInfo = atomInfo.Bonds[i].GetComponent<Bond>();
            //点上连有环，环不断开,可以将环的index换到 0、1位
            if (bondInfo.InRing)
            {
                //anchorAtom.transform.LookAt(anchorAtom.transform.TransformPoint(1, 0, 0), anchorAtom.transform.TransformPoint(0, -1, 0));
                AdjustVbondIdxOnAtom(anchorAtom);
                break;
            }

            BondType type = bondInfo.Type;
            int oppositeVbondIdx = (bondInfo.A1 == anchorAtom) ? bondInfo.A2Index : bondInfo.A1Index;
            GameObject oppositeAtom = bondInfo.getAdjacent(anchorAtom);
            atomInfo.Bonds[i].GetComponent<BondsAction>().TmpBreak();
            oppositeAtomWithBondType.Add(oppositeAtom, type);
            oppositeAtomWithVbondIdx.Add(oppositeAtom, oppositeVbondIdx);
        }

        // 更改 vbonds
        switch (vbondCount)
        {
            case 4:
                if (atomInfo.InRing)
                {
                    atomInfo.vbonds[2] = new Vector4(Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                    atomInfo.vbonds[3] = new Vector4(-Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                }
                else
                {
                    atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["-C-"]);
                    atomInfo.vbonds[2] = new Vector4(Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                    atomInfo.vbonds[3] = new Vector4(-Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                }
                break;
            case 3:
                atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["-C="]);
                atomInfo.vbonds[1] = new Vector4(Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                atomInfo.vbonds[2] = new Vector4(-Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                break;
            case 2:
                atomInfo.vbonds = new List<Vector4>(Config.BondAngleTable["=C="]);
                atomInfo.vbonds[0] = new Vector4(Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                atomInfo.vbonds[1] = new Vector4(-Mathf.Sin(angle), -Mathf.Cos(angle), 0, 0);
                break;
            default:
                print("Connect error");
                break;
        }

        //调整位置
        //将支路上的分子添加回来,这里不可能是环
        int curIdx = 0;
        foreach (var v in oppositeAtomWithBondType)
        {
            AccurateConnect(anchorAtom, curIdx++, v.Key, oppositeAtomWithVbondIdx[v.Key], v.Value, false);
        }

    }
    #endregion

    public void AccurateConnect(GameObject atom1, int a1Index, GameObject atom2, int a2Index, BondType bondType, bool onRing = true)
    {
        Atom a1 = atom1.GetComponent<Atom>();
        Atom a2 = atom2.GetComponent<Atom>();
        Vector3 a1Dir = a1.getVbondByIndex(a1Index);                                          
        Vector3 a2Dir = a2.getVbondByIndex(a2Index);
        if(a1Dir == Vector3.zero || a2Dir == Vector3.zero)
        {
            print("a1Idx: " + a1Index + " a2Idx: " + a2Index);
            print("a1Dir: " + a1Dir + " a2Dir: " + a2Dir);
            print("error in accurate connect: vbond has been used");
            return;
        }

        float length;
        if (!onRing)
            length = GetBondLength(a1, a2);
        else
            length = Config.BondLengthTable["CC"];

        GameObject bond;
        switch (bondType)
        {
            case BondType.SINGLE:
                bond = Instantiate(GameManager.prefebSingleBond);
                break;
            case BondType.DOUBLE:
                bond = Instantiate(GameManager.prefebDoubleBond);
                break;
            case BondType.TRIPLE:
                bond = Instantiate(GameManager.prefebTrippleBond);
                break;
            default:
                bond = null;
                print("error in accurate connect: bond type error");
                return;
        }


        

        Renderer[] renderers = bond.GetComponentsInChildren<Renderer>();
        foreach(var v in renderers)
        {
            v.material = DefaultMat;
        }

        Vector3 transformedDirection = atom1.transform.TransformDirection(new Vector3(a1Dir.x, a1Dir.y, a1Dir.z));

        bond.transform.position = Vector3.Lerp(atom1.transform.position, atom1.transform.position + transformedDirection * length, 0.5f);
        bond.transform.parent = atom1.transform.parent;

        //bond
        Vector3 scale = bond.transform.lossyScale;
        scale.y = length * 0.5f * (scale.y / Config.bondLengthScale);
        bond.transform.localScale = scale;
        bond.transform.LookAt(atom1.transform.position);
        bond.transform.Rotate(new Vector3(90, 0, 0));

        //atom2
        Vector3 Atom2expectedPos = atom1.transform.position + 2 * (bond.transform.position - atom1.transform.position);
        atom2.transform.parent.transform.position += Atom2expectedPos - atom2.transform.position;
        //rotate    
        Vector3 vec1 = (bond.transform.position - atom1.transform.position).normalized;
        Vector3 vec2 = (atom2.transform.TransformDirection(a2Dir)).normalized;

        float an = 180 - Vector3.Angle(vec1, vec2);

        if (vec1 == vec2)
        {
            Vector3 axis = CaculateUtil.GetVerticalDir(vec1);
            atom2.transform.parent.transform.RotateAround(atom2.transform.position, axis, 180);
        }
        else
        {
            atom2.transform.parent.transform.RotateAround(atom2.transform.position, Vector3.Cross(vec1, vec2), an);
        }

        //旋转使得碳原子在同一平面
        if(bondType == BondType.DOUBLE && !onRing)
        {
            Vector3 atom1Y = atom1.transform.up;
            Vector3 atom2Y = atom2.transform.up;

            float anOfY = Vector3.Angle(atom1Y, atom2Y);

            atom2.transform.parent.RotateAround(atom2.transform.position, vec1, anOfY);
        }

        MergeTwoMolecules(atom1.transform.parent.gameObject, atom2.transform.parent.gameObject);

        //set abstract bond
        Bond b = bond.AddComponent<Bond>();
        bond.AddComponent<BondsAction>();
        b.A1 = a1.gameObject;
        b.A2 = a2.gameObject;
        b.A1Index = a1Index;
        b.A2Index = a2Index;
        b.Type = bondType;
        b.InRing = onRing;
        a1.addBond(bond);
        a2.addBond(bond);
    }

    private void ConnectAsChain(GameObject atom1, GameObject atom2)
    {
        int a1Index = -1, a2Index = -1;
        Atom a1 = atom1.GetComponent<Atom>();
        Atom a2 = atom2.GetComponent<Atom>();
        Vector3 a1Dir = a1.getAngle(ref a1Index);
        Vector3 a2Dir = a2.getAngle(ref a2Index);

        float length = GetBondLength(a1, a2);
        
        GameObject bond = Instantiate(GameManager.prefebSingleBond);
        bond.GetComponent<Renderer>().material = DefaultMat;

        Vector3 transformedDirection = atom1.transform.TransformDirection(new Vector3(a1Dir.x, a1Dir.y, a1Dir.z));

        bond.transform.position = Vector3.Lerp(atom1.transform.position, atom1.transform.position + transformedDirection * length, 0.5f);
        bond.transform.parent = atom1.transform.parent;

        //bond
        Vector3 scale = bond.transform.lossyScale;
        scale.y = length * 0.5f;
        bond.transform.localScale = scale;
        bond.transform.LookAt(atom1.transform.position);
        bond.transform.Rotate(new Vector3(90, 0, 0));

        //atom2
        Vector3 Atom2expectedPos = atom1.transform.position + 2 * (bond.transform.position - atom1.transform.position);
        atom2.transform.parent.transform.position += Atom2expectedPos - atom2.transform.position;
        //rotate    
        Vector3 vec1 = (bond.transform.position - atom1.transform.position).normalized;
        Vector3 vec2 = (atom2.transform.TransformDirection(a2Dir)).normalized;

        float an = 180 - Vector3.Angle(vec1, vec2);

        if (vec1 == vec2)
        {
            Vector3 axis = CaculateUtil.GetVerticalDir(vec1);
            atom2.transform.parent.transform.RotateAround(atom2.transform.position, axis, 180);
        }
        else
        {

            atom2.transform.parent.transform.RotateAround(atom2.transform.position, Vector3.Cross(vec1, vec2), an);
        }

        MergeTwoMolecules(atom1.transform.parent.gameObject, atom2.transform.parent.gameObject);

        //set abstract bond
        Bond b = bond.AddComponent<Bond>();
        bond.AddComponent<BondsAction>();
        b.A1 = a1.gameObject;
        b.A2 = a2.gameObject;
        b.A1Index = a1Index;
        b.A2Index = a2Index;
        b.Type = BondType.SINGLE;
        a1.addBond(bond);
        a2.addBond(bond);
    }

    #endregion

    public void Connect()
	{
        if (sbond == null)
            return;

        sbond.GetComponent<Renderer>().material = DefaultMat;
        Destroy(satom);
        
        // get molecules and atoms
        Atom a1 = catom.GetComponent<Atom>();
        Atom a2 = GetComponent<Atom>();

        //set sbond's parent to molecule
        sbond.transform.parent = catom.transform.parent;


        // 更新目标原子的vbond,标记一个化学键为已使用,抓取原子的vbond由之后的getAngle()函数更新
        a1.setVbondUsed(catomBondIndex);

        //move and rotate grabbed atom to fix positon
        //translate
        transform.parent.transform.position += expectedPos - transform.position;
        //rotate    
        Vector3 vec1 = (sbond.transform.position - catom.transform.position).normalized;
        Vector3 vec2 = (transform.TransformDirection(a2.getAngle(ref selfBondIndex))).normalized;

        float an = 180 - Vector3.Angle(vec1, vec2);
        if (vec1 == vec2)
        {
            Vector3 axis = CaculateUtil.GetVerticalDir(vec1);
            transform.parent.transform.RotateAround(transform.position, axis, 180);
        }
        else
        {
            transform.parent.transform.RotateAround(transform.position, Vector3.Cross(vec1, vec2), an);
        }


        //then merge two molecules
        MergeTwoMolecules(catom.transform.parent.gameObject, transform.parent.gameObject);

        //set abstract bond
        Bond b = sbond.AddComponent<Bond>();
        b.A1 = a1.gameObject;
        b.A2 = a2.gameObject;
        b.A1Index = catomBondIndex;
        b.A2Index = selfBondIndex;
        b.Type = BondType.SINGLE;
        a1.addBond(sbond);
        a2.addBond(sbond);


        sbond = null;
    }
	   
    private void MergeTwoMolecules(GameObject dstMole, GameObject otherMole)
    {
        if (dstMole == otherMole)
            return;

        Molecule mole = dstMole.GetComponent<Molecule>();

        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in otherMole.transform)
        {
            children.Add(child.gameObject);
        }
        foreach (GameObject child in children)
        {
            if (child.tag != "Bond" && child.tag != "Component")
                continue;
            child.transform.parent = dstMole.transform;
            if (child.tag != "Bond")
            {
                child.GetComponent<Atom>().Id = mole.CurrentAtomId++;
                mole.AtomNum++;
            }
        }
        GameManager.RemoveMolecule(otherMole);
    }

    private float GetBondLength(Atom a1, Atom a2)
    {
        string key;
        if (a1.Symbol.Length <= a2.Symbol.Length && a1.Symbol[0] < a2.Symbol[0])
        {
            key = a1.Symbol + a2.Symbol;
        }
        else
        {
            key = a2.Symbol + a1.Symbol;
        }
        return Config.BondLengthTable[key];
    }
 

	void OnTriggerEnter(Collider collider)
	{
        if (collider.isTrigger || collider.transform.parent == null)
            return;

        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        if (!grabbed)
            return;


        if (collider.gameObject.transform.parent == null)
            return;


        if(sbond != null)
        {
            Destroy(sbond);
            Destroy(satom);
        }

        Atom otherAtom = collider.gameObject.GetComponent<Atom>();
        Atom thisAtom = gameObject.GetComponent<Atom>();

        uint otherValence = (uint)Mathf.Abs(otherAtom.Valence);
        otherValence = otherValence == 0 ? 1 : otherValence;
        uint thisValence = (uint)Mathf.Abs(thisAtom.Valence);
        thisValence = thisValence == 0 ? 1 : thisValence;

        if (otherAtom.Connected == otherValence || thisAtom.Connected == thisValence)
        {
            print(otherValence);
            Debug.Log("can not be connected");
            return;
        }

        if (otherAtom.Symbol != thisAtom.Symbol)
        {
            int bondValence = otherAtom.Valence / (int)otherValence + thisAtom.Valence / (int)thisValence;
            if (bondValence != 0) return;
        }

        Vector3 otherAtomPos = collider.gameObject.transform.position;
        Vector3 thisAtomPos = transform.position;


        //获取匹配的化学键位置
        float length = GetBondLength(otherAtom, thisAtom);

        //get nearest vbond index
        catomBondIndex = otherAtom.getVbondIdx(transform.position);
        if (catomBondIndex == -1)
            return;

        //提示分子当前原子可被连接
        gameObject.transform.parent.GetComponent<MoleculesAction>().SetConnectableAtom(gameObject);

        //calculate atom's expected position and bond's position
        Vector3 direction = otherAtom.vbonds[catomBondIndex];
        Vector3 trasformedDirection = collider.transform.TransformDirection(direction);
        expectedPos = otherAtomPos + trasformedDirection * length;

        //draw bond can be connected
        sbond = Instantiate(GameManager.prefebSingleBond);
        sbond.GetComponent<Renderer>().material = BondHalo;

        //translate bond

        sbond.transform.position = Vector3.Lerp(otherAtomPos, expectedPos, 0.5f);
        //rotate bond
        Vector3 scale = sbond.transform.lossyScale;
        scale.y = length * 0.5f;
        sbond.transform.localScale = scale;
        sbond.transform.LookAt(otherAtomPos);
        sbond.transform.Rotate(new Vector3(90, 0, 0));

        //show expected position
        satom = Instantiate(gameObject);
        satom.GetComponent<Renderer>().material = AtomHalo;
        satom.transform.position = expectedPos;

        //set catom
        catom = collider.gameObject;
    }

	void OnTriggerExit (Collider collider) {
        if (collider.gameObject != catom || collider.isTrigger)
            return;
        
        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        if (!grabbed)
            return;

        Destroy(sbond);
        Destroy(satom);
        gameObject.transform.parent.GetComponent<MoleculesAction>().ResetConnectable();
        sbond = null;
    }
}
