using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    GameObject playerCanvasPrefab;
    GameObject playerCanvasInstance;

    Camera sceneCamera;

	void Start() {
		if (!isLocalPlayer) {
			Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            DisableComponents();
            AssignRemoteLayer();
		} else {
            Cursor.visible = false; 
            Cursor.lockState = CursorLockMode.Locked;
            sceneCamera = Camera.main;
			if (sceneCamera != null) {
				sceneCamera.gameObject.SetActive(false);
			}
            playerCanvasInstance = Instantiate(playerCanvasPrefab);
            playerCanvasInstance.name = playerCanvasPrefab.name;
            GetComponent<Player>().uiManager = playerCanvasInstance.GetComponent<UIManager>();
            GetComponent<PlayerInventory>().uiManager = playerCanvasInstance.GetComponent<UIManager>();
            GetComponent<PlayerAbilities>().uiManager = playerCanvasInstance.GetComponent<UIManager>();
        }
        GetComponent<Player>().Setup();
	}

    public override void OnStartClient()
    {
        base.OnStartClient();
        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(netId, player);
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
        Destroy(playerCanvasInstance);

		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive(true);
		}

        GameManager.UnRegisterPlayer(transform.name);
	}
}
