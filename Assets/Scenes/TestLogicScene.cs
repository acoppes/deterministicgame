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
	public int lockstepMilliseconds = 500;

	public Camera camera;

	public FeedbackClick feedbackClick;

	void Awake()
	{
		lockstepGameLogic = new LockstepGameLogic (this, commandList);
		lockstepGameLogic.GameFramesPerLockstep = (lockstepMilliseconds / fixedTimestepMilliseconds);

		gameFixedUpdate = new GameFixedUpdate ();
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

		gameFixedUpdate.Init ();
		gameFixedUpdate.SetGameLogic (lockstepGameLogic);
	}
	
	// Update is called once per frame
	void Update () {

		// update values
		lockstepGameLogic.GameFramesPerLockstep = (lockstepMilliseconds / fixedTimestepMilliseconds);
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

//		int milliseconds = Mathf.RoundToInt(Time.deltaTime * 1000.0f);
		gameFixedUpdate.Update (Time.deltaTime);



		if (Input.GetMouseButtonUp (1)) {
			Vector2 position = camera.ScreenToWorldPoint(Input.mousePosition);
			commandList.AddCommand (new MoveCommand (unit, position));

			feedbackClick.ShowFeedback (position);
		}

		if (Input.touchCount > 0) {
		
			if (Input.GetTouch(0).phase == TouchPhase.Ended) {
				Vector2 position = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
				commandList.AddCommand (new MoveCommand (unit, position));			

				feedbackClick.ShowFeedback (position);
			}

		}

//		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
//			commandList.AddCommand (new MoveCommand (unit, -1));
//		} else if (Input.GetKeyUp (KeyCode.RightArrow)) {
//			commandList.AddCommand (new MoveCommand (unit, 1));
//		}

		commandList.IsReady = true;
	}

	#region DeterministicGameLogic implementation

	public void Update (float dt, int frame)
	{
		Debug.Log ("Timestep: " + frame);
		unit.GameUpdate (dt, frame);
	}

	#endregion
}
