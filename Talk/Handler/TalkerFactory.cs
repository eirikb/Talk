namespace Talk.Handler
{
    public class TalkerFactory : ITalkerFactory
    {
        private readonly ITalkerFactory _talkerFactory;

        public TalkerFactory(ITalkerFactory talkerFactory)
        {
            _talkerFactory = talkerFactory;
        }

        public Talker CreateTalker(TalkLib.Talk talk, dynamic caller)
        {
            return _talkerFactory.CreateTalker(talk, caller);
        }
    }

    public interface ITalkerFactory
    {
        Talker CreateTalker(TalkLib.Talk talk, dynamic caller);
    }
}