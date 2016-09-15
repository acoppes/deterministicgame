using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
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

}