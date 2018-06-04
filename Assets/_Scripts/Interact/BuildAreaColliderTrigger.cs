using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildAreaColliderTrigger : MonoBehaviour {

    public GameObject HighlightMark;

    Material highlight;
    Material neon;

    // Use this for initialization
    void Start () {
        highlight = Resources.Load("_Materials/Highlight") as Material;
        neon = Resources.Load("_Materials/Neon") as Material;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Component")
        {
            HighlightMark.GetComponent<Renderer>().material = highlight;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Component")
        {
            HighlightMark.GetComponent<Renderer>().material = neon;
        }
    }
}
