namespace Gemserk.Lockstep 
{
	/// <summary>
	/// Represents a specific state of the game, to be implemented custom in each game.
	/// </summary>
	public interface GameState : ChecksumProvider
	{
		
	}

//	/// <summary>
//	/// Basic implementation of GameState where the state is stored in a string. 
//	/// </summary>
//	public class GameStateStringImpl : GameState
//	{
//		public string State {
//			get;
//			set;
//		}
//	}

	/// <summary>
	/// Provides a clean instance of a GameState.
	/// </summary>
	public interface GameStateProvider
	{
		GameState GetGameState();
	}
}