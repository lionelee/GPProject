using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LinearMover : MonoBehaviour {

    public float moveSpeed = 100.0f;
    Vector3 position;
    Vector2 touchAxis;

    public void SetTouchAxis(Vector2 data)
    {
        touchAxis = data;
    }
    // Use this for initialization
    void Start()
    {
        position = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 jointPos = gameObject.GetComponent<MoleculesAction>().GetJointPosition();
        Vector3 RightControllerPos = VRTK_DeviceFinder.GetControllerRightHand().transform.position;
        Vector3 moveDir = (jointPos - RightControllerPos).normalized;

        transform.Translate(moveDir * touchAxis.y * moveSpeed * Time.deltaTime, Space.World);
    }
}
