namespace Gemserk.Lockstep 
{
	public interface Command
	{
		/// <summary>
		/// Returns the lockstep frame when the command should be processed.
		/// </summary>
		int ProcessFrame 
		{
			get; set;
		}
	}		
}