using UnityEngine;
using System.Collections;

public class DeleteOnEnvLoad : MonoBehaviour {
    void Awake()
    {
        if (GameObject.FindObjectOfType<EnvironmentLoader>() != null)
        {
            Destroy(this.gameObject);
        }
    }
}
