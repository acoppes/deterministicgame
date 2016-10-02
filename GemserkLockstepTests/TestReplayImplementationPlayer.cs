using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep.Replays;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;

namespace Gemserk.Lockstep.Tests
{
	public class TestReplayImplementationPlayer
	{

		[Test]
		public void TestPlayResetsGameToInitialState()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);

			replayPlayer.Play();

			gameReplay.Received ().Reset ();
		}

		[Test]
		public void TestPlayShouldntResetGameTwice()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);

			replayPlayer.Play();

			gameReplay.ClearReceivedCalls ();

			replayPlayer.Play();

			gameReplay.DidNotReceive ().Reset ();
		}

		[Test]
		public void TestReplayPlayerBasicAPI()
		{
			var gameReplay = NSubstitute.Substitute.For<GameReplayPlayer> ();

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);
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

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);
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

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);
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

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);

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

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);

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

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);
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

			var replayPlayer = new ReplayPlayerControlsImplementation (gameReplay);

			replayPlayer.Play ();
			replayPlayer.Update (5.0f);

			Assert.That (replayPlayer.GetPlaybackTime(), Is.EqualTo (2.0f));
			Assert.That (replayPlayer.State, Is.EqualTo (ReplayPlayerControlsState.Paused));
		}

	
	}
	
}
