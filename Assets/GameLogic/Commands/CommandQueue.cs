using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public interface CommandQueue 
	{
		void EnqueueCommand(Command command);

		bool IsReady();

		void SendCommands();
	}

	public interface CommandSender
	{
		void SendEmpty();

		void SendCommands (List<Command> commands);
	}

	/// <summary>
	/// This class responsibility is to enqueue commands and send them to process at the end of this 
	/// current lockstep frame, to be processed in next lockstep frame. In case no commands were 
	/// enqueued, it sends an empty command to keep the game lockstep working.
	/// </summary>
	public class CommandQueueBase : CommandQueue
	{
		readonly LockstepUpdate _lockstepUpdate;
		readonly CommandSender _commandSender;

		readonly List<Command> _queuedCommands = new List<Command>();

		public CommandQueueBase(LockstepUpdate lockstepUpdate, CommandSender commandSender)
		{
			_lockstepUpdate = lockstepUpdate;
			_commandSender = commandSender;
		}

		public bool IsReady()
		{
			return _lockstepUpdate.IsLastFrameForNextLockstep ();
		}

		public void EnqueueCommand(Command command)
		{
			_queuedCommands.Add (command);
		}

		public void SendCommands()
		{
			if (_queuedCommands.Count == 0) {
				_commandSender.SendEmpty ();
			} else {
				_commandSender.SendCommands (_queuedCommands);
				_queuedCommands.Clear ();
			}
		}
	}
}