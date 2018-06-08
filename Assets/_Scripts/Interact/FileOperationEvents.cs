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
        if (GameManager.molecules.Count == 0)
        {
            Debug.Log("Nothing to save");
            return;
        }
        string path = "Molecule" + DateTime.Now.ToString("yyMMddHHmmssff");
        FileOperator.SaveModel(path);
    }
}
