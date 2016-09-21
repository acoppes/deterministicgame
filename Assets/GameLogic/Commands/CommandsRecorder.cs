using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public class CommandsRecorder
	{
		public class RecordedCommand
		{
			public Command command;
			public int gameFrame;
		}

		readonly List<RecordedCommand> recordedCommandsQueue = new List<RecordedCommand>();

		public void AddCommand(int gameFrame, Command command)
		{
			recordedCommandsQueue.Add (new RecordedCommand () { 
				command = command,
				gameFrame = gameFrame
			});
		}

		public void GetCommandsForFrame(int frame, List<Command> commands)
		{
			for (int i = 0; i < recordedCommandsQueue.Count; i++) {
				var recordedCommand = recordedCommandsQueue [i];
				if (recordedCommand.gameFrame == frame)
					commands.Add (recordedCommand.command);
			}
		}
			
	}
}
