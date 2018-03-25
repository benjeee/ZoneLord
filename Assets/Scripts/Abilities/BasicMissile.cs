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

    float castDistance;

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

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, castDistance, mask))
        {
            if (hit.collider.tag == "VisionField") return;
            if (hit.collider.tag == "Player")
            {
                playerShoot.CmdPlayerShot(hit.collider.name, damage);
            }
            Vector3 reflectionAngle = Vector3.Reflect(transform.forward, hit.normal);
            Instantiate(ResourceManager.instance.missileImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, reflectionAngle));
            Destroy(this.gameObject);
        }
    }
}
