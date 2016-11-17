using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Talk.Handler;
using Talk.Handlers;
using TalkLib;

namespace TalkTest
{
    [TestClass]
    public class RegisteredHandlerTest : BaseTest
    {
        [TestMethod]
        public void TestRegistered()
        {
            var registeredHandler = new RegisteredHandler();
            var msg = new MsgEventArgs {Command = "396", Data = new[] {"hello_", "world", "oh, hai :)"}};
            var visited = false;
            Caller.registered = new Action<RegisteredAction>(res =>
            {
                Assert.AreEqual("oh, hai :)", res.Message);
                var user = res.User;
                Assert.AreEqual("hello_", user.Nick);
                Assert.AreEqual("", user.Name);
                Assert.AreEqual("world", user.Host);
                visited = true;
            });
            registeredHandler.Msg(Talker, Caller, msg);
            Assert.IsTrue(visited);
        }

        [TestMethod]
        public void TestHasRegisteredHandlerInTalker()
        {
            var registeredHandler = new RegisteredHandler();
            var msg = new MsgEventArgs {Command = "396", Data = new[] {"hello_", "world", "oh, hai :)"}};
            RegisteredAction action = null;
            Caller.registered = new Action<RegisteredAction>(res => { action = res; });
            Assert.IsNull(Talker.RegisteredAction);
            registeredHandler.Msg(Talker, Caller, msg);
            Assert.IsNotNull(Talker.RegisteredAction);
            Assert.AreEqual(action, Talker.RegisteredAction);
        }

        [TestMethod]
        public void TestHasBindingInModule()
        {
            var kernel = new StandardKernel(new TalkerModule());
            Assert.IsNotNull(kernel.TryGet<ITalk>("Registered"));
        }

        [TestMethod]
        public void TestOnlyTriggerOnRegistered()
        {
            var registeredHandler = new RegisteredHandler();
            Assert.AreEqual("396", registeredHandler.ForCommand());
        }
    }
}