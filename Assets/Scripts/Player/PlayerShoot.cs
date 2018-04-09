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
    PlayerAbilities abilities;

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
        abilities = GetComponent<PlayerAbilities>();
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

    [Client]
    void Shoot()
    {
        if (inventory.SpendMana(shootCost))
        {
            if (abilities.invisToggled)
            {
                abilities.ToggleInvis();
            }
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
        Player playerShot = GameManager.GetPlayer(playerID);
        playerShot.RpcTakeDamage(damage);
    }
}