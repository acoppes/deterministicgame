
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

	public virtual void Update(float dt)
	{
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

public class LockstepFixedUpdate : GameFixedUpdate
{
	int _gameFramesPerLockstep;

	CommandsList _pendingCommands;

	public LockstepFixedUpdate(CommandsList pendingCommands)
	{
		_pendingCommands = pendingCommands;
	}

	public int GameFramesPerLockstep {
		get {
			return _gameFramesPerLockstep;
		}
		set {
			_gameFramesPerLockstep = value;
		}
	}

	public override void Update (float dt)
	{
		if (IsLockstepTurn ()) {
		
			if (!IsReady ())
				return;

			ProcessLockstepLogic ();
		}

		// performs basic update logic...
		base.Update (dt);
	}

	bool IsReady()
	{
		// check for pending actions...
		return _pendingCommands.IsReady;
	}

	bool IsLockstepTurn() 
	{
		return (CurrentGameFrame % _gameFramesPerLockstep) == 0;
	}

	void ProcessLockstepLogic()
	{
		// process pending actions..
		_pendingCommands.Process();
	}
}
