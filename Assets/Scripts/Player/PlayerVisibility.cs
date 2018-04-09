using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerVisibility : NetworkBehaviour {

    static Dictionary<PlayerController.PlayerState, float> StateToVisibility = new Dictionary<PlayerController.PlayerState, float>()
    {
        { PlayerController.PlayerState.Stealth, 0.05f},
        { PlayerController.PlayerState.Still, 0.05f},
        { PlayerController.PlayerState.Running, 0.7f},
        { PlayerController.PlayerState.Jumping, 0.7f},
        { PlayerController.PlayerState.Combat, 1f},
        { PlayerController.PlayerState.Walking, 0.2f}
    };


    [SerializeField]
    private Renderer playerRenderer;

    [SerializeField]
    private Material material;
    private Material materialInstance;

    [SerializeField]
    private PlayerController controller;

    //[SyncVar]
    public float visibilityCoefficient;

    void Start () {
        visibilityCoefficient = 0.1f;
        playerRenderer = GetComponent<Renderer>();
        controller = GetComponent<PlayerController>();
        materialInstance = (Material)Instantiate(material);
        material.shader = Shader.Find("Unlit/Hologram");
        playerRenderer.material = materialInstance;
    }

    void Update()
    {
        float goalVal = StateToVisibility[controller._state];
        float time = Mathf.Abs(visibilityCoefficient - goalVal);
        if(controller._state == PlayerController.PlayerState.Stealth)
        {
            time /= 4;
        }
        else if(goalVal > visibilityCoefficient)
        {
            time /= 2;
        }else
        {
            time *= 3;
        }
        float newVal = Mathf.Lerp(visibilityCoefficient, goalVal, (Time.deltaTime / time));
        visibilityCoefficient = newVal;
        SetVis();
    }

    void SetVis()
    {
        materialInstance.SetFloat("_Transparency", visibilityCoefficient);
    }

    [Command]
    void CmdUpdateVis(float newVal)
    {
        visibilityCoefficient = newVal;
        //SetVis();
    }
}
