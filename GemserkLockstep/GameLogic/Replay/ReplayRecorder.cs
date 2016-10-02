using System.Collections.Generic;

namespace Gemserk.Lockstep.Replays
{		
	public class ReplayRecorder
	{
		readonly Replay _replay;

		readonly Commands _commands;

		public ReplayRecorder(Replay replay, Commands commands)
		{
			_replay = replay;
			_commands = commands;
		}
			
		readonly List<Command> _commandsToRecord = new List<Command>();

		void RecordCommands(int frame)
		{
			_commandsToRecord.Clear ();

			_commands.GetCommands (_commandsToRecord);

			for (int i = 0; i < _commandsToRecord.Count; i++) {
				var command = _commandsToRecord [i];
				_replay.Record (frame, command);
			}

			_commandsToRecord.Clear ();
		}
			
		public void Record (int frame, bool isChecksumFrame)
		{
			RecordCommands (frame);

			if (isChecksumFrame)
				_replay.RecordChecksum (frame);
		}
	}
}