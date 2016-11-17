using Microsoft.VisualStudio.TestTools.UnitTesting;
using Talk.Handler;

namespace TalkTest
{
    [TestClass]
    public class IrcUserTest
    {
        [TestMethod]
        public void TestIrcUser()
        {
            const string path = "hello!~world@herpa.derp";
            var talker = new Talker(null, null, null);
            var ircUser = IrcUser.GetOrCreate(talker.Users, path);
            Assert.AreEqual("hello", ircUser.Nick);
            Assert.AreEqual("~world", ircUser.Name);
            Assert.AreEqual("herpa.derp", ircUser.Host);
        }
    }
}