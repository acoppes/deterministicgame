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

	/// <summary>
	/// Provides a way to build a game state for saving it or checking synchronization across clients.
	/// </summary>
	public interface GameStateBuilder : ChecksumProvider
	{
		GameState GetGameState();

		void StartObject(string name);

		void EndObject();

		void SetInt(string name, int i);

		void SetFloat(string name, float f);

		void SetBool(string name, bool b);

		void Reset();
	}
}