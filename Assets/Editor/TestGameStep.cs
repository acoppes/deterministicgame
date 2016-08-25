using NUnit.Framework;
using System;

public class TestGameStep {

	public class GameStepEngineMock : DeterministicGameLogic
	{
		public float lastDt;
		public int lastFrame;

		#region GameStepEngine implementation
		public void Update (float dt, int frame)
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

		gameFixedUpdate.FixedStepTime = 0.05f;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (0.01f);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (0));

		gameFixedUpdate.Update (0.045f);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepGreaterThanFixedTime()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();

		gameFixedUpdate.FixedStepTime = 0.05f;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (0.051f);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepEngineCalled()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();
		GameStepEngineMock gameStepEngine = new GameStepEngineMock ();

		gameFixedUpdate.SetGameLogic (gameStepEngine);

		gameFixedUpdate.FixedStepTime = 0.05f;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (0.078f);

		Assert.That (gameStepEngine.lastDt, Is.EqualTo (0.05f));
		Assert.That (gameStepEngine.lastFrame, Is.EqualTo (0));
	}

	[Test]
	public void TestLockstepImplementation()
	{
		IntervalDeterministicGameLogic lockstepImplementation = new IntervalDeterministicGameLogic ();

		lockstepImplementation.GameFramesPerLockstepFrame = 2;

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		lockstepImplementation.Update (0.01f, 0);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		try {
			lockstepImplementation.Update (0.01f, 2);
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

		lockstepImplementation.Update (0.01f, 0);
		lockstepImplementation.Update (0.01f, 1);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (1));

		lockstepImplementation.Update (0.01f, 2);
		lockstepImplementation.Update (0.01f, 3);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (2));
	}

	const float distancePrecision = 0.001f;

	[Test]
	public void TestUnitViewGetPosition()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, 0));

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0), new UnityEngine.Vector2 (0, 0)) < distancePrecision);
	}

	[Test]
	public void TestUnitViewGetPosition2()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, 0));

		UnityEngine.Vector2 position = unitView.GetCurrentPosition (1);

		Assert.That (UnityEngine.Vector2.Distance (position, new UnityEngine.Vector2 (100, 0)) < distancePrecision);
	}

	[Test]
	public void TestUnitViewGetPosition3()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, 0));

		UnityEngine.Vector2 position = unitView.GetCurrentPosition (0.5f);

		Assert.That (UnityEngine.Vector2.Distance (position, new UnityEngine.Vector2 (50, 0)) < distancePrecision);
	}

	[Test]
	public void TestUnitViewGetPositionIncrementalDt()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, -100));

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (10, -10)) < distancePrecision);
		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (20, -20)) < distancePrecision);	
	}

	[Test]
	public void TestUnitViewGetPositionIncrementalDt2()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, -100));

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (10, -10)) < distancePrecision);
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (410, -410));
		Assert.AreEqual (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (50, -50));
//		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (40, -40)) < distancePrecision);	
	}

}
