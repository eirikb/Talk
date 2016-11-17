using System;
using System.Collections.Generic;
using System.Linq;

namespace Talk.Handler
{
    public abstract class Action
    {
        protected Action()
        {
            Date = DateTime.UtcNow;
            ChatNames = new HashSet<string>();
        }

        protected Action(string chatName) : this()
        {
            ChatNames.Add(chatName);
        }

        public string Type
        {
            get { return GetType().Name; }
        }

        public DateTime Date { get; private set; }
        public HashSet<string> ChatNames { get; private set; }
    }

    public class IrcUser
    {
        private IrcUser(string path)
        {
            Update(path);
        }

        public string Path { get; private set; }
        public string Nick { get; set; }
        public string Name { get; private set; }
        public string Host { get; private set; }

        public static IrcUser GetOrCreate(UserManager users, string path)
        {
            if (!path.Contains("!")) path += "!";
            if (!path.Contains("@")) path += "@";

            var newUser = new IrcUser(path);
            var user = users.FirstOrDefault(u => u.Nick == newUser.Nick);
            if (user != null)
            {
                user.Update(path);
            }
            else
            {
                user = newUser;
                users.Add(user);
            }

            return user;
        }

        private void Update(string path)
        {
            Path = path;
            var x = path.Split('!');
            Nick = x.First();

            x = x.Skip(1).First().Split('@');
            Name = x.First();
            Host = x.Last();
        }
    }

    public class ChatUser
    {
        public IrcUser IrcUser { get; set; }
        public IEnumerable<string> Modes { get; set; }
    }

    public class JoinAction : Action
    {
        public JoinAction(string chatName) : base(chatName)
        {
        }

        public IrcUser User { get; set; }
    }

    public class MessageAction : Action
    {
        public MessageAction(string chatName) : base(chatName)
        {
        }

        public string Message { get; set; }
        public IrcUser User { get; set; }
        public int UnreadMessages { get; set; }
        public bool Mentioned { get; set; }
        public bool FromSelf { get; set; }
    }

    public class ModeAction : Action
    {
        public ModeAction(string chatName) : base(chatName)
        {
        }

        public string Mode { get; set; }
        public string Message { get; set; }
        public IrcUser User { get; set; }
    }

    public class NamesListAction : Action
    {
        public NamesListAction(string chatName) : base(chatName)
        {
        }

        public IEnumerable<ChatUser> Users { get; set; }
    }

    public class NoticeAction : Action
    {
        public string Message { get; set; }
        public IrcUser User { get; set; }
    }

    public class NickInUseAction : Action
    {
        public string Nick { get; set; }
        public string Message { get; set; }
    }

    public class OpenChatAction : Action
    {
        public OpenChatAction(string chatName) : base(chatName)
        {
        }

        public string Title { get; set; }
    }

    public class PartAction : Action
    {
        public PartAction(string chatName) : base(chatName)
        {
        }

        public string Message { get; set; }
        public IrcUser User { get; set; }
    }

    public class QuitAction : Action
    {
        public string Message { get; set; }
        public IrcUser User { get; set; }
    }

    public class NickAction : Action
    {
        public string NewNick { get; set; }
        public IrcUser User { get; set; }
    }

    public class RegisteredAction : Action
    {
        public IrcUser User { get; set; }
        public string Message { get; set; }
    }

    public class Chat
    {
        public Chat(string name)
        {
            Name = name;
            History = new List<Action>();
            Users = new HashSet<ChatUser>();
        }

        public string Title { get; set; }
        public int UnreadMessages { get; set; }

        public List<Action> History { get; private set; }
        public string Name { get; private set; }
        public HashSet<ChatUser> Users { get; private set; }

        public void SetUser(IrcUser user)
        {
            var chatUser = Users.FirstOrDefault(u => u.IrcUser.Nick == user.Nick);
            if (chatUser == null)
            {
                chatUser = new ChatUser();
                Users.Add(chatUser);
            }
            chatUser.IrcUser = user;
        }
    }
}