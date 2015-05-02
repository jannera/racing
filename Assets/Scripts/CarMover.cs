using UnityEngine;
using System.Collections;

public class CarMover : MonoBehaviour {

	void Start() {
		doMove(transform);
	} 

    public static void doMove (Transform transform)
	{
        GameObject car = GameObject.FindGameObjectWithTag("Player");
        if (car == null)
        {
            Debug.LogError("Can't find car");
            return;
        }

        car.transform.position = transform.position;
        car.transform.rotation = transform.rotation;
    }
}
