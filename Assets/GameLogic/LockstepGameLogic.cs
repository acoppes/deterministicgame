using System.Collections.Generic;

public class LockstepGameLogic : DeterministicGameLogic
{
	DeterministicGameLogic _gameLogic;

	readonly CommandsList _pendingCommands;

	int _gameFramesPerLockstep = 2;

	int _gameFrameForLockstep;
	int _currentGameFrame;

	public int GameFramesPerLockstep {
		get {
			return _gameFramesPerLockstep;
		}
		set {
			_gameFramesPerLockstep = value;
			if (_gameFrameForLockstep >= _gameFramesPerLockstep)
				_gameFrameForLockstep = _gameFramesPerLockstep - 1;
		}
	}

	public LockstepGameLogic(DeterministicGameLogic gameLogic, CommandsList pendingCommands)
	{
		_gameLogic = gameLogic;
		_pendingCommands = pendingCommands;
	}

	#region DeterministicGameLogic implementation

	public void Update (int dt, int frame)
	{
		if (!ProcessLockstepTurn ())
			return;

		// if waiting for lockstep turn then do not update game logic nor game frame.

		_currentGameFrame++;

		_gameLogic.Update (dt, _currentGameFrame);
	}

	#endregion

	bool IsLockstepFrame()
	{
		return (_gameFrameForLockstep + 1) == _gameFramesPerLockstep;
	}

	bool ProcessLockstepTurn()
	{
		if (!IsLockstepFrame ()) {
			_gameFrameForLockstep++;
			return true;
		}

		// waiting for commands from other players
		if (!_pendingCommands.IsReady)
			return false;

		_pendingCommands.Process ();

		_gameFrameForLockstep = 0;

		return true;
	}
	
}
