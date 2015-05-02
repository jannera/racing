using UnityEngine;
using System.Collections;

public class DeleteOnEnvLoad : MonoBehaviour {
    void Awake()
    {
        if (GameObject.FindObjectOfType<EnvironmentLoader>() != null || GameObject.FindObjectOfType<CarImporter>() != null)
        {
            Destroy(this.gameObject);
        }
    }
}
