using VRTK;
using UnityEngine;

public class SaveAreaColliderTrigger : VRTK_InteractableObject
{
    public GameObject HighlightMark;
    public GameObject FileOpCanvas;

    Material highlight;
    Material neon;

    // Use this for initialization
    void Start()
    {
        highlight = Resources.Load("_Materials/Highlight") as Material;
        neon = Resources.Load("_Materials/Neon") as Material;
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        HighlightMark.GetComponent<Renderer>().material = highlight;
    }

    private void OnTriggerExit(Collider other)
    {
        HighlightMark.GetComponent<Renderer>().material = neon;
    }

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        FileOpCanvas.SetActive(true);
    }
}
