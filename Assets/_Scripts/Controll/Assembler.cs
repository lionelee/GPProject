using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour
{
    GameObject sbond;
    GameObject satom;

    GameObject catom;
    int catomBondIndex;
    Vector3 expectedPos;
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
    
	public void Connect()
	{
        if (sbond == null)
            return;

        sbond.GetComponent<Renderer>().material = Resources.Load("_Materials/Default") as Material;
        Destroy(satom);
        
        // get molecules and atoms
        Molecule m1 = catom.GetComponentInParent<Molecule>();
        Molecule m2 = GetComponentInParent<Molecule>();
        Atom a1 = catom.GetComponent<Atom>();
        Atom a2 = GetComponent<Atom>();

        //set sbond's parent to molecule
        sbond.transform.parent = catom.transform.parent;
        //set abstract bond
        Bond b = sbond.AddComponent<Bond>();
        b.A1 = catom;
        b.A2 = transform.gameObject;
        b.Type = BondType.SINGLE;
        a1.addBond(sbond);
        a2.addBond(sbond);

        //calculate expected position of this atom
        //Vector3 expectedPos = catom.transform.position + 2 * (sbond.transform.position - catom.transform.position);

        // 更新目标原子的vbond,标记一个化学键为已使用,抓取原子的vbond由之后的getAngle()函数更新
        a1.setVbondUsed(catomBondIndex);

        //move and rotate grabbed atom to fix positon
        //translate
        transform.parent.transform.position += expectedPos - transform.position;

        //rotate    
        Vector3 vec1 = (sbond.transform.position - catom.transform.position).normalized;
        Vector3 vec2 = (transform.TransformDirection(a2.getAngle())).normalized;
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
       
        //then merge the two molecules
        Molecule m = catom.transform.parent.gameObject.GetComponent<Molecule>();
        GameObject parent = transform.parent.gameObject;
        List<GameObject> children = new List<GameObject>();
        foreach(Transform child in transform.parent.transform)
        {
            children.Add(child.gameObject);
        }
        foreach(GameObject child in children)
        {
            if (child.tag != "Bond" && child.tag != "Component")
                continue;
            child.transform.parent = catom.transform.parent;
            if (child.tag != "Bond")
            {
                child.GetComponent<Atom>().Id = m.CurrentAtomId++;
            }
        }

        //destroy old molecule
        Destroy(parent);
        sbond = null;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.isTrigger)
            return;

        if (collider.gameObject.GetComponent<Atom>() == null)
            return;

        if (!grabbed)
            return;

        /*if (sbond != null)
        {
            Destroy(sbond);
            Destroy(satom);
        }*/

        Atom otherAtom = collider.gameObject.GetComponent<Atom>();
        Atom thisAtom = transform.gameObject.GetComponent<Atom>();
        uint otherValence = (uint)Mathf.Abs(otherAtom.Valence);
        otherValence = otherValence == 0 ? 1 : otherValence;
        uint thisValence = (uint)Mathf.Abs(thisAtom.Valence);
        thisValence = thisValence == 0 ? 1 : thisValence;

        if (otherAtom.Symbol != thisAtom.Symbol)
        {
            int bondValence = otherAtom.Valence / (int)otherValence + thisAtom.Valence / (int)thisValence;
            if (bondValence != 0) return;
        }

        if (otherAtom.Connected == otherValence || thisAtom.Connected == thisValence)
        {
            Debug.Log("can not be connected");
            return;
        }

        Vector3 otherAtomPos = collider.gameObject.transform.position;
        Vector3 thisAtomPos = transform.position;

        //get the bond length by key
        string key;
        if (otherAtom.Symbol.Length <= thisAtom.Symbol.Length && otherAtom.Symbol[0] < thisAtom.Symbol[0])
        {
            key = otherAtom.Symbol + thisAtom.Symbol;
        }
        else
        {
            key = otherAtom.Symbol + otherAtom.Symbol;
        }
        float length = Config.BondLengthTable[key];

        //get nearest vbond index
        catomBondIndex = otherAtom.getVbondIdx(transform.position);
        if (catomBondIndex == -1)
            return;

        //提示分子当前原子可被连接
        gameObject.transform.parent.GetComponent<MoleculesAction>().SetConnectableAtom(gameObject);

        //draw bond can be connected
        GameObject prefebBond = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        sbond = Instantiate(prefebBond);
        sbond.GetComponent<Renderer>().material = Resources.Load("_Materials/Highlight") as Material;

        //translate bond
        Vector3 direction = otherAtom.vbonds[catomBondIndex];
        Vector3 trasformedDirection = collider.transform.TransformDirection(direction);
        expectedPos = otherAtomPos + trasformedDirection * length;
        sbond.transform.position = Vector3.Lerp(otherAtomPos, expectedPos, 0.5f);

        //rotate bond
        Vector3 scale = prefebBond.transform.lossyScale;
        scale.y = length * 0.5f;
        sbond.transform.localScale = scale;
        sbond.transform.LookAt(otherAtomPos);
        sbond.transform.Rotate(new Vector3(90, 0, 0));

        //show expected position
        satom = Instantiate(collider.gameObject);
        satom.GetComponent<Renderer>().material = Resources.Load("_Materials/Red") as Material;
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
