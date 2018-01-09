using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {
	
	[SerializeField]
	private float speed = 12f;
	[SerializeField]
	private float sens = 2.5f;

	private PlayerMotor motor;

	void Start()
	{
		Debug.Log ("Started Controller");
		Cursor.visible = false;
		motor = GetComponent<PlayerMotor> ();
	}

	void Update()
	{
		
		//calc movement velocity as a 3d vector
		float xMov = Input.GetAxisRaw("Horizontal");
		float zMov = Input.GetAxisRaw ("Vertical");

		Vector3 movHorizontal = transform.right * xMov;
		Vector3 movVertical = transform.forward * zMov;

		//final movement vector
		Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

		//Apply movement
		motor.Move(velocity);

		//calculate rotation
		float yRot = Input.GetAxisRaw("Mouse X");
		float xRot = Input.GetAxisRaw("Mouse Y");

		Vector3 rotation = new Vector3 (-xRot, yRot, 0f) * sens;

		//apply rotation
		motor.Rotate(rotation);

		//calculate cam rotation

		//Vector3 camRotation = new Vector3 (xRot, 0f, 0f) * sens;

		//apply cam rotation
		//motor.CamRotate(-camRotation);
	}
}
