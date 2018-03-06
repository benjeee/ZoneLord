using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ZoneMovement : NetworkBehaviour {

    [SerializeField]
    float timeBetweenMoves = 30;

    [SerializeField]
    float startScale = 300;

    [SyncVar]
    float timeSinceMoved;

    [SyncVar(hook ="ShrinkZone")]
    float currScale;

	void Start () {
        timeSinceMoved = 0;
        currScale = startScale;
	}
	
	void Update () {
        if (isServer)
        {
            CmdUpdateTimeSinceMoved();
        }
	}

    [Command]
    void CmdUpdateTimeSinceMoved()
    {
        timeSinceMoved += Time.deltaTime;
        if (timeSinceMoved > timeBetweenMoves)
        {
            MoveToRandomMapPosition();
            CmdShrinkZone();
            timeSinceMoved = 0;
        }
    }

    //improve this to make sure it's fully in bounds when adding scale to position
    void MoveToRandomMapPosition()
    {
        Vector2 xz = Random.insideUnitCircle * GameManager.instance.matchSettings.mapSize / 2;
        transform.position = new Vector3(xz.x, 0, xz.y);
    }

    [Command]
    void CmdShrinkZone()
    {
        currScale *= .75f;
    }

    void ShrinkZone(float scale)
    {
        transform.localScale = new Vector3(scale, 100f, scale);
    }
}
