using System.Collections.Generic;

public class ChecksumValidatorBasic : ChecksumValidator
{
	readonly List<StoredChecksum> _checksums;

	public ChecksumValidatorBasic(List<StoredChecksum> checksums)
	{
		_checksums = checksums;
	}

	public bool IsValid (int gameFrame, Checksum checksum)
	{
		for (int i = 0; i < _checksums.Count; i++) {
			var _checksum = _checksums [i];
			if (_checksum.gameFrame == gameFrame)
				return _checksum.checksum.IsEqual (checksum);
		}
		return false;
	}

}
