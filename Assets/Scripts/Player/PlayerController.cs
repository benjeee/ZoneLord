using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerVisibility))]
public class PlayerController : NetworkBehaviour {

    public enum PlayerState
    {
        Still,
        Walking,
        Running,
        Jumping,
        Shooting
    }

    public static int STILL     = 1;
    public static int WALKING   = 2;
    public static int RUNNING   = 3;
    public static int JUMPING   = 4;
    public static int SHOOTING  = 5;

    [SyncVar]
    public int _state = STILL;
    [SyncVar]
    private int _prevState = STILL;

    [SerializeField]
    private float runSpeed = 12f;
    [SerializeField]
    private float sens = 2.5f;

    private PlayerMotor motor;
    private PlayerVisibility playerVis;
    private int finalStateThisFrame;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _state = STILL;
        _prevState = STILL;
    }

    void Start()
    {
        Debug.Log("Started Controller");
        Cursor.visible = false;

        motor = GetComponent<PlayerMotor>();
        playerVis = GetComponent<PlayerVisibility>();
    }

    void HandleMovement()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        Vector3 velocity = (movHorizontal + movVertical).normalized * runSpeed;
        if (!motor.inJump)
        {
            if (velocity.sqrMagnitude > 0)
            {
                finalStateThisFrame = RUNNING;
            }
            else
            {
                finalStateThisFrame = STILL;       
            }
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
            if (motor.inJump) finalStateThisFrame = JUMPING;
            if (_state != finalStateThisFrame)
            {
                _prevState = _state;
                _state = finalStateThisFrame;
                playerVis.CmdUpdateVis(_prevState, _state);
            }
        }
	}
}
