using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof (PlayerInventory))]
public class PlayerShoot : NetworkBehaviour
{
    public const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    public PlayerController controller;

    [SerializeField]
    Transform shootPosition;

    [SerializeField]
    float combatTimer;

    [SerializeField]
    float shootSpeed;

    [SerializeField]
    float shootCost;

    [SerializeField]
    Player player;

    [SerializeField]
    PlayerInventory inventory;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("No camera referenced!");
            this.enabled = false;
        }
        inventory = GetComponent<PlayerInventory>();
        controller = GetComponent<PlayerController>();
        player = GetComponent<Player>();
    }

    #region input
    void Update()
    {
        HandleShootInput();
        HandleFlareInput();
        HandleStopShooting();
        HandleCrowInput();
        HandleDPInput();
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

    void HandleFlareInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Flare();
        }
    }

    void HandleCrowInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnCrow();
        }
    }

    void HandleDPInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DistortionPlane();
        }
    }
    #endregion input

    #region shooting
    [Client]
    void Shoot()
    {
        if (inventory.SpendMana(shootCost))
        {
            CmdSpawnShot(shootPosition.transform.position, shootPosition.transform.rotation);
            if (controller.canChangeState)
            {
                controller.CmdUpdateState(PlayerController.PlayerState.Combat);
                controller.DisableStateChanging(combatTimer);
            }
        }
    }

    [Command]
    void CmdSpawnShot(Vector3 position, Quaternion rotation)
    {
        RpcSpawnShot(position, rotation);
    }

    [ClientRpc]
    void RpcSpawnShot(Vector3 position, Quaternion rotation)
    {
        Transform shot = Instantiate(ResourceManager.instance.shotPrefab, position, rotation);
        shot.GetComponent<BasicMissile>().SetPlayerShoot(this);
        shot.GetComponent<BasicMissile>().SetPlayer(player);
        NetworkServer.Spawn(shot.gameObject);
        shot.gameObject.SetActive(true);
    }

    [Command]
    public void CmdPlayerShot(string playerID, int damage)
    {
        Debug.Log(playerID + "has been shot!");

        Player playerShot = GameManager.GetPlayer(playerID);
        playerShot.RpcTakeDamage(damage);
    }
    #endregion shooting

    #region abilities
    [Client]
    void Flare()
    {
        if (inventory.UseFlare())
        {
            CmdSpawnFlare(shootPosition.transform.position, shootPosition.transform.rotation);
        }
    }

    [Command]
    void CmdSpawnFlare(Vector3 position, Quaternion rotation)
    {
        Transform flare = Instantiate(ResourceManager.instance.visionFlarePrefab, position, rotation);
        NetworkServer.Spawn(flare.gameObject);
    }


    [Client]
    void SpawnCrow()
    {
        if (inventory.UseCrow())
        {
            CmdSpawnCrow(shootPosition.transform.position, shootPosition.transform.rotation);
        }
    }

    [Command]
    void CmdSpawnCrow(Vector3 position, Quaternion rotation)
    {
        Transform crow = Instantiate(ResourceManager.instance.crowPrefab, position, rotation);
        crow.GetComponent<Crow>().SetPlayer(player);
        crow.gameObject.SetActive(true);
        NetworkServer.Spawn(crow.gameObject);
    }

    [Client]
    void DistortionPlane()
    {
        CmdSpawnDistortionPlane(shootPosition.transform.position, shootPosition.transform.rotation);
    }

    [Command]
    void CmdSpawnDistortionPlane(Vector3 position, Quaternion rotation)
    {
        Vector3 flip = rotation.eulerAngles;
        flip.y -= 180;
        flip.x = 0;
        Instantiate(ResourceManager.instance.distortionPlanePrefab, position, Quaternion.Euler(flip));
    }
    #endregion abilities
}