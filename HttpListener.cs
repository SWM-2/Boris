using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BorisProxy
{
    class BORISHttpListener : TcpSource
    {
        TcpListener list;

        public BORISHttpListener(IPAddress ip, short port) : base()
        {
            list = new TcpListener(ip,port);
        }



        public override void Start()
        {
            list.Start();

            AsyncCallback cback = new AsyncCallback(async (ctx)=>{
                TcpClient cli = list.EndAcceptTcpClient(ctx);
                list.BeginAcceptTcpClient(((AsyncCallback)ctx.AsyncState),ctx.AsyncState);
                var stream = cli.GetStream();
                byte[] buffer = new byte[1];
                try{
                    if(processing_fun != null)processing_fun(stream,null,this,new byte[]{});
                }catch(Exception e)
                {
                    Console.WriteLine("ERROR "+e.Message+" "+e.InnerException.Message);
                }finally{
                    stream.Close();
                    cli.Close();
                }
            });
            list.BeginAcceptTcpClient(cback,cback);
        }
    }
}