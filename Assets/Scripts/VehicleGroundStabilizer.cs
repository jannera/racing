using UnityEngine;
using System.Collections;

public class VehicleGroundStabilizer : MonoBehaviour {
    public float magnetAcceleration = 10f;
    public float maxMagnetDst = 2f;

    public BoxCollider bottomRectangle;

    public int detectorsVertically = 5;
    public int detectorsHorizontally = 7;

    private class GroundDetector
    {
        public RaycastHit raycastHit = new RaycastHit();
        public Vector3 rayStartPos = new Vector3(); // in world coordinates
    }

    private GroundDetector[,] detectors; // [ver][hor]

    private void CheckInit()
    {
        if (detectors == null || detectors.GetLength(1) != detectorsHorizontally || detectors.GetLength(0) != detectorsVertically)
        {
            Debug.Log("redoing detector grid");

            detectors = new GroundDetector[detectorsVertically, detectorsHorizontally];
            for (int i = 0; i < detectorsVertically; i++)
            {
                for (int j = 0; j < detectorsHorizontally; j++)
                {
                    detectors[i, j] = new GroundDetector();
                }
            }
        }
    }
    // apply magnet only if every wheel is touching or close enough
    void FixedUpdate()
    {
        CheckInit();

        if (!RebuildGroundPlane())
        {
            return;
        }

        
    }

    bool RebuildGroundPlane()
    {
        return true;
    }

    

    void Update()
    {
        Vector3 pos = new Vector3();
        Bounds b = bottomRectangle.bounds;
        Vector3 min = b.min;
        Vector3 max = b.max;
        return;

        float stepHorizontal = b.size.x / (float)(detectorsHorizontally - 1);
        float stepVertical = b.size.z / (float)(detectorsHorizontally - 1);

        for (int i = 0; i < detectorsVertically; i++)
        {
            for (int j = 0; j < detectorsHorizontally; j++)
            {
                pos.Set(min.x + j * stepHorizontal, min.y, min.z + i * stepVertical);
                Debug.DrawLine(pos, pos + Vector3.down);
            }
        }
    }
}
