using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	Camera sceneCamera;

	void Start() {
		if (!isLocalPlayer) {
			Cursor.visible = true;
			for (int i = 0; i < componentsToDisable.Length; i++) {
				componentsToDisable [i].enabled = false;
			}
		} else {
			Cursor.visible = false;
			sceneCamera = Camera.main;
			if (sceneCamera != null) {
				sceneCamera.gameObject.SetActive(false);
			}
		}
	}

	void OnDisable(){
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive(true);
		}
	}
}
