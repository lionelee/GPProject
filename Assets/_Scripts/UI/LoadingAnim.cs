using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnim : MonoBehaviour {

    private Coroutine AnimationPlay;

    public Sprite[] sprites;

    private Image img;
    private int i;

    // Use this for initialization
	void Start () {
        img =  GetComponent<UnityEngine.UI.Image>();
    }
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(LoadingAnimPlay(30));
    }
 
   IEnumerator LoadingAnimPlay(float fps)
    {
        while (true)
        {
            img.overrideSprite = sprites[0];
            yield return new WaitForSeconds(1/ fps);
        }
    }
}
