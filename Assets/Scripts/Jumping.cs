using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour {

    private WheelCollider[] wheels;

    private float chargingTimer = 0f;
    public float maxChargingTimer = 1.5f;

    public float fullJumpHeight = 3f;

    private float g;

	void Start () {
        wheels = GetComponentsInChildren<WheelCollider>();
        g = Physics.gravity.magnitude;
	}
	
	void Update () {
        // todo: only start charging if enough wheels are touching the ground
        // todo: if wheels haven't been touching the ground for long enough, stop charging
        if (Input.GetButtonDown("Jump")) 
        {
            StartChargingForJump();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            ReleaseJump();
        }
        else if (Input.GetButton("Jump"))
        {
            ContinueCharging();
        }       
	}

    void StartChargingForJump()
    {
        chargingTimer = 0;
    }

    void ContinueCharging()
    {
        chargingTimer += Time.deltaTime;
    }

    void ReleaseJump()
    {
        float relativeCharge = Mathf.Lerp(0, maxChargingTimer, chargingTimer);
        float fullStartVelocity = Mathf.Sqrt(2f * fullJumpHeight * g);
        rigidbody.AddForce(transform.up * relativeCharge * fullStartVelocity, ForceMode.VelocityChange);
        chargingTimer = 0;
    }
}
