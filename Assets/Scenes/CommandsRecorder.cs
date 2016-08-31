using System.Collections.Generic;

public class CommandsRecorder
{
	public class RecordedCommand
	{
		public Command command;
		public float gameTime;
		public int gameFrame;
	}

	readonly List<RecordedCommand> recordedCommandsQueue = new List<RecordedCommand>();

//	public int lastGameFrame;

	public void AddCommand(float gameTime, int gameFrame, Command command)
	{
		recordedCommandsQueue.Add (new RecordedCommand () { 
			command = command,
			gameTime = gameTime,
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

	public void Reset()
	{
		recordedCommandsQueue.Clear ();
	}
}
