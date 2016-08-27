using NUnit.Framework;

public class TestLockstepLogic {

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){

//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		CommandsList pendingCommands = new CommandsList ();
		pendingCommands.IsReady = false;

		LockstepGameLogic lockstepGameLogic = new LockstepGameLogic (gameLogic, pendingCommands);

		lockstepGameLogic.GameFramesPerLockstep = 1;

		lockstepGameLogic.Update (0.1f, 0);
		lockstepGameLogic.Update (0.1f, 1);
		lockstepGameLogic.Update (0.1f, 2);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

		pendingCommands.IsReady = true;

		lockstepGameLogic.Update (0.1f, 3);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
	}
}
