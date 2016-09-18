namespace Gemserk.Lockstep 
{
	public interface GameStateProvider
	{
		// save ?
		void Provide(GameState gameState);
	}
}