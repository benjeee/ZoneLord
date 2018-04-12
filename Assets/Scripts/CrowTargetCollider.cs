using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowTargetCollider : MonoBehaviour {

    Crow crow;

    float timeSinceSwitched;

    void Start()
    {
        crow = GetComponentInParent<Crow>();
        timeSinceSwitched = 0;
    }

    void Update()
    {
        timeSinceSwitched += Time.deltaTime;
        if(timeSinceSwitched > 1.0)
        {
            crow.HitTarget();
            timeSinceSwitched = 0;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("CrowTarget"))
        {
            crow.HitTarget();
            timeSinceSwitched = 0;
        }
    }
}
