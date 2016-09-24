namespace Gemserk.Lockstep 
{		
	public class ReplayController : GameLogic
	{
		readonly GameFixedUpdate _gameFixedUpdate;

		readonly Replay _replay;

		readonly ReplayRecorder _replayRecorder;
		readonly ReplayPlayer _replayPlayer;

		readonly ReplayView _recorderView;

		readonly Commands _commands;

		readonly ChecksumProvider _checksumProvider;

		ChecksumValidator _checksumValidator;

		bool _recording;

		int gameFramesPerChecksumCheck = 10;

		public Replay Replay {
			get {
				return _replay;
			}
		}

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

		public ReplayController(GameFixedUpdate gameFixedUpdate, ChecksumProvider checksumProvider, ReplayView recorderView, Commands commands, Replay replay)
		{
			_recording = true;

			_checksumProvider = checksumProvider;

			_replay = replay;

//			_replay = new ReplayBase (checksumProvider);

			_gameFixedUpdate = gameFixedUpdate;
			_recorderView = recorderView;
			_commands = commands;

			if (_recorderView != null)
				_recorderView.SetReplay (this);

			_replayRecorder = new ReplayRecorder (_replay, _commands);
			_replayPlayer = new ReplayPlayer (_replay, _commands);
		}

		public void StartPlayback()
		{
			_recording = false;

			if (_recorderView != null)
				_recorderView.StartPlayback ();

			_checksumValidator = new ChecksumValidatorBasic ();
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
			return _replay.LastRecordedFrame <= _gameFixedUpdate.CurrentGameFrame;
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
				_replayRecorder.Record (_gameFixedUpdate.CurrentGameFrame, isChecksumFrame);
			} else {
				_replayPlayer.Replay (_gameFixedUpdate.CurrentGameFrame);

				if (isChecksumFrame) {
					bool validState = _checksumValidator.IsValid (frame, _checksumProvider.CalculateChecksum (), _replay.StoredChecksums);
				}
			}
		}
		#endregion
	}


}