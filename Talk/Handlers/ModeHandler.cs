using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class ModeHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateMode(talker.Users, msg);
            caller.mode(action);
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "MODE";
        }

        public string ForInfo()
        {
            return null;
        }

        private static ModeAction CreateMode(UserManager users, MsgEventArgs msg)
        {
            return new ModeAction(msg.Data[0])
            {
                Mode = msg.Data[1],
                Message = msg.Data[2],
                User = IrcUser.GetOrCreate(users, msg.Meta)
            };
        }
    }
}