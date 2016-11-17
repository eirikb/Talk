using System;
using System.Collections.Concurrent;
using System.Web;
using Talk.Handler;

namespace Talk
{
    public class Global : HttpApplication
    {
        public static ConcurrentDictionary<string, Talker> Sessions { get; private set; }
        public static DateTime Start { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            Sessions = new ConcurrentDictionary<string, Talker>();
            Start = DateTime.Now;
        }
    }
}