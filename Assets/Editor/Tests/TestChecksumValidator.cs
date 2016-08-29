using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;


public class TestChecksumValidator {

	[Test]
	public void TestsChecksums(){
		Checksum checksum1 = new ChecksumString ("AB1234CD");
		Assert.IsTrue (checksum1.IsEqual (checksum1));

		Checksum checksum2 = new ChecksumString ("BOADASG1");
		Assert.IsFalse(checksum1.IsEqual (checksum2));

		Checksum checksum3 = new ChecksumString ("AB1234CD");
		Assert.IsTrue(checksum1.IsEqual (checksum3));
	}

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){
		
		ChecksumValidator gameStateValidator = new ChecksumValidatorBasic (new List<StoredChecksum>() {
			{ 
				new StoredChecksum() {
					gameFrame = 0,
					checksum = new ChecksumString("ABC1234")
				}
			}, 
			{
				new StoredChecksum() {
					gameFrame = 1,
					checksum = new ChecksumString("BOB3322")
				} 
			}
		});
	
		// invalid checksum in valid frame
		Assert.That(gameStateValidator.IsValid (0, new ChecksumString("DBC1231")), Is.False);
		// valid checksum in same frame
		Assert.That(gameStateValidator.IsValid (0, new ChecksumString("ABC1234")), Is.True);
		// valid checksum but on invalid frame
		Assert.That(gameStateValidator.IsValid (1, new ChecksumString("ABC1234")), Is.False);
		// valid checksum in same frame
		Assert.That(gameStateValidator.IsValid (1, new ChecksumString("BOB3322")), Is.True);
	}
}
