using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;

namespace Gemserk.Lockstep.Tests
{
	public class TestReplayPlayer
	{
		[Test]
		public void ReplayShouldntQueueSameCommandMultipleTimes(){
		
			var commands = NSubstitute.Substitute.For<Commands> ();
			var replay = NSubstitute.Substitute.For<Replay> ();

			ReplayPlayer replayPlayer = new ReplayPlayer (replay, commands);

			replay.When(r => r.GetStoredCommands(5, Arg.Any<List<Command>>())).Do(args => {
				List<Command> list = args.ArgAt<List<Command>>(1);
				list.Add(new CommandBase(){
					ProcessFrame = 13,
				});
			});

			replayPlayer.Replay (5);

			commands.Received ().AddCommand (Arg.Is<Command>(c => c.ProcessFrame == 13));

			replay.When(r => r.GetStoredCommands(10, Arg.Any<List<Command>>())).Do(args => {

			});

			commands.ClearReceivedCalls ();

			replayPlayer.Replay (10);

			commands.DidNotReceive ().AddCommand (Arg.Any<Command> ());
		}
	}
}
