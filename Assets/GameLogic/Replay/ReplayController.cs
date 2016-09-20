using UnityEngine;
using System.Collections.Generic;
using Gemserk.Lockstep;

public interface ReplayView
{
	void StartRecording ();

	void StartPlayback ();

	void SetReplay (ReplayController replayController);
}

public interface Replay
{
	// GameState GetInitialGameStateState();

	// For replay playback

	int LastRecordedFrame { get; set; } 

	List<StoredChecksum> StoredChecksums { get; }

	void GetStoredCommands (int frame, List<Command> commands);

	// For replay recording 

	void RecordChecksum(int frame);

	void Record (float time, int frame, Command command);
}

public class ReplayBase : Replay
{
	readonly CommandsRecorder _commandsRecorder;

	readonly ChecksumRecorder _checksumRecorder;

	public int LastRecordedFrame {
		get;
		set;
	}

	public ReplayBase(ChecksumProvider checksumProvider)
	{
		_commandsRecorder = new CommandsRecorder();
		_checksumRecorder = new ChecksumRecorder(checksumProvider);
	}

	#region Replay implementation

	public List<StoredChecksum> StoredChecksums {
		get {
			return _checksumRecorder.StoredChecksums;
		}
	}

	public void GetStoredCommands (int frame, List<Command> commands)
	{
		_commandsRecorder.GetCommandsForFrame (frame, commands);
	}

	public void RecordChecksum (int frame)
	{
		_checksumRecorder.RecordState (frame);
	}

	public void Record (float time, int frame, Command command)
	{
		_commandsRecorder.AddCommand (time, frame, command);
		LastRecordedFrame = frame;
	}

	#endregion
}

public class ReplayController : GameLogic
{
	readonly GameFixedUpdate _gameFixedUpdate;

	readonly Replay _replay;

	readonly ReplayRecorder _replayRecorder;
	readonly ReplayPlayer _replayPlayer;

	readonly ReplayView _recorderView;

	readonly Commands _commands;

	ChecksumValidator _checksumValidator;

//	readonly ChecksumProvider _checksumProvider;

	bool _recording;

	int gameFramesPerChecksumCheck = 10;

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
			return (float) _gameFixedUpdate.CurrentGameFrame / (float) _replay.LastRecordedFrame;
		}
	}

	public ReplayController(GameFixedUpdate gameFixedUpdate, ChecksumProvider checksumProvider, ReplayView recorderView, Commands commands)
	{
		_recording = true;

//		_checksumProvider = checksumProvider;

		_replay = new ReplayBase (checksumProvider);

		_gameFixedUpdate = gameFixedUpdate;
		_recorderView = recorderView;
		_commands = commands;

		if (_recorderView != null)
			_recorderView.SetReplay (this);

		_replayRecorder = new ReplayRecorder (_replay, _commands);
		_replayPlayer = new ReplayPlayer (checksumProvider, _commands);
	}

	public void StartPlayback()
	{
		_recording = false;

		if (_recorderView != null)
			_recorderView.StartPlayback ();

		_checksumValidator = new ChecksumValidatorBasic (_replay.StoredChecksums);
	}

	public void StartRecording()
	{
		_recording = true;

		if (_recorderView != null)
			_recorderView.StartRecording();

		// we could have a Reset() for the replay system if we want to start from 0...

		// _checksumRecorder.Reset ();
	}

	public bool IsFinished()
	{
		return _replay.LastRecordedFrame == _gameFixedUpdate.CurrentGameFrame;
	}

	bool IsChecksumFrame(int frame)
	{
		return (frame % gameFramesPerChecksumCheck) == 0;
	}

	#region GameLogic implementation
	public void GameUpdate (float dt, int frame)
	{
		bool isChecksumFrame = IsChecksumFrame (frame);

		if (_recording) {
			_replayRecorder.Record (_gameFixedUpdate.GameTime, _gameFixedUpdate.CurrentGameFrame, isChecksumFrame);
		} else {
			_replayPlayer.Replay (_gameFixedUpdate.CurrentGameFrame, isChecksumFrame, _checksumValidator);
		}
	}
	#endregion
}

public class ReplayPlayer
{
	readonly Replay _replay;

	readonly Commands _commands;

	readonly ChecksumProvider _checksumProvider;

	int gameFramesPerChecksumCheck = 10;

	public int GameFramesPerChecksumCheck {
		get {
			return gameFramesPerChecksumCheck;
		}
		set {
			gameFramesPerChecksumCheck = value;
		}
	}

	public ReplayPlayer(ChecksumProvider checksumProvider, Commands commands)
	{
		_checksumProvider = checksumProvider;
		_replay = new ReplayBase (checksumProvider);
		_commands = commands;
	}

	void ReplayCommands(int frame)
	{
		List<Command> recordedCommands = new List<Command> ();

		_replay.GetStoredCommands(frame, recordedCommands);

		for (int i = 0; i < recordedCommands.Count; i++) {
			var command = recordedCommands [i];
			_commands.AddCommand (command);
		}
	}
		
	#region GameLogic implementation
	public void Replay (int frame, bool isChecksumFrame, ChecksumValidator checksumValidator)
	{
		ReplayCommands (frame);

		if (isChecksumFrame) {
			bool validState = checksumValidator.IsValid (frame, _checksumProvider.CalculateChecksum ());

			#if DEBUG
			Debug.Log (string.Format ("State({0}): is {1}", frame, validState ? "valid" : "invalid!"));
			#endif
		}
	}
	#endregion
}

public class ReplayRecorder
{
	readonly Replay _replay;

	readonly Commands _commands;

	public ReplayRecorder(Replay replay, Commands commands)
	{
		_replay = replay;
		_commands = commands;
	}
		
	readonly List<Command> _commandsToRecord = new List<Command>();

	void RecordCommands(float time, int frame)
	{
		_commandsToRecord.Clear ();

		_commands.GetCommands (_commandsToRecord);

		for (int i = 0; i < _commandsToRecord.Count; i++) {
			var command = _commandsToRecord [i];
			_replay.Record (time, frame, command);
		}

		_commandsToRecord.Clear ();
	}
		
	public void Record (float time, int frame, bool isChecksumFrame)
	{
		RecordCommands (time, frame);

		if (isChecksumFrame)
			_replay.RecordChecksum (frame);
	}
}