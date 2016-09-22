using NUnit.Framework;
using Gemserk.Lockstep;

public class TestObject : GameStateProvider
{
	#region GameStateProvider implementation
	public void SaveState (GameStateBuilder gameState)
	{
		gameState.StartObject ("TestObject");
		gameState.SetInt ("x", 10);
		gameState.SetInt ("y", 5);
		gameState.SetInt ("z", 20);
		gameState.EndObject ();
	}
	#endregion
}

public class TestGameStateProvider {

	[Test]
	public void TestGameStateProvider1(){

		GameStateStringBuilderImpl gameStateBuilder = new GameStateStringBuilderImpl ();
		TestObject testObject = new TestObject ();

		gameStateBuilder.Reset ();
		testObject.SaveState (gameStateBuilder);
		testObject.SaveState (gameStateBuilder);

		GameStateStringImpl gameState = gameStateBuilder.GetGameState() as GameStateStringImpl;

		Assert.That (gameState.State, Is.EqualTo ("TestObject:(x:10,y:5,z:20),TestObject:(x:10,y:5,z:20)"));
	}

}
