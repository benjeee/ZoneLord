using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VisionField : NetworkBehaviour {

    [SerializeField]
    float startRadius;

    [SerializeField]
    float endRadius;

    float currRadius;

	void Start () {
        currRadius = startRadius;
        StartCoroutine(GrowFieldLerp(startRadius, endRadius));
	}
	
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController pc = col.gameObject.GetComponent<PlayerController>();
            pc.DisableStateChanging();
            pc.CmdUpdateState(PlayerController.PlayerState.Combat);
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
