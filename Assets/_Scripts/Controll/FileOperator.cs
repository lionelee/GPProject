using UnityEngine;
using System;
using System.IO;

public class FileOperator
{
    public static void SaveModel(string path)
    {
        /*FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(fs);
        foreach(GameObject mole in GameManager.molecules)
        {
            foreach(GameObject at in mole.GetComponentsInChildren<GameObject>())
            {
                string s = at.GetComponent<Atom>().toString() + at.transform.position.ToString();
                sw.WriteLine(s);
            }
            foreach(Bond b in mole.GetComponent<Molecule>().getBonds())
            {
                sw.WriteLine(b.toString());
            }
        }
        sw.Close();
        fs.Close();*/
    }

    public static void ReadModel(string path)
    {
        /*FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(fs);

        GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GameObject mole = gm.GenerateMolecule();

        while (sr.Peek() > 0)
        {
            string line = sr.ReadLine();
            string[] sArray = line.Split(' ');
            if(sArray[0] == "Bond")
            {
                if (sArray.Length != 4)
                {
                    Debug.Log("File Damaged.");
                    return;
                }
                Bond b = new Bond(int.Parse(sArray[2]), int.Parse(sArray[3]));
                mole.GetComponent<Molecule>().addBond(b);
            }
            else
            {
                if (sArray.Length != 4)
                {
                    Debug.Log("File Damaged.");
                    return;
                }
                gm.GenAtomForMole(sArray, mole);
            }
        }
        sr.Close();
        fs.Close();*/
    }
}
