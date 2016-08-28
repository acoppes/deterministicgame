using UnityEngine;
using System.Collections.Generic;

public class TestLogicScene : MonoBehaviour, GameLogic, GameState {

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

//	GameFixedUpdate gameFixedUpdate;

//	LockstepGameLogic lockstepGameLogic;

	LockstepFixedUpdate gameFixedUpdate;

	CommandsList commandList = new CommandsList();

	public Unit unit;

	public int fixedTimestepMilliseconds = 100;
	public int gameFramesPerLockstep = 4;

	public Camera camera;

	public FeedbackClick feedbackClick;

	CommandsRecorder _commandsRecorder;
	bool _recording;

	public RecorderView recorderView;

	ChecksumRecorder _checksumRecorder;

	#region GameState implementation

	public Checksum CalculateChecksum ()
	{
		return new ChecksumString("ABCDEF12341234");
	}

	#endregion

	void Awake()
	{
		_commandsRecorder = new CommandsRecorder ();

		_checksumRecorder = new ChecksumRecorder (10, this);

		gameFixedUpdate = new LockstepFixedUpdate (new CommandsListLockstepLogic(commandList));
		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

//		lockstepGameLogic = new LockstepGameLogic (this, commandList);
//		lockstepGameLogic.GameFramesPerLockstep = gameFramesPerLockstep;
//
//		gameFixedUpdate = new GameFixedUpdate ();
//		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

		gameFixedUpdate.Init ();
		gameFixedUpdate.SetGameLogic (this);

		StartRecording ();

		// debug...
		GameFixedUpdateDebug updateDebug = gameObject.AddComponent<GameFixedUpdateDebug> ();
		updateDebug.SetGameFixedUpdate (gameFixedUpdate);

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.AddComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.checksumRecorder = _checksumRecorder;

		Application.targetFrameRate = 60;
	}

	void ResetGameState()
	{
		gameFixedUpdate.Init ();
		unit.SetPosition (new Vector2 (0, 0));
	}

	void StartPlayback()
	{
		_commandsRecorder.lastGameFrame = gameFixedUpdate.CurrentGameFrame;
		_recording = false;

		// resets game fixed update state...
		ResetGameState();

		recorderView.StartPlayback ();
	}

	void StartRecording()
	{
//		_commandsRecorder.Reset ();
		_recording = true;
		// ResetGameState();
		recorderView.StartRecording();
	}
	
	// Update is called once per frame
	void Update () {

		// update values

		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

//		lockstepGameLogic.GameFramesPerLockstep = gameFramesPerLockstep;
//		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

//		int milliseconds = Mathf.RoundToInt(Time.deltaTime * 1000.0f);


		if (Input.GetKeyUp (KeyCode.P)) {
			StartPlayback ();

			return;
		}

		if (Input.GetKeyUp (KeyCode.R)) {

			// resets game fixed update state...
			StartRecording();

			return;
		}

		if (_recording) {

			gameFixedUpdate.Update (Time.deltaTime);

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

			// if already at last frame, then dont update anymore...
			if (_commandsRecorder.lastGameFrame == gameFixedUpdate.CurrentGameFrame)
				return;

			gameFixedUpdate.Update (Time.deltaTime);

			List<Command> recordedCommands = new List<Command> ();

			_commandsRecorder.GetCommandsForFrame(gameFixedUpdate.CurrentGameFrame, recordedCommands);

			for (int i = 0; i < recordedCommands.Count; i++) {
				var command = recordedCommands [i];
				commandList.AddCommand (command);
			}

			commandList.IsReady = true;
				
		}
	}

	#region DeterministicGameLogic implementation

	public void Update (float dt, int frame)
	{
		// Debug.Log ("Timestep: " + frame);
		_checksumRecorder.Update(dt, frame);
		unit.GameUpdate (dt, frame);
	}

	#endregion
}
