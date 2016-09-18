using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public interface Commands 
	{
		void AddCommand(Command command);

		bool HasCommands(int frame);

		void GetCommands(List<Command> commands);

		void GetCommands(int frame, List<Command> commands);

		void RemoveCommands(int frame);
	}

}