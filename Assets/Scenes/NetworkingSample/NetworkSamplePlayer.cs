using UnityEngine;
using UnityEngine.Networking;

public class NetworkSamplePlayer : NetworkBehaviour {

	Vector2 destination;

	Camera myCamera;

	NetworkSampleMovement movement;

	void Awake()
	{
		movement = GetComponent<NetworkSampleMovement> ();
		myCamera = Camera.main;
	}

	[ClientCallback]
	void Update () {

		if (!isLocalPlayer)
			return;

		if (Input.GetMouseButtonUp (0)) {
			Vector2 position = myCamera.ScreenToWorldPoint (Input.mousePosition);
			movement.CmdMoveTo (position);
		}
	}

}
