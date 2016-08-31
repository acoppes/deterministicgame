
public interface GameStateProvider
{
	string GetGameState();
}

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
		return new ChecksumString(ChecksumHelper.CalculateMD5(_gameStateProvider.GetGameState()));
	}

	#endregion

}