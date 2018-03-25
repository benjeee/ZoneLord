using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionField : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController pc = col.gameObject.GetComponent<PlayerController>();
            pc.DisableStateChanging();
            pc.UpdateState(PlayerController.PlayerState.Combat);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController pc = col.gameObject.GetComponent<PlayerController>();
            pc.canChangeState = true;
        }
    }
}
