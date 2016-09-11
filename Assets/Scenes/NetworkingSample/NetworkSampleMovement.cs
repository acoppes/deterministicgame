using UnityEngine;
using UnityEngine.Networking;

public class NetworkSampleMovement : NetworkBehaviour {

	Vector2 _gamePosition;

	Vector2 _destination;

	public float speed = 1.0f;

	bool _moving = false;

	void Awake()
	{
		_gamePosition = transform.position;
		_destination = _gamePosition;
		_moving = false;
	}

	public void SetPosition(Vector2 position)
	{
		transform.position = position;

		_gamePosition = transform.position;

		_destination = _gamePosition;
		_moving = false;
	}

	public void MoveTo(Vector2 destination)
	{
		_destination = destination;
		_moving = true;
	}

	[ServerCallback]
	public void Update()
	{
		if (!_moving)
			return;

		Vector2 direction = (_destination - _gamePosition).normalized;

		float realSpeed = speed * Time.deltaTime;

		Vector2 newPosition = _gamePosition + direction * realSpeed;

		if ((_destination - newPosition).SqrMagnitude() < realSpeed * realSpeed) {
			//			newPosition = _destination;
			_moving = false;
		}

		_gamePosition = newPosition;
	}

	[ServerCallback]
	void LateUpdate()
	{
		transform.position = _gamePosition;
	}

}
