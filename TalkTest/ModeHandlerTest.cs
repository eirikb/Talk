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
    public class ModeHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestMode()
        {
            var modeHandler = new ModeHandler();
            var msg = new MsgEventArgs {Command = "MODE", Data = new[] {"test", "+v", "tast"}, Meta = "a!b@c"};
            var visited = false;
            Caller.mode = new Action<ModeAction>(res =>
            {
                Assert.AreEqual("test", res.ChatNames.First());
                Assert.AreEqual("+v", res.Mode);
                Assert.AreEqual("tast", res.Message);
                var user = res.User;
                Assert.AreEqual("a", user.Nick);
                Assert.AreEqual("b", user.Name);
                Assert.AreEqual("c", user.Host);
                visited = true;
            });
            modeHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Mode"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnMode()
        {
            var modeHandler = new ModeHandler();
            Assert.AreEqual("MODE", modeHandler.ForCommand());
        }
    }
}