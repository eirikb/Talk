using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Talk.Handler;

namespace TalkTest
{
    [TestClass]
    public abstract class BaseTest
    {
        protected dynamic Caller;
        protected TalkLib.Talk Talk;
        protected Mock<TalkLib.Talk> TalkMock;
        protected Talker Talker;

        [TestInitialize]
        public void SetUp()
        {
            Caller = new ExpandoObject();
            TalkMock = new Mock<TalkLib.Talk>("", "");
            Talk = TalkMock.Object;

            var kernel = new StandardKernel(new TalkerModule());
            var talkerFactory = kernel.Get<TalkerFactory>();
            Talker = talkerFactory.CreateTalker(Talk, Caller);
        }

        internal IrcUser ToIrcUser(string nick)
        {
            return IrcUser.GetOrCreate(Talker.Users, nick);
        }

        internal void AddChatUsers(string chatName, params string[] nicks)
        {
            var chat = Talker.Chats.GetOrCreate(chatName);
            nicks.Select(ToIrcUser)
                .Select(user => new ChatUser {IrcUser = user})
                .ToList()
                .ForEach(user => chat.Users.Add(user));
        }
    }
}