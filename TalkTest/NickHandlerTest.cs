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
    public class NickHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestNick()
        {
            var nickHandler = new NickHandler();
            var msg = new MsgEventArgs {Command = "nick", Data = new[] {"test"}, Meta = "a!b@c"};
            var visited = false;
            Caller.nick = new Action<NickAction>(res =>
            {
                Assert.AreEqual("test", res.NewNick);
                var user = res.User;
                Assert.AreEqual("a", user.Nick);
                Assert.AreEqual("b", user.Name);
                Assert.AreEqual("c", user.Host);
                visited = true;
            });
            nickHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestSelfNickChange()
        {
            var nickHandler = new NickHandler();
            Talker.Nick = "a";
            var msg = new MsgEventArgs {Command = "nick", Data = new[] {"test"}, Meta = "a!b@c"};
            Caller.nick = new Action<NickAction>(res => { });
            nickHandler.Msg(Talker, Caller, msg);
            Assert.AreEqual("test", Talker.Nick);
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Nick"));
        }

        [TestMethod]
        public void TestSelfNickNoChangeIfNotSame()
        {
            var nickHandler = new NickHandler();
            Talker.Nick = "b";
            var msg = new MsgEventArgs {Command = "nick", Data = new[] {"test"}, Meta = "a!b@c"};
            Caller.nick = new Action<NickAction>(res => { });
            nickHandler.Msg(Talker, Caller, msg);
            Assert.AreEqual("b", Talker.Nick);
        }

        [TestMethod]
        public void TestNickChangeChangeInNamesList()
        {
            AddChatUsers("test", "a", "b", "c");
            AddChatUsers("test2", "a", "c");

            var msg = new MsgEventArgs {Command = "nick", Data = new[] {"d"}, Meta = "a!b@c"};
            Caller.nick = new Action<NickAction>(res => { });

            var nickHandler = new NickHandler();
            nickHandler.Msg(Talker, Caller, msg);
            var wat = Talker.Chats["test"].Users.Select(u => u.IrcUser.Nick).ToArray();
            CollectionAssert.AreEqual(new[] {"d", "b", "c"},
                Talker.Chats["test"].Users.Select(u => u.IrcUser.Nick).ToArray());
            CollectionAssert.AreEqual(new[] {"d", "c"},
                Talker.Chats["test2"].Users.Select(u => u.IrcUser.Nick).ToArray());
        }

        [TestMethod]
        public void TestOnlyTriggerOnNick()
        {
            var nickHandler = new NickHandler();
            Assert.AreEqual("NICK", nickHandler.ForCommand());
        }
    }
}