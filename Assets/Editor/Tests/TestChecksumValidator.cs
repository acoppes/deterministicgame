using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;

public interface GameState
{
	Checksum CalculateChecksum();
}

public interface GameStateChecksumObject
{
	
}

// a single checksum for the game state
public interface Checksum
{
//	// the frame when the checksum was calculated
//	int GetGameFrame();

	// true if both checksum are equals
	bool IsEqual (Checksum checksum);
}

public class ChecksumStringImpl : Checksum
{
	string _checksum;

	public ChecksumStringImpl(string checksum)
	{
		_checksum = checksum;
	}

	#region GameStateChecksum implementation
	public bool IsEqual (Checksum checksum)
	{
		if (checksum == this)
			return true;
		ChecksumStringImpl otherChecksum = checksum as ChecksumStringImpl;
		if (otherChecksum == null)
			return false;
		return otherChecksum._checksum.Equals(this._checksum);
	}
	#endregion
	
}

public interface GameStateValidator
{
	bool IsValid(int gameFrame, Checksum checksum);
}

public class GameStateValidatorImpl : GameStateValidator
{
	public class StoredChecksum 
	{
		public int gameFrame;
		public Checksum checksum;

	}

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

public class TestChecksumValidator {

	[Test]
	public void TestsChecksums(){
		Checksum checksum1 = new ChecksumStringImpl ("AB1234CD");
		Assert.IsTrue (checksum1.IsEqual (checksum1));

		Checksum checksum2 = new ChecksumStringImpl ("BOADASG1");
		Assert.IsFalse(checksum1.IsEqual (checksum2));

		Checksum checksum3 = new ChecksumStringImpl ("AB1234CD");
		Assert.IsTrue(checksum1.IsEqual (checksum3));
	}

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){
		
		GameStateValidator gameStateValidator = new GameStateValidatorImpl (new List<GameStateValidatorImpl.StoredChecksum>() {
			{ 
				new GameStateValidatorImpl.StoredChecksum() {
					gameFrame = 0,
					checksum = new ChecksumStringImpl("ABC1234")
				} 
			}
		});
	
		// invalid checksum in valid frame
		Assert.That(gameStateValidator.IsValid (0, new ChecksumStringImpl("DBC1231")), Is.False);
		// valid checksum in same frame
		Assert.That(gameStateValidator.IsValid (0, new ChecksumStringImpl("ABC1234")), Is.True);
		// valid checksum but on invalid frame
		Assert.That(gameStateValidator.IsValid (1, new ChecksumStringImpl("ABC1234")), Is.False);
	}
}
