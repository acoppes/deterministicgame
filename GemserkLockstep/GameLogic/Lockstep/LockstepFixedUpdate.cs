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

		int _currentLockstepFrame;

		public LockstepFixedUpdate(LockstepLogic lockstepLogic)
		{
			_lockstepLogic = lockstepLogic;
		}

		public int GameFramesPerLockstep {
			get;
			set;
		}

		public int CurrentLockstepFrame
		{
			get { 
				return _currentLockstepFrame;
			}
		}

		public override void Init ()
		{
			base.Init ();
			_currentLockstepFrame = 0;
		}

		protected override void FixedTimeUpdate ()
		{
			if (IsLockstepTurn ()) {
			
				if (!IsReady ())
					return;

				ProcessLockstepLogic ();
				// Don't process same lockstep twice
				_lastLockstepGameFrame = CurrentGameFrame;
				_currentLockstepFrame++;
			}

			// performs basic update logic...
			base.FixedTimeUpdate ();
		}

		bool IsReady()
		{
			// check for pending actions...
			return _lockstepLogic.IsReady(CurrentGameFrame);
		}

		int _lastLockstepGameFrame;

		public bool IsLockstepTurn() 
		{
			if (CurrentGameFrame == 0)
				return false;
			if (CurrentGameFrame == _lastLockstepGameFrame)
				return false;

			// since the game frame is always behind one frame (current game frame wasnt processed yet)

			return ((CurrentGameFrame + 1) % GameFramesPerLockstep) == 0;
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
