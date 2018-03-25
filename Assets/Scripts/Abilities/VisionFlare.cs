using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VisionFlare : NetworkBehaviour {

    [SerializeField]
    float speed = 5;

	void Start () {
	}
	
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        //spawn vision field here
        Destroy(this.gameObject);
    }

}
