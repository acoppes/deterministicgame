using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using System.Collections.Generic;

public class TestCommandSender {

	class CommandSenderProcessorMock : CommandSender
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
		var realCommandSender = NSubstitute.Substitute.For<CommandSender> ();

		lockstep.IsLastFrameForNextLockstep ().Returns (false);

		CommandQueue sender = new CommandQueueBase (lockstep, realCommandSender);

		Assert.That (sender.IsReady (), Is.False);
		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();

		lockstep.IsLastFrameForNextLockstep ().Returns (true);

		Assert.That (sender.IsReady (), Is.True);
		lockstep.ReceivedWithAnyArgs ().IsLastFrameForNextLockstep ();
	}

	[Test]
	public void TestSendEmptyCommandIfLockstepAndNoCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var realCommandSender = new CommandSenderProcessorMock();

		lockstep.GetNextLockstepFrame ().Returns (5);

		CommandQueue sender = new CommandQueueBase (lockstep, realCommandSender);

		sender.SendCommands ();

		Assert.That (realCommandSender.emptyCalled, Is.True);
	}

	[Test]
	public void TestSendCommandIfLockstep (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var realCommandSender = new CommandSenderProcessorMock();

		var aCommand = new CommandBase();

		CommandQueue sender = new CommandQueueBase (lockstep, realCommandSender);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.True);
	}

	[Test]
	public void TestDontSendCommandTwice (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var realCommandSender = new CommandSenderProcessorMock();

		var aCommand = new CommandBase() ;

		CommandQueue sender = new CommandQueueBase (lockstep, realCommandSender);
		sender.EnqueueCommand (aCommand);

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.True);
		realCommandSender.Reset ();

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.False);
	}

	[Test]
	public void TestSendMultipleCommands (){

		var lockstep = NSubstitute.Substitute.For<LockstepUpdate> ();
		var realCommandSender = new CommandSenderProcessorMock();

		var aCommand = new CommandBase() ;
		var anotherCommand = new CommandBase() ;

		CommandQueue sender = new CommandQueueBase (lockstep, realCommandSender);
		sender.EnqueueCommand (aCommand);
		sender.EnqueueCommand (anotherCommand);

		sender.SendCommands ();

		Assert.That (realCommandSender.commands.Contains (aCommand), Is.True);
		Assert.That (realCommandSender.commands.Contains (anotherCommand), Is.True);
	}

}
