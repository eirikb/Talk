using System.Linq;
using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class QuitHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateQuit(talker.Users, msg);
            caller.quit(action);
            var chats = talker.Chats.Where(c => c.Users.Any(u => u.IrcUser.Nick == action.User.Nick)).ToList();
            chats.ForEach(chat => chat.Users.RemoveWhere(u => u.IrcUser.Nick == action.User.Nick));
            chats.ForEach(chat => action.ChatNames.Add(chat.Name));
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "QUIT";
        }

        public string ForInfo()
        {
            return null;
        }

        private static QuitAction CreateQuit(UserManager users, MsgEventArgs msg)
        {
            return new QuitAction
            {
                Message = msg.Data[0],
                User = IrcUser.GetOrCreate(users, msg.Meta)
            };
        }
    }
}