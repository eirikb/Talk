using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Talk.Handler;

namespace Talk
{
    public class UserManager : HashSet<IrcUser>
    {
    }

    public class ChatManager : HashSet<Chat>
    {
        public Chat this[string name]
        {
            get { return GetOrCreate(name); }
        }

        public Chat GetOrCreate(string chatName)
        {
            var chat = this.FirstOrDefault(c => c.Name == chatName);
            if (chat != null) return chat;

            chat = new Chat(chatName);
            Add(chat);
            return chat;
        }

        public void OnAction(Action action)
        {
            var chats = action.ChatNames.Select(GetOrCreate);

            chats.ToList().ForEach(chat =>
            {
                chat.History.Add(action);
                Trace.TraceInformation(" Action: " + chat.Name + " " + action.GetType().Name);
            });
        }
    }
}