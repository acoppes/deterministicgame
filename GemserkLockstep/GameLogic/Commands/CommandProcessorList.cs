using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	// just a list of processors, if one not ready then none is ready.

	public class CommandProcessorList : CommandProcessor
	{
		readonly List<CommandProcessor> _commandProcessors;

		public CommandProcessorList(List<CommandProcessor> commandProcessors)
		{
			_commandProcessors = commandProcessors;
		}

		#region CommandProcessor implementation

		public bool CheckReady (Commands commands, int frame)
		{
			for (int i = 0; i < _commandProcessors.Count; i++) {
				var commandProcessor = _commandProcessors [i];
				if (!commandProcessor.CheckReady (commands, frame))
					return false;
			}

			return true;
		}

		public void Process (Command command, int frame)
		{
			for (int i = 0; i < _commandProcessors.Count; i++) {
				var commandProcessor = _commandProcessors [i];
				commandProcessor.Process (command, frame);
			}
		}

		#endregion
	}

}