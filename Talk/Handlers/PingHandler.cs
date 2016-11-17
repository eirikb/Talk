using System;
using Talk.Handler;
using TalkLib;
using Action = Talk.Handler.Action;

namespace Talk.Handlers
{
    public class PingHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            Console.WriteLine("PONG :" + msg.Data[0]);
            talker.Talk.Post("p", "PONG :" + msg.Data[0]);
            return null;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "PING";
        }

        public string ForInfo()
        {
            return null;
        }
    }
}