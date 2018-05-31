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

        //set abstract bond
        Bond b = sbond.AddComponent<Bond>();
        b.A1 = a1.Id;
        b.A2 = a2.Id;
        b.Type = BondType.SINGLE;
        a1.addBond(b);
        a2.addBond(b);

        //calculate expected position of this atom
        Vector3 expectedPos = catom.transform.position + 2 * (sbond.transform.position - catom.transform.position);

        // 更新目标原子和抓取原子的vbond,标记一个化学键为已使用
        Vector4 usedVbond = a1.vbonds[catomBondIndex];
        usedVbond.w = 1;
        a1.vbonds[catomBondIndex] = usedVbond;

        //move and rotate grabbed atom to fix positon
        //translate
        transform.parent.transform.Translate(expectedPos - transform.position);
        //rotate    
        Vector3 vec1 = (sbond.transform.position - catom.transform.position).normalized;
        Vector3 vec2 = (transform.TransformDirection(a2.getAngle())).normalized;

        float an = Vector3.Angle(vec1, vec2);

        print("catom pos:" + catom.transform.position + "sbond pos:" + sbond.transform.position);
        print("catom dir: " + vec1);
        print(catom.transform.TransformDirection(new Vector3(a1.vbonds[catomBondIndex].x, a1.vbonds[catomBondIndex].y, a1.vbonds[catomBondIndex].z)));
        print("self dir: " + vec2);

        print("angle: " + an);

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
        

        //transform.parent.transform.RotateAround(transform.position, Vector3.Cross(vec1, vec2), an);
        /*Vector3 vec1 = catom.transform.TransformDirection(angle);
        Vector3 vec2 = -transform.TransformDirection(a2.getAngle());

        print(vec1);
        print(vec2);
        float an = Vector3.Angle(vec1, vec2);
        print("angle: " + an);
        transform.parent.transform.RotateAround(transform.position, Vector3.Cross(vec1, vec2), an);*/

        //then merge two molecules
        Molecule m = catom.transform.parent.gameObject.GetComponent<Molecule>();
        GameObject parent = transform.parent.gameObject;
        foreach (Transform child in transform.parent.transform)
        {
            print("child info: " + child);
            child.parent = catom.transform.parent;
            if (child.gameObject.tag != "Bond")
            {
                child.gameObject.GetComponent<Atom>().Id = m.CurrentAtomId++;
            }
        }

        Destroy(parent);


        /*if (catom != null) {
            DeleteSbond(catom);
            // get molecules and atoms
            Molecule m1 = catom.GetComponentInParent<Molecule>();
            Molecule m2 = GetComponentInParent<Molecule>();
            Atom a1 = catom.GetComponent<Atom>();
            Atom a2 = GetComponent<Atom>();

            if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
            {
                return;
            }

            //calculate expected position of this atom
            string key;
            if(a1.Symbol.Length <= a2.Symbol.Length && a1.Symbol[0] < a2.Symbol[0])
            {
                key = a1.Symbol + a2.Symbol;
            }
            else
            {
                key = a2.Symbol + a1.Symbol;
            }
			float length = Config.BondLengthTable[key];
            Vector3 pos = catom.transform.position;
			Vector3 angle = a1.getAngle();
			Vector3 expectedPos = angle * length + pos;

			//draw bond
            GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
            GameObject bond = Instantiate(prefebMole);

            bond.transform.position = Vector3.Lerp(pos, expectedPos, 0.5f);
            Vector3 scale = prefebMole.transform.lossyScale;
            scale.y = length * 0.5f;
            bond.transform.localScale = scale;
            bond.transform.LookAt(expectedPos);
            bond.transform.Rotate(new Vector3(90, 0, 0));
            bond.transform.parent = catom.transform.parent;

            //set abstract bond
            Bond b = bond.AddComponent<Bond>();
            b.A1 = a1.Id;
            b.A2 = a2.Id;
            b.Type = BondType.SINGLE;
            a1.addBond(b);
            a2.addBond(b);

            //move and rotate this atom and the molecule to match bond angle vector
            transform.parent.transform.Translate(expectedPos - transform.position);
			Vector3 vec1 = catom.transform.TransformDirection(angle);
			Vector3 vec2 = -transform.TransformDirection(a2.getAngle());

            print(vec1);
            print(vec2);
			float an = Vector3.Angle(vec1, vec2);
            print("angle: " + an);
			transform.parent.transform.RotateAround(transform.position, Vector3.Cross(vec1, vec2), an);

            //merge two molecules into one
            Molecule m = catom.transform.parent.gameObject.GetComponent<Molecule>();
            GameObject parent = transform.parent.gameObject;
            foreach(Transform child in transform.parent.transform)
            {
                if (child.gameObject.tag != "Bond")
                {
                    child.parent = catom.transform.parent;
                    child.gameObject.GetComponent<Atom>().Id = m.CurrentAtomId++;
                }
            }

            Destroy(parent);
		}*/
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
            return;

        print("enter trigger");
        //DeleteSbond(collider.gameObject);


        Atom otherAtom = collider.gameObject.GetComponent<Atom>();
        Atom thisAtom = collider.gameObject.GetComponent<Atom>();

        if (otherAtom.Connected == otherAtom.Valence || thisAtom.Connected == thisAtom.Valence)
        {
            Debug.Log("can not be connected");
            return;
        }

        Vector3 otherAtomPos = collider.gameObject.transform.position;
        Vector3 thisAtomPos = transform.position;

        //获取匹配的化学键位置
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

        Vector3 trasformedDirection = collider.transform.TransformDirection(new Vector3(otherAtomVbonds[catomBondIndex].x, otherAtomVbonds[catomBondIndex].y, otherAtomVbonds[catomBondIndex].z));
        /*sbond.transform.position = new Vector3(otherAtomVbonds[catomBondIndex].x * 0.5f + otherAtomPos.x, 
            otherAtomVbonds[catomBondIndex].y * 0.5f + otherAtomPos.y, 
            otherAtomVbonds[catomBondIndex].z * 0.5f + otherAtomPos.z);*/
        sbond.transform.position = new Vector3(trasformedDirection.x * 0.5f + otherAtomPos.x,
            trasformedDirection.y * 0.5f + otherAtomPos.y,
            trasformedDirection.z * 0.5f + otherAtomPos.z);
        Vector3 scale = prefebBond.transform.lossyScale;
        scale.y = length * 0.5f;
        sbond.transform.localScale = scale;
        sbond.transform.LookAt(otherAtomPos);
        sbond.transform.Rotate(new Vector3(90, 0, 0));

        //记录连接的目标原子
        catom = collider.gameObject;
        /*Atom a1 = collider.gameObject.GetComponent<Atom>();
        Atom a2 = GetComponent<Atom> ();
        
        if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
        {
            Debug.Log("can not be connected");
            return;
        }

		// show bond cloud be connected
		Vector3 pos1 = collider.gameObject.transform.position;
		Vector3 pos2 = transform.position;

        GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
        GameObject bond = Instantiate(prefebMole);
        bond.GetComponent<Renderer>().material.color = Color.yellow;
        bond.transform.position = Vector3.Lerp(pos1, pos2, 0.5f);
        float distance = Vector3.Distance(pos1, pos2);
        Vector3 scale = prefebMole.transform.lossyScale;
        scale.y = distance * 0.5f;
        bond.transform.localScale = scale;

        bond.transform.LookAt(pos2);
        bond.transform.Rotate(new Vector3(90, 0, 0));


        sbonds.Add (collider.gameObject, bond);
        catom = collider.gameObject;*/
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
