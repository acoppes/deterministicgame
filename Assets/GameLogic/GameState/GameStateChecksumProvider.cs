namespace Gemserk.Lockstep 
{
	public class GameStateChecksumProvider : ChecksumProvider
	{
		readonly GameStateBuilder _gameStateBuilder;

		readonly GameStateProvider _gameStateProvider;

		public GameStateChecksumProvider(GameStateBuilder gameStateBuilder, GameStateProvider rootProvider)
		{
			_gameStateBuilder = gameStateBuilder;
			_gameStateProvider = rootProvider;
		}

		#region ChecksumProvider implementation

		public Checksum CalculateChecksum ()
		{
			_gameStateBuilder.Reset ();
			_gameStateProvider.SaveState (_gameStateBuilder);
			return _gameStateBuilder.CalculateChecksum ();
		}

		#endregion

	}

}