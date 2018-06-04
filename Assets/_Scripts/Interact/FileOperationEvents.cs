using UnityEngine;
using UnityEngine.UI;
using System;

public class FileOperationEvents : MonoBehaviour {

    public GameObject FileOpCanvas;

    public void OnOpenButtonClick(GameObject FileBrowser)
    {
        FileOpCanvas.SetActive(false);
        FileBrowser.SetActive(true);
    }

    public void OnSaveButtonClick()
    {
        FileOpCanvas.SetActive(false);
        string path = "Molecule" + DateTime.Now.ToString();
        FileOperator.SaveModel(path);
    }
}
