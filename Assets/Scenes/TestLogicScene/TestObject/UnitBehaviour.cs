using UnityEngine;

public class UnitBehaviour : MonoBehaviour {

	Unit unit;

	public bool interpolationEnabled = true;

	public Unit Unit {
		get {
			return unit;
		}
	}

	public float speed = 1.0f;

	void Awake()
	{
		unit = new Unit (transform.position);
	}

	void LateUpdate()
	{
		unit.Speed = this.speed;

		if (interpolationEnabled) {
			transform.position = unit.UnitView.GetCurrentPosition (Time.deltaTime);
		} else {
			transform.position = unit.Position;
		}
	}

	void OnDrawGizmos()
	{
		if (unit == null)
			return;
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (unit.PreviousPosition, 0.2f);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (unit.Position, 0.2f);
	}

}
