using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public float rotationSpeed = 180.0f;
	float x = 0.0f;
	float y = 0.0f;
    Vector2 touchAxis;
	Rigidbody rb;
        

    public void SetTouchAxis(Vector2 data)
    {
        touchAxis = data;
    }
    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody>();
	}
		

    private void FixedUpdate()
    {
        Rotate();
    }

    void Rotate()
    {
		x = touchAxis.x * rotationSpeed * Time.deltaTime;
		y = touchAxis.y * rotationSpeed * Time.deltaTime;


		Quaternion rotation = Quaternion.Euler (y, x, 0);
		rb.MoveRotation(rb.rotation * rotation);
    }
}
