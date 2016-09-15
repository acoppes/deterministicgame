namespace Gemserk.Lockstep 
{
	public class Command
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