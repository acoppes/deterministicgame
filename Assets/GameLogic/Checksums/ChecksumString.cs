
public class ChecksumString : Checksum
{
	string _checksum;

	public string Checksum
	{
		get { 
			return _checksum;
		}
	}

	public ChecksumString(string checksum)
	{
		_checksum = checksum;
	}

	#region GameStateChecksum implementation
	public bool IsEqual (Checksum checksum)
	{
		if (checksum == this)
			return true;
		ChecksumString otherChecksum = checksum as ChecksumString;
		if (otherChecksum == null)
			return false;
		return otherChecksum._checksum.Equals(this._checksum);
	}
	#endregion

}
