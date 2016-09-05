using UnityEngine;
using System.Collections.Generic;
using Gemserk.Lockstep;

public class ChecksumRecorderDebug : MonoBehaviour
{
	public ChecksumRecorder checksumRecorder;

	public List<string> checksums = new List<string>();

	public void Reset()
	{
		checksums.Clear ();
	}

	void Update()
	{
		while (checksumRecorder.StoredChecksums.Count > checksums.Count) {
			int i = checksums.Count;

			ChecksumString checksumString = checksumRecorder.StoredChecksums [i].checksum as ChecksumString;

			if (checksumString == null)
				return;
			
			checksums.Add (checksumString.Checksum);
		}

	}

}
