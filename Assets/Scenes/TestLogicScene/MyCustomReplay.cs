using System.Collections.Generic;
using Gemserk.Lockstep;

public class MyCustomReplay : ReplayBase
{
	public class StoredGameState
	{
		public int frame;
		public GameState gameState;
	}

	List<StoredGameState> storedGameStates = new List<StoredGameState> ();

	GameStateProvider _gameStateProvider;

	public MyCustomReplay(ChecksumProvider checksumProvider, GameStateProvider gameStateProvider) : base(checksumProvider)
	{
		_gameStateProvider = gameStateProvider;
	}

	public override void RecordChecksum (int frame)
	{
		base.RecordChecksum (frame);
		SaveGameState (frame, _gameStateProvider.GetGameState());
	}

	void SaveGameState(int frame, GameState gameState)
	{
		storedGameStates.Add (new StoredGameState () { 
			frame = frame,
			gameState = gameState
		});
	}

	public GameState GetStoredGameState(int frame)
	{
		for (int i = 0; i < storedGameStates.Count; i++) {
			var storedGameState = storedGameStates [i];
			if (storedGameState.frame == frame)
				return storedGameState.gameState;
		}
		return null;
	}
}
