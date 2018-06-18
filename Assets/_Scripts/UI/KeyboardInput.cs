using UnityEngine;
using UnityEngine.UI;
using System;

public class KeyboardInput : MonoBehaviour
{
    private InputField input;

    private void Start()
    {
        input = GetComponentInChildren<InputField>();
    }

    public void ClickKey(string character)
    {
        input.text += character;
    }

    public void Backspace()
    {
        if (input.text.Length > 0)
        {
            input.text = input.text.Substring(0, input.text.Length - 1);
        }
    }

    public void Enter()
    {
        string path = (input.text != "")? input.text : "Molecule" + DateTime.Now.ToString("yyMMddHHmmssff");
        gameObject.SetActive(false);
        FileOperator.SaveModel(path);
    }
}