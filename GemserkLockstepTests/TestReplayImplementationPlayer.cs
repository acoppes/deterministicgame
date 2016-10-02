using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;

namespace Gemserk.Lockstep.Tests
{
	public interface IReplayPlayer
	{
		bool IsPaused();

		float GetPlaybackSpeed();

		void Play(float speed);

		void Pause();

		void Seek(float time);

		float GetPlaybackTime();

		float GetTotalTime();

		void Restart();

		void Update (float dt);
	}

	public interface GameStateLoader
	{
		void Load(GameState gameState);
	}

	public interface GameUpdater
	{
		void Update(float dt);
	}

	public class MyReplayPlayer : IReplayPlayer
	{
		Replay _replay;
		GameStateLoader _gameStateLoader;
		GameUpdater _gameUpdater;

		public MyReplayPlayer(Replay replay, GameStateLoader gameStateLoader, GameUpdater gameUpdater)
		{
			_replay = replay;
			_gameStateLoader = gameStateLoader;
			_gameUpdater = gameUpdater;
		}

		#region IReplayPlayer implementation
		public bool IsPaused ()
		{
			return true;
		}
		public float GetPlaybackSpeed ()
		{
			throw new System.NotImplementedException ();
		}
		public void Play (float speed)
		{
			throw new System.NotImplementedException ();
		}
		public void Pause ()
		{
			
		}
		public void Seek (float time)
		{
			throw new System.NotImplementedException ();
		}
		public float GetPlaybackTime ()
		{
			throw new System.NotImplementedException ();
		}
		public float GetTotalTime ()
		{
			throw new System.NotImplementedException ();
		}
		public void Restart ()
		{
			_gameStateLoader.Load (_replay.GetInitialGameState ());
		}

		public void Update (float dt)
		{
			
		}
		#endregion
		
	}

	public class TestReplayImplementationPlayer
	{

		[Test]
		public void TestRestartShouldLoadInitialGameStateFromReplay()
		{
			var replay = NSubstitute.Substitute.For<Replay> ();
			// var gameStateProvider = NSubstitute.Substitute.For<GameStateProvider> ();
			var gameStateLoader = NSubstitute.Substitute.For<GameStateLoader> ();
			var gameUpdater = NSubstitute.Substitute.For<GameUpdater> ();

			GameState customGameState = NSubstitute.Substitute.For<GameState> ();

			replay.GetInitialGameState ().Returns (customGameState);

			var replayPlayer = new MyReplayPlayer (replay, gameStateLoader, gameUpdater);

			replayPlayer.Restart ();

			gameStateLoader.Received ().Load (customGameState);
		}

		[Test]
		public void TestReplayDontUpdateGameIfPaused()
		{
			var replay = NSubstitute.Substitute.For<Replay> ();
			// var gameStateProvider = NSubstitute.Substitute.For<GameStateProvider> ();
			var gameStateLoader = NSubstitute.Substitute.For<GameStateLoader> ();
			var gameUpdater = NSubstitute.Substitute.For<GameUpdater> ();

			var replayPlayer = new MyReplayPlayer (replay, gameStateLoader, gameUpdater);

			replayPlayer.Pause ();
			Assert.That (replayPlayer.IsPaused (), Is.True);

			replayPlayer.Update (1.0f);

			gameUpdater.DidNotReceiveWithAnyArgs ().Update (Arg.Any<float> ());
		}
	}
	
}
