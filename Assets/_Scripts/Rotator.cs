using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {


	public float rotationSpeed = 100.0f;
	public float x = 0.0f;
	public float y = 0.0f;
    Vector2 touchAxis;


    public void SetTouchAxis(Vector2 data)
    {
        touchAxis = data;
    }
    // Use this for initialization
    void Start () {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}

	public void reset(){
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}

	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        Rotate();
    }

    void Rotate()
    {
		x += touchAxis.x * rotationSpeed * Time.deltaTime;
		y += touchAxis.y * rotationSpeed * Time.deltaTime;

		Quaternion rotation = Quaternion.Euler (y, x, 0);
		transform.rotation = rotation;
    }
}
