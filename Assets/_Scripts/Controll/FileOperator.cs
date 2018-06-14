using UnityEngine;
using System;
using System.Collections.Generic;
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
            List<string> bonds = new List<string>(); 
            foreach (Transform child in mole.transform)
            {
                if (child.tag != "Bond" && child.tag != "Component")
                    continue;
                string s;
                if (child.tag == "Bond")
                {
                    bonds.Add(child.GetComponent<Bond>().toString());
                }
                else
                {
                    s = child.GetComponent<Atom>().toString() + TypeConvert.Vec3ToStr(child.position);
                    sw.WriteLine(s);
                }
            }
            //write bonds's info after all atoms' info have been written
            //in case atom doesn't exist when loading model
            foreach(string s in bonds)
            {
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
                mole = gm.GenerateMolecule(int.Parse(sArray[1]));
            }
            else if(sArray[0] == "Bond")
            {
                if (sArray.Length != 6)
                {
                    Debug.Log("File Damaged.");
                    return -1;
                }
                gm.GenBondForMole(sArray, mole);
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
