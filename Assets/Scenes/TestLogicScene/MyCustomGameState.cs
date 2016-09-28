using UnityEngine;
using Gemserk.Lockstep;
using System.Text;

public class MyCustomGameState : GameState
{
	public struct UnitData 
	{
		public Vector2 position;
		public Vector2 destination;
		public float speed;
		public bool moving;
	}

	public int currentGameFrame;

	public float fixedDeltaTime;
	public int gameFramesPerLockstep;

	public UnitData unitData;

	#region ChecksumProvider implementation
	public Checksum CalculateChecksum ()
	{
		return new ChecksumString(ChecksumHelper.CalculateMD5(GetStateString()));
	}
	#endregion

	public string GetStateString()
	{
		var strBuilder = new StringBuilder();
	
		strBuilder.Append (string.Format ("Engine:(frame:{0})", currentGameFrame));
		strBuilder.Append (string.Format ("Unit:(position:{0}, speed:{1}, moving:{2}, destination:{3})", unitData.position, unitData.speed, unitData.moving, unitData.destination));
	
		return strBuilder.ToString ();
	}
	
}
