using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using System.Collections.Generic;

public class TestCommandSender {

	class CommandSenderProcessorMock : CommandSenderProcessor
	{
		public bool emptyCalled;

		public List<Command> commands = new List<Command>();

		#region CommandSenderProcessor implementation
		public void SendEmpty ()
		{
			emptyCalled = true;
		}

		public void SendCommands (List<Command> commands)
		{
			this.commands.Clear ();
			this.commands.AddRange (commands);
		}
		#endregion

		public void Reset()
		{
			emptyCalled = false;
			commands.Clear ();
		}
		
	}

	[Test]
	public void TestIsReadyWhenLockstep (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var realCommandSender = NSubstitute.Substitute.For<CommandSenderProcessor> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (false);

		CommandSender sender = new CommandSenderBase (lockstep, realCommandSender);

		Assert.That (sender.IsReady (), Is.False);
		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);

		Assert.That (sender.IsReady (), Is.True);
		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
	}

	[Test]
	public void TestSendEmptyCommandIfLockstepAndNoCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
//		var realCommandSender = NSubstitute.Substitute.For<CommandSenderProcessor> ();
		var realCommandSender = new CommandSenderProcessorMock();

		lockstep.GetNextLockstepFrame ().Returns (5);

		CommandSender sender = new CommandSenderBase (lockstep, realCommandSender);

		sender.SendCommands ();

		Assert.That (realCommandSender.emptyCalled, Is.True);

//		realCommandSender.Received ().SendEmpty ();
//		commands.Received ().AddCommand (Arg.Is<Command> (c => c == emptyCommand && c.ProcessFrame == 5));
	}

	[Test]
	public void TestSendCommandIfLockstep (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
//		var realCommandSender = NSubstitute.Substitute.For<CommandSenderProcessor> ();
		var realCommandSender = new CommandSenderProcessorMock();

//		var emptyCommand = new CommandBase ();
//		realCommandSender.GetEmptyCommand ().Returns (emptyCommand);

		var aCommand = new CommandBase();

		CommandSender sender = new CommandSenderBase (lockstep, realCommandSender);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.True);

//		realCommandSender.Received().SendCommands(Arg.Is<List<Command>>(t => t.Contains(aCommand)));
//		realCommandSender.Received().SendCommands(Arg.Is<List<Command>>(new List<Command>() { aCommand }));

//		commands.Received ().AddCommand (aCommand);
//		commands.DidNotReceive ().AddCommand (emptyCommand);
	}

	[Test]
	public void TestDontSendCommandTwice (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
//		var realCommandSender = NSubstitute.Substitute.For<CommandSenderProcessor> ();
		var realCommandSender = new CommandSenderProcessorMock();

//		var emptyCommand = new CommandBase ();
//		realCommandSender.GetEmptyCommand ().Returns (emptyCommand);

		var aCommand = new CommandBase() ;

		CommandSender sender = new CommandSenderBase (lockstep, realCommandSender);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

//		realCommandSender.Received ().SendCommands (Arg.Is<List<Command>> (t => t.Contains (aCommand)));
//		commands.Received ().AddCommand (aCommand);
		Assert.That (realCommandSender.commands.Contains (aCommand), Is.True);
		realCommandSender.Reset ();

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.False);
//		realCommandSender.DidNotReceiveWithAnyArgs().SendCommands(Arg.Any<List<Command>>());
//		commands.Received ().AddCommand (emptyCommand);
	}

	[Test]
	public void TestSendMultipleCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
//		var realCommandSender = NSubstitute.Substitute.For<CommandSenderProcessor> ();
		var realCommandSender = new CommandSenderProcessorMock();

//		var emptyCommand = new CommandBase ();
//		realCommandSender.GetEmptyCommand ().Returns (emptyCommand);

		var aCommand = new CommandBase() ;
		var anotherCommand = new CommandBase() ;

		CommandSender sender = new CommandSenderBase (lockstep, realCommandSender);
		sender.EnqueueCommand (aCommand);
		sender.EnqueueCommand (anotherCommand);

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.True);
		Assert.That (realCommandSender.commands.Contains (anotherCommand), Is.True);

//		realCommandSender.Received ().SendCommands (Arg.Is<List<Command>> (t => t.Contains (aCommand) && t.Contains(anotherCommand)));

//		commands.Received ().AddCommand (aCommand);
//		commands.Received ().AddCommand (anotherCommand);
	}

}
