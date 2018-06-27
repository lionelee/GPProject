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
        testTextContent.Add(0, "请组装出乙烯醇的分子结构, 组装完成后选中分子");
        testTextContent.Add(1, "请组装出环己胺的分子结构, 组装完成后选中分子");
        testTextContent.Add(2, "请组装出间甲基苯甲醚的分子结构, 组装完成后选中分子");
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
        if (true)
        {
            if(testLevel == 2)
            {
                testText.GetComponent<Text>().text = "通过组装测试！";
                confirmButton.SetActive(false);

            }
            testLevel++;
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
        logoQuad.SetActive(true);

        testBackImage.SetActive(false);
        testText.SetActive(false);
        confirmButton.SetActive(false);
        exitButton.SetActive(false);
    }
}
