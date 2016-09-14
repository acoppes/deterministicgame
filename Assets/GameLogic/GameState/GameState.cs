namespace Gemserk.Lockstep 
{
	public interface GameState : ChecksumProvider
	{
		void SetInt(int i);

		void SetFloat(float f);

		void SetBool(bool b);

		void Reset();
	}
}