using System;
using Talk.Handler;
using TalkLib;
using Action = Talk.Handler.Action;

namespace Talk.Handlers
{
    public class DisconnectHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            return null;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
            if (!info.Info.Equals("disconnect", StringComparison.InvariantCultureIgnoreCase)) return;

            talker.DoDisconnect();
        }

        public string ForCommand()
        {
            return null;
        }

        public string ForInfo()
        {
            return "disconnect";
        }
    }
}