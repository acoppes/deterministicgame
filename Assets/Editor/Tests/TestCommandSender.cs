using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;

public class TestCommandSender {

	[Test]
	public void TestIsReadyWhenLockstep (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();
		var commandProvider = NSubstitute.Substitute.For<CommandEmptyProvider> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (false);

		CommandSender sender = new CommandSenderBase (lockstep, commands, commandProvider);

		Assert.That (sender.IsReady (), Is.False);
		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);

		Assert.That (sender.IsReady (), Is.True);
		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
	}

	[Test]
	public void TestSendEmptyCommandIfLockstepAndNoCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();
		var commandProvider = NSubstitute.Substitute.For<CommandEmptyProvider> ();

		lockstep.GetNextLockstepFrame ().Returns (5);

		var emptyCommand = new CommandBase ();
		commandProvider.GetEmptyCommand ().Returns (emptyCommand);

		CommandSender sender = new CommandSenderBase (lockstep, commands, commandProvider);

		sender.SendCommands ();

		commands.Received ().AddCommand (Arg.Is<Command> (c => c == emptyCommand && c.ProcessFrame == 5));
	}

	[Test]
	public void TestSendCommandIfLockstep (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();
		var commandProvider = NSubstitute.Substitute.For<CommandEmptyProvider> ();

		var emptyCommand = new CommandBase ();
		commandProvider.GetEmptyCommand ().Returns (emptyCommand);

		var aCommand = new CommandBase() ;

		CommandSender sender = new CommandSenderBase (lockstep, commands, commandProvider);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		commands.Received ().AddCommand (aCommand);
		commands.DidNotReceive ().AddCommand (emptyCommand);
	}

	[Test]
	public void TestDontSendCommandTwice (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();
		var commandProvider = NSubstitute.Substitute.For<CommandEmptyProvider> ();

		var emptyCommand = new CommandBase ();
		commandProvider.GetEmptyCommand ().Returns (emptyCommand);

		var aCommand = new CommandBase() ;

		CommandSender sender = new CommandSenderBase (lockstep, commands, commandProvider);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		commands.Received ().AddCommand (aCommand);

		sender.SendCommands ();

		commands.Received ().AddCommand (emptyCommand);
	}

	[Test]
	public void TestSendMultipleCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();
		var commandProvider = NSubstitute.Substitute.For<CommandEmptyProvider> ();

		var emptyCommand = new CommandBase ();
		commandProvider.GetEmptyCommand ().Returns (emptyCommand);

		var aCommand = new CommandBase() ;
		var anotherCommand = new CommandBase() ;

		CommandSender sender = new CommandSenderBase (lockstep, commands, commandProvider);
		sender.EnqueueCommand (aCommand);
		sender.EnqueueCommand (anotherCommand);

		sender.SendCommands ();

		commands.Received ().AddCommand (aCommand);
		commands.Received ().AddCommand (anotherCommand);
	}

}
