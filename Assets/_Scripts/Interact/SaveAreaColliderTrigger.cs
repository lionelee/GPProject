using UnityEngine;

public class SaveAreaColliderTrigger : MonoBehaviour
{
    public GameObject HighlightMark;

    Material highlight;
    Material neon;

    // Use this for initialization
    void Start()
    {
        highlight = Resources.Load("_Materials/Highlight") as Material;
        neon = Resources.Load("_Materials/Neon") as Material;
    }

    private void OnTriggerEnter(Collider other)
    {
        HighlightMark.GetComponent<Renderer>().material = highlight;
    }

    private void OnTriggerExit(Collider other)
    {
        HighlightMark.GetComponent<Renderer>().material = neon;
    }
}
