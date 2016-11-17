using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TalkLib;

namespace Talk.Handler
{
    public class Talker
    {
        private readonly dynamic _caller;
        private readonly List<ITalk> _handlers;
        private readonly TalkLib.Talk _talk;

        public Talker(TalkLib.Talk talk, dynamic caller, List<ITalk> handlers)
        {
            _handlers = handlers;
            _talk = talk;
            _caller = caller;
            Chats = new ChatManager();
            Users = new UserManager();
        }

        public ChatManager Chats { get; private set; }
        public UserManager Users { get; private set; }

        public TalkLib.Talk Talk
        {
            get { return _talk; }
        }

        public RegisteredAction RegisteredAction { get; set; }

        public string Nick { get; set; }
        public bool DisconnectOnClose { get; set; }

        public event EventHandler Disconnect;
        public event EventHandler Registed;

        public virtual void OnRegisted()
        {
            var handler = Registed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnDisconnect()
        {
            var handler = Disconnect;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void Msg(MsgEventArgs msg)
        {
            Trace.TraceInformation(msg.Type + " " + msg.Command + " " + msg.Meta + " " + string.Join(" ", msg.Data));
            var commonHandlers = _handlers.Where(h => h.ForCommand() == null && h.ForInfo() == null);
            var msgHandlers =
                _handlers.Where(
                    h => string.Equals(h.ForCommand(), msg.Command, StringComparison.InvariantCultureIgnoreCase));

            commonHandlers.Concat(msgHandlers).ToList().ForEach(handler =>
            {
                Trace.TraceInformation("Handler: " + handler.GetType().Name + " - " +
                                       JsonConvert.SerializeObject(handler));
                try
                {
                    var action = handler.Msg(this, _caller, msg);
                    OnAction(action);
                }
                catch (Exception e)
                {
                    Trace.TraceWarning(e.Message);
                }
            });
        }


        private void OnAction(Action action)
        {
            if (action == null) return;

            Trace.TraceInformation("Action: " + action.Type + " - " + JsonConvert.SerializeObject(action));
            _caller.action(action);
            Chats.OnAction(action);
        }

        public void Info(InfoEventArgs info)
        {
            Trace.TraceInformation(info.Info + " " + info.Message);
            var commonHandlers = _handlers.Where(h => h.ForCommand() == null && h.ForInfo() == null);
            var infoHandlers =
                _handlers.Where(h => string.Equals(h.ForInfo(), info.Info, StringComparison.InvariantCultureIgnoreCase));

            commonHandlers.Concat(infoHandlers).ToList().ForEach(handler =>
            {
                Trace.TraceInformation("Info Handler: " + handler.GetType().Name + " - " +
                                       JsonConvert.SerializeObject(handler));
                try
                {
                    handler.Info(this, _caller, info);
                }
                catch (Exception e)
                {
                    Trace.TraceWarning(e.Message);
                }
            });
        }

        public void Listen()
        {
            _talk.Msg += (sender, args) => Msg(args);
            _talk.Info += (sender, args) => Info(args);

            _talk.LoopDeLoop();
        }


        public Task<string> Send(string msg)
        {
            SelfMessageHack(msg);
            return _talk.Post("p", msg);
        }

        private void SelfMessageHack(string msg)
        {
            if (!("" + msg).StartsWith("privmsg", StringComparison.InvariantCultureIgnoreCase)) return;

            var parts = msg.Split(':');
            var chat = parts.First().Trim().Split(' ').Last();
            var message = string.Join(":", parts.Skip(1));
            Msg(new MsgEventArgs
            {
                Command = "PRIVMSG",
                Data = new[] {chat, message},
                Meta = Nick + "!@",
                Type = "c"
            });
        }

        public void DoDisconnect()
        {
            OnDisconnect();
        }

        public async Task<RegisteredAction> GetRegisteredAction()
        {
            if (RegisteredAction != null) return RegisteredAction;

            var tcs = new TaskCompletionSource<RegisteredAction>();

            Registed += (sender, args) => tcs.SetResult(RegisteredAction);
            return await tcs.Task;
        }
    }
}