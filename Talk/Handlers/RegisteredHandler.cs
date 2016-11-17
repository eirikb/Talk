using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class RegisteredHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateRegistered(talker.Users, msg);
            caller.registered(action);
            talker.Nick = action.User.Nick;
            talker.RegisteredAction = action;
            talker.OnRegisted();
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "396";
        }

        public string ForInfo()
        {
            return null;
        }

        private static RegisteredAction CreateRegistered(UserManager users, MsgEventArgs msg)
        {
            return new RegisteredAction
            {
                User = IrcUser.GetOrCreate(users, msg.Data[0] + "!@" + msg.Data[1]),
                Message = msg.Data[2]
            };
        }
    }
}