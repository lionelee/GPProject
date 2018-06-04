using UnityEngine;
using System;
using System.IO;

public class FileOperator
{
    public static string dirpath = Directory.GetCurrentDirectory();
    public static DirectoryInfo dir = new DirectoryInfo(dirpath);

    static string getPath(string name)
    {
        return dirpath + "\\" + name + ".smol";
    }

    public static void SaveModel(string name)
    {
        string path = getPath(name);
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(fs);
        foreach(GameObject mole in GameManager.molecules)
        {
            sw.WriteLine(mole.GetComponent<Molecule>().toString());
            foreach (Transform child in mole.transform)
            {
                if (child.tag != "Bond" && child.tag != "Component")
                    continue;
                string s;
                if (child.tag == "Bond")
                {
                    s = child.GetComponent<Bond>().toString();
                }
                else
                {
                    s = child.GetComponent<Atom>().toString() + child.position.ToString();
                }
                sw.WriteLine(s);
            }
        }
        sw.Close();
        fs.Close();
    }

    public static int ReadModel(string name)
    {
        string path = getPath(name);
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(fs);

        GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GameObject mole = null;

        while (sr.Peek() > 0)
        {
            string line = sr.ReadLine();
            string[] sArray = line.Split(' ');
            if(sArray[0] == "Molecule")
            {
                if (sArray.Length != 4)
                {
                    Debug.Log("File Damaged.");
                    return -1;
                }
                mole = gm.GenerateMolecule();
            }
            else if(sArray[0] == "Bond")
            {
                if (sArray.Length != 4)
                {
                    Debug.Log("File Damaged.");
                    return -1;
                }
                gm.GenBondForMole(int.Parse(sArray[1]), int.Parse(sArray[2]), int.Parse(sArray[3]), mole);
            }
            else
            {
                if (sArray.Length != 4)
                {
                    Debug.Log("File Damaged.");
                    return -1;
                }
                gm.GenAtomForMole(sArray, mole);
            }
        }
        sr.Close();
        fs.Close();
        return 0;
    }
}
