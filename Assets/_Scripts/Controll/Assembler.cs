using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour {
	   
	void OnTriggerEnter(Collider collider)
	{
		int id1 = int.Parse (collider.gameObject.GetComponent<Text> ().text);
		int id2 = int.Parse(GetComponent<Text>().text);
        Molecule m = GameManager.curMolecule;
        Atom a1 = m.getAtomById(id1);
        Atom a2 = m.getAtomById(id2);
        if (a1.Connected == a1.Valence || a2.Connected == a2.Valence)
        {
            Debug.Log("error connect");
            return;
        }

        Bond b = new Bond(id1, id2);
        GameManager.curMolecule.addBond(b);

        //calculate expected position of this atom
        string[] pair = new string[] { a1.Symbol, a2.Symbol };
        float length = Config.BondLengthTable[pair];
        Vector3 pos = collider.gameObject.transform.position;
        Vector3 angle = a1.getAngle();
        Vector3 expectedPos = angle * length + pos;

        //draw bond
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = Vector3.Lerp(pos, expectedPos, 0.5f);
        cylinder.transform.localScale = new Vector3(0.2f, length, 0.2f);
        float ang = Mathf.Atan((expectedPos.z - pos.z) / (expectedPos.y - pos.y)) * 180 / Mathf.PI;
        cylinder.transform.Rotate(new Vector3(angle.z, 0,angle.x), ang);
        GameManager.objs.Add(cylinder);

        a1.addBond(b);
        a2.addBond(b);

        //rotate this atom to match bond angle
        Vector3 angle2 = a2.getAngle();
        Vector3 vec1 = collider.gameObject.transform.TransformDirection(angle);
        Vector3 vec2 = transform.TransformDirection(angle2);

        float an = Vector3.Angle(vec1, vec2);
		transform.Rotate(Vector3.Cross(vec1, vec2), an);
		Vector3 offset = expectedPos - transform.position;

    }

	void OnTriggerExit (Collider collider) {
	}

}
