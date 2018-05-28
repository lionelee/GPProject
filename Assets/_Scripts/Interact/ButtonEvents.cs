using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour {

    public List<Button> valenceButtons;
    public GameObject valenceCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnAtomButtonClick(string symbol)
    {
        valenceCanvas.GetComponent<Text>().text = symbol;
        valenceCanvas.SetActive(true);
        List<int> valences = Config.ValenceTable[symbol];
        int size = valences.Count;

        foreach(Button b in valenceButtons)
        {
            b.gameObject.SetActive(false);
        }

        for(int i = 0;i < size; i++)
        {
            valenceButtons[i].GetComponentInChildren<Text>().text = valences[i].ToString();
            valenceButtons[i].gameObject.SetActive(true);
        }
    }

    public void OnValenceButtonClik(GameObject button)
    {
       
        int valence = int.Parse(button.GetComponentInChildren<Text>().text);
        string symbol = valenceCanvas.GetComponent<Text>().text;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GenerateAtom(symbol, valence);

    }

    public void OnCanvasCloseButtonClick(GameObject canvas)
    {
        valenceCanvas.SetActive(false);
        canvas.SetActive(false);
    }
}
