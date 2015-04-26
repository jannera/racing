using UnityEngine;
using System.Collections;

public class CarSteering : MonoBehaviour {
    public WheelCollider[] steeringWheels;
    public WheelCollider[] torqueWheels;
    public WheelCollider[] brakingWheels;

    public float steerMinSpeed = 20;
    public float steerMaxSpeed = 1;
    public float motorMax = 50;
    public float brakeMax = 100;
    public float maxVelocity = 100; // km/h
    public float reverseMax = 40;
    public float dragMax;

    public float steerFactor;

    public float boosterTorqueModifier = 3f;
    public float boosterMaxVelocityModifier = 2f;
    public float boosterSteerModifier = 0.2f;

    public AudioSource engineSource;

    void Start() {        
        engineSource = GetComponent<AudioSource>();
    }

    void FixedUpdate() {
        steer();

        float verticalDir = Input.GetAxis("Vertical");
        if (verticalDir > 0) {
            accelerate();
            drag(0);
        } else if(verticalDir < 0) {
            decelerate();
            drag(0);
        }
        else
        {
            drag(1f);
        }
        
        brake(1f);
    }


    private void steer() {
        float steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
        float currentVel = GetCurrentVelocityKmPerH();
        // Debug.Log(currentVel);
        // Steering depends on speed of the car
        float speedFactor = currentVel / maxVelocity;
        steerFactor = Mathf.Lerp(steerMinSpeed, steerMaxSpeed, speedFactor);
        engineSound();

        for (int i = 0; i < steeringWheels.Length; i++) {
            steeringWheels[i].steerAngle = steer * steerFactor;
        }
    }

    private void engineSound() {
        float motor = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
        float velocity = GetCurrentFractionOfMaxVelocity();
        float pitch;
        if (atLeastOnetorqueWheelOnGround()) {
            pitch = velocity;                 
        } else {
            pitch = motor;
        }

       engineSource.pitch = pitch + 0.5f;       
    }

    private bool atLeastOnetorqueWheelOnGround() {
        for (int i = 0; i < torqueWheels.Length; i++) {
            if (torqueWheels[i].isGrounded) {
                return true;
            }
        }
        return false;
    }

    private void accelerate() {
        float motor = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
        float maxVelocityModifier = 1f;
        float torqueModifier = 1f;
        float currentVel = GetCurrentVelocityKmPerH();

        if (Input.GetButton("Boost")) {
            maxVelocityModifier = boosterMaxVelocityModifier;
            torqueModifier = boosterTorqueModifier;
            steerFactor *= boosterSteerModifier;
        }

        for (int i = 0; i < torqueWheels.Length; i++) {
            if (currentVel < (maxVelocity * maxVelocityModifier)) {
                torqueWheels[i].motorTorque = motor * motorMax * torqueModifier * torqueWheels[i].mass;
            } else {
                torqueWheels[i].motorTorque = 0;
            }

        }
    }

    private void decelerate() {
        float currentVel = GetCurrentVelocityKmPerH();
        float brakeValue = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0);

        for (int i = 0; i < torqueWheels.Length; i++) {
            if (currentVel < reverseMax || headingForward()) {
                torqueWheels[i].motorTorque = brakeValue * -reverseMax * torqueWheels[i].mass;
            } else {                
                torqueWheels[i].motorTorque = 0;
            }
        }

    }

    private void brake(float brakeValue) {
        for (int i = 0; i < brakingWheels.Length; i++) {
            if (Input.GetButton("Break")) {
                brakingWheels[i].brakeTorque = brakeValue * brakeMax * rigidbody.mass;
            } else {
                brakingWheels[i].brakeTorque = 0;
            }
        }
    }

    private void drag(float relative)
    {
        for (int i = 0; i < torqueWheels.Length; i++)
        {
            torqueWheels[i].brakeTorque = relative * dragMax * rigidbody.mass;
        }
    }

    private float MetersPerSecondToKmPerH(float v) {
        return v * 60 * 60 / 1000f;
    }

    private bool headingForward()
    {
        return transform.InverseTransformVector(rigidbody.velocity).z >= 0;
    }

    public float GetCurrentVelocityKmPerH() {
        return MetersPerSecondToKmPerH(rigidbody.velocity.magnitude);
    }

    public float GetCurrentFractionOfMaxVelocity() {
        return GetCurrentVelocityKmPerH() / maxVelocity;

    }    
}
