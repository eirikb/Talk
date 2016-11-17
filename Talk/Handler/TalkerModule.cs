using Ninject.Extensions.Factory;
using Ninject.Modules;
using Talk.Handlers;

namespace Talk.Handler
{
    public class TalkerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITalkerFactory>().ToFactory();

            Bind<ITalk>().To<AllDataHandler>().Named("AllData");
            Bind<ITalk>().To<PingHandler>().Named("Ping");
            Bind<ITalk>().To<JoinHandler>().Named("Join");
            Bind<ITalk>().To<MessageHandler>().Named("Message");
            Bind<ITalk>().To<ModeHandler>().Named("Mode");
            Bind<ITalk>().To<NamesListHandler>().Named("NamesList");
            Bind<ITalk>().To<NoticeHandler>().Named("Notice");
            Bind<ITalk>().To<OpenChatHandler>().Named("OpenChat");
            Bind<ITalk>().To<PartHandler>().Named("Part");
            Bind<ITalk>().To<QuitHandler>().Named("Quit");
            Bind<ITalk>().To<ConnectedHandler>().Named("Connect");
            Bind<ITalk>().To<NickHandler>().Named("Nick");
            Bind<ITalk>().To<NickInUseHandler>().Named("NickInUse");
            Bind<ITalk>().To<DisconnectHandler>().Named("Disconnect");
            Bind<ITalk>().To<RegisteredHandler>().Named("Registered");
        }
    }
}