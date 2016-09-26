namespace Gemserk.Lockstep 
{
	/// <summary>
	/// Implemented by objects that want to collaborate with the GameState.
	/// </summary>
	public interface GameStateCollaborator
	{
		void SaveState(GameState gameState);
	}
}