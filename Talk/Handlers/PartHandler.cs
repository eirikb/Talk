using System.Linq;
using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class PartHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreatePart(talker.Users, msg);
            var chats = action.ChatNames.Select(talker.Chats.GetOrCreate).ToList();
            chats.ForEach(chat => chat.Users.RemoveWhere(u => u.IrcUser.Nick == action.User.Nick));

            if (action.User.Nick == talker.Nick)
            {
                chats.ForEach(chat =>
                {
                    talker.Chats.Remove(chat);
                    caller.closeChat(chat.Name);
                });
            }
            caller.part(action);
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "PART";
        }

        public string ForInfo()
        {
            return null;
        }

        private static PartAction CreatePart(UserManager users, MsgEventArgs msg)
        {
            var message = msg.Data.Length > 1 ? msg.Data[1] : null;
            return new PartAction(msg.Data[0])
            {
                Message = message,
                User = IrcUser.GetOrCreate(users, msg.Meta)
            };
        }
    }
}