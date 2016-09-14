using NUnit.Framework;
using Gemserk.Lockstep;

public class TestObject : GameStateProvider
{
	#region GameStateProvider implementation
	public void Provide (GameState gameState)
	{
		gameState.SetInt (10);
		gameState.SetInt (5);
		gameState.SetInt (20);
	}
	#endregion
}

public class TestGameStateProvider {

	[Test]
	public void TestGameStateProvider1(){

		GameStateStringBuilderImpl gameState = new GameStateStringBuilderImpl ();
		TestObject testObject = new TestObject ();

		gameState.Reset ();
		testObject.Provide (gameState);

		string state = gameState.State;
		Assert.That (state, Is.EqualTo ("10520"));
	}

}
