namespace Gemserk.Lockstep 
{
	public interface GameState : ChecksumProvider
	{
		void StartObject(string name);

		void EndObject();

		void SetInt(string name, int i);

		void SetFloat(string name, float f);

		void SetBool(string name, bool b);

		void Reset();
	}
}