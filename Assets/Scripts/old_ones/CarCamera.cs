using UnityEngine;
using System.Collections;

[System.Serializable]
public class MinMax
{
	public MinMax()
	{
		min = 0;
		max = 1;
	}
	
	public MinMax(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
	
	public float min;
	public float max;
}	

public class CarCamera : MonoBehaviour
{
	public Transform target = null;
	
	/// <summary>
	/// Camera height adjustment. This sets the "LookAt" position higher (positive) or lower (negative).
	/// </summary>
	public float height = 2.2f;

	/// <summary>
	/// Aggressivity of camera turning. Lower value slows the turn.
	/// </summary>
	public float positionDamping = 2f;

	/// <summary>
	/// Layers to ignore in RayCast.
	/// </summary>
	public LayerMask ignoreLayers = -1;
	
	public MinMax fieldOfView = new MinMax(55, 72);
	public MinMax viewDistance = new MinMax(8.5f, 12.5f);
	
	/// <summary>
	/// Maximum speed used for Lerps in FOV and VD calculations.
	/// </summary>
	public float maxSpeedFactor = 70f;
	
	/// <summary>
	/// Angle in which the camera views to target. Positive value points camera downwards.
	/// </summary>
	public int cameraAngleAdjust = 20;

	/// <summary>
	/// Velocity that the target must have before its velocity vector is used in calculation.
	/// </summary>
	public float directionChangeThreshold = 0.2f;

	public int peekAngle = 60;

	public float peekSpeed = 11f;

	/// <summary>
	/// The difference of normal angle and current angle under which the peeking speed usage is ended after peeking.
	/// </summary>
	public int peekEndAngle = 10;

	private RaycastHit hit = new RaycastHit();

	/// <summary>
	/// Layers that hit RayCast. Inversion of ignoreLayers.
	/// </summary>
	private LayerMask raycastLayers = -1;

	private Vector3 currentVelocity = Vector3.zero;
	private Vector3 lastUsedVelocity = Vector3.forward;
	private bool wasPeeking = false;

	void Start()
	{
		raycastLayers = ~ignoreLayers;
	}

	void FixedUpdate()
	{
		currentVelocity = target.root.rigidbody.velocity;
		currentVelocity.y = 0;
	}

	void LateUpdate()
	{
		float speedFactor = Mathf.Clamp01(target.root.rigidbody.velocity.magnitude / maxSpeedFactor);
		camera.fieldOfView = Mathf.Lerp(fieldOfView.min, fieldOfView.max, speedFactor);
		float currentDistance = Mathf.Lerp(viewDistance.min, viewDistance.max, speedFactor);

		// Whether to use current velocity or last known velocity (to avoid looking at zero vector).
		Quaternion velocityRotation;
		if (currentVelocity.magnitude > directionChangeThreshold) {
			velocityRotation = Quaternion.LookRotation(currentVelocity);
			lastUsedVelocity = currentVelocity;
		} else {
			velocityRotation = Quaternion.LookRotation(lastUsedVelocity);
		}

		/* Top view of targetAngle calculation:
		 *
		 *  ----> velocityRotation (= target camera direction)
		 * |\
		 * | v targetAngle (= result angle)
		 * |
		 * v transform (= current camera direction)
		 */

		float velocityAngle = velocityRotation.eulerAngles.y;
		bool peekLeft = Input.GetButton("PeekLeft");
		bool peekRight = Input.GetButton("PeekRight");
		bool anyPeek = peekLeft || peekRight;
		if (peekLeft && peekRight) {
			velocityAngle += 180;
		} else if (anyPeek) {
			velocityAngle += peekAngle * (peekRight ? 1 : -1);
		}

		// Speed to use to rotate the camera.
		float rotationSpeed = positionDamping;
		if (anyPeek || wasPeeking) {
			rotationSpeed = peekSpeed;
			wasPeeking = true;
		}

		// Target direction.
		float targetAngle = Mathf.LerpAngle(
			transform.eulerAngles.y,
			velocityAngle,
			rotationSpeed * Time.deltaTime
		);

		if (!anyPeek && wasPeeking && Mathf.Abs(Mathf.DeltaAngle(targetAngle, velocityAngle)) < peekEndAngle) {
			wasPeeking = false;
		}

		/* Side picture:
		 *
		 *  < distancedCamera(world) = -cameraVector(local)
		 *   \__
		 *      \(hit.point(world))
		 *       \__
		 *          \
		 *           ^viewTargetPosition(world)
		 *   (height)|
		 *           |
		 *        target ------> velocity
		 */
		Quaternion targetRotation;
		Vector3 cameraVector;
		Vector3 viewTargetPosition;
		Vector3 distancedCamera;

		targetRotation = Quaternion.Euler(cameraAngleAdjust, targetAngle, 0);
		cameraVector = (targetRotation * Vector3.forward * currentDistance);

		viewTargetPosition = target.position + Vector3.up * height;
		distancedCamera = viewTargetPosition - cameraVector;

		// Find out whether view is occupied.
		float cameraSourceHeight = currentDistance;
		if (Physics.Raycast(viewTargetPosition, -cameraVector, out hit, cameraSourceHeight, raycastLayers)) {
			float resultDistance = (hit.point - viewTargetPosition).magnitude;
			distancedCamera = viewTargetPosition - cameraVector.normalized * resultDistance;
		}

		transform.position = distancedCamera;
		transform.LookAt(viewTargetPosition);
	}
}
