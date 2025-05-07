
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace BorisProxy
{
    public class HttpHeaderDecisionSink : TcpSink
    {
        Dictionary<string,Action<Stream, dynamic,TcpSource,byte[]>> choices = new Dictionary<string, Action<Stream, dynamic, TcpSource, byte[]>>();
        Action<Stream, dynamic,TcpSource,byte[]> dft;


        Dictionary<string,TcpSink> choices_pp;
        TcpSink dft_pp;

        string kee;
        public HttpHeaderDecisionSink(string key)
        {
            kee = key.ToLower();
        }

        public void ProcessReferences(Dictionary<string,TcpSink> values, TcpSink _default)
        {
            choices_pp = values;
            dft_pp = _default;
        }

        public void FinalizeReferences()
        {
            dft = dft_pp.CreatePfun();
            foreach(KeyValuePair<string,TcpSink> snk in choices_pp)
            {
                choices.Add(snk.Key,snk.Value.CreatePfun());
            }
        }

        public override Action<Stream, dynamic,TcpSource,byte[]> CreatePfun()
        {
            return new Action<Stream, dynamic,TcpSource,byte[]>((s,d,t,b)=>{
                string found = null;


                List<byte> backlog = new List<byte>();
                backlog.AddRange(b);
                string line = "";

                byte[] buffer = new byte[1];
                while((s.Read(buffer,0,1) > 0))
                {
                    backlog.Add(buffer[0]);
                    if(((char)buffer[0]) == '\n')
                    {
                        if(line.Length == 1 && line[0] == '\r')
                            break;
                        if(line.Contains(":"))
                        {
                            string[] kv = line.Replace(" ","").ToLower().Split(':');
                            if(kv[0] == kee)
                            {
                                found = kv[1].TrimEnd('\r');
                            }
                        }
                        line = "";
                    }else{
                        line += ((char)buffer[0]);
                        
                    }
                }
                if(found != null && choices.ContainsKey(found))
                {
                    choices[found](s,d,t,backlog.ToArray());
                }else{
                    dft(s,d,t,backlog.ToArray());
                }
            });
        }
    }
}