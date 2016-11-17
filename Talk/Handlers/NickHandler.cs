using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class NickHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateNick(talker.Users, msg);
            caller.nick(action);

            if (action.User.Nick == talker.Nick)
            {
                talker.Nick = action.NewNick;
            }

            action.User.Nick = action.NewNick;
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "NICK";
        }

        public string ForInfo()
        {
            return null;
        }

        private static NickAction CreateNick(UserManager users, MsgEventArgs msg)
        {
            return new NickAction
            {
                NewNick = msg.Data[0],
                User = IrcUser.GetOrCreate(users, msg.Meta)
            };
        }
    }
}