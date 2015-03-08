using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JumpingSlider : MonoBehaviour {

    private Jumping jumping;
    private Slider slider;

	// Use this for initialization
	void Start () {
        GameObject car = GameObject.FindGameObjectWithTag("Player");
        jumping = car.GetComponent<Jumping>();
        slider = GetComponent<Slider>();
        if (!jumping.enabled)
        {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        slider.value = jumping.GetChargeStatus();
	}
}
