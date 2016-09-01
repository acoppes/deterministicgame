
public interface LockstepLogic
{
	bool IsReady();

	void Process(int frame);
}

public class LockstepFixedUpdate : GameFixedUpdate
{
	readonly LockstepLogic _lockstepLogic;

	public LockstepFixedUpdate(LockstepLogic lockstepLogic)
	{
		_lockstepLogic = lockstepLogic;
	}

	public int GameFramesPerLockstep {
		get;
		set;
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
		return _lockstepLogic.IsReady();
	}

	bool IsLockstepTurn() 
	{
		return (CurrentGameFrame % GameFramesPerLockstep) == 0;
	}

	void ProcessLockstepLogic()
	{
		// process pending actions..
		_lockstepLogic.Process(CurrentGameFrame);
	}

	public int GetNextLockstepFrame ()
	{
		return GetNextLockstepFrame (CurrentGameFrame);
	}

	public int GetNextLockstepFrame(int currentFrame)
	{
		int d = (currentFrame / GameFramesPerLockstep) + 1;
		return GameFramesPerLockstep * d;
	}
}
