public class MultipleDeterministicGameEngine : GameLogic
{
	readonly GameLogic[] gameEngines;

	public MultipleDeterministicGameEngine(GameLogic[] gameEngines)
	{
		this.gameEngines = gameEngines;
	}

	#region DeterministicGameEngine implementation
	public void Update (float dt, int frame)
	{
		for (int i = 0; i < gameEngines.Length; i++) {
			var gameEngine = gameEngines [i];
			gameEngine.Update (dt, frame);
		}
	}
	#endregion
}
