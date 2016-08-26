using NUnit.Framework;

public class TestUnitView {

	const float distancePrecision = 0.001f;

	[Test]
	public void TestUnitViewGetPosition()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, 0));

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0), new UnityEngine.Vector2 (0, 0)) < distancePrecision);
	}

	[Test]
	public void TestUnitViewGetPosition2()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, 0));

		UnityEngine.Vector2 position = unitView.GetCurrentPosition (1);

		Assert.That (UnityEngine.Vector2.Distance (position, new UnityEngine.Vector2 (100, 0)) < distancePrecision);
	}

	[Test]
	public void TestUnitViewGetPosition3()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, 0));

		UnityEngine.Vector2 position = unitView.GetCurrentPosition (0.5f);

		Assert.That (UnityEngine.Vector2.Distance (position, new UnityEngine.Vector2 (50, 0)) < distancePrecision);
	}

	[Test]
	public void TestUnitViewGetPositionIncrementalDt()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, -100));

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (10, -10)) < distancePrecision);
		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (20, -20)) < distancePrecision);	

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.8f), new UnityEngine.Vector2 (100, -100)) < distancePrecision);	
	}

	[Test]
	public void TestUnitViewGetPositionIncrementalDt2()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (100, -100));

		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (10, -10)) < distancePrecision);
		unitView.UpdatePosition (1, new UnityEngine.Vector2 (410, -410));
		Assert.AreEqual (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (50, -50));
		//		Assert.That (UnityEngine.Vector2.Distance (unitView.GetCurrentPosition (0.1f), new UnityEngine.Vector2 (40, -40)) < distancePrecision);	
	}

	[Test]
	public void TestUnitViewGetPositionIncrementalDt3()
	{
		UnitView unitView = new UnitView ();
		unitView.SetPosition (0, new UnityEngine.Vector2 (0, 0));
		unitView.UpdatePosition (0.05f, new UnityEngine.Vector2 (100, -100));

		Vector2Assert.AreEqual (new UnityEngine.Vector2 (32.0f, -32.0f), unitView.GetCurrentPosition (0.016f), distancePrecision);
	}

}
