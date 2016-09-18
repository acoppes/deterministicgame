using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;

public class TestCommandSender {

	[Test]
	public void TestShouldNotSendCommandsBeforeLastFrame (){
	
		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (false);

		var emptyCommand = new CommandBase ();

		CommandSender sender = new CommandSender (lockstep, commands, emptyCommand);
	
		sender.SendCommands ();

		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
		commands.DidNotReceiveWithAnyArgs ().AddCommand (null);
	}

	[Test]
	public void TestSendEmptyCommandIfLockstepAndNoCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);
		lockstep.GetCurrentFrame().Returns (2);
		lockstep.GetNextLockstepFrame ().Returns (5);

		var emptyCommand = new CommandBase ();

		CommandSender sender = new CommandSender (lockstep, commands, emptyCommand);

		sender.SendCommands ();

		lockstep.Received ().IsLastFrameForNextLockstep ();
		// commands.Received ().AddCommand (emptyCommand);

		commands.Received ().AddCommand (Arg.Is<Command> (c => c == emptyCommand && c.ProcessFrame == 5 && c.CreationFrame == 2));

		//  .Add(Arg.Any<int>(), Arg.Is<int>(x => x >= 500));
	}

	[Test]
	public void TestSendCommandIfLockstep (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);

		var emptyCommand = new CommandBase ();
		var aCommand = new CommandBase() ;

		CommandSender sender = new CommandSender (lockstep, commands, emptyCommand);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
		commands.Received ().AddCommand (aCommand);
		commands.DidNotReceive ().AddCommand (emptyCommand);
	}

	[Test]
	public void TestDontSendCommandTwice (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);

		var emptyCommand = new CommandBase ();
		var aCommand = new CommandBase() ;

		CommandSender sender = new CommandSender (lockstep, commands, emptyCommand);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
		commands.Received ().AddCommand (aCommand);

		sender.SendCommands ();

		commands.Received ().AddCommand (emptyCommand);
	}

	[Test]
	public void TestSendMultipleCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var commands = NSubstitute.Substitute.For<Commands> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);

		var emptyCommand = new CommandBase ();

		var aCommand = new CommandBase() ;
		var anotherCommand = new CommandBase() ;

		CommandSender sender = new CommandSender (lockstep, commands, emptyCommand);
		sender.EnqueueCommand (aCommand);
		sender.EnqueueCommand (anotherCommand);

		sender.SendCommands ();

		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
		commands.Received ().AddCommand (aCommand);
		commands.Received ().AddCommand (anotherCommand);
	}

}
