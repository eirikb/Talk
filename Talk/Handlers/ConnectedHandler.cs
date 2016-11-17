using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class ConnectedHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            talker.Nick = msg.Data[0];
            return null;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "001";
        }

        public string ForInfo()
        {
            return null;
        }
    }
}