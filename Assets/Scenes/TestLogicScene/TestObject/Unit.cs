using UnityEngine;
using System.Text;

public class UnitView
{
	Vector2 _p0;
	Vector2 _p1;
	Vector2 _currentPosition;

	float _accumulatedDt;

	float _totalTime;

	public void SetPosition (float time, Vector2 position)
	{
		_p0 = position;
		_p1 = position;
	}

	public void UpdatePosition(float time, Vector2 position) 
	{
		_p0 = _currentPosition;
		_p1 = position;

		// _accumulatedDt -= time;
		_accumulatedDt = 0;
		_totalTime = time;
	}

	public Vector2 GetCurrentPosition(float dt)
	{
		_accumulatedDt += dt;
		_currentPosition = Vector2.Lerp (_p0, _p1, _accumulatedDt / _totalTime);
		return _currentPosition;
	}

}

public class Unit : MonoBehaviour {

	Vector2 _gamePosition;

	Vector2 _destination;

	Vector2 _debugLastGamePosition;

	public float speed = 1.0f;

	public bool interpolationEnabled = true;

	bool _moving = false;

	UnitView unitView;

	// TODO: this is used for checksum, add interface for this
	public void AddState(StringBuilder strBuilder)
	{
		strBuilder.Append (_gamePosition.x);
		strBuilder.Append (_gamePosition.y);
		strBuilder.Append (speed);
		strBuilder.Append (_moving);
		strBuilder.Append (_destination.x);
		strBuilder.Append (_destination.y);
	}

	void Awake()
	{
		_gamePosition = transform.position;
		_debugLastGamePosition = _gamePosition;

		unitView = new UnitView ();
		unitView.SetPosition (0, _gamePosition);

		_destination = _gamePosition;
		_moving = false;
	}

	public void SetPosition(Vector2 position)
	{
		transform.position = position;

		_gamePosition = transform.position;
		_debugLastGamePosition = _gamePosition;

		unitView.SetPosition (0, _gamePosition);

		_destination = _gamePosition;
		_moving = false;
	}

	public void MoveTo(Vector2 destination)
	{
		_destination = destination;
		_moving = true;
	}

	public void GameUpdate(float dt, int frame)
	{
		if (!_moving)
			return;

		_debugLastGamePosition = _gamePosition;

		Vector2 direction = (_destination - _gamePosition).normalized;

		float realSpeed = speed * dt;

		Vector2 newPosition = _gamePosition + direction * realSpeed;
	
		if ((_destination - newPosition).SqrMagnitude() < realSpeed * realSpeed) {
//			newPosition = _destination;
			_moving = false;
		}

		_gamePosition = newPosition;

		unitView.UpdatePosition (dt, _gamePosition);
	}

	void LateUpdate()
	{
		if (interpolationEnabled) {
			transform.position = unitView.GetCurrentPosition (Time.deltaTime);
		} else {
			transform.position = _gamePosition;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (_debugLastGamePosition, 0.2f);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (_gamePosition, 0.2f);
	}

}
