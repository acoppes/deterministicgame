
public interface ChecksumProvider
{
	Checksum CalculateChecksum();
}

// a single checksum for the game state
public interface Checksum
{
	// true if both checksum are equals
	bool IsEqual (Checksum checksum);
}

// a single checksum for the game state

public class StoredChecksum 
{
	public int gameFrame;
	public Checksum checksum;

}

public interface ChecksumValidator
{
	bool IsValid(int gameFrame, Checksum checksum);
}


