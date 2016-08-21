using System;

public interface DeterministicGameLogic
{
	void Update (int dt, int frame);
}

public class MultipleDeterministicGameEngine : DeterministicGameLogic
{
	readonly DeterministicGameLogic[] gameEngines;

	public MultipleDeterministicGameEngine(DeterministicGameLogic[] gameEngines)
	{
		this.gameEngines = gameEngines;
	}

	#region DeterministicGameEngine implementation
	public void Update (int dt, int frame)
	{
		for (int i = 0; i < gameEngines.Length; i++) {
			var gameEngine = gameEngines [i];
			gameEngine.Update (dt, frame);
		}
	}
	#endregion
}

public class LockstepGameLogic : DeterministicGameLogic
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

	public void Update (int dt, int frame)
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

public class GameFixedUpdate {

	int _fixedTimeStepMilliseconds = 50;

	int _currentGameFrame;

	int _accumulatedMilliseconds;

	DeterministicGameLogic _deterministicGameLogic;

	public int FixedTimeStepMilliseconds {
		get {
			return _fixedTimeStepMilliseconds;
		}
		set {
			_fixedTimeStepMilliseconds = value;
		}
	}

	public int CurrentGameFrame {
		get {
			return _currentGameFrame;
		}
	}

	public void SetGameLogic(DeterministicGameLogic deterministicGameLogic)
	{
		_deterministicGameLogic = deterministicGameLogic;
	}

	public void Init()
	{
		_currentGameFrame = 0;
		_accumulatedMilliseconds = 0;
	}

	public void Update(int milliseconds)
	{
		_accumulatedMilliseconds += milliseconds;

		while (_accumulatedMilliseconds >= _fixedTimeStepMilliseconds) {
			if (_deterministicGameLogic != null)
				_deterministicGameLogic.Update (_fixedTimeStepMilliseconds, _currentGameFrame);
			_currentGameFrame++;
			_accumulatedMilliseconds -= _fixedTimeStepMilliseconds;
		}
	}
}
