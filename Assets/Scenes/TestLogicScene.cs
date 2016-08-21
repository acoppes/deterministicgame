using UnityEngine;

public class TestLogicScene : MonoBehaviour, DeterministicGameLogic {

	public class MoveCommand : Command
	{
		GameObject gameObject;
		int direction;

		public MoveCommand(GameObject gameObject, int direction)
		{
			this.gameObject = gameObject;
			this.direction = direction;
		}

		public override void Process ()
		{
			base.Process ();
			gameObject.transform.position = gameObject.transform.position + new Vector3 (1 * direction, 0, 0);
		}
	}

	GameFixedUpdate gameFixedUpdate;

	LockstepGameLogic lockstepGameLogic;

	CommandsList commandList = new CommandsList();

	public GameObject testObject;

	public int fixedTimestepMilliseconds = 100;
	public int framesPerLockstep = 5;

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

		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			commandList.AddCommand (new MoveCommand (testObject, -1));
		} else if (Input.GetKeyUp (KeyCode.RightArrow)) {
			commandList.AddCommand (new MoveCommand (testObject, 1));
		}

		commandList.IsReady = true;
	}

	#region DeterministicGameLogic implementation

	public void Update (int dt, int frame)
	{
		Debug.Log ("Timestep: " + frame);
	}

	#endregion
}
