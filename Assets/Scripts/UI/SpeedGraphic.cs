using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeedGraphic : MonoBehaviour {

	public Rigidbody target;
	public float normalMaxSpeed = 120f;
	public float turboMaxSpeed = 210f;
	public float normalFillAmount = 0.73f;
	private Image graphic;

	public void Start ()
	{
		if (target == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");
			target = go.GetComponent<Rigidbody> ();
		}
		graphic = GetComponent< Image > ();
	}

	public void Update ()
	{
		float speed = target.velocity.magnitude * 3600 / 1000f;
		if (speed < normalMaxSpeed) {
			graphic.fillAmount = speed / normalMaxSpeed * normalFillAmount;
		} else {
			float amount = normalFillAmount + (speed - normalMaxSpeed) / (turboMaxSpeed - normalMaxSpeed) * (1 - normalFillAmount);
			graphic.fillAmount = Mathf.Min (amount, 1.0f);
		}
	}
}
