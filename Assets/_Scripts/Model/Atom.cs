using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    //max size is 117 scale 0.6
    //const float scale = 1.0f;

    public int Id;

    public string Symbol;
    public int Valence;
	public int Connected;
    public bool InRing;
    public BondType MaxBomdType;
    public List<Vector4> vbonds;
	public List<GameObject> Bonds;

    //public static int CurrentId = 0;

    public Atom()
    {
        Connected = 0;
        InRing = false;
        MaxBomdType = BondType.SINGLE;
        Bonds = new List<GameObject>();
    }
	
	public void addBond(GameObject b)
	{
        Bonds.Add (b);
        BondType type = b.GetComponent<Bond>().Type;
        switch (type)
        {
            case BondType.SINGLE:
                Connected++;
                break;
            case BondType.DOUBLE:
                Connected += 2;
                break;
            case BondType.TRIPLE:
                Connected += 3;
                break;
        }
        
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
        BondType type = bond.GetComponent<Bond>().Type;
        switch (type)
        {
            case BondType.SINGLE:
                Connected--;
                break;
            case BondType.DOUBLE:
                Connected -= 2;
                break;
            case BondType.TRIPLE:
                Connected -= 3;
                break;
        }
    }

    //get angle of a single bind
	public Vector3 getAngle(ref int returnIndex)
    {
        for (int i = 0; i < vbonds.Count; ++i)
        {
            if (vbonds[i].w == 0)
            {
                Vector4 v = vbonds[i];
                v.w = 1;
                vbonds[i] = v;
                returnIndex = i;
                return new Vector3(v.x, v.y, v.z);
            }
        }
        return Vector3.zero;
    }

    public int getFreeVbondIdx()
    {
        int ret = -1;
        for (int i = 0; i < vbonds.Count; ++i)
        {
            if (vbonds[i].w == 0)
            {
                ret = i;
                break;
            }
        }
        return ret;
    }

    public Vector3 getVbondByIndex(int index)
    {
        if (vbonds[index].w == 1)
            return Vector3.zero;
        Vector4 v = vbonds[index];
        v.w = 1;
        vbonds[index] = v;
        return new Vector3(v.x, v.y, v.z);
    }

    public GameObject getBond(GameObject adjacent)
    {
        foreach(GameObject b in Bonds)
        {
            if(b.GetComponent<Bond>().getAdjacent(gameObject) == adjacent)
                return b;
        }
        return null;
    }

    public int getVbondIdx(Vector3 pos)
    {
        int idx = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < vbonds.Count; i++)
        {
            if (vbonds[i].w == 1)
            {
                print(vbonds[i].ToString());
                continue;
            }
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

    public int getVbondCount()
    {
        return vbonds.Count;
    }

    public string toString()
    {
        return Symbol + " " + Id.ToString() + " " + Valence.ToString() + " ";
    }

    public void UpdateMaxBond()
    {
        MaxBomdType = BondType.SINGLE;
        foreach(GameObject bond in Bonds)
        {
            if(bond.GetComponent<Bond>().Type > MaxBomdType)
            {
                MaxBomdType = bond.GetComponent<Bond>().Type;
            }
        }
    }

    public int getVbondIdxOfAtom(GameObject atom, GameObject bond)
    {
        int idx = -1;
        if (bond.GetComponent<Bond>().A1 == atom)
            idx = bond.GetComponent<Bond>().A1Index;
        else if(bond.GetComponent<Bond>().A2 == atom)
            idx = bond.GetComponent<Bond>().A2Index;

        return idx;
    }
}