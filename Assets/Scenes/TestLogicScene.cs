using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
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
//		MemoryStream stream = new MemoryStream ();
//		stream.

		StringBuilder strBuilder = new StringBuilder ();
//		strBuilder.Append( // local state
		// strBuilder.Append(gameFixedUpdate.CurrentGameFrame);
		unit.AddState (strBuilder);

		byte[] md5hash = MD5.Create ().ComputeHash (Encoding.UTF8.GetBytes (strBuilder.ToString ()));

		StringBuilder sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < md5hash.Length; i++)
		{
			sBuilder.Append(md5hash[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return new ChecksumString(sBuilder.ToString());
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

		_checksumRecorder = new ChecksumRecorder (this);

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.AddComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.checksumRecorder = _checksumRecorder;
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

		if ((frame % gameFramesPerChecksumCheck) == 0) {
			if (!_recording && _checksumValidator != null) {
				bool validState = _checksumValidator.IsValid (frame, CalculateChecksum ());
				Debug.Log (string.Format("State({0}): is {1}", frame, validState ? "valid" : "invalid!"));
			}
			_checksumRecorder.RecordState(frame);
		}

		unit.GameUpdate (dt, frame);
	}

	#endregion
}
