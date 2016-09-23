using System;

namespace Gemserk.Lockstep 
{
	[Serializable]
	public class CommandBase : Command
	{
		public int processFrame;

		public int ProcessFrame 
		{
			get {
				return processFrame;
			} 
			set {
				processFrame = value;
			}
		}
	}
}