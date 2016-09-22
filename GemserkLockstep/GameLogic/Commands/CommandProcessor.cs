namespace Gemserk.Lockstep 
{
	public interface CommandProcessor
	{
		// check if there are commands to process
		bool CheckReady (Commands commands, int frame);

		// process each individual command
		void Process(Command command, int frame);
	}
}