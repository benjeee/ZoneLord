using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Steez : NetworkBehaviour
{

    [SerializeField]
    float startSize;

    [SerializeField]
    float endSize;

    [SerializeField]
    float growTime;

    [SerializeField]
    float dps;

    [SerializeField]
    float speed;

    List<Player> playersToDot;

    float elapsedTime;

    float currRadius;

    PlayerShoot playerShoot;

    public Vector3 Forward { get; set; }

    void Awake()
    {
        playersToDot = new List<Player>();
        Forward = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z).normalized;
    }

    void Start()
    {
        playerShoot = GameManager.instance.networkManager.client.connection.playerControllers[0].gameObject.GetComponent<PlayerShoot>();
        StartCoroutine(GrowFieldLerp(startSize, endSize));
    }

    void Update()
    {
        transform.position += Forward * speed;
        DamagePlayers();
    }

    IEnumerator GrowFieldLerp(float lerpFrom, float lerpTo)
    {
        float elapsedTime = 0;
        while (elapsedTime < growTime)
        {
            currRadius = Mathf.Lerp(lerpFrom, lerpTo, (elapsedTime / growTime));
            //transform.localScale = new Vector3(currRadius, currRadius, currRadius);
            if (isServer) RpcUpdateScale();
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            StartDamaging(col.GetComponent<Player>());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            StopDamaging(col.GetComponent<Player>());
        }
    }

    void StartDamaging(Player p)
    {
        playersToDot.Add(p);
    }

    void StopDamaging(Player p)
    {
        playersToDot.Remove(p);
    }

    void DamagePlayers()
    {
        if (playersToDot == null) return;
        float damageToDeal = dps * Time.deltaTime;
        List<Player> newList = new List<Player>();
        foreach (Player p in playersToDot)
        {
            playerShoot.CmdPlayerShot(p.name, damageToDeal);
            if (!p.isDead) newList.Add(p);
        }
        playersToDot = newList;
    }

    [ClientRpc]
    void RpcUpdateScale()
    {
        transform.localScale = new Vector3(currRadius, currRadius, currRadius);
    }
}

