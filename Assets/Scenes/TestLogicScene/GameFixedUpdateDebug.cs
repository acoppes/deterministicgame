using UnityEngine;
using Gemserk.Lockstep;

public class GameFixedUpdateDebug : MonoBehaviour
{
	LockstepFixedUpdate _gameFixedUpdate;

	public float gameTime;
	public int gameFrame;

	public int lockstepFrame;

	public float fixedUpdateTime;
	public float lockstepTime;

	public void SetGameFixedUpdate(LockstepFixedUpdate gameFixedUpdate)
	{
		_gameFixedUpdate = gameFixedUpdate;
	}

	void LateUpdate()
	{
		gameTime = _gameFixedUpdate.GameTime;
		gameFrame = _gameFixedUpdate.CurrentGameFrame;
		lockstepFrame = _gameFixedUpdate.CurrentLockstepFrame;
	
		fixedUpdateTime = _gameFixedUpdate.FixedStepTime;
		lockstepTime = _gameFixedUpdate.GameFramesPerLockstep * fixedUpdateTime;
	}
}
