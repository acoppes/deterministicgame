using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	/// <summary>
	/// This class responsibility is to enqueue commands and send them to process at the end of this 
	/// current lockstep frame, to be processed in next lockstep frame. In case no commands were 
	/// enqueued, it sends an empty command to keep the game lockstep working.
	/// </summary>
	public class CommandSender
	{
		readonly LockstepUpdate _lockstepUpdate;
		readonly Commands _commands;
		readonly Command _emptyCommand;

		readonly List<Command> _queuedCommands = new List<Command>();

		public CommandSender(LockstepUpdate lockstepUpdate, Commands commands, Command emptyCommand)
		{
			_lockstepUpdate = lockstepUpdate;
			_commands = commands;
			_emptyCommand = emptyCommand;
		}

		public void EnqueueCommand(Command command)
		{
			_queuedCommands.Add (command);
		}

		public void SendCommands()
		{
			if (_lockstepUpdate.IsLastFrameForNextLockstep ()) {

				if (_queuedCommands.Count == 0) {
					_emptyCommand.CreationFrame = _lockstepUpdate.GetCurrentFrame ();
					_emptyCommand.ProcessFrame = _lockstepUpdate.GetNextLockstepFrame ();
					_commands.AddCommand (_emptyCommand);
				} else {
					for (int i = 0; i < _queuedCommands.Count; i++) {
						var command = _queuedCommands [i];
						_commands.AddCommand (command);
					}
					_queuedCommands.Clear ();
				}
			}
		}
	}
}