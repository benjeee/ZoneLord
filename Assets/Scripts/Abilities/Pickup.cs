using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickupSpawn))]
public class Pickup : MonoBehaviour {

    public PickupSpawn SpawnPoint { get; set; }

    public enum PickupType
    {
        Crow,
        Flare
    }

    [SerializeField]
    PickupType type;

    void Update()
    {
        float yPos = .5f + (Mathf.Sin(3 * Time.fixedTime) / 3);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerInventory>().PickupItem(type);
            SpawnPoint.PickupNotify();
            Destroy(this.gameObject);
        }
    }
}
