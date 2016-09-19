namespace Gemserk.Lockstep 
{
	/// <summary>
	/// Represents a specific state of the game.
	/// </summary>
	public interface GameState
	{
		
	}

	/// <summary>
	/// Basic implementation of GameState where the state is stored in a string. 
	/// </summary>
	public class GameStateStringImpl : GameState
	{
		public string State {
			get;
			set;
		}
	}
}