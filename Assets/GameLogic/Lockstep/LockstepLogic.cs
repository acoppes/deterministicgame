namespace Gemserk.Lockstep 
{
	public interface LockstepLogic
	{
		bool IsReady(int frame);

		void Process(int frame);
	}	
}
