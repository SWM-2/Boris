using System.Net.Sockets;

namespace BorisProxy
{
    public class TcpSource
    {
        protected Action<Stream,dynamic,TcpSource,byte[]> processing_fun;
        public void SetProcessingFunction(Action<Stream,dynamic,TcpSource,byte[]> pfun)
        {
            processing_fun = pfun;
        }

        ServerStateDisplay.SourceState state = new ServerStateDisplay.SourceState();

        public void AssignState(string name)
        {
            state.name = name;
            state.upload = 0;
            state.download = 0;
            ServerStateDisplay.states.Add(state);
        }

        public void Upload(long amount)
        {
            state.upload += amount;
        }

        public void Download(long amount)
        {
            state.download += amount;
        }

        public virtual void Start()
        {

        }
    }
}