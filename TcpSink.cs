namespace BorisProxy
{
    public class TcpSink
    {
        public TcpSink()
        {

        }

        public virtual Action<Stream,dynamic,TcpSource,byte[]> CreatePfun()
        {
            return new Action<Stream, dynamic,TcpSource,byte[]>((s,e,t,b)=>{

            });
        }
    }
}