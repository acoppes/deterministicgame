using NUnit.Framework;
using NSubstitute;

public class TestLockstepLogic {

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){

//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.GameFramesPerLockstep = 1;
		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady ().Returns (false);

//		LockstepGameLogic lockstepGameLogic = new LockstepGameLogic (gameLogic, pendingCommands);

		lockstepGameLogic.GameFramesPerLockstep = 1;

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

		lockstepLogic.IsReady ().Returns (true);

		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestNextLockstepFrame(){

		//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.GameFramesPerLockstep = 3;
		lockstepGameLogic.FixedStepTime = 0.1f;

		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady ().Returns (true);

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (), Is.EqualTo (3));

		lockstepGameLogic.Update (0.1f);

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (), Is.EqualTo (3));

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (), Is.EqualTo (6));

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (), Is.EqualTo (9));

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (0), Is.EqualTo (3));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (1), Is.EqualTo (3));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (2), Is.EqualTo (3));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (3), Is.EqualTo (6));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (6), Is.EqualTo (9));
	}

//	[Test]
//	public void TestCommandListLockstepLogic(){
//
//		CommandsListLockstepLogic lockstepLogic = new CommandsListLockstepLogic (new CommandsList ());
//
//		lockstepLogic.LockstepFrames = 3;
//		lockstepLogic.SetCommunicationsDelayInFrames (2);
//
//		Assert.That (lockstepLogic.GetCommandProcessFrame(0), Is.EqualTo (3));
//		Assert.That (lockstepLogic.GetCommandProcessFrame(1), Is.EqualTo (3));
//
//		Assert.That (lockstepLogic.GetCommandProcessFrame(2), Is.EqualTo (6));
//		Assert.That (lockstepLogic.GetCommandProcessFrame(3), Is.EqualTo (6));
//	
//		Assert.That (lockstepLogic.GetCommandProcessFrame(4), Is.EqualTo (9));
//	}
}
