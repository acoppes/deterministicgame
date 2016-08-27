using NUnit.Framework;

public class TestLockstepLogic {

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){

//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		CommandsList pendingCommands = new CommandsList ();
		pendingCommands.IsReady = false;

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (pendingCommands);
		lockstepGameLogic.GameFramesPerLockstep = 1;
		lockstepGameLogic.SetGameLogic (gameLogic);

//		LockstepGameLogic lockstepGameLogic = new LockstepGameLogic (gameLogic, pendingCommands);

		lockstepGameLogic.GameFramesPerLockstep = 1;

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

		pendingCommands.IsReady = true;

		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
	}
}
