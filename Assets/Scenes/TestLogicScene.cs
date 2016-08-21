using UnityEngine;

public class TestLogicScene : MonoBehaviour, DeterministicGameLogic {

	public class MoveCommand : Command
	{
		Unit unit;
		Vector2 destination;

		public MoveCommand(Unit unit, Vector2 destination)
		{
			this.unit = unit;
			this.destination = destination;
		}

		public override void Process ()
		{
			base.Process ();
			unit.MoveTo (destination);
//			gameObject.transform.position = gameObject.transform.position + new Vector3 (1 * direction, 0, 0);
		}
	}

	GameFixedUpdate gameFixedUpdate;

	LockstepGameLogic lockstepGameLogic;

	CommandsList commandList = new CommandsList();

	public Unit unit;

	public int fixedTimestepMilliseconds = 100;
	public int framesPerLockstep = 5;

	public Camera camera;

	void Awake()
	{
		lockstepGameLogic = new LockstepGameLogic (this, commandList);
		lockstepGameLogic.GameFramesPerLockstep = framesPerLockstep;

		gameFixedUpdate = new GameFixedUpdate ();
		gameFixedUpdate.FixedTimeStepMilliseconds = fixedTimestepMilliseconds;

		gameFixedUpdate.Init ();
		gameFixedUpdate.SetGameLogic (lockstepGameLogic);
	}
	
	// Update is called once per frame
	void Update () {

		// update values
		lockstepGameLogic.GameFramesPerLockstep = framesPerLockstep;
		gameFixedUpdate.FixedTimeStepMilliseconds = fixedTimestepMilliseconds;

		int milliseconds = Mathf.RoundToInt(Time.deltaTime * 1000.0f);
		gameFixedUpdate.Update (milliseconds);



		if (Input.GetMouseButtonUp (1)) {
		
			Vector2 position = camera.ScreenToWorldPoint(Input.mousePosition);

			commandList.AddCommand (new MoveCommand (unit, position));
		}

//		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
//			commandList.AddCommand (new MoveCommand (unit, -1));
//		} else if (Input.GetKeyUp (KeyCode.RightArrow)) {
//			commandList.AddCommand (new MoveCommand (unit, 1));
//		}

		commandList.IsReady = true;
	}

	#region DeterministicGameLogic implementation

	public void Update (int dt, int frame)
	{
		Debug.Log ("Timestep: " + frame);
		unit.GameUpdate (dt, frame);
	}

	#endregion
}
