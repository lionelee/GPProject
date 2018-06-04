using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadFileList : MonoBehaviour {

    public GameObject FileEntry;

	// Use this for initialization
	void Start () {
        FileInfo[] files = FileOperator.dir.GetFiles("*.smol",SearchOption.TopDirectoryOnly);
        string dirpath = FileOperator.dirpath+ "\\";
        string suffix = ".smol";
        foreach (FileInfo fi in files)
        {
            GameObject entry = Instantiate(FileEntry, transform) as GameObject;
            string filename = fi.ToString();
            filename = filename.Remove(filename.IndexOf(dirpath), dirpath.Length);
            filename = filename.Remove(filename.IndexOf(suffix), suffix.Length);
            entry.transform.GetComponentInChildren<Text>().text = filename;
        }
    }
	
}
