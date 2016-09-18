using System.Text;

namespace Gemserk.Lockstep 
{
	public class GameStateStringBuilderImpl : GameState
	{
		StringBuilder state = new StringBuilder();

		public string State {
			get {
				return state.ToString();
			}
		}

		#region ChecksumProvider implementation
		public Checksum CalculateChecksum ()
		{
			return new ChecksumString(ChecksumHelper.CalculateMD5(state.ToString()));
		}
		#endregion

		public void StartObject (string name)
		{
			throw new System.NotImplementedException ();
		}

		public void EndObject ()
		{
			throw new System.NotImplementedException ();
		}

		public void SetInt (string name, int i)
		{
			state.Append (i);
		}

		public void SetFloat (string name, float f)
		{
			state.Append (f);
		}

		public void SetBool (string name, bool b)
		{
			state.Append (b);
		}

		public void Reset ()
		{
			state = new StringBuilder ();
		}
	}
}