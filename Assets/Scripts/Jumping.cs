using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour {

    private WheelCollider[] wheels;

    private float chargingTimer = 0f;
    public float maxChargingTimer = 1.5f;

    public float fullJumpHeight = 3f;

	// Use this for initialization
	void Start () {
        wheels = GetComponentsInChildren<WheelCollider>();
	}
	
	// Update is called once per frame
	void Update () {
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
        float force = Mathf.Lerp(0, maxChargingTimer, chargingTimer);

        float fullJumpTime = fullJumpHeight / Physics.gravity.magnitude;

        force *= (fullJumpHeight - 0.5f * Physics.gravity.magnitude * fullJumpTime * fullJumpTime) / fullJumpTime;
        Debug.Log(force);
        rigidbody.AddForce(transform.up * force, ForceMode.VelocityChange);
    }
}
