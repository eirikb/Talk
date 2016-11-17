using TalkLib;

namespace Talk.Handler
{
    public interface ITalk
    {
        Action Msg(Talker talker, dynamic caller, MsgEventArgs msg);
        void Info(Talker talker, dynamic caller, InfoEventArgs info);
        string ForCommand();
        string ForInfo();
    }
}