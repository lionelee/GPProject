using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeConvert
{
    
    public static Vector3 StrToVec3(string s)
    {
        if (s.Length <= 0)
            return Vector3.zero;
        string[] pos = s.Split(',');
        if(pos != null && pos.Length == 3)
        {
            float x = float.Parse(pos[0]);
            float y = float.Parse(pos[1]);
            float z = float.Parse(pos[2]);
            return new Vector3(x, y, z);
        }
        return Vector3.zero;
    }

    public static string Vec3ToStr(Vector3 pos)
    {
        return pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString(); 
    }
}
