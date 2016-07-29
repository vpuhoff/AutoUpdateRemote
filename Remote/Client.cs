using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels.Tcp;


namespace Remote
{
   public class Client
    {
        public ISharedTypeInterface RemoteObject;
        SharedType st = new SharedType();
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
        string ProjDir = "\\Projects\\";
        public void GetUpdates(string projectName)
        {
            var ps = RemoteObject.GetProjectState(projectName);
            List<Remote.SharedType.FileState> needfiles = new List<SharedType.FileState>();
            foreach (var fs in ps)
            {
                string curfile = GetCurFile(fs);
                if (File.Exists(curfile))
                {
                    if (st.HASHer.Hash(File.ReadAllBytes(curfile))!=fs.hash )
                    {
                        needfiles.Add(fs);
                    }
                }
                else
                {
                    needfiles.Add(fs);
                }
            }
            if (needfiles.Count>0)
            {
                var pd = RemoteObject.GetProjectData(needfiles, projectName);
                //var pd = st.GetProjectData(needfiles, projectName);
                foreach (var fs in pd)
                {
                    string curfile = GetCurFile(fs);
                    Console.WriteLine(curfile+":"+fs.data.Length );
                    if (File.Exists(curfile))
                    {
                        File.Delete(curfile);
                    }
                    File.WriteAllBytes(curfile, fs.data);
                    if (st.HASHer.Hash(File.ReadAllBytes(curfile))!=fs.hash )
                    {
                        throw new Exception("Сохраненный файл отличается от ожидаемого.");
                    }
                    GC.Collect();
                }
            }
        }

        private string GetCurFile(SharedType.FileState fs)
        {
            string curfile = fs.Info.FullName.Substring(fs.Info.FullName.LastIndexOf(ProjDir));
            curfile = curfile.Substring(ProjDir.Length);
            curfile = curfile.Substring(curfile.IndexOf("\\"));
            curfile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + curfile;
            return curfile;
        }
    }
}
