using UnityEngine;
using UnityEngine.UI;

public class VehicleUI : MonoBehaviour {

	public Text speedText;
	public Rigidbody target;

	public void Start ()
	{
		if (speedText == null) {
			speedText = GetComponent<Text> ();
		}
		if (target == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");
			target = go.GetComponent<Rigidbody> ();
		}
	}

	public void Update ()
	{
		float speed = target.velocity.magnitude * 3600 / 1000f;
		speedText.text = string.Format ("Speed: {0,3:d}", (int)speed);
	}
}
