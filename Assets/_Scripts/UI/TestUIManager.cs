using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIManager : MonoBehaviour {

    public GameObject enterTestButton;
    public GameObject testText;
    public GameObject testBackImage;
    public GameObject confirmButton;
    public GameObject exitButton;
    public GameObject continueButton;

    public GameObject logoQuad;


    int testLevel = 0;

    Dictionary<int, string> testTextContent;

    
	// Use this for initialization
	void Start () {
        confirmButton.GetComponent<Button>().interactable = false;
        testTextContent = new Dictionary<int, string>();
        testTextContent.Add(0, "1. 请组装出乙烯醇的分子结构, 组装完成后选中分子");
        testTextContent.Add(1, "2. 请组装出环己胺的分子结构, 组装完成后选中分子");
        testTextContent.Add(2, "3. 请组装分子式为 C7H6O，且包含苯环的分子");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnEnterTestButtonClick()
    {
        logoQuad.SetActive(false);
        enterTestButton.SetActive(false);
        testBackImage.SetActive(true);
        testText.SetActive(true);
        testText.GetComponent<Text>().text = testTextContent[testLevel];
        confirmButton.SetActive(true);
        
        exitButton.SetActive(true);
        
    }

    public void OnConfirmButtonClick()
    {
        //exam
        string type = "";
        if (GameManager.MoleculeMatch(testLevel, ref type))
        {
            if(testLevel == 2)
            {
                confirmButton.SetActive(false);
                exitButton.SetActive(true);
                continueButton.SetActive(true);
                testText.GetComponent<Text>().text = "组装的结构为: " + type + "通过组装测试！";
            }
            else
            {
                confirmButton.SetActive(false);
                exitButton.SetActive(false);
                continueButton.SetActive(true);
                testText.GetComponent<Text>().text = "通过组装测试！";
                testLevel++;
            }
        }
        else
        {
            testText.GetComponent<Text>().text = "结构不符合要求，请继续组装";
            confirmButton.SetActive(false);
            exitButton.SetActive(false);
            continueButton.SetActive(true);
        }
    }

    public void OnContinueButtonClick()
    {
        enterTestButton.SetActive(false);
        continueButton.SetActive(false);

        testBackImage.SetActive(true);
        testText.SetActive(true);
        testText.GetComponent<Text>().text = testTextContent[testLevel];
        confirmButton.SetActive(true);

        exitButton.SetActive(true);
    }

    public void OnExitButtonClick()
    {
        testLevel = 0;
        enterTestButton.SetActive(true);
        enterTestButton.GetComponent<Button>().interactable = true;
        logoQuad.SetActive(true);

        testBackImage.SetActive(false);
        testText.SetActive(false);
        confirmButton.SetActive(false);
        exitButton.SetActive(false);
    }
}
