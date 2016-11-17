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
    public class OpenChatHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestOpen()
        {
            var openChannelHandler = new OpenChatHandler();
            var msg = new MsgEventArgs {Command = "332", Data = new[] {"", "#web", "Hello, world!"}};
            var visited = false;
            Caller.openChat = new Action<OpenChatAction>(res =>
            {
                Assert.AreEqual("#web", res.ChatNames.First());
                Assert.AreEqual("Hello, world!", res.Title);
                visited = true;
            });
            openChannelHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("OpenChat"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnOpen()
        {
            var openChatHandler = new OpenChatHandler();
            Assert.AreEqual("332", openChatHandler.ForCommand());
        }
    }
}