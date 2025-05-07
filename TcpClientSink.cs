
using System.Net.Sockets;

namespace BorisProxy
{
    public class TcpClientSink : TcpSink
    {
        string addr = "";
        short prt = 0;
        public TcpClientSink(string address, short port)
        {
            addr = address;
            prt = port;
        }

        public override Action<Stream, dynamic,TcpSource,byte[]> CreatePfun()
        {
            return new Action<Stream, dynamic,TcpSource,byte[]>((s,d,t,b)=>{
                TcpClient new_cli = new TcpClient(addr,prt);
                NetworkStream strm = new_cli.GetStream();

                strm.Write(b,0,b.Length);
                Task forward2 = StreamUtils.PipeStream(s, strm,t,false);
                Task forward1 = StreamUtils.PipeStream(strm, s,t,true);

                Task.WhenAny(forward1, forward2).Wait(); 
            });
        }
    }
}