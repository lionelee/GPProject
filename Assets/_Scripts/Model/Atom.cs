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

    public Vector3 getAngle()
    {
        for (int i = 0; i < vbonds.Count; ++i)
        {
            if (vbonds[i].w == 0)
            {
                Vector4 v = vbonds[i];
                v.w = 1;
                vbonds[i] = v;
                print(vbonds[i]);
                return new Vector3(v.x, v.y, v.z);
            }
        }
        return Vector3.zero;
    }

    public int getVbondIdx(Vector3 pos)
    {
        int idx = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < vbonds.Count; i++)
        {
            if (vbonds[i].w == 1)
                continue;
            Vector3 dir = transform.TransformDirection(new Vector3(vbonds[i].x, vbonds[i].y, vbonds[i].z));
            float distance = (transform.position + dir - pos).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                idx = i;
            }
        }
        return idx;
    }

    public void setVbondUsed(int idx)
    {
        Vector4 usedVbond = vbonds[idx];
        usedVbond.w = 1;
        vbonds[idx] = usedVbond;
    }
    
    public string toString()
    {
        return Symbol + " " + Id.ToString() + " " + Valence.ToString() ;
    }
}