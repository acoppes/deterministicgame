namespace Gemserk.Lockstep 
{
	/// <summary>
	/// Implemented by objects that want to collaborate in the state of the game.
	/// </summary>
	public interface GameStateProvider
	{
		void SaveState(GameStateBuilder gameState);
	}
}