namespace Gemserk.Lockstep 
{
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