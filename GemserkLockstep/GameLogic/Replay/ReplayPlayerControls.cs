using System.Collections.Generic;

namespace Gemserk.Lockstep.Replays
{
	public enum ReplayPlayerControlsState {
		Paused,
		Playing, 
		Seeking
	}

	public interface ReplayPlayerControls
	{
		ReplayPlayerControlsState State { get; }

		float PlaybackSpeed { get; set; }

		bool IsPaused();

		void Play();

		void Pause();

		void Seek(float time);

		float GetPlaybackTime();

		void Update (float dt);
	}

	//	public interface GameStateLoader
	//	{
	//		void Load(GameState gameState);
	//	}

	public interface GameReplayPlayer
	{
		/// <summary>
		/// Resets the game to the initial state.
		/// </summary>
		void Reset();

		/// <summary>
		/// Returns the maximum allowed update time.
		/// </summary>
		float GetMaxAllowedUpdateTime();

		float GetTotalTime();

		void Update(float dt);
	}

	public class ReplayPlayerControlsImplementation : ReplayPlayerControls
	{
		GameReplayPlayer _gameReplayPlayer;

		ReplayPlayerControlsState _state;

		float _playbackSpeed;
		float _playbackTime;

		float _seekTime;

		bool _init;

		public ReplayPlayerControlsImplementation(GameReplayPlayer gameReplayPlayer)
		{
			_gameReplayPlayer = gameReplayPlayer;
			_playbackSpeed = 1.0f;
			_init = false;
		}

		#region IReplayPlayer implementation

		public ReplayPlayerControlsState State { 
			get { 
				return _state;	
			}
		}

		public float PlaybackSpeed
		{
			get {
				return _playbackSpeed;
			}
			set { 
				_playbackSpeed = value;
			}
		}

		public bool IsPaused()
		{
			return _state == ReplayPlayerControlsState.Paused;
		}

		public void Play ()
		{
			if (!_init) {
				_gameReplayPlayer.Reset ();
				_init = true;
			}
			_state = ReplayPlayerControlsState.Playing;
		}

		public void Pause ()
		{
			_state = ReplayPlayerControlsState.Paused;
		}

		public void Seek (float seekTime)
		{
			_state = ReplayPlayerControlsState.Seeking;
			_seekTime = seekTime;

			if (_seekTime < _playbackTime) {
				_gameReplayPlayer.Reset ();
				_playbackTime = 0.0f;
			}
		}

		public float GetPlaybackTime ()
		{
			return _playbackTime;
		}

		public void Update (float dt)
		{
			if (IsPaused())
				return;

			float maxDt = _gameReplayPlayer.GetMaxAllowedUpdateTime ();

			if (_state == ReplayPlayerControlsState.Playing) {
				dt *= _playbackSpeed;

				if (dt > maxDt)
					dt = maxDt;
			} else if (_state == ReplayPlayerControlsState.Seeking) {
				dt = maxDt;

				if (_playbackTime + dt > _seekTime) {
					dt = _seekTime - _playbackTime;
					_state = ReplayPlayerControlsState.Paused;
				}
			}

			var totalTime = _gameReplayPlayer.GetTotalTime ();

			if (_playbackTime + dt > totalTime) {
				dt = totalTime - _playbackTime;
				_state = ReplayPlayerControlsState.Paused;
			}

			_playbackTime += dt;
			_gameReplayPlayer.Update (dt);
		}

		#endregion

	}

}