using System.Collections.Generic;

namespace Gemserk.Lockstep 
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

		void RecordCommands(float time, int frame)
		{
			_commandsToRecord.Clear ();

			_commands.GetCommands (_commandsToRecord);

			for (int i = 0; i < _commandsToRecord.Count; i++) {
				var command = _commandsToRecord [i];
				_replay.Record (time, frame, command);
			}

			_commandsToRecord.Clear ();
		}
			
		public void Record (float time, int frame, bool isChecksumFrame)
		{
			RecordCommands (time, frame);

			if (isChecksumFrame)
				_replay.RecordChecksum (frame);
		}
	}
}