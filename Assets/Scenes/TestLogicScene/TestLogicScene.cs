using UnityEngine;
using System.Collections.Generic;
using Gemserk.Lockstep;

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

	ReplayController _replay;

	CommandQueue _commandSender;

	public int gameFramesPerChecksumCheck = 10;

	GameStateBuilder _gameStateBuilder;

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

		ChecksumRecorder checksumRecorder = new ChecksumRecorder (new GameStateChecksumProvider (_gameStateBuilder, this));

		// TODO: set replay....

		gameFixedUpdate = new LockstepFixedUpdate (new CommandsListLockstepLogic(commandList, this));
		gameFixedUpdate.GameFramesPerLockstep = gameFramesPerLockstep;
		gameFixedUpdate.FixedStepTime = fixedTimestepMilliseconds / 1000.0f;
		gameFixedUpdate.SetGameLogic (this);

		GameFixedUpdateDebug updateDebug = gameObject.AddComponent<GameFixedUpdateDebug> ();
		updateDebug.SetGameFixedUpdate (gameFixedUpdate);

		ChecksumRecorderDebug checksumRecorderDebug = gameObject.AddComponent<ChecksumRecorderDebug> ();
		checksumRecorderDebug.checksumRecorder = checksumRecorder;

		_commandSender = new CommandQueueBase (gameFixedUpdate, this);

		ResetGameState ();

		_replay = new ReplayController (gameFixedUpdate, checksumRecorder, recorderView, commandList);
		_replay.GameFramesPerChecksumCheck = gameFramesPerChecksumCheck;

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

		if (_replay.IsRecording) {

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
			if (_replay.IsFinished())
				return;

			gameFixedUpdate.Update (Time.deltaTime);

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

	public void GameUpdate (float dt, int frame)
	{
		if (_commandSender.IsReady ())
			_commandSender.SendCommands ();

		_replay.GameUpdate (dt, frame);

		unit.Unit.GameUpdate (dt, frame);
	}

	#endregion
}
