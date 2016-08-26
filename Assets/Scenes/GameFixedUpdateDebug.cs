using UnityEngine;
using System.Collections.Generic;

public class GameFixedUpdateDebug : MonoBehaviour
{
	GameFixedUpdate _gameFixedUpdate;

	public float gameTime;
	public int gameFrame;

	public void SetGameFixedUpdate(GameFixedUpdate gameFixedUpdate)
	{
		_gameFixedUpdate = gameFixedUpdate;
	}

	void LateUpdate()
	{
		gameTime = _gameFixedUpdate.GameTime;
		gameFrame = _gameFixedUpdate.CurrentGameFrame;
	}
}
