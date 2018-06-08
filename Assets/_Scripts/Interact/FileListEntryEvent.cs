using UnityEngine;
using UnityEngine.UI;

public class FileListEntryEvent : MonoBehaviour {

    public void OnClick(Text text)
    {
        FileOperationEvents foe = GameObject.Find("EventManager").GetComponent<FileOperationEvents>();
        if(foe != null)
        {
            foe.FileOpCanvas.SetActive(false);
        }
        FileOperator.ReadModel(text.text);
    }

}
