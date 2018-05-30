using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour
{
    GameObject catom;
	Dictionary<GameObject, GameObject> sbonds;

	void Start()
	{
        sbonds = new Dictionary<GameObject, GameObject>();
        catom = null;
	}

	public void Connect()
	{
		if (catom != null) {
            // get molecules and atoms
            Molecule m1 = catom.GetComponentInParent<Molecule>();
            Molecule m2 = GetComponentInParent<Molecule>();
            Atom a1 = catom.GetComponent<Atom>();
            Atom a2 = GetComponent<Atom>();
            
			//calculate expected position of this atom
			string[] pair = new string[] { a1.Symbol, a2.Symbol };
			float length = Config.BondLengthTable[pair];
            Vector3 pos = catom.transform.position;
			Vector3 angle = a1.getAngle();
			Vector3 expectedPos = angle * length + pos;

			//draw bond
            GameObject prefebMole = (GameObject)Resources.Load("_Prefebs/SingleBond") as GameObject;
            GameObject bond = Instantiate(prefebMole);
            bond.transform.parent = transform.parent;
            bond.transform.position = Vector3.Lerp(pos, expectedPos, 0.5f);
            Vector3 scale = prefebMole.transform.lossyScale;
            scale.y = length;
            bond.transform.localScale = scale;
            bond.transform.LookAt(expectedPos);
            bond.transform.Rotate(new Vector3(90, 0, 0));
			GameManager.bonds.Add(bond);

            //set up abstract bond
            Bond b = bond.AddComponent<Bond>();
            b.A1 = a1.Id;
            b.A2 = a2.Id;
            b.Type = BondType.SINGLE;
            a1.addBond(b);
            a2.addBond(b);

            //move and rotate this atom and the molecule to match bond angle vector
            transform.parent.transform.Translate(expectedPos - transform.position);
            Vector3 angle2 = a2.getAngle();
			Vector3 vec1 = catom.transform.TransformDirection(angle);
			Vector3 vec2 = transform.TransformDirection(angle2);

			float an = Vector3.Angle(vec1, vec2);
			transform.parent.transform.Rotate(Vector3.Cross(vec1, vec2), an);

            //merge two molecules into one
            Molecule m = catom.transform.parent.gameObject.GetComponent<Molecule>();
            foreach(Transform child in transform.parent.transform)
            {
                child.parent = catom.transform.parent;
                child.gameObject.GetComponent<Atom>().Id = m.CurrentAtomId++;
            }
            Destroy(transform.parent.gameObject);
		}
	}
	   
	void OnTriggerEnter(Collider collider)
	{
        if (collider.gameObject.GetComponent<Atom>() == null)
            return;
        if (sbonds.ContainsKey(collider.gameObject))
            DeleteSbond(collider.gameObject);
        Atom a1 = collider.gameObject.GetComponent<Atom>();
        Atom a2 = GetComponent<Atom>();
        
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
        float distance = 0.5f * Vector3.Distance(pos1, pos2);
        Vector3 scale = prefebMole.transform.lossyScale;
        scale.y = distance;
        bond.transform.localScale = scale;
        bond.transform.LookAt(pos2);
        bond.transform.Rotate(new Vector3(90, 0, 0));

        sbonds.Add (collider.gameObject, bond);

    }

	void OnTriggerExit (Collider collider) {
        if (sbonds.ContainsKey(collider.gameObject))
        {
            DeleteSbond(collider.gameObject);
        }
        if (catom == gameObject)
        {
            catom = null;
        }
    }

    void DeleteSbond(GameObject gameObject)
    {
        //delete bonds that shown before
        GameObject sbond = sbonds[gameObject];
        sbonds.Remove(gameObject);
        Destroy(sbond);
    }
}
