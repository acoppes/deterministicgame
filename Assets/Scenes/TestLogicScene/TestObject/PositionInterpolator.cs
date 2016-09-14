using UnityEngine;

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
