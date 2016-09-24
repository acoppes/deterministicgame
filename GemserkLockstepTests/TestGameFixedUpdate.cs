using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;

namespace Gemserk.Lockstep.Tests
{
	public class TestGameFixedUpdate
	{
		[Test]
		public void TestFramesStartsZeroIndexed(){

			var gameLogic = new TestGameStep.GameStepEngineMock ();

			GameFixedUpdate gameUpdate = new GameFixedUpdate ();
			gameUpdate.FixedStepTime = 0.1f;
			gameUpdate.SetGameLogic (gameLogic);

			gameUpdate.Update (0.1f);

			Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

			gameUpdate.Update (0.1f);
		
			Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
		}

		[Test]
		public void TestDoubleTimeIncreasesTwoFixedUpdates(){

			var gameLogic = new TestGameStep.GameStepEngineMock ();

			GameFixedUpdate gameUpdate = new GameFixedUpdate ();
			gameUpdate.FixedStepTime = 0.1f;
			gameUpdate.SetGameLogic (gameLogic);

			gameUpdate.Update (0.1f * 2);

			Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
			Assert.That (gameLogic.lastDt, Is.EqualTo(0.1f).Within(0.001f));
		}

		[Test]
		public void TestNoFixedUpdateIfNotEnoughTime(){

			var gameLogic = new TestGameStep.GameStepEngineMock ();

			GameFixedUpdate gameUpdate = new GameFixedUpdate ();
			gameUpdate.FixedStepTime = 0.1f;
			gameUpdate.SetGameLogic (gameLogic);

			gameUpdate.Update (0.09f);

			Assert.That (gameLogic.UpdateTimes, Is.EqualTo(0));
		}


	}

}