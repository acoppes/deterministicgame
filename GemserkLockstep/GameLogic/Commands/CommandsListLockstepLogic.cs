using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public class CommandsListLockstepLogic : LockstepLogic 
	{
		readonly Commands _commands;

		readonly CommandProcessor _commandProcessor;

		readonly List<Command> frameCommands = new List<Command>();

		public CommandsListLockstepLogic(Commands commands, CommandProcessor commandProcessor)
		{
			_commands = commands;
			_commandProcessor = commandProcessor;
		}

		#region LockstepLogic implementation

		public bool IsReady (int frame)
		{
			if (!_commands.HasCommands (frame))
				return false;
			return _commandProcessor.CheckReady(_commands, frame);
		}

		public void Process (int frame)
		{
			frameCommands.Clear ();

			_commands.GetCommands (frame, frameCommands);

			for (int i = 0; i < frameCommands.Count; i++) {
				var command = frameCommands [i];
				_commandProcessor.Process (command, frame);
			}

			frameCommands.Clear ();

			_commands.RemoveCommands (frame);
		}

		#endregion
	}
}