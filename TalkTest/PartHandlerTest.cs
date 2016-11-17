using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class PartHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestPart()
        {
            var partHandler = new PartHandler();
            var msg = new MsgEventArgs {Command = "PART", Data = new[] {"test", "Hello, world!"}, Meta = "a!b@c"};
            var visited = false;
            Caller.part = new Action<PartAction>(res =>
            {
                Assert.AreEqual("test", res.ChatNames.First());
                Assert.AreEqual("Hello, world!", res.Message);
                var user = res.User;
                Assert.AreEqual("a", user.Nick);
                Assert.AreEqual("b", user.Name);
                Assert.AreEqual("c", user.Host);
                visited = true;
            });
            partHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestPartWithoutMessage()
        {
            var partHandler = new PartHandler();
            var msg = new MsgEventArgs {Command = "PART", Data = new[] {"test"}, Meta = "a!b@c"};
            var visited = false;
            Caller.part = new Action<PartAction>(res => { visited = true; });
            partHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestPartSelfShouldKillChat()
        {
            Talker.Nick = "a";
            Talker.Chats.GetOrCreate("test");
            var partHandler = new PartHandler();
            Assert.IsNotNull(Talker.Chats["test"]);
            var msg = new MsgEventArgs {Command = "PART", Data = new[] {"test", "Hello, world!"}, Meta = "a!b@c"};
            Caller.part = new Action<PartAction>(res => { });
            Caller.closeChat = new Action<string>(res => { });
            partHandler.Msg(Talker, Caller, msg);
            Assert.IsNull(Talker.Chats.FirstOrDefault(c => c.Name == "test"));
        }

        [TestMethod]
        public void TestPartRemoveName()
        {
            var partHandler = new PartHandler();
            var msg = new MsgEventArgs {Command = "PART", Data = new[] {"test", "Hello, world!"}, Meta = "a!b@c"};
            Caller.part = new Action<PartAction>(res => { });
            AddChatUsers("test", "a", "b", "c");
            partHandler.Msg(Talker, Caller, msg);
            CollectionAssert.AreEqual(new[] {"b", "c"}, Talker.Chats["test"].Users.Select(u => u.IrcUser.Nick).ToArray());
        }

        [TestMethod]
        public void TestCloseChannel()
        {
            var partHandler = new PartHandler();
            Talker.Nick = "a";
            Talker.Chats.GetOrCreate("test");
            Assert.AreEqual("test", Talker.Chats["test"].Name);
            var msg = new MsgEventArgs {Command = "PART", Data = new[] {"test", "Hello, world!"}, Meta = "a!b@c"};
            var visited = false;
            Caller.part = new Action<PartAction>(res => { });
            Caller.closeChat = new Action<string>(res =>
            {
                Assert.AreEqual("test", res);
                visited = true;
            });
            partHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
            Assert.IsNull(Talker.Chats.FirstOrDefault(c => c.Name == "test"));
            Assert.AreEqual("PART", partHandler.ForCommand());
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Part"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnPart()
        {
            var partHandler = new PartHandler();
            Assert.AreEqual("PART", partHandler.ForCommand());
        }
    }
}