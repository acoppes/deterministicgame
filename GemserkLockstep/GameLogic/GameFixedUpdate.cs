namespace Gemserk.Lockstep 
{
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

		public virtual void Init()
		{
			_currentGameFrame = 0;
			_accumulatedTime = 0;
			_gameTime = 0;
		}

		// fixed time step based on http://gafferongames.com/game-physics/fix-your-timestep/

		float maxAllowedFrameTime = 0.25f;

		public float MaxAllowedFrameTime {
			get {
				return maxAllowedFrameTime;
			}
			set {
				maxAllowedFrameTime = value;
			}
		}

		public void Update(float dt)
		{
			if (dt > maxAllowedFrameTime)
				dt = maxAllowedFrameTime;

			_accumulatedTime += dt;

			while (_accumulatedTime >= _fixedStepTime) {
				FixedTimeUpdate ();
				_accumulatedTime -= _fixedStepTime;
			}
		}

		protected virtual void FixedTimeUpdate()
		{
			if (_gameLogic != null)
				_gameLogic.GameUpdate (_fixedStepTime, _currentGameFrame);
			_gameTime += _fixedStepTime;
			_currentGameFrame++;
		}
	}
}