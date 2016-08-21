using NUnit.Framework;

public class TestGameStep {

	[Test]
	public void TestFixedGameStepTwoUpdates()
	{
		GameStepCode gameStepCode = new GameStepCode ();

		gameStepCode.FixedTimeStepMilliseconds = 50;

		gameStepCode.Init ();

		gameStepCode.Update (10);

		Assert.That (gameStepCode.CurrentUpdate, Is.EqualTo (0));

		gameStepCode.Update (40);

		Assert.That (gameStepCode.CurrentUpdate, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepGreaterThanFixedTime()
	{
		GameStepCode gameStepCode = new GameStepCode ();

		gameStepCode.FixedTimeStepMilliseconds = 50;

		gameStepCode.Init ();

		gameStepCode.Update (51);

		Assert.That (gameStepCode.CurrentUpdate, Is.EqualTo (1));
	}

}
