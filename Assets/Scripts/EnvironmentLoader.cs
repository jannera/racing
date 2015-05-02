using UnityEngine;
using System.Collections;

public class EnvironmentLoader : MonoBehaviour {
    public int sceneIndex = 0;

	void Start ()
	{
		if (GameObject.FindObjectOfType<CarImporter> () == null) {
			Application.LoadLevelAdditive (sceneIndex);
		} else {
			Destroy(this.gameObject);
		}
	}
}
