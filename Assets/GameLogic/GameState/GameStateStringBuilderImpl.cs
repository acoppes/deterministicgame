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

		public void SetInt (int i)
		{
			state.Append (i);
		}

		public void SetFloat (float f)
		{
			state.Append (f);
		}

		public void SetBool (bool b)
		{
			state.Append (b);
		}

		public void Reset ()
		{
			state = new StringBuilder ();
		}
	}
}