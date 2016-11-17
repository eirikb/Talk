using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class PingHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestPong()
        {
            var pingHandler = new PingHandler();
            pingHandler.Msg(Talker, Caller, new MsgEventArgs {Command = "PING", Data = new[] {"someserver.com"}});
            TalkMock.Verify(mock => mock.Post("p", "PONG :someserver.com"));
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Ping"));
        }

        [TestMethod]
        public void TestPongOnlyOnPing()
        {
            var pingHandler = new PingHandler();
            Assert.AreEqual("PING", pingHandler.ForCommand());
        }
    }
}