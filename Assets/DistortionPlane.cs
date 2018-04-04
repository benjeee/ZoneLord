using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionPlane : MonoBehaviour {

    [SerializeField]
    float startWidth;

    [SerializeField]
    float endWidth;

    float currWidth;

    // Use this for initialization
    void Start()
    {
        currWidth = startWidth;
        StartCoroutine(GrowPlaneLerp(startWidth, endWidth));
    }

    IEnumerator GrowPlaneLerp(float lerpFrom, float lerpTo)
    {
        float elapsedTime = 0;
        float time = 2;
        while (elapsedTime < time)
        {
            currWidth = Mathf.Lerp(lerpFrom, lerpTo, (elapsedTime / time));
            transform.localScale = new Vector3(currWidth, currWidth, currWidth);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
