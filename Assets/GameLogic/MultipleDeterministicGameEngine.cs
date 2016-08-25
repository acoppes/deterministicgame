public class MultipleDeterministicGameEngine : DeterministicGameLogic
{
	readonly DeterministicGameLogic[] gameEngines;

	public MultipleDeterministicGameEngine(DeterministicGameLogic[] gameEngines)
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
