using Talk.Handler;
using TalkLib;

namespace Talk.Handlers
{
    public class NickInUseHandler : ITalk
    {
        public Action Msg(Talker talker, dynamic caller, MsgEventArgs msg)
        {
            var action = CreateNickInUse(msg);
            caller.nickInUse(action);
            talker.Send("NICK " + action.Nick + "_");
            return action;
        }

        public void Info(Talker talker, dynamic caller, InfoEventArgs info)
        {
        }

        public string ForCommand()
        {
            return "433";
        }

        public string ForInfo()
        {
            return null;
        }

        private static NickInUseAction CreateNickInUse(MsgEventArgs msg)
        {
            return new NickInUseAction
            {
                Nick = msg.Data[1],
                Message = msg.Data[2]
            };
        }
    }
}