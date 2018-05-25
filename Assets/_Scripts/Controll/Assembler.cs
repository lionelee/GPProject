using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour {

	void OnTriggerEnter(Collider collider)
	{
		int id1 = int.Parse (collider.gameObject.GetComponent<Text> ().text);
		int id2 = int.Parse(GetComponent<Text>().text);
		Connnect (id1, id2);
	}

	public void Connnect(int id1, int id2){
        Molecule m = GameManager.curMolecule;
		Atom a1 = m.getAtomById (id1);
		Atom a2 = m.getAtomById (id2);
        if(a1.Connected == a1.Valence || a2.Connected == a2.Valence)
        {
            Debug.Log("error connect");
            return;
        }
		Bond b = new Bond (id1, id2);
		m.addBond (b);
		DrawBond (ref a1, ref a2);
		a1.addBond (b);
		a2.addBond (b);
	}

	public void DrawBond(ref Atom a1, ref Atom a2)
	{
		string[] pair = new string[]{ a1.Symbol, a2.Symbol};
		float length = Config.BondLengthTable [pair];
		GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
		obj.transform.localScale = new Vector3 (0.1f, length, 0.1f);

		Vector4 vec1, vec2;
		vec1 = a1.getAngle ();
        vec2 = a2.getAngle ();

	}

}
