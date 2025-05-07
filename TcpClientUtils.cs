using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BorisProxy
{
    class C
    {
        public static bool IsConnected(TcpClient client)
        {
            try{
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] checkConn = new byte[1];

                    if (client.Client.Receive(checkConn, SocketFlags.Peek) == 0)
                        throw new IOException();
                }
                return true;
            }catch{
                return false;
            }
        }
    }
}