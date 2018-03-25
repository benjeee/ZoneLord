using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ZoneMovement : NetworkBehaviour {

    [SerializeField]
    public float timeBetweenMoves = 30;

    [SerializeField]
    float startScale = 300;

    [SerializeField]
    float shrinkFactor = .75f;

    [SyncVar]
    public float timeSinceMoved;

    [SyncVar]
    float currScale;

    void Start()
    {
        timeSinceMoved = 0;
        currScale = startScale;
    }

    void Update()
    {
        if (isServer)
        {
            UpdateTimeSinceMoved();
        }
    }
    
    void UpdateTimeSinceMoved()
    {
        timeSinceMoved += Time.deltaTime;
        if (timeSinceMoved > timeBetweenMoves)
        {
            RpcMoveToRandomMapPosition();
            RpcShrinkZone();
            timeSinceMoved = 0;
        }
    }
    
    [ClientRpc]
    void RpcMoveToRandomMapPosition()
    {
        GameManager.instance.NotifyPlayersZoneMoved(transform.position, currScale / 2);
        Vector2 xz = Random.insideUnitCircle * (GameManager.instance.matchSettings.mapSize - currScale / 2) / 2;
        transform.position = new Vector3(xz.x, 0, xz.y);
    }

    [ClientRpc]
    void RpcShrinkZone()
    {
        currScale *= shrinkFactor;
        transform.localScale = new Vector3(currScale, 100f, currScale);
    }
}
