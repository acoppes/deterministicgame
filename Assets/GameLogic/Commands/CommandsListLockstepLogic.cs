using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public class CommandsListLockstepLogic : LockstepLogic 
	{
		readonly CommandsList _commandsList;

		readonly CommandProcessor _commandProcessor;

		readonly List<Command> frameCommands = new List<Command>();

		public CommandsListLockstepLogic(CommandsList commandsList, CommandProcessor commandProcessor)
		{
			_commandsList = commandsList;
			_commandProcessor = commandProcessor;
		}

		#region LockstepLogic implementation

		public bool IsReady (int frame)
		{
			frameCommands.Clear ();
			_commandsList.GetCommands (frame, frameCommands);
	
			bool isReady = frameCommands.Count > 0;
			frameCommands.Clear ();

			return isReady;
		}

		public void Process (int frame)
		{
			frameCommands.Clear ();
			_commandsList.GetCommands (frame, frameCommands);

			for (int i = 0; i < frameCommands.Count; i++) {
				var command = frameCommands [i];
				_commandProcessor.Process (command, frame);
			}

			_commandsList.RemoveCommands (frameCommands);

			frameCommands.Clear ();
		}

		#endregion
	}
}