
public class GameFixedUpdate {

	float _fixedStepTime = 0.05f;

	int _currentGameFrame;

	float _accumulatedTime;

	float _gameTime;

	GameLogic _gameLogic;

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

	public void SetGameLogic(GameLogic gameLogic)
	{
		_gameLogic = gameLogic;
	}

	public void Init()
	{
		_currentGameFrame = 0;
		_accumulatedTime = 0;
		_gameTime = 0;
	}

	// fixed time step based on http://gafferongames.com/game-physics/fix-your-timestep/

	const float maxAllowedFrameTime = 0.25f;

	public virtual void Update(float dt)
	{
		if (dt > maxAllowedFrameTime)
			dt = maxAllowedFrameTime;

		_accumulatedTime += dt;
		_gameTime += dt;

		while (_accumulatedTime >= _fixedStepTime) {
			if (_gameLogic != null)
				_gameLogic.Update (_fixedStepTime, _currentGameFrame);
			_currentGameFrame++;
			_accumulatedTime -= _fixedStepTime;
		}
	}
}

