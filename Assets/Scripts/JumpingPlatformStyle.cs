using UnityEngine;
using System.Collections;

public class JumpingPlatformStyle : MonoBehaviour {

    private WheelCollider[] wheels;

    private float chargingTimer = 0f;
    private int currentStage = 0;

    private float g;

    Vector3 jumpDirection;

    [System.Serializable]
    public class JumpConfig
    {
        public float duration = 0.25f;
        public float jumpHeight = 3f;
    }

    public JumpConfig[] jumpStages;

	void Start () {
        wheels = GetComponentsInChildren<WheelCollider>();
        g = Physics.gravity.magnitude;
	}
	
	void FixedUpdate () {
        if (Input.GetButtonDown("Jump") && WheelsOnGround()) 
        {
            chargingTimer += Time.deltaTime;
            jumpDirection = transform.up;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            chargingTimer = 0;
            currentStage = 0;
        }
        else if (Input.GetButton("Jump") && chargingTimer > 0)
        {
            chargingTimer += Time.deltaTime;
        }
        else
        {
            chargingTimer = 0;
            currentStage = 0;
        }


        if (chargingTimer >= GetFullDuration())
        {
            // time to switch to next stage, if it is possible and player is still charging
            currentStage++;
            Debug.Log("increasing stage");
            if (currentStage >= jumpStages.Length)
            {
                Debug.Log("last stage reached");
                currentStage = 0;
                chargingTimer = 0;
                return;
            }
        }
        
        
        if (chargingTimer > 0)
        {
            // burn time
            float acceleration = GetAcceleration() ;
            // Debug.Log("Burning for " + acceleration + " vs " + g * Time.deltaTime);
            rigidbody.AddForce(jumpDirection * acceleration, ForceMode.Acceleration);
        }
        
	}

    private float GetFullDuration()
    {
        float total = 0;
        for (int i = 0; i <= currentStage; i++)
        {
            total += jumpStages[currentStage].duration;
        }
        return total;
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

    private float GetAcceleration()
    {
        float t = jumpStages[currentStage].duration;
        float s = jumpStages[currentStage].jumpHeight;

        float a = 1 / g;
        float b = 1;
        float c = -2f * s / (t * t);

        float y = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        // Debug.Log(y + g);
        return y + g;
    }
}
