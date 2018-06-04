using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUtil : MonoBehaviour {
    /// <summary>
    /// 从 start 原子开始进行 DFS 遍历分子，将遍历结果加到 reault list 中，excepted list 中的组件会在开始时设为已访问
    /// </summary>
    /// <param name="start"> 起点原子 </param>
    /// <param name="excepted"> 忽略的组件列表 </param>
    /// <param name="result"> 结果列表 </param>
	public static void DfsMolecule(GameObject start, List<GameObject> excepted, List<GameObject> result)
    {
        List<GameObject> visited = new List<GameObject>();
        Stack<GameObject> stack = new Stack<GameObject>();
        GameObject cur;

        //起点只能是原子
        if (start.GetComponent<Atom>() == null)
            return;

        // 初始化忽略列表中的元素，设为已访问
        foreach(GameObject component in excepted)
        {
            visited.Add(component);
        }

        stack.Push(start);
        result.Add(start);
        visited.Add(start);
        while(stack.Count != 0)
        {
            cur = stack.Pop();
            //Atoms
            if(cur.GetComponent<Atom>() != null)
            {
                foreach(GameObject bond in cur.GetComponent<Atom>().Bonds)
                {
                    if (!visited.Contains(bond))
                    {
                        stack.Push(bond);
                        result.Add(bond);
                        visited.Add(bond);
                    }
                }

            }
            //  Bonds
            else
            {
                GameObject atom1 = cur.GetComponent<Bond>().A1,
                    atom2 = cur.GetComponent<Bond>().A2;
                if (!visited.Contains(atom1))
                {
                    stack.Push(atom1);
                    result.Add(atom1);
                    visited.Add(atom1);
                }
                if (!visited.Contains(atom2))
                {
                    stack.Push(atom2);
                    result.Add(atom2);
                    visited.Add(atom2);
                }
            }
        }
    }

    /// <summary>
    /// 检查两个原子是否相邻，即恰好位于某个键的两端。
    /// </summary>
    /// <param name="atom1"></param>
    /// <param name="atom2"></param>
    /// <returns></returns>
    public static bool IsAdjacent(GameObject atom1, GameObject atom2)
    {
        foreach(GameObject bond in atom1.GetComponent<Atom>().Bonds)
        {
            if (atom2 == bond.GetComponent<Bond>().A1 || atom2 == bond.GetComponent<Bond>().A2)
                return true;
        }
        return false;
    }
}
