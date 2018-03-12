using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerVisibility : NetworkBehaviour {


    static Dictionary<int, float> StateToVisibility = new Dictionary<int, float>()
    {
        { PlayerController.STILL, 0.05f},
        { PlayerController.RUNNING, 0.7f},
        { PlayerController.JUMPING, 0.7f},
        { PlayerController.SHOOTING, 1f},
        { PlayerController.WALKING, 0.2f}
    };


    [SerializeField]
    private Renderer playerRenderer;

    [SerializeField]
    private Material material;
    private Material materialInstance;

    [SyncVar]
    public float visibilityCoefficient;

    void Start () {
        visibilityCoefficient = 0.1f;
        playerRenderer = GetComponent<Renderer>();
        materialInstance = (Material)Instantiate(material);
        material.shader = Shader.Find("Unlit/Hologram");
        playerRenderer.material = materialInstance;
    }

    void Update()
    {
        materialInstance.SetFloat("_Transparency", visibilityCoefficient);
    }

    [Command]
    public void CmdUpdateVis(int prevState, int newState)
    {
        float lerpFrom = StateToVisibility[prevState];
        float lerpTo = StateToVisibility[newState];
        StartCoroutine(VisibilityLerp(lerpFrom, lerpTo));
    }

    IEnumerator VisibilityLerp(float lerpFrom, float lerpTo)
    {
        float elapsedTime = 0;
        float time = Mathf.Abs(lerpFrom - lerpTo);
        while (elapsedTime < time)
        {
            visibilityCoefficient = Mathf.Lerp(lerpFrom, lerpTo, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
