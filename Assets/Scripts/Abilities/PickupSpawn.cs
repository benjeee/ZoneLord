using UnityEngine;
using UnityEngine.Networking;

public class PickupSpawn : NetworkBehaviour {

    [SerializeField]
    float respawnTime;

    public static Pickup.PickupType[] types =
    {
        Pickup.PickupType.Crow,
        Pickup.PickupType.Flare
    };

    System.Random rand;

    void Start()
    {
        rand = new System.Random();
        Spawn();
    }

    public void PickupNotify()
    {
        Invoke("Spawn", respawnTime);
    }

    void Spawn()
    {
        Pickup.PickupType type = (Pickup.PickupType)types.GetValue(rand.Next(types.Length));
        Transform pickup;
        switch (type)
        {
            case Pickup.PickupType.Crow:
                pickup = Instantiate(ResourceManager.instance.crowPickupPrefab, transform);
                pickup.GetComponent<Pickup>().SpawnPoint = this;
                NetworkServer.Spawn(pickup.gameObject);
                break;
            case Pickup.PickupType.Flare:
                pickup = Instantiate(ResourceManager.instance.visionFlarePickupPrefab, transform);
                pickup.GetComponent<Pickup>().SpawnPoint = this;
                NetworkServer.Spawn(pickup.gameObject);
                break;
            default:
                Debug.LogError("Trying to spawn unknown pickup type!");
                break;
        } 
    }
}
