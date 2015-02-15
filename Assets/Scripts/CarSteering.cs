﻿using UnityEngine;
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

    public float steerFactor;
	
	void Start () {

	}
	
	
	void FixedUpdate () {
        float steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
        float motor = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
        float brake = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0);

        // Steering depends on speed of the car
        float speedFactor = rigidbody.velocity.magnitude / motorMax;
        steerFactor = Mathf.Lerp(steerMinSpeed, steerMaxSpeed, speedFactor);

        for (int i = 0; i < steeringWheels.Length; i++)
        {
            steeringWheels[i].steerAngle = steer * steerFactor;
        }

        float currentVel = rigidbody.velocity.magnitude;
        currentVel = MetersPerSecondToKmPerH(currentVel);
        // Debug.Log(currentVel);

        
        
        for (int i = 0; i < torqueWheels.Length; i++)
        {
            if (currentVel < maxVelocity)
            {
                torqueWheels[i].motorTorque = motor * motorMax * torqueWheels[i].mass;
            }
            else
            {
                torqueWheels[i].motorTorque = 0;
            }
                
        }
        

        for (int i = 0; i < brakingWheels.Length; i++)
        {
            brakingWheels[i].brakeTorque = brake * brakeMax * rigidbody.mass;
        }
	}

    private float MetersPerSecondToKmPerH(float v)
    {
        Debug.Log(v + " -> " + v * 60 * 60 / 1000f);
        return v * 60 * 60 / 1000f;
    }
}
