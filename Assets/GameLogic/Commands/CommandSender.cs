using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public interface CommandSender 
	{
		void EnqueueCommand(Command command);

		bool IsReady();

		void SendCommands();
	}

	public interface CommandEmptyProvider
	{
		Command GetEmptyCommand();
	}

	public interface CommandSenderProcessor
	{
		void SendEmpty();

		void SendCommands (List<Command> commands);
	}

	/// <summary>
	/// This class responsibility is to enqueue commands and send them to process at the end of this 
	/// current lockstep frame, to be processed in next lockstep frame. In case no commands were 
	/// enqueued, it sends an empty command to keep the game lockstep working.
	/// </summary>
	public class CommandSenderBase : CommandSender
	{
		readonly LockstepUpdate _lockstepUpdate;
		readonly CommandSenderProcessor _commandProcessor;

		readonly List<Command> _queuedCommands = new List<Command>();

		public CommandSenderBase(LockstepUpdate lockstepUpdate, CommandSenderProcessor commandProcessor)
		{
			_lockstepUpdate = lockstepUpdate;
			_commandProcessor = commandProcessor;
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
				_commandProcessor.SendEmpty ();
			} else {
				_commandProcessor.SendCommands (_queuedCommands);
				_queuedCommands.Clear ();
			}
		}
	}
}