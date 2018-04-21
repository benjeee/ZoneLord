using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerAbilities))]
public class PlayerShoot : NetworkBehaviour
{
    public const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    public PlayerController controller;

    [SerializeField]
    Transform leftHandPosition;
    [SerializeField]
    Transform rightHandPosition;
    Transform activeShootPosition;
    int shootPositionInd;

    [SerializeField]
    float combatTimer;

    [SerializeField]
    float shootSpeed;

    [SerializeField]
    float shootCost;

    [SerializeField]
    Player player;

    [SerializeField]
    PlayerAbilities abilities;

    [SerializeField]
    PlayerInventory inventory;

    [SerializeField]
    Animator handAnimator;
    

    public LayerMask mask;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("No camera referenced!");
            this.enabled = false;
        }
        inventory = GetComponent<PlayerInventory>();
        controller = GetComponent<PlayerController>();
        abilities = GetComponent<PlayerAbilities>();
        shootPositionInd = 1;
        activeShootPosition = rightHandPosition;
    }

    void Update()
    {
        HandleShootInput();
        HandleStopShooting();
    }

    void HandleShootInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            InvokeRepeating("Shoot", 0f, shootSpeed);
        }
    }

    void HandleStopShooting()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            CancelInvoke();
        }
    }

    RaycastHit hit;
    [Client]
    void Shoot()
    {
        if (inventory.SpendMana(shootCost))
        {
            if (shootPositionInd == 0)
            {
                activeShootPosition = leftHandPosition;
                //handAnimator.SetTrigger("LeftShot");
            }
            else
            {
                //handAnimator.SetTrigger("RightShot");
                activeShootPosition = rightHandPosition;
            }

            if (CloseTargetShot())
            {
                SwapShootPosition();
                return;
            }
            
            if (abilities.invisToggled)
            {
                abilities.ToggleInvis();
            }
            FixAngleBetweenReticuleAndShootPosition();
            CmdSpawnShot(activeShootPosition.transform.position, activeShootPosition.rotation);
            if (controller.canChangeState)
            {
                controller.CmdUpdateState(PlayerController.PlayerState.Combat);
                controller.DisableStateChanging(combatTimer);
            }
            SwapShootPosition();
        }
    }

    void FixAngleBetweenReticuleAndShootPosition()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 600, mask))
        {
            activeShootPosition.LookAt(hit.point);
        }else
        {
            Debug.LogError("Reticule pointing OOB");
        }
    }

    void SwapShootPosition()
    {
        shootPositionInd = 1 - shootPositionInd;
    }

    bool CloseTargetShot()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1.5f, mask))
        {
            if (hit.collider.CompareTag("Crow")) return false;
            Vector3 reflectionAngle = Vector3.Reflect(cam.transform.forward, hit.normal);
            if (hit.collider.tag == "Player")
            {
                if (hit.collider.name != player.name)
                {
                    CmdPlayerShot(hit.collider.name, 25);
                    Instantiate(ResourceManager.instance.missilePlayerImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, reflectionAngle));
                }
            }
            else
            {
                Instantiate(ResourceManager.instance.missileImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, reflectionAngle));
            }
            return true;
        }
        return false;
    }

    [Command]
    void CmdSpawnShot(Vector3 position, Quaternion rotation)
    {
        Transform shot = Instantiate(ResourceManager.instance.shotPrefab, position, rotation);
        NetworkServer.Spawn(shot.gameObject);
    }

    [Command]
    public void CmdPlayerShot(string playerID, float damage)
    {
        Player playerShot = GameManager.GetPlayer(playerID);
        if(playerShot != this.player)
            playerShot.RpcTakeDamage(damage);
    }

}