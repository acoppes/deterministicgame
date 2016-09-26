namespace Gemserk.Lockstep 
{
	public class GameStateChecksumProvider : ChecksumProvider
	{
		readonly GameStateProvider _gameStateProvider;

		readonly GameStateCollaborator _rootGameStateCollaborator;

		public GameStateChecksumProvider(GameStateProvider gameStateProvider, GameStateCollaborator rootProvider)
		{
			_gameStateProvider = gameStateProvider;
			_rootGameStateCollaborator = rootProvider;
		}

		#region ChecksumProvider implementation

		public Checksum CalculateChecksum ()
		{
			var gameState = _gameStateProvider.GetGameState ();
			_rootGameStateCollaborator.SaveState (gameState);
			return gameState.CalculateChecksum ();
		}

		#endregion

	}

}