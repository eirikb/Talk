using System.Linq;
using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class OpenChatHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateOpenChat(msg);

            var chats = action.ChatNames.Select(talker.Chats.GetOrCreate).ToList();
            chats.ForEach(chat => chat.Title = action.Title);
            caller.openChat(action);
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "332";
        }

        public string ForInfo()
        {
            return null;
        }

        private static OpenChatAction CreateOpenChat(MsgEventArgs msg)
        {
            return new OpenChatAction(msg.Data[1])
            {
                Title = msg.Data[2]
            };
        }
    }
}