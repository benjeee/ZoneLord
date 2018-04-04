using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : NetworkBehaviour {

    public enum PlayerState
    {
        Still,
        Walking,
        Running,
        Jumping,
        Combat
    }

    [SyncVar]
    public PlayerState _state = PlayerState.Still;

    [SerializeField]
    private float runSpeed = 12f;
    [SerializeField]
    private float walkSpeed = 4f;
    [SerializeField]
    private float sens = 2.5f;

    private PlayerMotor motor;
    private PlayerState finalStateThisFrame;

    public bool canChangeState;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _state = PlayerState.Still;
        canChangeState = true;
    }

    void Start()
    {
        Debug.Log("Started Controller");
        Cursor.visible = false;

        motor = GetComponent<PlayerMotor>();
    }

    public void DisableStateChanging()
    {
        CancelInvoke();
        canChangeState = false;
    }

    public void DisableStateChanging(float seconds)
    {
        CancelInvoke();
        canChangeState = false;
        Invoke("EnableStateChanging", seconds);
    }

    public void EnableStateChanging()
    {
        canChangeState = true;
    }

    void HandleMovement()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        Vector3 velocity = (movHorizontal + movVertical).normalized;

        if (velocity != Vector3.zero)
        {
            if (Input.GetButton("Fire3"))
            {
                if(!motor.inJump) finalStateThisFrame = PlayerState.Walking;
                velocity *= walkSpeed;
            }else
            {
                if (!motor.inJump) finalStateThisFrame = PlayerState.Running;
                velocity *= runSpeed;
            }
        }
        else
        {
            finalStateThisFrame = PlayerState.Still;       
        }
        motor.Move(velocity);   

        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 playerRotation = new Vector3(0f, yRot, 0f) * sens;
        motor.Rotate(playerRotation);

        float camRotation = xRot * sens;
        motor.CamRotate(camRotation);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            motor.Jump();
        }
    }


	void Update()
	{
        if (isLocalPlayer)
        {
            HandleMovement();
            HandleJump();
            if (motor.inJump) finalStateThisFrame = PlayerState.Jumping;
            if (_state != finalStateThisFrame && canChangeState)
            {
                CmdUpdateState(finalStateThisFrame);
            }
        }
	}

    [Command]
    public void CmdUpdateState(PlayerState newState)
    {
        _state = newState;
    }
}
