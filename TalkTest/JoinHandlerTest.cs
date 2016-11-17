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
    public class JoinHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestJoin()
        {
            var joinHandler = new JoinHandler();
            var msg = new MsgEventArgs {Command = "JOIN", Data = new[] {"test"}, Meta = "a!b@c"};
            var visited = false;
            Caller.join = new Action<JoinAction>(res =>
            {
                Assert.AreEqual("test", res.ChatNames.First());
                var user = res.User;
                Assert.AreEqual("a", user.Nick);
                Assert.AreEqual("b", user.Name);
                Assert.AreEqual("c", user.Host);
                visited = true;
            });
            joinHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestJoinAddName()
        {
            AddChatUsers("test", "a", "b", "c");

            Caller.join = new Action<JoinAction>(res => { });
            var joinHandler = new JoinHandler();
            var msg = new MsgEventArgs {Command = "JOIN", Data = new[] {"test"}, Meta = "d!b@c"};
            joinHandler.Msg(Talker, Caller, msg);
            CollectionAssert.AreEqual(new[] {"a", "b", "c", "d"},
                Talker.Chats["test"].Users.Select(u => u.IrcUser.Nick).ToArray());
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Join"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnJoin()
        {
            var joinHandler = new JoinHandler();
            Assert.AreEqual("JOIN", joinHandler.ForCommand());
        }
    }
}