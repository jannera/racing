using UnityEngine;
using System.Collections;

// todo: bah. try creating one first with three sensors, see if it works.. then try doing the more general version
public class VehicleGroundStabilizer3 : MonoBehaviour {
    public float maxGroundDst = 2f;
    public float minSensorCoverage = 0.7f;

    public Transform sensorA, sensorB, sensorC;

    private RaycastHit raycastInfo;

    public LayerMask ground;

    // these two are response forces that balance the angular velocities
    public AnimationCurve angularVelocityDampenerLengthwise;
    public AnimationCurve angularVelocityDampenerBreadthwise;

    public AnimationCurve responseToSteepness;

    public AnimationCurve velocityModifier;

    private class GroundSensor {
        public Transform t;
        public Vector3 ground = new Vector3();
        public bool groundFound = false;

        public void Found(RaycastHit c)
        {
            groundFound = true;
            ground = c.point;
        }

        public void NotFound()
        {
            groundFound = false;
        }
    }

    private GroundSensor[] sensors;

	void Start () {
        Transform[] transforms = { sensorA, sensorB, sensorC };
        sensors = new GroundSensor[transforms.Length];
        for (int i = 0; i < sensors.Length; i++)
        {
            sensors[i] = new GroundSensor();
            sensors[i].t = transforms[i];
        }
	}

	void FixedUpdate () {
        int hitting = 0;
	    for (int i=0; i < sensors.Length; i++) {
            Color c;
            Transform t = sensors[i].t;
            if (Physics.Raycast(t.position, -t.up, out raycastInfo, maxGroundDst, ground)) {
                sensors[i].Found(raycastInfo);
                hitting++;
                c = Color.blue;
                
            }
            else {
                sensors[i].NotFound();
                c = Color.white;
            }
            // Debug.DrawLine(t.position, t.position - t.up * maxGroundDst, c);
        }

        if (hitting < minSensorCoverage * sensors.Length)
        {
            Debug.Log("not enough coverage " + hitting + " vs " + minSensorCoverage * sensors.Length);
            return;
        }


        UpdateGroundPlane();

        UpdateVehiclePlane();

        Vector3 localVehicleNormal = transform.InverseTransformDirection(vehicleNormal);
        Vector3 localGroundNormal = transform.InverseTransformDirection(groundNormal);

        q.SetFromToRotation(localGroundNormal, localVehicleNormal);

        Vector3 angles = q.eulerAngles;

        while (angles.x >= 180) {
            angles.x -= 360;
        }
        while (angles.y >= 180)
        {
            angles.y -= 360;
        }
        while (angles.z >= 180)
        {
            angles.z -= 360;
        }

        // z is the horizontal angle. minus means left side wheels are off the ground, and should be brought closer
        // x is the vertical angle. minus means the front wheels are off the ground, and should be brought closer

        Vector3 currentVel = transform.InverseTransformDirection(rigidbody.angularVelocity);

        Vector3 correction = new Vector3();
        float xDir = 1, zDir = 1;
        if (angles.x < 0)
        {
            xDir = -1;
        }
        if (angles.z > 0)
        {
            zDir = -1;
        }
        correction.x = xDir * angularVelocityDampenerLengthwise.Evaluate(Mathf.Abs(angles.x)) * currentVel.x * Time.deltaTime;
        correction.z = zDir * angularVelocityDampenerBreadthwise.Evaluate(Mathf.Abs(angles.z)) * currentVel.z * Time.deltaTime;

        // todo: try to find the correct shape for the steepness curves
        // todo: separate curves to breadthwise / lengthwise curves
        // todo: check if the dampener curves actually matter at all, or do the steepness curve dominate
        correction.x += -1f * xDir * responseToSteepness.Evaluate(Mathf.Abs(angles.x)) * Time.deltaTime;
        correction.z += 3f * zDir * responseToSteepness.Evaluate(Mathf.Abs(angles.z)) * Time.deltaTime;

        correction *= velocityModifier.Evaluate(GetComponent<CarSteering>().GetCurrentFractionOfMaxVelocity());


        
        //Debug.Log(angles.z + ", " + currentVel.z + " -> " + correction.z);

        Debug.Log(angles.z + " -> " + zDir * responseToSteepness.Evaluate(Mathf.Abs(angles.z)));

        

        /*
        if (Mathf.Abs(angles.x) > minResponseAngle)
        {
            correction.x = -currentVel.x;
        }

        if (Mathf.Abs(angles.z) > minResponseAngle)
        {
            correction.z = -currentVel.z;
        }
         * */

        if (correction.x != 0 || correction.z != 0)
        {
            // Debug.Log("Angle " + angles + " too high, current velocity " + currentVel + " applying " + correction);
            rigidbody.AddRelativeTorque(correction, ForceMode.VelocityChange);
        }
	}

    Quaternion q = new Quaternion();

    void Update()
    {
        int hitting = 0;
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i].groundFound)
            {
                hitting++;
                // Debug.DrawLine(sensors[i].ground, sensors[i].ground + Vector3.up);
            }
        }

        if (hitting < minSensorCoverage * sensors.Length)
        {
            return;
        }

        Debug.DrawLine(transform.position, transform.position + vehicleNormal, Color.green);
        Debug.DrawLine(transform.position, transform.position + groundNormal, Color.blue);
    }

    Vector3 groundNormal = new Vector3(), vehicleNormal = new Vector3();

    private void UpdateGroundPlane()
    {
        p.Set3Points(sensors[0].ground, sensors[1].ground, sensors[2].ground);
        groundNormal = -p.normal;

        
    }

    private Plane p = new Plane(), groundPlane = new Plane();

    private void UpdateVehiclePlane()
    {
        int a = 0, b = 1, c = 2;
        p.Set3Points(sensors[a].t.position, sensors[b].t.position, sensors[c].t.position);
        vehicleNormal = -p.normal.normalized;
    }
}
