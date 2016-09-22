using System.Collections.Generic;

namespace Gemserk.Lockstep 
{		
	public class ReplayPlayer
	{
		readonly Replay _replay;

		readonly Commands _commands;

		public ReplayPlayer(Replay replay, Commands commands)
		{
			_replay = replay;
			_commands = commands;
		}

		public void Replay (int frame)
		{
			List<Command> recordedCommands = new List<Command> ();

			_replay.GetStoredCommands(frame, recordedCommands);

			for (int i = 0; i < recordedCommands.Count; i++) {
				var command = recordedCommands [i];
				_commands.AddCommand (command);
			}
		}
	}
	
}