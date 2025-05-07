
using System.Net.Sockets;

namespace BorisProxy
{
    public class SimpleStaticSink : TcpSink
    {
        string fpth = "";
        public SimpleStaticSink(string path)
        {
            fpth = path;
        }

        public override Action<Stream, dynamic,TcpSource,byte[]> CreatePfun()
        {
            return new Action<Stream, dynamic,TcpSource,byte[]>((s,d,t,b)=>{
                byte[] file_data = File.ReadAllBytes(fpth);
                t.Upload(file_data.Length);
                s.Write(file_data,0,file_data.Length);
            });
        }
    }
}