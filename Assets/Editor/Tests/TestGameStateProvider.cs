using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using System.Text;

public class TestObject : GameStateProvider
{
	#region GameStateProvider implementation
	public void Provide (GameState gameState)
	{
		gameState.SetInt (10);
	}
	#endregion
	
}

public class TestGameStateProvider {

	[Test]
	public void TestGameStateProvider1(){

		GameState gameState = new GameStateStringBuilderImpl ();
		TestObject testObject = new TestObject ();

		testObject.Provide (gameState);
	}

}
