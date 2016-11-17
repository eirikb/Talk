using System.Collections.Generic;
using System.Linq;
using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class NamesListHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var nickModes = GetNickModes(msg);
            var chatUsers = nickModes.Select(nickMode => new ChatUser
            {
                IrcUser = IrcUser.GetOrCreate(talker.Users, nickMode.Nick),
                Modes = new[] {nickMode.Mode}
            }).ToList();

            var action = new NamesListAction(msg.Data[2])
            {
                Users = chatUsers
            };

            caller.namesList(action);

            var chats = action.ChatNames.Select(talker.Chats.GetOrCreate).ToList();
            chats.ForEach(chat => chatUsers.ForEach(chatUser => chat.Users.Add(chatUser)));

            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "353";
        }

        public string ForInfo()
        {
            return null;
        }

        private static IEnumerable<NickMode> GetNickModes(MsgEventArgs msg)
        {
            return msg.Data[3].Split(' ').Select(nick =>
            {
                var o = nick.StartsWith("@");
                var v = nick.StartsWith("+");
                var mode = o ? "o" : "v";

                if (o || v) nick = nick.Substring(1);
                else mode = "";
                return new NickMode
                {
                    Nick = nick,
                    Mode = mode
                };
            });
        }

        internal class NickMode
        {
            internal string Nick { get; set; }
            internal string Mode { get; set; }
        }
    }
}