
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
			_gameStateProvider.Provide (_gameState);

			return _gameState.CalculateChecksum ();
//			return new ChecksumString(ChecksumHelper.CalculateMD5(_gameStateProvider.GetGameState()));
		}

		#endregion

	}

}