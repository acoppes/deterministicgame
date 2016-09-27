namespace Gemserk.Lockstep 
{
	public class GameStateChecksumProvider : ChecksumProvider
	{
		readonly GameStateProvider _gameStateProvider;

		public GameStateChecksumProvider(GameStateProvider gameStateProvider)
		{
			_gameStateProvider = gameStateProvider;
		}

		#region ChecksumProvider implementation

		public Checksum CalculateChecksum ()
		{
			var gameState = _gameStateProvider.GetGameState ();
			return gameState.CalculateChecksum ();
		}

		#endregion

	}

}