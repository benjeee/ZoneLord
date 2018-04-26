using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    Transform camTransform;

    [SerializeField]
    float shakeDuration = 0f;

    [SerializeField]
    public float shakeAmount = 0.7f;

    Vector3 originalPos;

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    public void DoShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }

    IEnumerator Shake(float duration)
    {
        float timePassed = 0;
        while(timePassed < duration)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            timePassed += Time.deltaTime;
            yield return new WaitForSeconds(.02f);
        }
    }
}