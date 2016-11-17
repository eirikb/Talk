using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class NickInUseHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestNickInUse()
        {
            var nickInUseHandler = new NickInUseHandler();
            var msg = new MsgEventArgs
            {
                Command = "433",
                Data = new[] {"*", "test", "Nickname already in use"},
                Meta = "derp"
            };
            var visited = false;
            Caller.nickInUse = new Action<NickInUseAction>(res =>
            {
                Assert.AreEqual("test", res.Nick);
                Assert.AreEqual("Nickname already in use", res.Message);
                visited = true;
            });
            nickInUseHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }


        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("NickInUse"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnNickInUse()
        {
            var nickInUseHandler = new NickInUseHandler();
            Assert.AreEqual("433", nickInUseHandler.ForCommand());
        }
    }
}