using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour {

    private WheelCollider[] wheels;

    private float chargingTimer = 0f;
    private float atMaxTimer = 0f;
    private float maxChargingTimer;
    public float timeToHoldAtMaxCharge = 0.2f;

    public float minSecsToBeConsideredOffGround = 0.5f; // after at least one wheel has been off the ground for at least this long, charging of jump is stopped and jumping is not possible

    private float g;

    private float wheelsOffGroundTimer = 0;
    private float[] normalSuspensionDistance;

    private enum GolfCharger { INCREASING, DECREASING, MAX };

    private GolfCharger risingStatus = GolfCharger.INCREASING;
    
    public AudioClip jumpClip;
    private AudioSource audioSource;

    [System.Serializable]
    public class JumpConfig
    {
        public float maxTime = 0;
        public float jumpHeight;
    }

    public JumpConfig[] jumpStages;

	void Start () {
        wheels = GetComponentsInChildren<WheelCollider>();
        g = Physics.gravity.magnitude;
        normalSuspensionDistance = new float[wheels.Length];
        for (int i = 0; i < wheels.Length; i++)
        {
            normalSuspensionDistance[i] = wheels[i].suspensionDistance;
        }
        maxChargingTimer = jumpStages[jumpStages.Length - 1].maxTime;
        
        GameObject jumpSource = GameObject.Find("JumpSource");
        audioSource = jumpSource.GetComponent<AudioSource>();
	}
	
	void Update () {
        if (WheelsOffGroundLongEnough() && GetChargeStatus() > 0)
        {
            ResetCharging();
            // todo: maybe earlier charging could remain the same, instead of resetting?
            return;
        }
        
        if (Input.GetButtonDown("Jump")) 
        {
            StartChargingForJump();
        }
        else if (Input.GetButtonUp("Jump") && !WheelsOffGroundLongEnough())
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
        if (!WheelsOnGround())
        {
            wheelsOffGroundTimer += Time.deltaTime;
        }
        else
        {
            wheelsOffGroundTimer = 0;
        }
    }

    private bool WheelsOnGround()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (!wheels[i].isGrounded)
            {
                return false;
            }
        }
        return true;
    }

    bool WheelsOffGroundLongEnough()
    {
        return wheelsOffGroundTimer > minSecsToBeConsideredOffGround;
    }

    void StartChargingForJump()
    {
        chargingTimer = 0;
        risingStatus = GolfCharger.INCREASING;
    }

    void ContinueCharging()
    {
        if (risingStatus == GolfCharger.INCREASING)
        {
            chargingTimer += Time.deltaTime;

            if (chargingTimer > maxChargingTimer)
            {
                chargingTimer = maxChargingTimer;
                atMaxTimer = 0;
                risingStatus = GolfCharger.MAX;
            }
        }
        else if (risingStatus == GolfCharger.DECREASING)
        {
            chargingTimer -= Time.deltaTime;

            if (chargingTimer < 0)
            {
                chargingTimer = 0;
                risingStatus = GolfCharger.INCREASING;
            }
        }
        else if (risingStatus == GolfCharger.MAX)
        {
            atMaxTimer += Time.deltaTime;

            if (atMaxTimer > timeToHoldAtMaxCharge)
            {
                risingStatus = GolfCharger.DECREASING;
            }
        }
        
        UpdateSuspensionDistances();
    }

    void ReleaseJump()
    {
        rigidbody.AddForce(transform.up * GetJumpStartingVelocity(), ForceMode.VelocityChange);
        ResetCharging();
        audioSource.PlayOneShot(jumpClip);
    }

    public float GetChargeStatus()
    {
        return chargingTimer/maxChargingTimer;
    }

    private float GetJumpStartingVelocity()
    {
        int i = 0;
        for (; i < jumpStages.Length; i++)
        {
            if (chargingTimer <= jumpStages[i].maxTime)
            {
                break;
            }
        }

        if (i == jumpStages.Length)
        {
            i--;
        }

        // Debug.Log(chargingTimer + " vs " + maxChargingTimer + " => " + jumpStages[i].jumpHeight);

        return Mathf.Sqrt(2f * jumpStages[i].jumpHeight * g);
    }

    void ResetCharging()
    {
        chargingTimer = 0;
        UpdateSuspensionDistances();
    }

    void UpdateSuspensionDistances()
    {
        float status = GetChargeStatus();
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].suspensionDistance = normalSuspensionDistance[i] - Mathf.Lerp(0, normalSuspensionDistance[i], status);
        }
    }
}
