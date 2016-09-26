using NUnit.Framework;
using Gemserk.Lockstep;

public class TestObject : GameStateCollaborator
{
	#region GameStateProvider implementation
	public void SaveState (GameState gameState)
	{
		var myCustomGameState = gameState as GameStateStringBuilderImpl;

		myCustomGameState.StartObject ("TestObject");
		myCustomGameState.SetInt ("x", 10);
		myCustomGameState.SetInt ("y", 5);
		myCustomGameState.SetInt ("z", 20);

		myCustomGameState.EndObject ();

	}
	#endregion
}

public class TestGameStateProvider {

	[Test]
	public void TestGameStateProvider1(){

		TestObject testObject = new TestObject ();

		var gameState = new GameStateStringBuilderImpl ();

		testObject.SaveState (gameState);
		testObject.SaveState (gameState);

		Assert.That (gameState.State, Is.EqualTo ("TestObject:(x:10,y:5,z:20),TestObject:(x:10,y:5,z:20)"));
	}

}
