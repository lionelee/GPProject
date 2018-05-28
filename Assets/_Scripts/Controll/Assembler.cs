using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour {

	bool is_grab;
	int aid;
	List<GameObject> sbonds;

	void Start()
	{
		sbonds = new List<GameObject> ();
		is_grab = true;
		aid = -1;
	}

	void Update()
	{
		if (!is_grab && aid >= 0) { // connect two atom and build bond
			int id1 = aid; 
			int id2 = int.Parse(GetComponent<Text>().text);
			Molecule m = GameManager.curMolecule;
			Atom a1 = m.getAtomById(id1);
			Atom a2 = m.getAtomById(id2);

			Bond b = new Bond(id1, id2);
			GameManager.curMolecule.addBond(b);

			//calculate expected position of this atom
			string[] pair = new string[] { a1.Symbol, a2.Symbol };
			float length = Config.BondLengthTable[pair];
			Vector3 pos = GetComponent<Collider>().gameObject.transform.position;
			Vector3 angle = a1.getAngle();
			Vector3 expectedPos = angle * length + pos;

			//draw bond
			GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cylinder.transform.position = Vector3.Lerp(pos, expectedPos, 0.5f);
			cylinder.transform.localScale = new Vector3(0.2f, length, 0.2f);
			float ang = Mathf.Acos ((expectedPos.y - pos.y) / length) * 180 / Mathf.PI;
			cylinder.transform.Rotate(new Vector3(angle.z, 0,angle.x), ang);
			GameManager.objs.Add(cylinder);

			/*a1.addBond(b);
			a2.addBond(b);*/

			//rotate this atom to match bond angle
			Vector3 angle2 = a2.getAngle();
			Vector3 vec1 = GetComponent<Collider>().gameObject.transform.TransformDirection(angle);
			Vector3 vec2 = transform.TransformDirection(angle2);

			float an = Vector3.Angle(vec1, vec2);
			transform.Rotate(Vector3.Cross(vec1, vec2), an);
			Vector3 offset = expectedPos - transform.position;

			//reset aid
			aid = -1;
		}
	}
	   
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

		//set aid to record atom collided
		aid = id1;

		// show bond cloud be connected
		Vector3 pos1 = collider.gameObject.transform.position;
		Vector3 pos2 = transform.position;
		GameObject cylinder = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
		cylinder.transform.position = Vector3.Lerp (pos1, pos2, 0.5f);
		float distance = Vector3.Distance (pos1, pos2);
		cylinder.transform.localScale = new Vector3 (0.2f, 0.5f * distance, 0.2f);
		float deltaX = pos1.x - pos2.x;
		float deltaY = pos1.y - pos2.y;
		float deltaZ = pos1.z - pos2.z;
		float an = Mathf.Acos((pos1.y - pos2.y) / distance) * 180 / Mathf.PI;
		cylinder.transform.Rotate (new Vector3 (pos2.z - pos1.z , 0, pos1.x - pos2.x), an);
        
		sbonds.Add (cylinder);

    }

	void OnTriggerExit (Collider collider) {
		//delete bonds that shown before
		sbonds.Remove(collider.gameObject);
		GameObject.Destroy (collider.gameObject);
	}

}
