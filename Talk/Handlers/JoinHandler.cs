using System.Linq;
using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class JoinHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var user = IrcUser.GetOrCreate(talker.Users, msg.Meta);
            var action = new JoinAction(msg.Data[0])
            {
                User = user
            };
            caller.join(action);

            var chats = action.ChatNames.Select(talker.Chats.GetOrCreate).ToList();
            chats.ForEach(chat => chat.SetUser(user));

            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "JOIN";
        }

        public string ForInfo()
        {
            return null;
        }
    }
}