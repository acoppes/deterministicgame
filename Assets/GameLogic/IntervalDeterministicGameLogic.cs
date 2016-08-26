using System;

public class IntervalDeterministicGameLogic : GameLogic
{
	int _gameFramesPerLockstepFrame = 4;

	int _currentLockstepFrame = 0;

	int _lastGameFrame = -1;

	int _gameFramesAccumulator = 0;

	public int CurrentLockstepFrame {
		get {
			return _currentLockstepFrame;
		}
	}

	public int GameFramesPerLockstepFrame {
		get {
			return _gameFramesPerLockstepFrame;
		}
		set {
			_gameFramesPerLockstepFrame = value;
		}
	}

	#region GameStepEngine implementation

	public void Update (float dt, int frame)
	{
		if (_lastGameFrame != frame - 1)
			throw new Exception ();

		_gameFramesAccumulator++;

		if (_gameFramesAccumulator == _gameFramesPerLockstepFrame) {
			_currentLockstepFrame++;
			_gameFramesAccumulator = 0;
		}

		_lastGameFrame = frame;
	}

	#endregion
}
