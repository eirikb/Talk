using Microsoft.VisualStudio.TestTools.UnitTesting;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class ConnectedHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestConnected()
        {
            var connectedHandler = new ConnectedHandler();
            var msg = new MsgEventArgs {Command = "001", Data = new[] {"test"}};
            connectedHandler.Msg(Talker, Caller, msg);
            Assert.AreEqual("test", Talker.Nick);
        }

        [TestMethod]
        public void TestOnlyTriggerOnConnected()
        {
            var connectedHandler = new ConnectedHandler();
            Assert.AreEqual("001", connectedHandler.ForCommand());
        }
    }
}