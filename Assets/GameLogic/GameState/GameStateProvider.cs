using System.Text;

namespace Gemserk.Lockstep 
{
	public interface GameState : ChecksumProvider
	{
		void SetInt(int i);

		void SetFloat(float f);

		void SetBool(bool b);

		void Reset();
	}

	public interface GameStateProvider
	{
		void Provide(GameState gameState);
	}

	public class GameStateChecksumProvider : ChecksumProvider
	{
		readonly GameState _gameState;

		GameStateProvider _gameStateProvider;

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

	public class GameStateStringBuilderImpl : GameState
	{
		StringBuilder state = new StringBuilder();

		#region ChecksumProvider implementation
		public Checksum CalculateChecksum ()
		{
			return new ChecksumString(ChecksumHelper.CalculateMD5(state.ToString()));
		}
		#endregion

		public void SetInt (int i)
		{
			state.Append (i);
		}

		public void SetFloat (float f)
		{
			state.Append (f);
		}

		public void SetBool (bool b)
		{
			state.Append (b);
		}

		public void Reset ()
		{
			state = new StringBuilder ();
		}
	}
}