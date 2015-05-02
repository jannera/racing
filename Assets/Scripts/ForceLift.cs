using UnityEngine;
using System.Collections;

public class ForceLift : MonoBehaviour
{

    public float metersPerSecond;

    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody)
        {
            other.attachedRigidbody.AddForce(Vector3.up * metersPerSecond, ForceMode.VelocityChange);
        }
    }
}
