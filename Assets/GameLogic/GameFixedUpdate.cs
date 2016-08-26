
public class GameFixedUpdate {

	float _fixedStepTime = 0.05f;

	int _currentGameFrame;

	float _accumulatedTime;

	float _gameTime;

	DeterministicGameLogic _deterministicGameLogic;

	public float FixedStepTime {
		get {
			return _fixedStepTime;
		}
		set {
			_fixedStepTime = value;
		}
	}

	public int CurrentGameFrame {
		get {
			return _currentGameFrame;
		}
	}

	public float GameTime
	{
		get { 
			return _gameTime;
		}
	}

	public void SetGameLogic(DeterministicGameLogic deterministicGameLogic)
	{
		_deterministicGameLogic = deterministicGameLogic;
	}

	public void Init()
	{
		_currentGameFrame = 0;
		_accumulatedTime = 0;
		_gameTime = 0;
	}

	public void Update(float dt)
	{
		_accumulatedTime += dt;
		_gameTime += dt;

		while (_accumulatedTime >= _fixedStepTime) {
			if (_deterministicGameLogic != null)
				_deterministicGameLogic.Update (_fixedStepTime, _currentGameFrame);
			_currentGameFrame++;
			_accumulatedTime -= _fixedStepTime;
		}
	}
}
