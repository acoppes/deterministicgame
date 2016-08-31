using System.Collections.Generic;

public class CommandsList
{
	readonly List<Command> commands = new List<Command>();

	public bool IsReady {
		get;
		set;
	}

	public List<Command> Commands {
		get {
			return commands;
		}
	}

	public void AddCommand(Command command)
	{
		commands.Add (command);
	}

	public void Process()
	{
		for (int i = 0; i < commands.Count; i++) {
			var command = commands [i];
			command.Process ();
		}

		commands.Clear ();

		IsReady = false;
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
	public bool IsReady ()
	{
		return _commandsList.IsReady;
	}
	public void Process ()
	{
		_commandsList.Process ();
	}
	#endregion
	
}