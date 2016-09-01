using System.Collections.Generic;

public class CommandsList
{
	readonly List<Command> _commands = new List<Command>();

	public List<Command> Commands {
		get {
			return _commands;
		}
	}

	public void AddCommand(Command command)
	{
		_commands.Add (command);
	}

	public void GetCommands(int frame, List<Command> commands)
	{
		for (int i = 0; i < _commands.Count; i++) {
			if (_commands [i].ProcessFrame == frame)
				commands.Add (_commands [i]);
		}
	}

	public void RemoveCommands(List<Command> commands)
	{
		for (int i = 0; i < commands.Count; i++) {
			var command = commands [i];
			_commands.Remove (command);
		}
	}
}

public class CommandsListLockstepLogic : LockstepLogic 
{
	readonly CommandsList _commandsList;

	public CommandsListLockstepLogic(CommandsList commandsList)
	{
		_commandsList = commandsList;
	}

	#region LockstepLogic implementation

	public bool IsReady (int frame)
	{
		frameCommands.Clear ();
		_commandsList.GetCommands (frame, frameCommands);

		return frameCommands.Count > 0;
	}

	readonly List<Command> frameCommands = new List<Command>();

	public void Process (int frame)
	{
		frameCommands.Clear ();
		_commandsList.GetCommands (frame, frameCommands);

		for (int i = 0; i < frameCommands.Count; i++) {
			var command = frameCommands [i];
			command.Process ();
		}

		_commandsList.RemoveCommands (frameCommands);

		frameCommands.Clear ();
	}
	#endregion
}