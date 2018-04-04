using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BasicMissile : NetworkBehaviour {

    [SerializeField]
    float speed;

    [SerializeField]
    int damage;

    public LayerMask mask;

    PlayerShoot playerShoot;

    Player player;

    float castDistance;

    void Awake()
    {
        gameObject.SetActive(false);
    }

	void Start () {
        castDistance = 4 * speed * Time.fixedDeltaTime;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void SetPlayerShoot(PlayerShoot p)
    {
        playerShoot = p;
    }

    public void SetPlayer(Player p)
    {
        player = p;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, castDistance, mask))
        {
            if (hit.collider.CompareTag("Crow")) return;
            if (hit.collider.tag == "Player" && hit.collider.name != player.name)
            {
                playerShoot.CmdPlayerShot(hit.collider.name, damage);
            }
            Vector3 reflectionAngle = Vector3.Reflect(transform.forward, hit.normal);
            Instantiate(ResourceManager.instance.missileImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, reflectionAngle));
            Destroy(this.gameObject);
        }
    }
}
