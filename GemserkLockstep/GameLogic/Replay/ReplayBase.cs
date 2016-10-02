using System.Collections.Generic;

namespace Gemserk.Lockstep.Replays
{		
	public class ReplayBase : Replay
	{
		readonly CommandsRecorder _commandsRecorder;

		readonly ChecksumRecorder _checksumRecorder;

		GameState _initialGameState;

		public int LastRecordedFrame {
			get;
			set;
		}

		public ReplayBase(ChecksumProvider checksumProvider)
		{
			_commandsRecorder = new CommandsRecorder();
			_checksumRecorder = new ChecksumRecorder(checksumProvider);
		}

		#region Replay implementation

		public List<StoredChecksum> StoredChecksums {
			get {
				return _checksumRecorder.StoredChecksums;
			}
		}

		public void GetStoredCommands (int frame, List<Command> commands)
		{
			_commandsRecorder.GetCommandsForFrame (frame, commands);
		}

		public virtual void RecordChecksum (int frame)
		{
			_checksumRecorder.RecordState (frame);
		}

		public void Record (int frame, Command command)
		{
			_commandsRecorder.AddCommand (frame, command);
			LastRecordedFrame = frame;
		}

		public GameState GetInitialGameState()
		{
			return _initialGameState;
		}

		public void SaveInitialGameState(GameState gameState)
		{
			_initialGameState = gameState;
		}

		#endregion
	}
	
}