using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {

	[SerializeField]
	private Camera cam;

    [SerializeField]
    private float maxCamRotation = 85f;

    [SerializeField]
    private float jumpSpeed = 5.0f;


    public Vector3 velocity = Vector3.zero;
	public Vector3 rotation = Vector3.zero;

	private float camRotation = 0;
    private float currCamRotation = 0;
    
    public bool inJump = false;
	private Rigidbody rb;

    Player player;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public bool Jump()
    {
        if (!inJump)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            return true;
        }
        return false;
    }

    public void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;
	}

	public void CamRotate(float _camRotation)
	{
		camRotation = _camRotation;
	}

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
            currCamRotation -= camRotation;
            currCamRotation = Mathf.Clamp(currCamRotation, -maxCamRotation, maxCamRotation);
            cam.transform.localEulerAngles = new Vector3(currCamRotation, 0f, 0f);
		}
	}

    void OnCollisionStay(Collision collisionInfo)
    {
        inJump = false;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        inJump = true;
    }
}
