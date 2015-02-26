using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour {

    private WheelCollider[] wheels;

    private float chargingTimer = 0f;
    public float maxChargingTimer = 1.5f;

    public float fullJumpHeight = 3f;

    public float minSecsToBeConsideredOffGround = 0.5f; // after at least one wheel has been off the ground for at least this long, charging of jump is stopped and jumping is not possible

    private float g;

    private float wheelsOffGroundTimer = 0;

	void Start () {
        wheels = GetComponentsInChildren<WheelCollider>();
        g = Physics.gravity.magnitude;
	}
	
	void Update () {
        if (WheelsOffGroundLongEnough())
        {
            ResetCharging();
            // todo: maybe earlier charging could remain the same, instead of resetting?
            return;
        }
        
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

    void FixedUpdate()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (!wheels[i].isGrounded)
            {
                wheelsOffGroundTimer += Time.deltaTime;
                return;
            }
        }

        wheelsOffGroundTimer = 0;
    }

    bool WheelsOffGroundLongEnough()
    {
        return wheelsOffGroundTimer > minSecsToBeConsideredOffGround;
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
        ResetCharging();
    }

    void ResetCharging()
    {
        chargingTimer = 0;
    }
}
