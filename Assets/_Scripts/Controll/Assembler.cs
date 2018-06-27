﻿using System;
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
        
        List<GameObject> visited = new List<GameObject>();
        Dictionary<int, GameObject> parent = new Dictionary<int, GameObject>();
        visited.Add(atom1);
        int aid = a1.Id;
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
                    aid = adj.GetComponent<Atom>().Id;
                    if (visited.Contains(adj)) continue;
                    parent.Add(aid, par);
                    if (aid == a2.Id)
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
            path.Add(atom2);
           while (parent[aid] != null)
            {
                path.Add(parent[aid]);
                aid = parent[aid].GetComponent<Atom>().Id;
            }
            
        }
        return path;
    }

    private void ConnectAsRing(GameObject atom1, GameObject atom2)
    {
        //分单路径成环和多路径成环
        List<GameObject> path = DFSPath(atom1, atom2);
        int num = path.Count;
        if (num == 0) return;
        
        
        List<GameObject> subMole = new List<GameObject>();
        List<GameObject> bonds = new List<GameObject>();
        for (int i = 0; i < num - 1; ++i)
        {
            string carbond = "";
            GameObject bond = path[i].GetComponent<Atom>().getBond(path[i + 1]);
            bonds.Add(bond);
            carbond += bond.GetComponent<Bond>().toSymbol();
            carbond += "C";
            //break bond between atoms in the ring
            List<GameObject> objectToDetach = new List<GameObject>();
            GameObject startAtom = path[i];
            GameObject oppositeAtom = path[i+1];
            List<GameObject> ignoreComponent = new List<GameObject>();
            ignoreComponent.Add(bond);
            if (i > 0)
            {
                GameObject prev_bond = path[i].GetComponent<Atom>().getBond(path[i - 1]);
                if (prev_bond != null)
                {
                    ignoreComponent.Add(prev_bond);
                    carbond += prev_bond.GetComponent<Bond>().toSymbol();
                }
            }
            else
                carbond += "-";
            print("carbond");

            //DFS
            StructureUtil.DfsMolecule(startAtom, ignoreComponent, objectToDetach);
            
            //seperate to new molecule
            GameObject mole = Instantiate(GameManager.prefebMole);
            mole.transform.position = startAtom.transform.position;
            foreach (GameObject component in objectToDetach)
            {
                component.transform.parent = mole.transform;
            }
            subMole.Add(mole);

            //reset startAtom's vbond
            //startAtom.GetComponent<Atom>().vbonds = new List<Vector4>(Config.BondAngleTable[carbond]);

            //Destroy bond
            startAtom.GetComponent<Atom>().removeBond(bond);
            oppositeAtom.GetComponent<Atom>().removeBond(bond);
            bond.SetActive(false);
        }
        subMole.Add(path[num - 1].transform.parent.gameObject);

        //calculate position of atoms and center of gravity and seperate branches
        Dictionary<GameObject, List<GameObject>> subBranch = new Dictionary<GameObject, List<GameObject>>();
        Vector3 center = new Vector3(0, 0, 0);
        double angle = Math.PI * (0.5 - 1 / num);
        float length = 0.756f, an = 0f, delta = (float)(2*Math.PI / num);
        for (int i = num-1; i >= 0; --i)
        {
            if (i < num - 1)
            {
                Vector3 dir = new Vector3(Mathf.Cos(an), Mathf.Sin(an), 0);
                Vector3 expectedPos = path[i + 1].transform.position + dir * length;
                path[i].transform.parent.position += (expectedPos - path[i].transform.position);
                path[i].transform.position = expectedPos;
                an -= delta;
                center += expectedPos;
            }

            //seperate sub-branch
            foreach (GameObject b in path[i].GetComponent<Atom>().Bonds)
            {
                List<GameObject> branch = new List<GameObject>();
                if (b != null && !bonds.Contains(b))
                {
                    List<GameObject> objectToSep = new List<GameObject>();
                    List<GameObject> ignoreComponent = new List<GameObject>() { b };
                    GameObject adj = b.GetComponent<Bond>().getAdjacent(path[i]);
                    StructureUtil.DfsMolecule(adj, ignoreComponent, objectToSep);
                    GameObject mole = Instantiate(GameManager.prefebMole);
                    mole.transform.position = adj.transform.position;
                    foreach (GameObject component in objectToSep)
                    {
                        component.transform.parent = mole.transform;
                    }
                    branch.Add(mole);
                }
                subBranch.Add(path[i], branch);
            }

        }
        center += path[num - 1].transform.position;
        center = center / num;
        
        //rotate atoms and sub-branches
        foreach(GameObject at in path)
        {
            at.transform.LookAt(center);
            foreach(GameObject mole in subBranch[at])
            {
                //mole.transform.position.Rotate();
            }
        }

        // recalculate bond angle

        //redraw the bonds
        for (int i = 0; i < num-1; ++i)
        {
            Bond b = bonds[i].GetComponent<Bond>();
            bonds[i].transform.position = Vector3.Lerp(b.A1.transform.position, b.A2.transform.position, 0.5f);
            //rotate bond
            Vector3 scale = bonds[i].transform.lossyScale;
            scale.y = length * 0.5f;
            bonds[i].transform.localScale = scale;
            bonds[i].transform.LookAt(b.A2.transform.position);
            bonds[i].transform.Rotate(new Vector3(90, 0, 0));
            //b.A1Index = 
            //b.A2Index =
            b.InRing = true;
            bonds[i].SetActive(true);
        }
        //draw bond between src & dst atom
        GameObject fbond = Instantiate(GameManager.prefebSingleBond);
        fbond.transform.position = Vector3.Lerp(atom1.transform.position, atom2.transform.position, 0.5f);
        Vector3 fscale = fbond.transform.lossyScale;
        fscale.y = length * 0.5f;
        fbond.transform.localScale = fscale;
        fbond.transform.LookAt(atom2.transform.position);
        fbond.transform.Rotate(new Vector3(90, 0, 0));
        //fbond.transform.parent = 
        Bond fb = fbond.AddComponent<Bond>();
        fb.A1 = atom1.GetComponent<Atom>().gameObject;
        fb.A2 = atom2.GetComponent<Atom>().gameObject;
        //fb.A1Index = catomBondIndex;
        //fb.A2Index = selfBondIndex;
        fb.Type = BondType.SINGLE;
        fb.InRing = true;
        atom1.GetComponent<Atom>().addBond(sbond);
        atom2.GetComponent<Atom>().addBond(sbond);
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
            print("rotate 180");
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
        Molecule mole = dstMole.GetComponent<Molecule>();

        print("after");
        print("child count: " + gameObject.transform.parent.transform.childCount);

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
