using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public class ChecksumValidatorBasic : ChecksumValidator
	{
		public bool IsValid (int gameFrame, Checksum checksum, List<StoredChecksum> checksums)
		{
			for (int i = 0; i < checksums.Count; i++) {
				var _checksum = checksums [i];
				if (_checksum.gameFrame == gameFrame)
					return _checksum.checksum.IsEqual (checksum);
			}
			// if no stored checksum to validate, then current frame checksum is valid.
			return true;
		}

	}
}