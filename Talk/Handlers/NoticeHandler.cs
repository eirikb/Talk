using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class NoticeHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateNotice(talker.Users, msg);
            caller.notice(action);
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "NOTICE";
        }

        public string ForInfo()
        {
            return null;
        }

        private static NoticeAction CreateNotice(UserManager users, MsgEventArgs msg)
        {
            return new NoticeAction
            {
                Message = msg.Data[1],
                User = IrcUser.GetOrCreate(users, msg.Meta)
            };
        }
    }
}