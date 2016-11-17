using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Ninject;
using Talk.Handler;
using TalkLib;
using Action = Talk.Handler.Action;

namespace Talk
{
    public class ChatHub : Hub
    {
        private static readonly IKernel Kernel = new StandardKernel(new TalkerModule());

        public string GetSession(string username, string password)
        {
            return GetSession(username, password, false);
        }

        public override Task OnDisconnected()
        {
            var talker = Global.Sessions[Context.ConnectionId];
            if (talker.DisconnectOnClose) talker.Send("QUIT");
            return base.OnDisconnected();
        }

        public string GetSession(string username, string password, bool disconnectOnClose)
        {
            var key = Encrypt(username, password);
            Talker talker;
            if (!Global.Sessions.TryGetValue(key, out talker))
            {
                Trace.TraceInformation("New talker! " + key);
                var talk = new TalkLib.Talk(username, password);
                var talkerFactory = Kernel.Get<TalkerFactory>();
                talker = talkerFactory.CreateTalker(talk, Clients.Group(key));
                talker.Disconnect += (sender, args) => Global.Sessions.TryRemove(key, out talker);
                talker.DisconnectOnClose = disconnectOnClose;
            }

            talker.Disconnect += (sender, args) =>
            {
                Global.Sessions.TryRemove(Context.ConnectionId, out talker);
                Groups.Remove(Context.ConnectionId, key);
            };

            Global.Sessions[Context.ConnectionId] = Global.Sessions[key] = talker;

            return key;
        }

        public void ClearUnreadMessages(string chatName)
        {
            var talker = Global.Sessions[Context.ConnectionId];
            var chat = talker.Chats.GetOrCreate(chatName);
            chat.UnreadMessages = 0;
        }

        public bool ValidateSession(string session)
        {
            var validSession = Global.Sessions.ContainsKey(session);
            if (validSession) Global.Sessions[Context.ConnectionId] = Global.Sessions[session];
            return validSession;
        }

        public async Task<bool> ValidateCaptcha(string captcha, string code)
        {
            var talker = Global.Sessions[Context.ConnectionId];
            if (talker.Talk.Connected) return true;

            var validateCaptcha = await Captcha.ValidateCaptcha(talker.Talk, captcha, code);
            if (!validateCaptcha) return false;

            var loginOk = await talker.Talk.Login();
            if (!loginOk) return false;

            talker.Listen();
            return true;
        }

        public Task<string> Send(string msg)
        {
            var talker = Global.Sessions[Context.ConnectionId];
            return talker.Send(msg);
        }

        public Chat GetChat(string chatName)
        {
            var talker = Global.Sessions[Context.ConnectionId];
            var chat = talker.Chats.GetOrCreate(chatName);
            return chat == null ? null : GetChatWithoutHistory(chat);
        }

        private static Chat GetChatWithoutHistory(Chat chat)
        {
            var clone = new Chat(chat.Name)
            {
                Title = chat.Title
            };
            //clone.Users.UnionWith(chat.Users);
            return clone;
        }

        public IEnumerable<Chat> GetChats()
        {
            var talker = Global.Sessions[Context.ConnectionId];
            return talker.Chats.Select(GetChatWithoutHistory);
        }

        public IEnumerable<Action> GetHistory(string chatName, int n = 1000)
        {
            var talker = Global.Sessions[Context.ConnectionId];
            var chat = talker.Chats.GetOrCreate(chatName);
            if (chat == null) return null;

            return chat.History.Skip(Math.Max(0, chat.History.Count - n)).Take(n);
        }

        public async Task<RegisteredAction> Attach(string sessionKey)
        {
            Talker talker;
            if (!Global.Sessions.TryGetValue("" + sessionKey, out talker))
                return await Task.FromResult<RegisteredAction>(null);
            if (!talker.Talk.Connected) return await Task.FromResult<RegisteredAction>(null);

            Global.Sessions[Context.ConnectionId] = Global.Sessions[sessionKey];
            Groups.Add(Context.ConnectionId, sessionKey);

            return await talker.GetRegisteredAction();
        }

        private static string Encrypt(string username, string password)
        {
            var data = Encoding.UTF8.GetBytes("This isn't even a secret " + username + " " + password);
            SHA256 shaM = new SHA256Managed();
            data = shaM.ComputeHash(data);
            return Convert.ToBase64String(data);
        }
    }
}