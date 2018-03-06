using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerVisibility : NetworkBehaviour {

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
    public void CmdUpdateVis(Vector3 velocity)
    {
        visibilityCoefficient = ((float)velocity.sqrMagnitude / 144f) * .5f + .05f;
    }
}
