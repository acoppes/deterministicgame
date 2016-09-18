namespace Gemserk.Lockstep 
{
	public class GameStateChecksumProvider : ChecksumProvider
	{
		readonly GameState _gameState;

		readonly GameStateProvider _gameStateProvider;

		public GameStateChecksumProvider(GameState gameState, GameStateProvider rootProvider)
		{
			_gameState = gameState;
			_gameStateProvider = rootProvider;
		}

		#region ChecksumProvider implementation

		public Checksum CalculateChecksum ()
		{
			_gameState.Reset ();
			_gameStateProvider.SaveState (_gameState);

			return _gameState.CalculateChecksum ();
		}

		#endregion

	}

}