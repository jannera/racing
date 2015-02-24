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
    public float reverseMax = 40;

    public float steerFactor;

    public float boosterTorqueModifier = 3f;
    public float boosterMaxVelocityModifier = 2f;
    public float boosterSteerModifier = 0.2f;

    void FixedUpdate() {
        steer();

        float verticalDir = Input.GetAxis("Vertical");
        if (verticalDir > 0) {
            accelerate();
        } else if(verticalDir < 0) {
            decelerate();
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

        for (int i = 0; i < steeringWheels.Length; i++) {
            steeringWheels[i].steerAngle = steer * steerFactor;
        }
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
            if (currentVel > -reverseMax) {                
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

    private float MetersPerSecondToKmPerH(float v) {
        return v * 60 * 60 / 1000f;
    }

    public float GetCurrentVelocityKmPerH() {
        return MetersPerSecondToKmPerH(rigidbody.velocity.magnitude);
    }

    public float GetCurrentFractionOfMaxVelocity() {
        return GetCurrentVelocityKmPerH() / maxVelocity;

    }

}
