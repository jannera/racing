using UnityEngine;

public class CarImporter : MonoBehaviour {

	private int isInitialized = 0;

	void Start ()
	{
		if (GameObject.FindObjectOfType<EnvironmentLoader> () == null) {
			Application.LoadLevelAdditive ("car");
		}
	}

	void Update ()
	{
		if (isInitialized < 2) {
			isInitialized ++;
			if (isInitialized == 2) {
				CarMover.doMove (transform);
			}
		}
	}
}

