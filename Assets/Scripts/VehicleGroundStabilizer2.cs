using UnityEngine;
using System.Collections;

// todo: this does not work correctly, because fitting points to a plane does not work correctly atm. 
// this is in version control because VehicleGroundStabilizer will need this later on
public class VehicleGroundStabilizer2 : MonoBehaviour {
    public GameObject sensorContainer;
    public float maxGroundDst = 2f;
    public float minSensorCoverage = 0.7f;

    private RaycastHit raycastInfo;

    public LayerMask ground;

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
        Transform[] transforms = sensorContainer.GetComponentsInChildren<Transform>();
        sensors = new GroundSensor[transforms.Length - 1];
        for (int i = 0; i < sensors.Length; i++)
        {
            sensors[i] = new GroundSensor();
            sensors[i].t = transforms[i + 1];
        }
	}

	void FixedUpdate () {
        int hitting = 0;
	    for (int i=0; i < sensors.Length; i++) {
            Transform t = sensors[i].t;
            if (Physics.Raycast(t.position, -t.up, out raycastInfo, maxGroundDst, ground)) {
                sensors[i].Found(raycastInfo);
                hitting++;
            }
            else {
                sensors[i].NotFound();
            }
            // Debug.DrawLine(t.position, t.position - t.up * maxGroundDst, c);
        }

        if (hitting < minSensorCoverage * sensors.Length)
        {
            Debug.Log("not enough coverage");
            return;
        }


        UpdateGroundPlane();

        UpdateVehiclePlane();

        Vector3 localVehicleNormal = transform.InverseTransformDirection(vehicleNormal);
        Vector3 localGroundNormal = transform.InverseTransformDirection(groundNormal);

        q.SetFromToRotation(localGroundNormal, localVehicleNormal);

        Debug.Log(q.eulerAngles);
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
        int n = 0;
        float xx = 0, yy = 0;
        float xy = 0, xz = 0, yz = 0;
        float x = 0, y = 0, z = 0;
        for (int i = 0; i < sensors.Length; i++)
        {
            GroundSensor s = sensors[i];
            if (!s.groundFound)
            {
                continue;
            }
            n++;
            Vector3 pos = s.t.position;
            xx += pos.x * pos.x;
            yy += pos.y * pos.y;

            xy += pos.x * pos.y;
            xz += pos.x * pos.z;
            yz += pos.y * pos.z;

            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        
        Matrix3x3 m = new Matrix3x3(
            xx, xy, x,
            xy, yy, y,
             x, y, n);

        Vector3 b = new Vector3(xz, yz, z);
        Vector3 result = m.Inverse().MultiplyVector(b).normalized;

        groundNormal = result;
    }

    private Plane p = new Plane();

    private void UpdateVehiclePlane()
    {
        int a = 0, b = sensors.Length - 1, c = sensors.Length / 2;
        p.Set3Points(sensors[a].t.position, sensors[b].t.position, sensors[c].t.position);
        vehicleNormal = -p.normal.normalized;
    }
}
