using NUnit.Framework;
using System;

public class TestGameStep {

	public class GameStepEngineMock : DeterministicGameLogic
	{
		public int lastDt;
		public int lastFrame;

		#region GameStepEngine implementation
		public void Update (int dt, int frame)
		{
			lastDt = dt;
			lastFrame = frame;
		}
		#endregion
	}

	[Test]
	public void TestFixedGameStepTwoUpdates()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();

		gameFixedUpdate.FixedTimeStepMilliseconds = 50;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (10);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (0));

		gameFixedUpdate.Update (40);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepGreaterThanFixedTime()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();

		gameFixedUpdate.FixedTimeStepMilliseconds = 50;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (51);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepEngineCalled()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();
		GameStepEngineMock gameStepEngine = new GameStepEngineMock ();

		gameFixedUpdate.SetGameLogic (gameStepEngine);

		gameFixedUpdate.FixedTimeStepMilliseconds = 50;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (78);

		Assert.That (gameStepEngine.lastDt, Is.EqualTo (50));
		Assert.That (gameStepEngine.lastFrame, Is.EqualTo (0));
	}

	[Test]
	public void TestLockstepImplementation()
	{
		IntervalDeterministicGameLogic lockstepImplementation = new IntervalDeterministicGameLogic ();

		lockstepImplementation.GameFramesPerLockstepFrame = 2;

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		lockstepImplementation.Update (10, 0);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		try {
		lockstepImplementation.Update (10, 2);
		} catch {
			Assert.Pass ();
			return;
		}

		Assert.Fail ();

		// Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestLockstepImplementation2()
	{
		IntervalDeterministicGameLogic lockstepImplementation = new IntervalDeterministicGameLogic ();

		lockstepImplementation.GameFramesPerLockstepFrame = 2;

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		lockstepImplementation.Update (10, 0);
		lockstepImplementation.Update (10, 1);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (1));

		lockstepImplementation.Update (10, 2);
		lockstepImplementation.Update (10, 3);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (2));
	}

}
