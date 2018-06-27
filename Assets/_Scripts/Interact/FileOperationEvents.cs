using UnityEngine;
using UnityEngine.UI;
using System;

public class FileOperationEvents : MonoBehaviour {

    public GameObject FileOpCanvas;
    public GameObject KeyboardInput;

    public void OnOpenButtonClick(GameObject FileBrowser)
    {
        FileOpCanvas.SetActive(false);
        FileBrowser.SetActive(true);
    }

    public void OnSaveButtonClick(GameObject KeyboardInput)
    {
        FileOpCanvas.SetActive(false);
        if (GameManager.molecules.Count == 0)
        {
            Debug.Log("Nothing to save");
            return;
        }
        KeyboardInput.SetActive(true);
    }
}
