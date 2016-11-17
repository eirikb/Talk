using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class NoticeHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestNotice()
        {
            var noticeHandler = new NoticeHandler();
            var msg = new MsgEventArgs {Command = "NOTICE", Data = new[] {"", "Hello, world!"}, Meta = "a!b@c"};
            var visited = false;
            Caller.notice = new Action<NoticeAction>(res =>
            {
                Assert.AreEqual("Hello, world!", res.Message);
                var user = res.User;
                Assert.AreEqual("a", user.Nick);
                Assert.AreEqual("b", user.Name);
                Assert.AreEqual("c", user.Host);
                visited = true;
            });
            noticeHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Notice"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnNotice()
        {
            var noticeHandler = new NoticeHandler();
            Assert.AreEqual("NOTICE", noticeHandler.ForCommand());
        }
    }
}