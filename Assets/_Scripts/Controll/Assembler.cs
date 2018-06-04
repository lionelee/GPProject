using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour
{
    //GameObject sbond;
    GameObject sbond;
    GameObject catom;
    int catomBondIndex;
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
	}

    public void SetGrabbed()
    {
        grabbed = true;
    }

    public void ResetGrabbed()
    {
        grabbed = false;
    }

    public void SelectionConnect(GameObject otherAtom)
    {
        //先判断两个原子是否都有多余键
        Atom a1 = otherAtom.GetComponent<Atom>();
        Atom a2 = gameObject.GetComponent<Atom>();
        if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
        {
            Debug.Log("can not be connected");
            return;
        }

        GameObject selfMole = transform.parent.gameObject;
        GameObject otherMole = otherAtom.transform.parent.gameObject;
        if(selfMole == otherMole)
        {
            //两原子已经通过单键连接，直接返回不做操作。
            if (StructureUtil.IsAdjacent(gameObject, otherAtom))
                return;
            else
                ConnectAsRing(gameObject, otherAtom);
        } else
        {
            ConnectAsChain(gameObject, otherAtom);
        }
    }


    private void ConnectAsRing(GameObject atom1, GameObject atom2)
    {
        //分单路径成环和多路径成环
    }


    private void ConnectAsChain(GameObject atom1, GameObject atom2)
    {
        int a1Index = -1, a2Index = -1;
        Atom a1 = atom1.GetComponent<Atom>();
        Atom a2 = atom2.GetComponent<Atom>();
        Vector3 a1Dir = a1.getAngle(a1Index);
        Vector3 a2Dir = a2.getAngle(a2Index);

        float length = GetBondLength(a1, a2);

        GameObject prefebBond = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        GameObject bond = Instantiate(prefebBond);
        bond.GetComponent<Renderer>().material = Resources.Load("_Materials/Default") as Material;

        Vector3 transformedDirection = atom1.transform.TransformDirection(new Vector3(a1Dir.x, a1Dir.y, a1Dir.z));

        bond.transform.position = Vector3.Lerp(atom1.transform.position, atom1.transform.position + transformedDirection * length, 0.5f);
        bond.transform.parent = atom1.transform.parent;

        //bond
        Vector3 scale = prefebBond.transform.lossyScale;
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


	public void Connect()
	{
        sbond.GetComponent<Renderer>().material = Resources.Load("_Materials/Default") as Material;
        print("child count: " + gameObject.transform.parent.transform.childCount);
        for(int i = 0;i < gameObject.transform.parent.transform.childCount; i++)
        {
            print("child info: " + gameObject.transform.parent.transform.GetChild(i));
        }
        // get molecules and atoms
        Molecule m1 = catom.GetComponentInParent<Molecule>();
        Molecule m2 = GetComponentInParent<Molecule>();
        Atom a1 = catom.GetComponent<Atom>();
        Atom a2 = GetComponent<Atom>();

        //set sbond's parent to molecule
        sbond.transform.parent = catom.transform.parent;

        //calculate expected position of this atom
        Vector3 expectedPos = catom.transform.position + 2 * (sbond.transform.position - catom.transform.position);

        // 更新目标原子的vbond,标记一个化学键为已使用,抓取原子的vbond由之后的getAngle()函数更新
        Vector4 usedVbond = a1.vbonds[catomBondIndex];
        usedVbond.w = 1;
        a1.vbonds[catomBondIndex] = usedVbond;

        //move and rotate grabbed atom to fix positon
        //translate
        transform.parent.transform.position += expectedPos - transform.position;
        //rotate    
        Vector3 vec1 = (sbond.transform.position - catom.transform.position).normalized;
        Vector3 vec2 = (transform.TransformDirection(a2.getAngle(selfBondIndex))).normalized;

        float an = 180 - Vector3.Angle(vec1, vec2);

        if (vec1 == vec2)
        {
            print("rotate 180");
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
        if (collider.isTrigger)
            return;

        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        if (!grabbed)
            return;

        if (sbond != null)
            Destroy(sbond);

        print("enter trigger");


        Atom otherAtom = collider.gameObject.GetComponent<Atom>();
        Atom thisAtom = gameObject.GetComponent<Atom>();

        if (otherAtom.Connected == otherAtom.Valence || thisAtom.Connected == thisAtom.Valence)
        {
            Debug.Log("can not be connected");
            return;
        }

        Vector3 otherAtomPos = collider.gameObject.transform.position;
        Vector3 thisAtomPos = transform.position;

        //获取匹配的化学键位置
        float length = GetBondLength(otherAtom, thisAtom);

        List<Vector4> otherAtomVbonds = otherAtom.vbonds;
        float minDistance = float.MaxValue;
        catomBondIndex = -1;
        for (int i = 0;i < otherAtomVbonds.Count;i++)
        {
            if (otherAtomVbonds[i].w == 1)
                continue;
            float distance = (otherAtomPos + collider.transform.TransformDirection(new Vector3(otherAtomVbonds[i].x, otherAtomVbonds[i].y, otherAtomVbonds[i].z)) - thisAtomPos).sqrMagnitude;
            print("index: " + i + "distance: " + distance);
            if (distance < minDistance)
            {
                minDistance = distance;
                catomBondIndex = i;
            }
        }
        

        print("selectedIndex: " + catomBondIndex);
        //无可用化学键， 返回
        if (catomBondIndex == -1)
            return;

        //计算结束， 开始显示

        //提示分子当前原子可被连接
        gameObject.transform.parent.GetComponent<MoleculesAction>().SetConnectableAtom(gameObject);
        //绘制匹配的化学键
        GameObject prefebBond = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        sbond = Instantiate(prefebBond);
        sbond.GetComponent<Renderer>().material = Resources.Load("_Materials/Highlight") as Material;

        Vector3 transformedDirection = collider.transform.TransformDirection(new Vector3(otherAtomVbonds[catomBondIndex].x, otherAtomVbonds[catomBondIndex].y, otherAtomVbonds[catomBondIndex].z));
        /*sbond.transform.position = new Vector3(trasformedDirection.x * 0.5f + otherAtomPos.x,
            trasformedDirection.y * 0.5f + otherAtomPos.y,
            trasformedDirection.z * 0.5f + otherAtomPos.z);*/
        sbond.transform.position = Vector3.Lerp(otherAtomPos, otherAtomPos + transformedDirection * length, 0.5f);

        Vector3 scale = prefebBond.transform.lossyScale;
        scale.y = length * 0.5f;
        sbond.transform.localScale = scale;
        sbond.transform.LookAt(otherAtomPos);
        sbond.transform.Rotate(new Vector3(90, 0, 0));

        //记录连接的目标原子
        catom = collider.gameObject;

    }

    /*void DeleteSbond(GameObject gameObject)
    {
        //delete bonds that shown before
        print("exit");
        if (sbonds.ContainsKey(gameObject))
        {
            GameObject sbond = sbonds[gameObject];
            sbonds.Remove(gameObject);
            Destroy(sbond);
        }

    }*/

	void OnTriggerExit (Collider collider) {

        
        if (collider.gameObject != catom)
            return;

        if (collider.isTrigger)
            return;

        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        if (!grabbed)
            return;

        print("exit trigger");
        Destroy(sbond);
        gameObject.transform.parent.GetComponent<MoleculesAction>().ResetConnectable();
        sbond = null;
        /*DeleteSbond(collider.gameObject);

        if (catom == gameObject)
        {
            catom = null;
        }*/

    }

}
