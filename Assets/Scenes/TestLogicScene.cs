using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class TestLogicScene : MonoBehaviour, GameLogic, ChecksumProvider {

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

	ChecksumValidator _checksumValidator;

	public int gameFramesPerChecksumCheck = 10;

	#region GameState implementation

	public Checksum CalculateChecksum ()
	{
		StringBuilder strBuilder = new StringBuilder ();

		strBuilder.Append(gameFixedUpdate.CurrentGameFrame);
		unit.AddState (strBuilder);

		return new ChecksumString(ChecksumHelper.CalculateMD5(strBuilder.ToString()));
//		return new ChecksumString(strBuilder.ToString());
	}

	#endregion

	void Awake()
	{
		_commandsRecorder = new CommandsRecorder ();

		_checksumRecorder = new ChecksumRecorder (this);

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.AddComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.checksumRecorder = _checksumRecorder;

		gameFixedUpdate = new LockstepFixedUpdate (new CommandsListLockstepLogic(commandList));
		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

		gameFixedUpdate.Init ();
		gameFixedUpdate.SetGameLogic (this);

		StartRecording ();

		// debug...
		GameFixedUpdateDebug updateDebug = gameObject.AddComponent<GameFixedUpdateDebug> ();
		updateDebug.SetGameFixedUpdate (gameFixedUpdate);

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

		_checksumValidator = new ChecksumValidatorBasic (_checksumRecorder.StoredChecksums);
	}

	void StartRecording()
	{
		_recording = true;
		recorderView.StartRecording();

		_checksumRecorder.Reset ();

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.GetComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.Reset ();
	}
	
	// Update is called once per frame
	void Update () {

		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;
	
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

	bool IsChecksumFrame(int frame)
	{
		return (frame % gameFramesPerChecksumCheck) == 0;
	}

	public void Update (float dt, int frame)
	{
		// Debug.Log ("Timestep: " + frame);

		if (IsChecksumFrame(frame)) {
			if (!_recording && _checksumValidator != null) {
				bool validState = _checksumValidator.IsValid (frame, CalculateChecksum ());
				Debug.Log (string.Format ("State({0}): is {1}", frame, validState ? "valid" : "invalid!"));
			} else {
				_checksumRecorder.RecordState (frame);
			}
		}

		unit.GameUpdate (dt, frame);
	}

	#endregion
}
