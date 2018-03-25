using UnityEngine.Networking;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    public const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    PlayerController controller;

    [SerializeField]
    Transform shootPosition;

    [SerializeField]
    float combatTimer;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("No camera referenced!");
            this.enabled = false;
        }
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    [Client]
    void Shoot()
    {
        CmdSpawnShot(shootPosition.transform.position, shootPosition.transform.rotation);
        if (controller.canChangeState)
        {
            controller.UpdateState(PlayerController.PlayerState.Combat);
            controller.DisableStateChanging(combatTimer);
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
        shot.GetComponent<BasicMissile>().mask = mask;
        NetworkServer.Spawn(shot.gameObject);
    }

    [Command]
    public void CmdPlayerShot(string playerID, int damage)
    {
        Debug.Log(playerID + "has been shot!");

        Player playerShot = GameManager.GetPlayer(playerID);
        playerShot.RpcTakeDamage(damage);
    }

    
}