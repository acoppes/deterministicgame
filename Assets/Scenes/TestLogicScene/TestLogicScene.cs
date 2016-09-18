using UnityEngine;
using System.Collections.Generic;
using Gemserk.Lockstep;

public class Replay
{
	readonly GameFixedUpdate _gameFixedUpdate;

	readonly CommandsRecorder _commandsRecorder;

	readonly RecorderView _recorderView;

	readonly ChecksumRecorder _checksumRecorder;

	readonly Commands _commandsList;

	ChecksumValidator _checksumValidator;

	bool _recording;

	int gameFramesPerChecksumCheck = 10;

	int _lastRecordedGameFrame; 

	public int GameFramesPerChecksumCheck {
		get {
			return gameFramesPerChecksumCheck;
		}
		set {
			gameFramesPerChecksumCheck = value;
		}
	}

	public bool IsRecording
	{
		get {
			return _recording;
		}
	}

	public float NormalizedTime {
		get { 
			return (float) _gameFixedUpdate.CurrentGameFrame / (float) _lastRecordedGameFrame;
		}
	}

	public Replay(GameFixedUpdate gameFixedUpdate, ChecksumRecorder checksumRecorder, RecorderView recorderView, Commands commandsList)
	{
		_recording = true;
		_lastRecordedGameFrame = 0;
		_commandsRecorder = new CommandsRecorder ();
		_checksumRecorder = checksumRecorder;
		_gameFixedUpdate = gameFixedUpdate;
		_recorderView = recorderView;
		_commandsList = commandsList;

		_recorderView.SetReplay (this);
	}

	public void StartPlayback()
	{
		_recording = false;

		_recorderView.StartPlayback ();

		_checksumValidator = new ChecksumValidatorBasic (_checksumRecorder.StoredChecksums);
	}

	public void StartRecording()
	{
		_recording = true;
		_recorderView.StartRecording();

		// we could have a Reset() for the replay system if we want to start from 0...

		// _checksumRecorder.Reset ();
	}

	readonly List<Command> _commandsToRecord = new List<Command>();

	public void RecordCommands()
	{
		_commandsToRecord.Clear ();

		_commandsList.GetCommands (_commandsToRecord);

		for (int i = 0; i < _commandsToRecord.Count; i++) {
			var command = _commandsToRecord [i];
			_commandsRecorder.AddCommand (_gameFixedUpdate.GameTime, _gameFixedUpdate.CurrentGameFrame, command);
		}
	}

	public bool IsFinished()
	{
		return _lastRecordedGameFrame == _gameFixedUpdate.CurrentGameFrame;
	}

	public void ReplayCommands()
	{
		List<Command> recordedCommands = new List<Command> ();

		_commandsRecorder.GetCommandsForFrame(_gameFixedUpdate.CurrentGameFrame, recordedCommands);

		for (int i = 0; i < recordedCommands.Count; i++) {
			var command = recordedCommands [i];
			_commandsList.AddCommand (command);
		}

		// _commandsList.IsReady = true;
	}

	bool IsChecksumFrame(int frame)
	{
		return (frame % gameFramesPerChecksumCheck) == 0;
	}

	public void Update(int frame)
	{
		ChecksumProvider checksumProvider = _checksumRecorder.ChecksumProvider;

		if (_recording) {
			_lastRecordedGameFrame = _gameFixedUpdate.CurrentGameFrame;

			if (IsChecksumFrame (frame)) {
				_checksumRecorder.RecordState (frame);
			}
		} else {
			
			if (IsChecksumFrame (frame)) {
				bool validState = _checksumValidator.IsValid (frame, checksumProvider.CalculateChecksum ());
				Debug.Log (string.Format ("State({0}): is {1}", frame, validState ? "valid" : "invalid!"));
			}
		}
	}

}

public class TestLogicScene : MonoBehaviour, GameLogic, GameStateProvider, CommandProcessor, CommandEmptyProvider {

	public class MoveCommand : CommandBase
	{
		public Vector2 destination;

		public MoveCommand(Vector2 destination)
		{
			this.destination = destination;
		}
	}
		
	LockstepFixedUpdate gameFixedUpdate;

	Commands commandList;

	public UnitBehaviour unit;

	public int fixedTimestepMilliseconds = 100;
	public int gameFramesPerLockstep = 4;

	public Camera camera;

	public FeedbackClick feedbackClick;

	public RecorderView recorderView;

	Replay _replay;

	CommandSender _commandSender;

	public int gameFramesPerChecksumCheck = 10;

	GameState gameState;

	#region GameStateProvider implementation

	public void Provide (GameState gameState)
	{
		gameState.SetInt (gameFixedUpdate.CurrentGameFrame);
		unit.Unit.Provide (gameState);
	}

