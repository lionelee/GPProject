using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour {

    public List<Button> valenceButtons;
    public List<Button> componentOpButtons;
    public List<Button> bondTypeButtons;
    public GameObject valenceCanvas;
    public GameObject bondTypeCanvas;
    public GameObject AtomNumCanvas;

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

        GameManager.GenerateAtom(symbol, valence);

    }

    public void OnComplexPrefabClick(string type)
    {
        AtomNumCanvas.GetComponent<Text>().text = type;
        AtomNumCanvas.SetActive(true);
    }

    public void OnAtomNumClick(GameObject button)
    {
        int num = int.Parse(button.GetComponentInChildren<Text>().text);
        string type = AtomNumCanvas.GetComponent<Text>().text;

        GameManager.GenerateComplexPrefab(type, num);
    }

    public void OnSimplePrefabClick(string type)
    {
        AtomNumCanvas.SetActive(false);

        GameManager.GenerateSimplePrefab(type);
    }

    public void OnCanvasCloseButtonClick(GameObject canvas)
    {
        
        canvas.SetActive(false);
        string name = canvas.name;

        if(name == "SelectAtomCanvas")
        {
            valenceCanvas.SetActive(false);
        } else if(name == "ComponentOperationCanvas")
        {
            bondTypeCanvas.SetActive(false);
        } else if(name == "PrefabCanvas")
        {
            AtomNumCanvas.SetActive(false);
        }
    }

    public void OnDeleteComponentButtonClick()
    {
        GameObject selected = GameManager.GetSelectedComponent();

        //just delete
        GameManager.RemoveAtom(selected);
        Destroy(selected);
    }

    public void SetComponentOpButton(GameObject component)
    {
        if(component.GetComponent<Atom>() != null)
        {
            componentOpButtons[0].GetComponentInChildren<Text>().text = "Connect";
            componentOpButtons[1].GetComponentInChildren<Text>().text = "Detach";
        }
        else
        {
            componentOpButtons[0].GetComponentInChildren<Text>().text = "Change Type";
            componentOpButtons[1].GetComponentInChildren<Text>().text = "Break";
        }
    }

    public void OnComponentOpButtonClick(GameObject button)
    {
        string op = button.GetComponentInChildren<Text>().text;

        if(op == "Connect")
        {
            GameManager.SetConnectable(true);
        } else if(op == "Detach")
        {
            GameManager.GetSelectedComponent().GetComponent<AtomsAction>().Detach();
        } else if(op == "Change Type")
        {
            bondTypeCanvas.SetActive(true);
        } else if(op == "Break"){
            GameManager.GetSelectedComponent().GetComponent<BondsAction>().Break();
        }
        else
        {
            return;
        }
    }


    public void OnBindTypeClick(string type)
    {
        Bond selectedBond = GameManager.GetSelectedComponent().GetComponent<Bond>();
        GameObject oldBond = GameManager.GetSelectedComponent();
        BondType bondType;
        if(type == "single")
        {
            bondType = BondType.SINGLE;
        } else if(type == "double")
        {
            bondType = BondType.DOUBLE;
        }
        else
        {
            bondType = BondType.TRIPLE;
        }

        GameManager.ChangeBondType(oldBond, bondType);
        print("233");
        OnCanvasCloseButtonClick(GetComponent<UiDisplayController>().ComponentOpCanvas);
    }

}
