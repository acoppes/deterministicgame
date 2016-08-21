
public class GameStepCode {

	int _fixedTimeStepMilliseconds = 50;

	int _currentUpdate;

	int _accumulatedMilliseconds;

	public int FixedTimeStepMilliseconds {
		get {
			return _fixedTimeStepMilliseconds;
		}
		set {
			_fixedTimeStepMilliseconds = value;
		}
	}


	public int CurrentUpdate {
		get {
			return _currentUpdate;
		}
	}

	public void Init()
	{
		_currentUpdate = 0;
		_accumulatedMilliseconds = 0;
	}

	public void Update(int milliseconds)
	{
		_accumulatedMilliseconds += milliseconds;

		while (_accumulatedMilliseconds >= _fixedTimeStepMilliseconds) {
			_currentUpdate++;
			_accumulatedMilliseconds -= _fixedTimeStepMilliseconds;
		}
	}
}
