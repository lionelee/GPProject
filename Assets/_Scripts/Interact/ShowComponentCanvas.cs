using VRTK;
using UnityEngine;
using UnityEngine.UI;

public class ShowComponentCanvas : VRTK_InteractableObject
{
    
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    protected override void Update()
    {
		
	}

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        print("use");
        base.StartUsing(usingObject);
        GameObject.FindGameObjectWithTag("EventManager").GetComponent<UiDisplayController>().ShowSelectAtomCanvas(true);
    }
}
