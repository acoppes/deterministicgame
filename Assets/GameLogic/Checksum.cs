using System.Collections.Generic;

public interface GameState
{
	Checksum CalculateChecksum();
}

// a single checksum for the game state
public interface Checksum
{
	// true if both checksum are equals
	bool IsEqual (Checksum checksum);
}

// a single checksum for the game state

public class ChecksumString : Checksum
{
	string _checksum;

	public string Checksum
	{
		get { 
			return _checksum;
		}
	}

	public ChecksumString(string checksum)
	{
		_checksum = checksum;
	}

	#region GameStateChecksum implementation
	public bool IsEqual (Checksum checksum)
	{
		if (checksum == this)
			return true;
		ChecksumString otherChecksum = checksum as ChecksumString;
		if (otherChecksum == null)
			return false;
		return otherChecksum._checksum.Equals(this._checksum);
	}
	#endregion

}

public class StoredChecksum 
{
	public int gameFrame;
	public Checksum checksum;

}

public interface GameStateValidator
{
	bool IsValid(int gameFrame, Checksum checksum);
}

public class GameStateValidatorImpl : GameStateValidator
{
	readonly List<StoredChecksum> _checksums;

	public GameStateValidatorImpl(List<StoredChecksum> checksums)
	{
		_checksums = checksums;
	}

	#region GameStateValidator implementation

	public bool IsValid (int gameFrame, Checksum checksum)
	{
		for (int i = 0; i < _checksums.Count; i++) {
			var _checksum = _checksums [i];
			if (_checksum.gameFrame == gameFrame)
				return _checksum.checksum.IsEqual (checksum);
		}
		return false;
	}

	#endregion

}

public class ChecksumRecorder : GameLogic
{
	readonly GameState _gameState;

	readonly List<StoredChecksum> _storedChecksums = new List<StoredChecksum>();

	readonly int _gameFramesPerChecksum;

	int _checksumFrame;

	public List<StoredChecksum> StoredChecksums {
		get {
			return _storedChecksums;
		}
	}

	public ChecksumRecorder(int gameFramesPerChecksum, GameState gameState)
	{
		_gameFramesPerChecksum = gameFramesPerChecksum;
		_gameState = gameState;

		_checksumFrame = _gameFramesPerChecksum;
	}

	public void Update (float dt, int frame)
	{
		_checksumFrame--;
		if (_checksumFrame <= 0) {
			_storedChecksums.Add (new StoredChecksum () {
				gameFrame = frame,
				checksum = _gameState.CalculateChecksum ()
			});
			_checksumFrame = _gameFramesPerChecksum;
		}
	}
}