	#endregion

	#region CommandProcessor implementation

	public bool CheckReady (Commands commands, int frame)
	{
		// check if each player has commands enqueued in the Commands api
		return true;
	}

	public void Process (Command command, int frame)
	{
		MoveCommand moveCommand = command as MoveCommand;

		if (moveCommand != null) {
			unit.Unit.MoveTo (moveCommand.destination);
		}
	}

	#endregion

	#region CommandEmptyProvider implementation

	public Command GetEmptyCommand ()
	{
		return new CommandBase ();
	}

	#endregion


	void Awake()
	{
		gameState = new GameStateStringBuilderImpl ();

		commandList = new CommandsList();

		ChecksumRecorder checksumRecorder = new ChecksumRecorder (new GameStateChecksumProvider (gameState, this));

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.AddComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.checksumRecorder = checksumRecorder;

		// TODO: set replay....

		gameFixedUpdate = new LockstepFixedUpdate (new CommandsListLockstepLogic(commandList, this));
		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;
		gameFixedUpdate.SetGameLogic (this);

		_commandSender = new CommandSenderBase (gameFixedUpdate, commandList, this);

		ResetGameState ();

		_replay = new Replay (gameFixedUpdate, checksumRecorder, recorderView, commandList);
		_replay.GameFramesPerChecksumCheck = gameFramesPerChecksumCheck;

		StartRecording ();

		// debug...
		GameFixedUpdateDebug updateDebug = gameObject.AddComponent<GameFixedUpdateDebug> ();
		updateDebug.SetGameFixedUpdate (gameFixedUpdate);

		Application.targetFrameRate = 60;

//		commandList.AddCommand (new Command () {
//			CreationFrame = 0,
//			ProcessFrame = gameFixedUpdate.GetFirstLockstepFrame()
//		});
	}

	void ResetGameState()
	{
		gameFixedUpdate.Init ();
		unit.Unit.SetPosition (new Vector2 (0, 0));

		// by default enqueues an empty command for first lockstep frame
		commandList.AddCommand (new CommandBase () {
			ProcessFrame = gameFixedUpdate.GetFirstLockstepFrame()
		});
	}

	public void ToggleRecording()
	{
		if (_replay.IsRecording) {
			StartPlayback ();
		} else {
			StartRecording ();
		}
	}

	void StartRecording()
	{
		_replay.StartRecording ();

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.GetComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.Reset ();
	}

	void StartPlayback()
	{
		ResetGameState ();
		_replay.StartPlayback ();
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

		if (_replay.IsRecording) {

			gameFixedUpdate.Update (Time.deltaTime);

			if (Input.GetMouseButtonUp (1)) {
				Vector2 position = camera.ScreenToWorldPoint (Input.mousePosition);
			
				_lastCommand = ConfigureCommand (new MoveCommand (position));
//				_commandSender.EnqueueCommand (_lastCommand);
//				AddCommand (new MoveCommand (position));

				feedbackClick.ShowFeedback (position);

//				_replay.RecordCommands ();
			}

			if (Input.touchCount > 0) {
		
				if (Input.GetTouch (0).phase == TouchPhase.Ended) {
					Vector2 position = camera.ScreenToWorldPoint (Input.GetTouch (0).position);

					_lastCommand = ConfigureCommand (new MoveCommand (position));
//					AddCommand (new MoveCommand (position));

					feedbackClick.ShowFeedback (position);

//					_replay.RecordCommands ();
				}

			}

		//	commandList.IsReady = true;
		} else {
		
			// playback...

			// if already at last frame, then dont update anymore...
			if (_replay.IsFinished())
				return;

			gameFixedUpdate.Update (Time.deltaTime);

			_replay.ReplayCommands ();
		}
	}

	Command _lastCommand;

	Command ConfigureCommand(Command command)
	{
		command.ProcessFrame = gameFixedUpdate.GetNextLockstepFrame ();
		return command;
	}

	void SendCommand(Command command)
	{
		commandList.AddCommand (command);
	}

	#region DeterministicGameLogic implementation

	public void GameUpdate (float dt, int frame)
	{
		// Debug.Log ("Timestep: " + frame);

		// add empty command each fixed step just in case...

		if (_lastCommand != null) {
			_commandSender.EnqueueCommand (_lastCommand);
			_lastCommand = null;
		}

		if (_commandSender.IsReady())
			_commandSender.SendCommands ();

		if (_replay.IsRecording) {
			_replay.RecordCommands ();
		}

		_replay.Update (frame);

		// update game state...
		unit.Unit.GameUpdate (dt, frame);
	}

	#endregion
}
