using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VisionFlare : NetworkBehaviour {

    [SerializeField]
    float startRadius;

    [SerializeField]
    float endRadius;

    float currRadius;

    [SerializeField]
    float speed = 5;

    [SerializeField]
    LayerMask visionLayer;

    bool flying;

	void Start () {
        flying = true;
        currRadius = startRadius;
        
    }
	
	void Update () {
        if(flying)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        if (flying)
        {
            if (col.CompareTag("CrowTarget") || col.CompareTag("Crow")) return;
            flying = false;
            gameObject.layer = 10;
            GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(GrowFieldLerp(startRadius, endRadius));
        }
        else
        {
            if (col.gameObject.CompareTag("Player"))
            {
                PlayerController pc = col.gameObject.GetComponent<PlayerController>();
                pc.DisableStateChanging();
                pc.CmdUpdateState(PlayerController.PlayerState.Combat);
            }
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

    IEnumerator GrowFieldLerp(float lerpFrom, float lerpTo)
    {
        float elapsedTime = 0;
        float time = 1;
        while (elapsedTime < time)
        {
            currRadius = Mathf.Lerp(lerpFrom, lerpTo, (elapsedTime / time));
            transform.localScale = new Vector3(currRadius, currRadius, currRadius);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

}
