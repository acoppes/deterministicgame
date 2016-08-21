using System.Collections.Generic;

public class CommandsList
{
	readonly List<Command> commands = new List<Command>();

	public bool IsReady {
		get;
		set;
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
