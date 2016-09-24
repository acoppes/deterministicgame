using UnityEngine;
using System.Collections.Generic;
using Gemserk.Lockstep;

public class MyCustomReplay : ReplayBase
{
	public class StoredGameState
	{
		public int frame;
		public GameState gameState;
	}

	List<StoredGameState> storedGameStates = new List<StoredGameState> ();

	GameStateBuilder _gameStateBuilder;

	public MyCustomReplay(ChecksumProvider checksumProvider, GameStateBuilder gameStateBuilder) : base(checksumProvider)
	{
		_gameStateBuilder = gameStateBuilder;
	}

	public override void RecordChecksum (int frame)
	{
		base.RecordChecksum (frame);
		SaveGameState (frame, _gameStateBuilder.GetGameState ());
	}

	void SaveGameState(int frame, GameState gameState)
	{
		storedGameStates.Add (new StoredGameState () { 
			frame = frame,
			gameState = gameState
		});
	}

	public GameState GetStoredGameState(int frame)
	{
		for (int i = 0; i < storedGameStates.Count; i++) {
			var storedGameState = storedGameStates [i];
			if (storedGameState.frame == frame)
				return storedGameState.gameState;
		}
		return null;
	}
}

public class TestLogicScene : MonoBehaviour, GameLogic, GameStateProvider, CommandProcessor, CommandSender {

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

	public RecorderViewCanvas recorderView;

	ReplayController _replayController;

	CommandQueue _commandSender;

	public int gameFramesPerChecksumCheck = 10;

	GameStateBuilder _gameStateBuilder;

	ChecksumProvider _checksumProvider;

	[Range(1, 16)]
	public int replayPlaybackSpeedMultiplier = 1;

	#region GameStateProvider implementation

	public void SaveState (GameStateBuilder gameState)
	{
		gameState.StartObject ("Engine");
		gameState.SetInt ("frame", gameFixedUpdate.CurrentGameFrame);
		gameState.EndObject ();

		unit.Unit.SaveState (gameState);
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

	#region CommandSenderProcessor implementation

	public void SendEmpty ()
	{
		this.commandList.AddCommand(ConfigureCommand(new CommandBase()));
	}

	public void SendCommands (List<Command> commands)
	{
		foreach (var command in commands) {
			this.commandList.AddCommand (command);
		}
	}

	#endregion


	void Awake()
	{
		_gameStateBuilder = new GameStateStringBuilderImpl ();

		commandList = new CommandsList();

//		ChecksumRecorder checksumRecorder = new ChecksumRecorder (new GameStateChecksumProvider (_gameStateBuilder, this));

		// TODO: set replay....

		gameFixedUpdate = new LockstepFixedUpdate (new CommandsListLockstepLogic(commandList, this));
		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;
		gameFixedUpdate.SetGameLogic (this);

		GameFixedUpdateDebug updateDebug = gameObject.AddComponent<GameFixedUpdateDebug> ();
		updateDebug.SetGameFixedUpdate (gameFixedUpdate);

		_checksumProvider = new GameStateChecksumProvider (_gameStateBuilder, this);

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.AddComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.checksumRecorder = new ChecksumRecorder(_checksumProvider);

		_commandSender = new CommandQueueBase (gameFixedUpdate, this);

		ResetGameState ();

		_replayController = new ReplayController (gameFixedUpdate, _checksumProvider, recorderView, commandList, new MyCustomReplay(_checksumProvider, _gameStateBuilder));
		_replayController.GameFramesPerChecksumCheck = gameFramesPerChecksumCheck;

		StartRecording ();

		// debug...

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
		if (_replayController.IsRecording) {
			StartPlayback ();
		} else {
			StartRecording ();
		}
	}

	void StartRecording()
	{
		_replayController.StartRecording ();

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.GetComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.Reset ();
	}

	void StartPlayback()
	{
		ResetGameState ();
		_replayController.StartPlayback ();
	}
	
	// Update is called once per frame
	void Update () {

		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;

		if (Input.GetKeyUp (KeyCode.S)) {
			var stateBuilder = new GameStateStringBuilderImpl ();
			SaveState (stateBuilder);
			Debug.Log ((stateBuilder.GetGameState() as GameStateStringImpl).State);
		}
	
		if (Input.GetKeyUp (KeyCode.P)) {
			StartPlayback ();
			return;
		}

		if (Input.GetKeyUp (KeyCode.R)) {

			// resets game fixed update state...
			StartRecording();

			return;
		}

		if (_replayController.IsRecording) {

			gameFixedUpdate.Update (Time.deltaTime);

			if (Input.GetMouseButtonUp (1)) {
				Vector2 position = camera.ScreenToWorldPoint (Input.mousePosition);
			
				_commandSender.EnqueueCommand (ConfigureCommand (new MoveCommand (position)));

				feedbackClick.ShowFeedback (position);
			}

			if (Input.touchCount > 0) {
		
				if (Input.GetTouch (0).phase == TouchPhase.Ended) {
					Vector2 position = camera.ScreenToWorldPoint (Input.GetTouch (0).position);

					_commandSender.EnqueueCommand (ConfigureCommand (new MoveCommand (position)));

					feedbackClick.ShowFeedback (position);
				}

			}
				
		} else {
		
			// playback...

			// if already at last frame, then dont update anymore...
			if (_replayController.IsFinished())
				return;

			gameFixedUpdate.Update (Time.deltaTime * replayPlaybackSpeedMultiplier);

//			_replay.ReplayCommands ();
		}
	}
		
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

	readonly ChecksumValidator _checkumValidator = new ChecksumValidatorBasic();

	public void GameUpdate (float dt, int frame)
	{
		if (_commandSender.IsReady ())
			_commandSender.SendCommands ();

		if (!_replayController.IsRecording) {
			var isValid = _checkumValidator.IsValid (frame, _checksumProvider.CalculateChecksum (), _replayController.Replay.StoredChecksums);

			if (isValid)
				Debug.LogFormat ("State for frame {0} is: {1}", frame, "VALID");
			else { 
				Debug.LogWarningFormat ("State for frame {0} is: {1}", frame, "INVALID");

				var myCustomReplay = _replayController.Replay as MyCustomReplay;

				var storedGameState = myCustomReplay.GetStoredGameState (frame) as GameStateStringImpl;
				var currentGameState = _gameStateBuilder.GetGameState () as GameStateStringImpl;

				Debug.LogWarningFormat ("Stored gamestate: {0}", storedGameState.State);
				Debug.LogWarningFormat ("Current gamestate: {0}", currentGameState.State);

			}
			//			var storedChecksums = _replayController.Replay.StoredChecksums;

		}

		_replayController.GameUpdate (dt, frame);
	


		unit.Unit.GameUpdate (dt, frame);
	}

	#endregion
}
