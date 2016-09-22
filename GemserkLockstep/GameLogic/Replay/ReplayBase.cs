using System.Collections.Generic;

namespace Gemserk.Lockstep 
{		
	public class ReplayBase : Replay
	{
		readonly CommandsRecorder _commandsRecorder;

		readonly ChecksumRecorder _checksumRecorder;

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

		public void RecordChecksum (int frame)
		{
			_checksumRecorder.RecordState (frame);
		}

		public void Record (int frame, Command command)
		{
			_commandsRecorder.AddCommand (frame, command);
			LastRecordedFrame = frame;
		}

		#endregion
	}
	
}