using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public class CommandsList : Commands
	{
		readonly List<Command> _commands = new List<Command>();

		readonly List<Command> _tmpRemoveCommands = new List<Command>();

		public List<Command> Commands {
			get {
				return _commands;
			}
		}

		public void AddCommand(Command command)
		{
			_commands.Add (command);
		}

		public bool HasCommands(int frame)
		{
			for (int i = 0; i < _commands.Count; i++) {
				if (_commands [i].ProcessFrame == frame)
					return true;
			}
			return false;
		}

		public void GetCommands(List<Command> commands)
		{
			for (int i = 0; i < _commands.Count; i++)
				commands.Add (_commands [i]);
		}

		public void GetCommands(int frame, List<Command> commands)
		{
			for (int i = 0; i < _commands.Count; i++) {
				if (_commands [i].ProcessFrame == frame)
					commands.Add (_commands [i]);
			}
		}

		public void RemoveCommands(int frame)
		{
			// collect commands to be removed

			for (int i = 0; i < _commands.Count; i++) {
				var command = _commands [i];
				if (command.ProcessFrame == frame)
					_tmpRemoveCommands.Add (command);
			}

			for (int i = 0; i < _tmpRemoveCommands.Count; i++) {
				var removedCommand = _tmpRemoveCommands [i];
				_commands.Remove (removedCommand);
			}

			_tmpRemoveCommands.Clear ();
		}
	}

}