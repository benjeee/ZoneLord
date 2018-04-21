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
        player = GameManager.instance.networkManager.client.connection.playerControllers[0].gameObject.GetComponent<Player>();
        playerShoot = GameManager.instance.networkManager.client.connection.playerControllers[0].gameObject.GetComponent<PlayerShoot>();
    }

	void Start () {
        castDistance = 2 * speed * Time.fixedDeltaTime;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void SetPlayerShoot(PlayerShoot p)
    {
        playerShoot = p;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, castDistance, mask))
        {
            if (hit.collider.CompareTag("Crow")) return;
            Vector3 reflectionAngle = Vector3.Reflect(transform.forward, hit.normal);
            if (hit.collider.tag == "Player")
            {
                if(hit.collider.name != player.name)
                {
                    playerShoot.CmdPlayerShot(hit.collider.name, damage);
                    Instantiate(ResourceManager.instance.missilePlayerImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, reflectionAngle));
                }
            }
            else
            {
                Instantiate(ResourceManager.instance.missileImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, reflectionAngle));
            }
            Destroy(this.gameObject);
        }
    }
}
