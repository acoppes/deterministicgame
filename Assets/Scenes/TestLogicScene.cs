using UnityEngine;
using System.Collections.Generic;

public class TestLogicScene : MonoBehaviour, DeterministicGameLogic {

	public class RecordedCommand
	{
		public Command command;
		public float gameTime;
		public int gameFrame;
	}

	public class CommandsRecorder
	{
		readonly List<RecordedCommand> recordedCommandsQueue = new List<RecordedCommand>();

		public void AddCommand(float gameTime, int gameFrame, Command command)
		{
			recordedCommandsQueue.Add (new RecordedCommand () { 
				command = command,
				gameTime = gameTime,
				gameFrame = gameFrame
			});
		}

		public RecordedCommand GetFirstCommand()
		{
			if (recordedCommandsQueue.Count == 0)
				return null;
			return recordedCommandsQueue[0];
		}

		public void RemoveCommand(RecordedCommand recordedCommand)
		{
			recordedCommandsQueue.Remove (recordedCommand);
		}

		public bool HasCommands()
		{
			return recordedCommandsQueue.Count > 0;
		}

		public void Reset()
		{
			recordedCommandsQueue.Clear ();
		}
	}

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

	CommandsRecorder _commandsRecorder;
	bool _recording;

	void Awake()
	{
		_commandsRecorder = new CommandsRecorder ();

		lockstepGameLogic = new LockstepGameLogic (this, commandList);
		lockstepGameLogic.GameFramesPerLockstep = (lockstepMilliseconds / fixedTimestepMilliseconds);

		gameFixedUpdate = new GameFixedUpdate ();
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

		gameFixedUpdate.Init ();
		gameFixedUpdate.SetGameLogic (lockstepGameLogic);

		_recording = true;
		_commandsRecorder.Reset ();

		// debug...
		GameFixedUpdateDebug updateDebug = gameObject.AddComponent<GameFixedUpdateDebug> ();
		updateDebug.SetGameFixedUpdate (gameFixedUpdate);
	}

	void ResetGameState()
	{
		gameFixedUpdate.Init ();
		unit.SetPosition (new Vector2 (0, 0));
	}
	
	// Update is called once per frame
	void Update () {

		// update values
		lockstepGameLogic.GameFramesPerLockstep = (lockstepMilliseconds / fixedTimestepMilliseconds);
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

//		int milliseconds = Mathf.RoundToInt(Time.deltaTime * 1000.0f);
		gameFixedUpdate.Update (Time.deltaTime);

		if (Input.GetKeyUp (KeyCode.P)) {

			// resets game fixed update state...
			ResetGameState();

			_recording = false;

			return;
		}

		if (Input.GetKeyUp (KeyCode.R)) {

			// resets game fixed update state...
			ResetGameState();

			_recording = true;
			_commandsRecorder.Reset ();

			return;
		}

		if (_recording) {

			if (Input.GetMouseButtonUp (1)) {
				Vector2 position = camera.ScreenToWorldPoint (Input.mousePosition);
				var moveCommand = new MoveCommand (unit, position);

				commandList.AddCommand (moveCommand);
				feedbackClick.ShowFeedback (position);

				_commandsRecorder.AddCommand (gameFixedUpdate.GameTime, gameFixedUpdate.CurrentGameFrame, moveCommand);
			}

			if (Input.touchCount > 0) {
		
				if (Input.GetTouch (0).phase == TouchPhase.Ended) {
					Vector2 position = camera.ScreenToWorldPoint (Input.GetTouch (0).position);
					var moveCommand = new MoveCommand (unit, position);
					commandList.AddCommand (moveCommand);			
					feedbackClick.ShowFeedback (position);

					_commandsRecorder.AddCommand (gameFixedUpdate.GameTime, gameFixedUpdate.CurrentGameFrame, moveCommand);
				}

			}

			commandList.IsReady = true;
		} else {
		
			// playback...

			do {
				RecordedCommand recordedCommand = _commandsRecorder.GetFirstCommand ();

				if (recordedCommand == null)
					break;

				if (recordedCommand.gameFrame != gameFixedUpdate.CurrentGameFrame)
					break;

				commandList.AddCommand (recordedCommand.command);

				_commandsRecorder.RemoveCommand(recordedCommand);

			} while (_commandsRecorder.HasCommands());

			commandList.IsReady = true;
				
		}
	}

	#region DeterministicGameLogic implementation

	public void Update (float dt, int frame)
	{
		// Debug.Log ("Timestep: " + frame);
		unit.GameUpdate (dt, frame);
	}

	#endregion
}
