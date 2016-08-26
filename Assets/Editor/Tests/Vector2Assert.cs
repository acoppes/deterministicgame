using NUnit.Framework;

public static class Vector2Assert {

	public static void AreEqual(UnityEngine.Vector2 v0, UnityEngine.Vector2 v1, float precision)
	{
		if (UnityEngine.Vector2.Distance (v0, v1) < precision)
			return;
		Assert.Fail (string.Format("Vectors {0}, {1} are not equal with precision {2}", v0, v1, precision));
	}

}
