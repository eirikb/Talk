using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;
using Action = Talk.Handler.Action;

namespace TalkTest
{
    [TestClass]
    public class QuitHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestQuit()
        {
            var quitHandler = new QuitHandler();
            var msg = new MsgEventArgs {Command = "QUIT", Data = new[] {"test"}, Meta = "a!b@c"};
            var visited = false;
            Caller.quit = new Action<QuitAction>(res =>
            {
                Assert.AreEqual("test", res.Message);
                var user = res.User;
                Assert.AreEqual("a", user.Nick);
                Assert.AreEqual("b", user.Name);
                Assert.AreEqual("c", user.Host);
                visited = true;
            });
            quitHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestQuitRemoveName()
        {
            AddChatUsers("test-a", "a", "b", "c");
            AddChatUsers("test-b", "b", "c");
            AddChatUsers("test-c", "a", "c");
            var quitHandler = new QuitHandler();
            var msg = new MsgEventArgs {Command = "QUIT", Data = new[] {"test"}, Meta = "a!b@c"};
            Caller.quit = new Action<QuitAction>(res => { });
            quitHandler.Msg(Talker, Caller, msg);
            CollectionAssert.AreEqual(new[] {"b", "c"},
                Talker.Chats["test-a"].Users.Select(u => u.IrcUser.Nick).ToArray());
            CollectionAssert.AreEqual(new[] {"b", "c"},
                Talker.Chats["test-b"].Users.Select(u => u.IrcUser.Nick).ToArray());
            CollectionAssert.AreEqual(new[] {"c"}, Talker.Chats["test-c"].Users.Select(u => u.IrcUser.Nick).ToArray());
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Quit"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnQuit()
        {
            var quitHandler = new QuitHandler();
            Assert.AreEqual("QUIT", quitHandler.ForCommand());
        }

        [TestMethod]
        public void TestHistory()
        {
            AddChatUsers("test-a", "a", "b", "c");
            AddChatUsers("test-b", "b", "c");
            AddChatUsers("test-c", "a", "c");
            var quitHandler = new QuitHandler();
            var msg = new MsgEventArgs {Command = "QUIT", Data = new[] {"test"}, Meta = "a!b@c"};
            Caller.quit = new Action<QuitAction>(res => { });
            Caller.action = new Action<Action>(res => { });
            var action = quitHandler.Msg(Talker, Caller, msg) as QuitAction;
            Assert.IsNotNull(action);
            CollectionAssert.AreEqual(new[] {"test-a", "test-c"}, action.ChatNames.ToArray());
        }
    }
}