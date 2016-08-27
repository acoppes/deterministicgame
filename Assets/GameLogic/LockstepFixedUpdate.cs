
public class LockstepFixedUpdate : GameFixedUpdate
{
	int _gameFramesPerLockstep;

	readonly CommandsList _pendingCommands;

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
