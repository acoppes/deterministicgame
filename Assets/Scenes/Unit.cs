using UnityEngine;

public class Unit : MonoBehaviour {

	Vector2 _destination;

	public float speed = 1.0f;

	bool _moving = false;

	public void MoveTo(Vector2 destination)
	{
		_destination = destination;
		_moving = true;
	}

	public void GameUpdate(int dt, int frame)
	{
		if (!_moving)
			return;

		Vector2 position = transform.position;
		
		Vector2 direction = (_destination - position).normalized;

		float realSpeed = speed * (dt / 1000.0f);

		Vector2 newPosition = position + direction * realSpeed;
	
		if ((_destination - newPosition).SqrMagnitude() < realSpeed * realSpeed) {
			newPosition = _destination;
			_moving = false;
		}

		transform.position = newPosition;
	}

}
