using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;

namespace Gemserk.Lockstep.Tests
{
	public interface ReplayPlayerControls
	{
		bool IsPaused();

		float PlaybackSpeed { get; set; }

		void Play();

		void Pause();

		void Seek(float time);

		float GetPlaybackTime();

		float GetTotalTime();

		void Update (float dt);
	}

	public interface GameStateLoader
	{
		void Load(GameState gameState);
	}

	public interface GameUpdater
	{
		// max allowed time to avoid spiral of death?

		// float GetMaxUpdateTime(float dt);

		void Update(float dt);
	}

	public interface GameReplayPlayer
	{
		void Reset();

		float GetMaxAllowedUpdateTime();

		void Update(float dt);
	}

	// TODO: separate playback timeline from replay commands and stuff logic

	public class MyReplayPlayer : ReplayPlayerControls
	{
		GameReplayPlayer _gameReplayPlayer;

		bool _paused;
		float _playbackSpeed;
		float _playbackTime;

		bool _init;

		public MyReplayPlayer(GameReplayPlayer gameReplayPlayer)
		{
			_gameReplayPlayer = gameReplayPlayer;
			_playbackSpeed = 1.0f;
			_init = false;
		}

		#region IReplayPlayer implementation

		public bool IsPaused ()
		{
			return _paused;
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

		public void Play ()
		{
			if (!_init) {
				_gameReplayPlayer.Reset ();
				_init = true;
			}
			_paused = false;	
		}

		public void Pause ()
		{
			_paused = true;
		}

		public void Seek (float time)
		{
			throw new System.NotImplementedException ();
		}

		public float GetPlaybackTime ()
		{
			return _playbackTime;
		}

		public float GetTotalTime ()
		{
			throw new System.NotImplementedException ();
		}

		public void Update (float dt)
		{
			if (_paused)
				return;

			float maxDt = _gameReplayPlayer.GetMaxAllowedUpdateTime ();

			if (dt > maxDt)
				dt = maxDt;

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
		public void TestSeekShouldRestartIfBeforeCurrentTime()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new MyReplayPlayer (gameReplay);

			replayPlayer.Play ();


		

		}

		// TEST: speed up playback time...

		// TEST: dont update over total time
	}
	
}
