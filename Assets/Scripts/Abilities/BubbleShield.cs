using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BubbleShield : NetworkBehaviour {

    [SerializeField]
    float startRadius;
        
    [SerializeField]
    float endRadius;

    [SerializeField]
    float growthTime;

    float currRadius;

    void Start()
    {
        currRadius = startRadius;
        StartCoroutine(GrowFieldLerp(startRadius, endRadius));
        Invoke("ShrinkField", 24f);
    }

    IEnumerator GrowFieldLerp(float lerpFrom, float lerpTo)
    {
        float elapsedTime = 0;
        while (elapsedTime < growthTime)
        {
            currRadius = Mathf.Lerp(lerpFrom, lerpTo, (elapsedTime / growthTime));
            transform.localScale = new Vector3(currRadius, currRadius, currRadius);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void ShrinkField()
    {
        StartCoroutine(GrowFieldLerp(endRadius, startRadius));
    }
}
