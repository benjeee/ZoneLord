using UnityEngine;
using UnityEngine.Networking;

public class PickupSpawn : NetworkBehaviour {

    [SerializeField]
    float respawnTime;

    [SyncVar]
    int nextPickup;

    public static AbilityItem.AbilityItemType[] types =
    {
        AbilityItem.AbilityItemType.Crow,
        AbilityItem.AbilityItemType.Flare,
        AbilityItem.AbilityItemType.Bubble
    };

    void Start()
    {
        if (isServer) CmdPickNextPickup();
        Spawn();
    }

    public void PickupNotify()
    {
        if (isServer) CmdPickNextPickup();
        Invoke("Spawn", respawnTime);
    }

    [Command]
    void CmdPickNextPickup()
    {
        nextPickup = Random.Range(0, 3);
    }

    void Spawn()
    {
        AbilityItem.AbilityItemType type = (AbilityItem.AbilityItemType)types.GetValue(nextPickup);
        Transform pickup;
        switch (type)
        {
            case AbilityItem.AbilityItemType.Crow:
                pickup = Instantiate(ResourceManager.instance.crowPickupPrefab, transform);
                pickup.GetComponent<Pickup>().SpawnPoint = this;
                break;
            case AbilityItem.AbilityItemType.Flare:
                pickup = Instantiate(ResourceManager.instance.visionFlarePickupPrefab, transform);
                pickup.GetComponent<Pickup>().SpawnPoint = this;
                break;
            case AbilityItem.AbilityItemType.Bubble:
                pickup = Instantiate(ResourceManager.instance.bubbleShieldPickupPrefab, transform);
                pickup.GetComponent<Pickup>().SpawnPoint = this;
                break;
            default:
                Debug.LogError("Trying to spawn unknown pickup type!");
                break;
        }
    }
}
