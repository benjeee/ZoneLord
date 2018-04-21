using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour {

    public PickupSpawn SpawnPoint { get; set; }

    [SerializeField]
    AbilityItem.AbilityItemType type;

    void Update()
    {
        float yPos = .5f + (Mathf.Sin(3 * Time.fixedTime) / 3);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerAbilities>().PickupItem(type);
            SpawnPoint.PickupNotify();
            Destroy(this.gameObject);
        }
    }
}
