using UnityEngine;
using System.Collections;

public class EnvironmentLoader : MonoBehaviour {
    public int sceneIndex = 0;

	void Start () {
        Application.LoadLevelAdditive(sceneIndex);
	}
}
