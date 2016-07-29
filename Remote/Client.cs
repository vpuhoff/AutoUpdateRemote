using System;
using System.Runtime.Remoting.Channels.Tcp;


namespace Remote
{
   public class Client
    {
        public ISharedTypeInterface RemoteObject;
        public Client(string host = "localhost",int port =9998)
        {
            var tcpChannel = new TcpChannel();
            var needreg = true;
            foreach (var item in System.Runtime.Remoting.Channels.ChannelServices.RegisteredChannels)
            {
                if (item.ChannelName==tcpChannel.ChannelName )
                {
                    needreg = false;
                }
            }
            if (needreg )
            {
                try
                {
                    System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(tcpChannel, true);
                }
                catch (Exception)
                {
                }
            }
            

            Type requiredType = typeof(ISharedTypeInterface);
           ret1: try
            {
                RemoteObject = (ISharedTypeInterface)Activator.GetObject(requiredType,
           "tcp://" + host + ":" + port + "/RemoteService");
                string stat = RemoteObject.GetStatus();
                if (stat != "")
                {
                    Console.WriteLine(stat);
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
                goto ret1;
            }
        }
    }
}
