using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VisionFlare : NetworkBehaviour {

    [SerializeField]
    float speed = 5;

	void Start () {
	}
	
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        if (isServer)
        {
            if (col.CompareTag("CrowTarget") || col.CompareTag("Crow")) return;
            RpcSpawnField(transform.position, transform.rotation);
        }
    }


    [ClientRpc]
    void RpcSpawnField(Vector3 position, Quaternion rotation)
    {
        Transform field = Instantiate(ResourceManager.instance.visionFieldPrefab, position, rotation);
        NetworkServer.Spawn(field.gameObject);
        Destroy(this.gameObject);
    }

}
