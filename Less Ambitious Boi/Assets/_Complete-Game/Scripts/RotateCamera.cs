using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.forward, 90);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.forward, -90);
        }
    }
}
