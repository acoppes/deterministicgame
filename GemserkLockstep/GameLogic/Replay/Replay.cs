using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
//	public struct ReplayGameState
//	{
//		public float fixedDeltaTime;
//		public int updatesPerLockstep;
//	}

	public interface Replay
	{
		// GameState GetInitialGameStateState();

		// For replay playback

		int LastRecordedFrame { get; set; } 

		List<StoredChecksum> StoredChecksums { get; }

		void GetStoredCommands (int frame, List<Command> commands);

		// For replay recording 

		void RecordChecksum(int frame);

		void Record (int frame, Command command);

		// 

//		GameState GetInitialGameState();
//
//		void SaveInitialGameState(GameState gameState);
	}
}