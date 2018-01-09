using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private Vector3 camRotation = Vector3.zero;

	private Rigidbody rb;


	void Start()
	{
		rb = GetComponent<Rigidbody> ();
	}
		
	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
	}

	public void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;
	}

	public void CamRotate(Vector3 _camRotation)
	{
		camRotation = _camRotation;
	}

	//run every physics tick
	void FixedUpdate()
	{
		PerformMovement ();
		PerformRotation ();
	}

	private void PerformMovement()
	{
		if (velocity != Vector3.zero) 
		{
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
	}

	private void PerformRotation()
	{
		rb.MoveRotation (rb.rotation * Quaternion.Euler (rotation));
		if (cam != null) 
		{
			cam.transform.Rotate (camRotation);
		}
	}
}
