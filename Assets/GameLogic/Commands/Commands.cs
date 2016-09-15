using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public interface Commands 
	{
		void AddCommand(Command command);

		void GetCommands(List<Command> commands);

		void GetCommands(int frame, List<Command> commands);

		void RemoveCommands(int frame);
	}
}