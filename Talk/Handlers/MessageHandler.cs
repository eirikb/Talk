using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class MessageHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateMessage(talker, msg);
            caller.message(action);
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "PRIVMSG";
        }

        public string ForInfo()
        {
            return null;
        }

        private static MessageAction CreateMessage(Talker talker, MsgEventArgs msg)
        {
            var chatName = msg.Data[0];
            var chat = talker.Chats.GetOrCreate(chatName);
            var user = IrcUser.GetOrCreate(talker.Users, msg.Meta);
            var message = msg.Data[1];
            chat.UnreadMessages++;
            return new MessageAction(chatName)
            {
                Message = message,
                User = user,
                UnreadMessages = chat.UnreadMessages,
                FromSelf = user.Nick == talker.Nick,
                Mentioned = message.Contains("" + talker.Nick)
            };
        }
    }
}