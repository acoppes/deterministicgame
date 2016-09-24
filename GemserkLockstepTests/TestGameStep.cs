using NUnit.Framework;
using Gemserk.Lockstep;

public class TestGameStep {

	public class GameStepEngineMock : GameLogic
	{
		public float lastDt;
		public int lastFrame = -1;

		public int UpdateTimes {
			get { 
				return lastFrame + 1;
			}
		}

		#region GameStepEngine implementation
		public void GameUpdate (float dt, int frame)
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
		
}
