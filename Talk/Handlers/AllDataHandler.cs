using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class AllDataHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            caller.msg(new
            {
                type = msg.Type,
                command = msg.Command,
                meta = msg.Meta,
                data = msg.Data
            });
            return null;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
            caller.info(new
            {
                info = info.Info,
                message = info.Message
            });
        }

        public string ForCommand()
        {
            return null;
        }

        public string ForInfo()
        {
            return null;
        }
    }
}