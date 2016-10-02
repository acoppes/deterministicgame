namespace Gemserk.Lockstep.Replays 
{
	public interface ReplayView
	{
		void StartRecording ();

		void StartPlayback ();

		void SetReplay (ReplayController replayController);
	}	
}