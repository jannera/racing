using UnityEngine;
using System.Collections;

public class WheelSync : MonoBehaviour {
    public WheelCollider wheelCollider;

    private Vector3 tmp;
    private WheelHit wheelHit = new WheelHit();

	void Update () {
        // spin the wheel to create illusion of moving
        transform.Rotate(wheelCollider.rpm * 6 * Time.deltaTime, 0, 0);

        // turn the wheel to match steering
        tmp = transform.localEulerAngles;
        tmp.y = wheelCollider.steerAngle - transform.localEulerAngles.z;
        transform.localEulerAngles = tmp;

        // move the wheel to match the suspension
        if (wheelCollider.GetGroundHit(out wheelHit))
        {            
            transform.position = wheelHit.point + wheelCollider.transform.up * wheelCollider.radius;
        }
        else
        {
            // the wheel is in air
            transform.position = wheelCollider.transform.position - wheelCollider.transform.up * wheelCollider.suspensionDistance;
        }
	}
}
