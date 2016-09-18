namespace Gemserk.Lockstep 
{
	public class GameStateChecksumProvider : ChecksumProvider
	{
		readonly GameStateBuilder _gameState;

		readonly GameStateProvider _gameStateProvider;

		public GameStateChecksumProvider(GameStateBuilder gameState, GameStateProvider rootProvider)
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