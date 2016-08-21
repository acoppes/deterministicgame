
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
