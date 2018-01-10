using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

	void Start() {
		if (!isLocalPlayer) {
			Cursor.visible = true;
            DisableComponents();
            AssignRemoteLayer();
		} else {
			Cursor.visible = false;
			sceneCamera = Camera.main;
			if (sceneCamera != null) {
				sceneCamera.gameObject.SetActive(false);
			}
		}
        RegisterPlayer();
	}

    void RegisterPlayer()
    {
        string ID = "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name = ID;
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

	void OnDisable(){
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive(true);
		}
	}
}
