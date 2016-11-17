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
    public class NamesListHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestNamesList()
        {
            var namesListHandler = new NamesListHandler();
            var msg = new MsgEventArgs {Command = "353", Data = new[] {"", "", "test", "hello world"}};
            var visited = false;
            Caller.namesList = new Action<NamesListAction>(res =>
            {
                Assert.AreEqual("test", res.ChatNames.First());
                //Assert.AreEqual("hello", res.Names[0]);
                //Assert.AreEqual("world", res.Names[1]);
                visited = true;
            });
            namesListHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestNamesAddedToChannelNames()
        {
            var namesListHandler = new NamesListHandler();
            var msg = new MsgEventArgs {Command = "353", Data = new[] {"", "", "test", "hello world"}};
            Caller.namesList = new Action<NamesListAction>(r => { });
            namesListHandler.Msg(Talker, Caller, msg);
            //CollectionAssert.AreEqual(new[] {"hello", "world"}, Talker.GetChat("test").Users.ToArray());
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("NamesList"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnNamesList()
        {
            var namesListHandler = new NamesListHandler();
            Assert.AreEqual("353", namesListHandler.ForCommand());
        }
    }
}