namespace Gemserk.Lockstep 
{
	public interface ReplayView
	{
		void StartRecording ();

		void StartPlayback ();

		void SetReplay (ReplayController replayController);
	}	
}