using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;

public class TestLockstepLogic {

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){

//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.GameFramesPerLockstep = 1;
		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady (0).ReturnsForAnyArgs (false);

//		LockstepGameLogic lockstepGameLogic = new LockstepGameLogic (gameLogic, pendingCommands);

		lockstepGameLogic.GameFramesPerLockstep = 1;

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

		lockstepLogic.IsReady (0).ReturnsForAnyArgs (true);

		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestNextLockstepFrameIsZeroIndexed(){

		//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

//		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.GameFramesPerLockstep = 4;
		lockstepGameLogic.FixedStepTime = 0.1f;

		// 0, 1, 2, 3
		// 4, 5, 6, 7
		// 8, 9, 10, 11

//		lockstepGameLogic.SetGameLogic (gameLogic);

//		lockstepLogic.IsReady (0).Returns (true);

		Assert.That (lockstepGameLogic.GetFirstLockstepFrame (), Is.EqualTo (3));

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (0), Is.EqualTo (7));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (1), Is.EqualTo (7));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (2), Is.EqualTo (7));

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (4), Is.EqualTo (11));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (5), Is.EqualTo (11));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (6), Is.EqualTo (11));

//		Assert.That (lockstepGameLogic.GetNextLockstepFrame (8), Is.EqualTo (16));
	}

	[Test]
	public void LockstepLogicShouldNotProcessAgainIfNoFixedGameStep(){

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.GameFramesPerLockstep = 2;
		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady (0).ReturnsForAnyArgs (false);

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		// didnt process lockstep turn yet
//		lockstepLogic.DidNotReceive ().IsReady (Arg.Any<int> ());
		lockstepLogic.DidNotReceive ().Process (Arg.Any<int> ());

		Assert.That (lockstepGameLogic.IsLockstepTurn(), Is.True);

		lockstepLogic.ClearReceivedCalls ();

		// if update too low, then even if it is lockstep turn, it didnt process it 
		lockstepGameLogic.Update (0.002f);

		lockstepLogic.DidNotReceive ().IsReady (Arg.Any<int> ());
		lockstepLogic.DidNotReceive ().Process (Arg.Any<int> ());
	}

	[Test]
	public void TestGetLockstepFrameForCurrentFrame(){

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.GameFramesPerLockstep = 4;

		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (0), Is.EqualTo (false));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (1), Is.EqualTo (false));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (2), Is.EqualTo (false));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (3), Is.EqualTo (true));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (7), Is.EqualTo (true));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (8), Is.EqualTo (false));
	}

	[Test]
	public void TestProcessCallsWhenBigUpdateTime(){

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.MaxAllowedFrameTime = 100.0f;
		lockstepGameLogic.GameFramesPerLockstep = 5;

		lockstepLogic.IsReady (Arg.Any<int> ()).Returns (true);

		lockstepGameLogic.Update (1.2f);

		lockstepLogic.Received (2).Process(Arg.Any<int>());

//		lockstepLogic.Recei
	}

	[Test]
	public void TestCurrentLockstepFrame(){

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.MaxAllowedFrameTime = 100.0f;
		lockstepGameLogic.GameFramesPerLockstep = 5;

		lockstepLogic.IsReady (Arg.Any<int> ()).Returns (true);

		lockstepGameLogic.Update (0.1f);

		Assert.That (lockstepGameLogic.CurrentLockstepFrame, Is.EqualTo (0));

		lockstepGameLogic.Update (0.5f);

		Assert.That (lockstepGameLogic.CurrentLockstepFrame, Is.EqualTo (1));

		lockstepGameLogic.Update (0.5f);

		Assert.That (lockstepGameLogic.CurrentLockstepFrame, Is.EqualTo (2));
	}

	[Test]
	public void TestProcessLockstepInProperFrame(){

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.MaxAllowedFrameTime = 100.0f;
		lockstepGameLogic.GameFramesPerLockstep = 5;

		lockstepLogic.IsReady (Arg.Any<int> ()).Returns (true);

		lockstepGameLogic.Update (0.5f);

		Assert.That (lockstepGameLogic.CurrentGameFrame, Is.EqualTo (5));
		Assert.That (lockstepGameLogic.CurrentLockstepFrame, Is.EqualTo (1));


		lockstepGameLogic.Update (0.5f);

		Assert.That (lockstepGameLogic.CurrentGameFrame, Is.EqualTo (10));
		Assert.That (lockstepGameLogic.CurrentLockstepFrame, Is.EqualTo (2));
	}

	[Test]
	public void TestIsReadyShouldCheckForProperGameFrame(){

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.MaxAllowedFrameTime = 100.0f;
		lockstepGameLogic.GameFramesPerLockstep = 5;

		lockstepLogic.IsReady (Arg.Any<int> ()).ReturnsForAnyArgs(true);

		lockstepGameLogic.Update (0.5f);

		// changed to 4 now that frame is zero indexed 

		lockstepLogic.Received (1).IsReady(Arg.Is<int>(4));
		lockstepLogic.Received (1).Process(Arg.Is<int>(4));
	}

	[Test]
	public void TestLastUpdatedFrameShouldBePreviousToLockstep(){

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.MaxAllowedFrameTime = 1.0f;
		lockstepGameLogic.GameFramesPerLockstep = 4;

		lockstepLogic.IsReady (Arg.Any<int> ()).ReturnsForAnyArgs(false);

		lockstepGameLogic.Update (0.8f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (lockstepGameLogic.GetFirstLockstepFrame() - 1));
	}
		
}

