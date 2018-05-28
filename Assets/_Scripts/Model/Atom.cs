using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

    //max size is 117 scale 0.6
    const float scale = 1.0f;

    public int Id { set; get; }

    public string Symbol { set; get; }
    public int Valence { set; get; }
    public Vector3 Pos { set; get; }
	public int Connected { set; get; }
    public List<Vector4> vbonds;
	public List<Bond> Bonds;

    //public static int CurrentId = 0;

    public Atom()
    {
        Connected = 0;
        vbonds = new List<Vector4>();
    }
	
	public void addBond(Bond b)
	{
		Bonds.Add (b);
        int count = vbonds.Count;
		for(int i = 0; i < count; ++i) {
			if (vbonds[i].w == 0) {
                Vector4 v = vbonds[i];
                v.w = 1;
                vbonds[i] = v;
				break;
			}
		}
		Connected += 1;
	}
	
	public Vector3 getAngle()
	{
		foreach(Vector4 v in vbonds){
			if (v.w == 0)
				return new Vector3 (v.x, v.y, v.z);
		}
        return new Vector3(-1, -1, -1);
	}
    
}