using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class MessageHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestMessage()
        {
            var messageHandler = new MessageHandler();
            var msg = new MsgEventArgs {Command = "PRIVMSG", Data = new[] {"test", "Hello, world!"}, Meta = "a!b@c"};
            var visited = false;
            Caller.message = new Action<MessageAction>(res => { visited = true; });
            messageHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestUnreadMessages()
        {
            var messageHandler = new MessageHandler();
            var msg = new MsgEventArgs {Command = "PRIVMSG", Data = new[] {"test", "Hello, world!"}, Meta = "a!b@c"};
            var visited = false;
            Caller.message = new Action<MessageAction>(res => { visited = true; });
            messageHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
            Assert.AreEqual(1, Talker.Chats["test"].UnreadMessages);
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Message"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnMessage()
        {
            var messageHandler = new MessageHandler();
            Assert.AreEqual("PRIVMSG", messageHandler.ForCommand());
        }
    }
}