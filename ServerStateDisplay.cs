using System;

namespace BorisProxy
{
    public class ServerStateDisplay
    {
        public class SourceState
        {
            public string name;
            public long upload;
            public long download;
        }

        public static List<SourceState> states = new List<SourceState>();
    }
}