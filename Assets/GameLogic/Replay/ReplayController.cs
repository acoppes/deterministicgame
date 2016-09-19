using UnityEngine;
using System.Collections.Generic;
using Gemserk.Lockstep;

public interface ReplayView
{
	void StartRecording ();

	void StartPlayback ();

	void SetReplay (ReplayController replayController);
}
	
public class ReplayController : GameLogic
{
	readonly GameFixedUpdate _gameFixedUpdate;

	readonly CommandsRecorder _commandsRecorder;

	readonly ReplayView _recorderView;

	readonly ChecksumRecorder _checksumRecorder;

	readonly Commands _commands;

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

	public ReplayController(GameFixedUpdate gameFixedUpdate, ChecksumRecorder checksumRecorder, ReplayView recorderView, Commands commands)
	{
		_recording = true;
		_lastRecordedGameFrame = 0;
		_commandsRecorder = new CommandsRecorder ();
		_checksumRecorder = checksumRecorder;
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

		_checksumValidator = new ChecksumValidatorBasic (_checksumRecorder.StoredChecksums);
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
			_commandsRecorder.AddCommand (_gameFixedUpdate.GameTime, _gameFixedUpdate.CurrentGameFrame, command);
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

		_commandsRecorder.GetCommandsForFrame(_gameFixedUpdate.CurrentGameFrame, recordedCommands);

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
		ChecksumProvider checksumProvider = _checksumRecorder.ChecksumProvider;

		if (_recording) {
			RecordCommands ();

			_lastRecordedGameFrame = _gameFixedUpdate.CurrentGameFrame;

			if (IsChecksumFrame (frame)) {
				_checksumRecorder.RecordState (frame);
			}
		} else {

			ReplayCommands ();

			if (IsChecksumFrame (frame)) {
				bool validState = _checksumValidator.IsValid (frame, checksumProvider.CalculateChecksum ());
				#if DEBUG
				Debug.Log (string.Format ("State({0}): is {1}", frame, validState ? "valid" : "invalid!"));
				#endif
			}
		}
	}
	#endregion
}
