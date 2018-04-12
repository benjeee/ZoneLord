using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowTarget : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("CrowTargetCollider"))
        {
            col.GetComponentInParent<Crow>().HitTarget();
        }
    }
}
