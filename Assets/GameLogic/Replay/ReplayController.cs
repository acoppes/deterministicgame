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

	List<StoredChecksum> StoredChecksums { get; }

	void RecordChecksum(int frame);

	void GetStoredCommands (int frame, List<Command> commands);

	void Record (float time, int frame, Command command);
}

public class ReplayBase : Replay
{
	readonly CommandsRecorder _commandsRecorder;

	readonly ChecksumRecorder _checksumRecorder;

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
	}

	#endregion
}

public class ReplayController : GameLogic
{
	readonly GameFixedUpdate _gameFixedUpdate;

	readonly Replay _replay;

	readonly ReplayView _recorderView;

	readonly Commands _commands;

	ChecksumValidator _checksumValidator;

	ChecksumProvider _checksumProvider;

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

	public ReplayController(GameFixedUpdate gameFixedUpdate, ChecksumProvider checksumProvider, ReplayView recorderView, Commands commands)
	{
		_recording = true;
		_lastRecordedGameFrame = 0;

		_checksumProvider = checksumProvider;

		_replay = new ReplayBase (checksumProvider);

		_gameFixedUpdate = gameFixedUpdate;
		_recorderView = recorderView;
		_commands = commands;

		if (_recorderView != null)
			_recorderView.SetReplay (this);
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

	readonly List<Command> _commandsToRecord = new List<Command>();

	public void RecordCommands()
	{
		_commandsToRecord.Clear ();

		_commands.GetCommands (_commandsToRecord);

		for (int i = 0; i < _commandsToRecord.Count; i++) {
			var command = _commandsToRecord [i];
			_replay.Record (_gameFixedUpdate.GameTime, _gameFixedUpdate.CurrentGameFrame, command);
		}

		_commandsToRecord.Clear ();
	}

	public bool IsFinished()
	{
		return _lastRecordedGameFrame == _gameFixedUpdate.CurrentGameFrame;
	}

	public void ReplayCommands()
	{
		List<Command> recordedCommands = new List<Command> ();

		_replay.GetStoredCommands(_gameFixedUpdate.CurrentGameFrame, recordedCommands);

		for (int i = 0; i < recordedCommands.Count; i++) {
			var command = recordedCommands [i];
			_commands.AddCommand (command);
		}

		// _commandsList.IsReady = true;
	}

	bool IsChecksumFrame(int frame)
	{
		return (frame % gameFramesPerChecksumCheck) == 0;
	}

	#region GameLogic implementation
	public void GameUpdate (float dt, int frame)
	{
		if (_recording) {
			RecordCommands ();

			_lastRecordedGameFrame = _gameFixedUpdate.CurrentGameFrame;

			if (IsChecksumFrame (frame)) {
				_replay.RecordChecksum (frame);
				//				_checksumRecorder.RecordState (frame);
			}
		} else {

			ReplayCommands ();

			if (IsChecksumFrame (frame)) {
				bool validState = _checksumValidator.IsValid (frame, _checksumProvider.CalculateChecksum ());
				#if DEBUG
				Debug.Log (string.Format ("State({0}): is {1}", frame, validState ? "valid" : "invalid!"));
				#endif
			}
		}
	}
	#endregion
}