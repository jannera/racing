using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {
    public float rocketAcceleration;
    Vector3 force = new Vector3();

	void FixedUpdate () {
        float hor = - Mathf.Clamp(Input.GetAxis("RocketHorizontal"), -1, 1);
        float ver = - Mathf.Clamp(Input.GetAxis("RocketVertical"), -1, 1);
        force.Set(ver, 0, hor);
        force *= rocketAcceleration * Time.deltaTime;
        rigidbody.AddRelativeTorque(force, ForceMode.VelocityChange);
	}
}
