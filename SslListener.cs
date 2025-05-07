using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BorisProxy
{
    class SslListener : TcpSource
    {
        X509Certificate2 cert;
        TcpListener list;

        public SslListener(IPAddress ip, short port, string pfx_path, string pfx_password) : base()
        {
            cert = new X509Certificate2(pfx_path,pfx_password);
            list = new TcpListener(ip,port);
        }



        public override void Start()
        {
            list.Start();

            AsyncCallback cback = new AsyncCallback(async (ctx)=>{
                TcpClient cli = list.EndAcceptTcpClient(ctx);
                list.BeginAcceptTcpClient(((AsyncCallback)ctx.AsyncState),ctx.AsyncState);
                var sslStream = new SslStream(cli.GetStream(), false);
                byte[] buffer = new byte[1];
                try{
                    sslStream.AuthenticateAsServer(cert, clientCertificateRequired: false, checkCertificateRevocation: false);
                    if(processing_fun != null)processing_fun(sslStream,null,this,new byte[]{});
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                }finally{
                    sslStream.Close();
                    cli.Close();
                }
            });
            list.BeginAcceptTcpClient(cback,cback);
        }
    }
}