using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {
    public float rocketAcceleration;
    private WheelCollider[] wheels;
    public AudioClip steerRocketSound;
    private AudioSource audioSource;

    void Start() {
        wheels = GetComponentsInChildren<WheelCollider>();
        
        GameObject rocketSource = GameObject.Find("RocketSource");
        audioSource = rocketSource.GetComponent<AudioSource>();
        audioSource.clip = steerRocketSound;
    }

	void FixedUpdate () {
        StabilizationRockets();
        RotationRockets();
        PlayRocketSound();
	}

    void StabilizationRockets() {
        float hor = -Mathf.Clamp(Input.GetAxis("RocketHorizontal"), -1, 1);
        float ver = -Mathf.Clamp(Input.GetAxis("RocketVertical"), -1, 1);
        Vector3 force = new Vector3();
        force.Set(ver, 0, hor);
        addRocketForce(force);
    }

    void RotationRockets() {
        if (inAir()) {
            float hor = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
            Vector3 force = new Vector3();
            force.Set(0, hor, 0);
            addRocketForce(force);
        }        
    }

    void addRocketForce(Vector3 force) {
        force *= rocketAcceleration * Time.deltaTime;
        rigidbody.AddRelativeTorque(force, ForceMode.VelocityChange);
    }

    bool inAir() {
        foreach (WheelCollider col in wheels) {
            if (col.isGrounded) {
                return false;
            }            
        }
        return true;
    }
    
    private void PlayRocketSound() {
        bool rocketOn = Input.GetButton("RocketHorizontal") || Input.GetButton("RocketVertical");
        if(rocketOn) {
            if(!audioSource.isPlaying) {
                audioSource.Play();   
            }                         
        } else {
            audioSource.Stop();
        }
    }
}
