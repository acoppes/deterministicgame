using System.Text;

namespace Gemserk.Lockstep 
{
	// TODO: a better implementation could be to generate a tree that represents the state
	// and then have a way to create a string or another, base don the tree representation.

	public class GameStateStringBuilderImpl : GameStateBuilder
	{
		StringBuilder state = new StringBuilder();

		#region ChecksumProvider implementation
		public Checksum CalculateChecksum ()
		{
			return new ChecksumString(ChecksumHelper.CalculateMD5(state.ToString()));
		}
		#endregion

		public GameState GetGameState ()
		{
			return new GameStateStringImpl () { 
				State = this.state.ToString()
			};
		}

		bool firstObject = true;
		bool firstElement = true;

		public void StartObject (string name)
		{
			firstElement = true;
			state.AppendFormat ("{1}{0}:(", name, firstObject ? "" : ",");
		}

		public void EndObject ()
		{
			state.Append (")");
			firstObject = false;
		}

		public void SetInt (string name, int i)
		{
			state.AppendFormat ("{2}{0}:{1}", name, i, firstElement ? "" : ",");
			firstElement = false;
		}

		public void SetFloat (string name, float f)
		{
			state.AppendFormat ("{2}{0}:{1}", name, f, firstElement ? "" : ",");
			firstElement = false;
		}

		public void SetBool (string name, bool b)
		{
			state.AppendFormat ("{2}{0}:{1}", name, b, firstElement ? "" : ",");
			firstElement = false;
		}

		public void Reset ()
		{
			state = new StringBuilder ();
			firstObject = true;
			firstElement = true;
		}
	}
}