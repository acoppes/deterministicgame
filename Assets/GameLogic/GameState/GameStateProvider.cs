namespace Gemserk.Lockstep 
{
	public interface GameStateProvider
	{
		void SaveState(GameStateBuilder gameState);
	}
}