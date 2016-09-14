using UnityEngine;
using System.Text;
using Gemserk.Lockstep;

public class PositionInterpolator
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

//public interface GameStateProvider 
//{
//	
//}

public class UnitImpl : GameLogic
{
	Vector2 _gamePosition;

	Vector2 _destination;

	Vector2 _lastGamePosition;

	float speed = 1.0f;

	bool _moving = false;

	PositionInterpolator unitView;

	public Vector2 Position {
		get {
			return _gamePosition;
		}
	}

	public Vector2 PreviousPosition {
		get {
			return _lastGamePosition;
		}
	}

	public PositionInterpolator UnitView {
		get {
			return unitView;
		}
	}

	public float Speed {
		get {
			return speed;
		}
		set {
			speed = value;
		}
	}

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

	public UnitImpl(Vector2 position)
	{
		_gamePosition = position;

		unitView = new PositionInterpolator ();
		unitView.SetPosition (0, _gamePosition);

		_destination = _gamePosition;
		_moving = false;
	}

	public void SetPosition(Vector2 position)
	{
		_gamePosition = position;
		_lastGamePosition = _gamePosition;

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

		_lastGamePosition = _gamePosition;

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

}

public class Unit : MonoBehaviour {

	UnitImpl unitImpl;

	public bool interpolationEnabled = true;

	public UnitImpl UnitImpl {
		get {
			return unitImpl;
		}
	}

	public float speed = 1.0f;

	void Awake()
	{
		unitImpl = new UnitImpl (transform.position);
	}

	public void SetPosition(Vector2 position)
	{
		transform.position = position;
		unitImpl.SetPosition (position);
	}

	public void MoveTo(Vector2 destination)
	{
		unitImpl.MoveTo (destination);
	}

	void LateUpdate()
	{
		unitImpl.Speed = this.speed;

		if (interpolationEnabled) {
			transform.position = unitImpl.UnitView.GetCurrentPosition (Time.deltaTime);
		} else {
			transform.position = unitImpl.Position;
		}
	}

	void OnDrawGizmos()
	{
		if (unitImpl == null)
			return;
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (unitImpl.PreviousPosition, 0.2f);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (unitImpl.Position, 0.2f);
	}

}
