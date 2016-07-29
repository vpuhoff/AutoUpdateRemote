using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;

namespace Remote
{
    public class Server
    {
        private int port;
        public int Port { get { return port; } }
        public Server(int _port = 9998)
        {
            port = _port;
            CreateServer();
            //Console.WriteLine("Press ENTER to quitnn");
            //Console.ReadLine();
        }

        public void CreateServer()
        {
            Console.WriteLine("Remote Server started...");

            var tcpChannel = new TcpChannel(port);
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(tcpChannel, true);

            var commonInterfaceType = Type.GetType("Remote.SharedType");

            System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(commonInterfaceType, "RemoteService", WellKnownObjectMode.SingleCall);

        }
    }

    
}
