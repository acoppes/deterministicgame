namespace Gemserk.Lockstep 
{
	public interface Command
	{
		int CreationFrame 
		{
			get; set;
		}

		int ProcessFrame 
		{
			get; set;
		}
	}

	public class CommandBase : Command
	{
		public int CreationFrame 
		{
			get; set;
		}

		public int ProcessFrame 
		{
			get; set;
		}
	}

	public interface CommandProcessor
	{
		void Process(Command command, int frame);
	}
}