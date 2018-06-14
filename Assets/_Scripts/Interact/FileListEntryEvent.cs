using UnityEngine;
using UnityEngine.UI;

public class FileListEntryEvent : MonoBehaviour {

    public void OnClick(Text text)
    {
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<FileOperationEvents>().FileOpCanvas.SetActive(false);
        FileOperator.ReadModel(text.text);
    }

}
