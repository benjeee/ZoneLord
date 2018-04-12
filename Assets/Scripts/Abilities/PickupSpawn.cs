using UnityEngine;
using UnityEngine.Networking;

public class PickupSpawn : NetworkBehaviour {

    [SerializeField]
    float respawnTime;

    public static AbilityItem.AbilityItemType[] types =
    {
        AbilityItem.AbilityItemType.Crow,
        AbilityItem.AbilityItemType.Flare,
        AbilityItem.AbilityItemType.Bubble
    };

    void Start()
    {
        Debug.Log(Time.fixedTime);
        Spawn();
    }

    public void PickupNotify()
    {
        Invoke("Spawn", respawnTime);
    }

    void Spawn()
    {
        if (isServer)
        {
            int rnd = Random.Range(0, 3);
            AbilityItem.AbilityItemType type = (AbilityItem.AbilityItemType)types.GetValue(rnd);
            Transform pickup;
            switch (type)
            {
                case AbilityItem.AbilityItemType.Crow:
                    pickup = Instantiate(ResourceManager.instance.crowPickupPrefab, transform);
                    pickup.GetComponent<Pickup>().SpawnPoint = this;
                    NetworkServer.Spawn(pickup.gameObject);
                    break;
                case AbilityItem.AbilityItemType.Flare:
                    pickup = Instantiate(ResourceManager.instance.visionFlarePickupPrefab, transform);
                    pickup.GetComponent<Pickup>().SpawnPoint = this;
                    NetworkServer.Spawn(pickup.gameObject);
                    break;
                case AbilityItem.AbilityItemType.Bubble:
                    pickup = Instantiate(ResourceManager.instance.bubbleShieldPickupPrefab, transform);
                    pickup.GetComponent<Pickup>().SpawnPoint = this;
                    NetworkServer.Spawn(pickup.gameObject);
                    break;
                default:
                    Debug.LogError("Trying to spawn unknown pickup type!");
                    break;
            }
        }
    }
}
