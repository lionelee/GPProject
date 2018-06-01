using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    //max size is 117 scale 0.6
    //const float scale = 1.0f;

    public int Id { set; get; }

    public string Symbol { set; get; }
    public int Valence { set; get; }
	public int Connected { set; get; }
    public List<Vector4> vbonds;
	public List<GameObject> Bonds;

    //public static int CurrentId = 0;

    public Atom()
    {
        Connected = 0;
        Bonds = new List<GameObject>();
    }
	
	public void addBond(GameObject b)
	{
        Bonds.Add (b);
		Connected ++;
	}
	
    public void removeBond(GameObject bond)
    {
        Bonds.Remove(bond);

        int vbondsIndex;
        if(bond.GetComponent<Bond>().A1 == gameObject)
        {
            vbondsIndex = bond.GetComponent<Bond>().A1Index;
        } else
        {
            vbondsIndex = bond.GetComponent<Bond>().A2Index;
        }

        Vector4 v = vbonds[vbondsIndex];
        v.w = 0;
        vbonds[vbondsIndex] = v;
        Connected--;
    }

	public Vector3 getAngle(int returnIndex)
    {
        for (int i = 0; i < vbonds.Count; ++i)
        {
            if (vbonds[i].w == 0)
            {
                Vector4 v = vbonds[i];
                v.w = 1;
                vbonds[i] = v;
                print(vbonds[i]);
                returnIndex = i;
                return new Vector3(v.x, v.y, v.z);
            }
        }
        return Vector3.zero;
	}
    

    public string toString()
    {
        return Symbol + " " + Id.ToString() + " " + Valence.ToString() ;
    }
}