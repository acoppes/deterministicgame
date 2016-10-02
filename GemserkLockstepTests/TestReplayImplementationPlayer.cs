using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;

namespace Gemserk.Lockstep.Tests
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
		void Reset();

		float GetMaxAllowedUpdateTime();

		float GetTotalTime();

		void Update(float dt);
	}

	// TODO: separate playback timeline from replay commands and stuff logic

	public class MyReplayPlayer : ReplayPlayerControls
	{
		GameReplayPlayer _gameReplayPlayer;

		ReplayPlayerControlsState _state;

		float _playbackSpeed;
		float _playbackTime;

		float _seekTime;

		bool _init;

		public MyReplayPlayer(GameReplayPlayer gameReplayPlayer)
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

	public class TestReplayImplementationPlayer
	{

		[Test]
		public void TestPlayResetsGameToInitialState()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new MyReplayPlayer (gameReplay);

			replayPlayer.Play();

			gameReplay.Received ().Reset ();
		}

		[Test]
		public void TestPlayShouldntResetGameTwice()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new MyReplayPlayer (gameReplay);

			replayPlayer.Play();

			gameReplay.ClearReceivedCalls ();

			replayPlayer.Play();

			gameReplay.DidNotReceive ().Reset ();
		}

		[Test]
		public void TestReplayPlayerBasicAPI()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new MyReplayPlayer (gameReplay);
			replayPlayer.Pause ();
			Assert.That (replayPlayer.IsPaused (), Is.True);

			replayPlayer.Play();
			Assert.That (replayPlayer.IsPaused (), Is.False);

			replayPlayer.PlaybackSpeed = 2.0f;
			Assert.That (replayPlayer.PlaybackSpeed, Is.EqualTo (2.0f));
		}

		[Test]
		public void TestGetPlaybackTimeWhenUpdateCalledAndNotPaused()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();
			gameReplay.GetTotalTime ().Returns (100.0f);
			gameReplay.GetMaxAllowedUpdateTime ().Returns (1.0f);

			var replayPlayer = new MyReplayPlayer (gameReplay);
			replayPlayer.Play();

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (0.0f));
	
			// cant receive bigger update than max allowed time...
			replayPlayer.Update (5.0f);
	
			gameReplay.Received ().GetMaxAllowedUpdateTime ();
			gameReplay.Received().Update (1.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (1.0f));

			replayPlayer.Update (1.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (2.0f));
		}
			
		[Test]
		public void TestReplayDontUpdateGameIfPaused()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new MyReplayPlayer (gameReplay);
			replayPlayer.Pause ();
			Assert.That (replayPlayer.IsPaused (), Is.True);

			replayPlayer.Update (1.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (0.0f));

			gameReplay.DidNotReceiveWithAnyArgs ().Update (Arg.Any<float> ());
		}

		[Test]
		public void TestSeekShouldUpdateAtMaxSpeedUntilSeekFinished()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();
			gameReplay.GetTotalTime ().Returns (100.0f);
			gameReplay.GetMaxAllowedUpdateTime ().Returns (1.0f);

			var replayPlayer = new MyReplayPlayer (gameReplay);

			replayPlayer.Play ();
		
			replayPlayer.Seek (1.8f);

			Assert.That (replayPlayer.State, Is.EqualTo (ReplayPlayerControlsState.Seeking));

			replayPlayer.Update (0.1f);

			Assert.That (replayPlayer.GetPlaybackTime(), Is.EqualTo (1.0f));

			replayPlayer.Update (0.1f);

			Assert.That (replayPlayer.GetPlaybackTime(), Is.EqualTo (1.8f));

			Assert.That (replayPlayer.State, Is.EqualTo (ReplayPlayerControlsState.Paused));
		}

		[Test]
		public void TestSeekResetStateIfBeforeCurrentTime()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();
			gameReplay.GetTotalTime ().Returns (100.0f);
			gameReplay.GetMaxAllowedUpdateTime ().Returns (1.0f);

			var replayPlayer = new MyReplayPlayer (gameReplay);

			replayPlayer.Play ();
			replayPlayer.Update (1.0f);

			gameReplay.ClearReceivedCalls ();

			replayPlayer.Seek (0.25f);

			gameReplay.Received ().Reset ();

			Assert.That (replayPlayer.State, Is.EqualTo (ReplayPlayerControlsState.Seeking));
			Assert.That (replayPlayer.GetPlaybackTime(), Is.EqualTo (0.0f));
		}

		[Test]
		public void TestPlaybackSpeedWhenUpdateCalled()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();
			gameReplay.GetTotalTime ().Returns (100.0f);
			gameReplay.GetMaxAllowedUpdateTime ().Returns (1.0f);

			var replayPlayer = new MyReplayPlayer (gameReplay);
			replayPlayer.PlaybackSpeed = 5.0f;

			replayPlayer.Play ();
			replayPlayer.Update (0.1f);

			gameReplay.Received ().Update (0.5f);

			gameReplay.ClearReceivedCalls ();

			// never recieve bigger than max allowed dt
			replayPlayer.Update (1.0f);

			gameReplay.Received ().Update (1.0f);
		}

		// TEST: dont update over total time

		[Test]
		public void TestPlaybackTimeNeverExceedsTotalTime()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();
			gameReplay.GetMaxAllowedUpdateTime ().Returns (100.0f);
			gameReplay.GetTotalTime ().Returns (2.0f);

			var replayPlayer = new MyReplayPlayer (gameReplay);

			replayPlayer.Play ();
			replayPlayer.Update (5.0f);

			Assert.That (replayPlayer.GetPlaybackTime(), Is.EqualTo (2.0f));
			Assert.That (replayPlayer.State, Is.EqualTo (ReplayPlayerControlsState.Paused));
		}

	
	}
	
}
