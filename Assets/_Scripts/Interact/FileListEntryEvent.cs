using UnityEngine;
using UnityEngine.UI;

public class FileListEntryEvent : MonoBehaviour {

    public void OnClick(Text text)
    {
        FileOperator.ReadModel(text.text);
    }
}
