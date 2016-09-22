namespace Gemserk.Lockstep 
{
	public interface LockstepUpdate 
	{
		bool IsLastFrameForNextLockstep();

		int GetNextLockstepFrame ();

		int GetCurrentFrame();
	}
		
	public class LockstepFixedUpdate : GameFixedUpdate, LockstepUpdate
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
				// Don't process same lockstep twice
				_lastLockstepTurn = CurrentGameFrame;
			}

			// performs basic update logic...
			base.Update (dt);
		}

		bool IsReady()
		{
			// check for pending actions...
			return _lockstepLogic.IsReady(CurrentGameFrame);
		}

		int _lastLockstepTurn;

		public bool IsLockstepTurn() 
		{
			if (CurrentGameFrame == 0)
				return false;
			if (CurrentGameFrame == _lastLockstepTurn)
				return false;
			return (CurrentGameFrame % GameFramesPerLockstep) == 0;
		}

		void ProcessLockstepLogic()
		{
			// process pending actions..
			_lockstepLogic.Process(CurrentGameFrame);
		}

		public int GetCurrentFrame()
		{
			return CurrentGameFrame;
		}

		public int GetFirstLockstepFrame()
		{
			return GameFramesPerLockstep;
		}

		public int GetNextLockstepFrame ()
		{
			return GetNextLockstepFrame (CurrentGameFrame);
		}

		public int GetNextLockstepFrame(int currentFrame)
		{
			int d = (currentFrame / GameFramesPerLockstep) + 2;
			return GameFramesPerLockstep * d;
		}

		public bool IsLastFrameForNextLockstep(int frame)
		{
			return GetNextLockstepFrame (frame) != GetNextLockstepFrame (frame + 1);
		}

		public bool IsLastFrameForNextLockstep()
		{
			return IsLastFrameForNextLockstep (CurrentGameFrame);
		}
	}
}
