using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        int updates = 0;
        public bool  GetUpdates(string projectName)
        {
            int errcnt = 0;
            int trycnt = 0;
            
        ret1: Console.WriteLine("Загрузка информации об обновлении...");
            var ps = RemoteObject.GetProjectState(projectName);
            List<SharedType.FileState> needfiles = new List<SharedType.FileState>();
            Console.WriteLine("Проверка приложения...");
            foreach (var fs in ps)
            {
                string curfile = GetCurFile(fs);
                Console.WriteLine("Проверка файла:"+curfile);
                CheckFile(needfiles, fs, curfile);
            }
            if (needfiles.Count>0)
            {
                Console.WriteLine("Файлов обновлено:" + needfiles.Count.ToString());
                Console.WriteLine("Загрузка обновлений..." );
                var pd = RemoteObject.GetProjectData(needfiles, projectName);
                trycnt++;
                //var pd = st.GetProjectData(needfiles, projectName);
                Console.WriteLine("Установка обновлений...");
                foreach (var fs in pd)
                {
                    string curfile = GetCurFile(fs);
                    string backup = curfile + ".bak";
                    Console.WriteLine(curfile+":"+fs.data.Length );
                    Console.WriteLine("Создание резервной копии:" + curfile);
                    CreateBackup(curfile, backup);

                    Console.WriteLine("Обновление файла:" + curfile);
                    errcnt = WriteAndCheck(errcnt, fs, curfile, backup);
                }
                if (errcnt>0)
                {
                    Console.WriteLine("Обновление завершено с ошибками в количестве: " + errcnt.ToString());
                    Console.WriteLine("Повторная попытка №" + trycnt.ToString());
                    if (trycnt<3)
                    {
                        goto ret1;
                    }
                }
                else
                {
                    Console.WriteLine("Обновление успешно завершено.");
                }
                if (updates>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Обновление не требуется.");
                return false;
            }
        }

        private void CheckFile(List<SharedType.FileState> needfiles, SharedType.FileState fs, string curfile)
        {
            if (File.Exists(curfile))
            {
                if (st.HASHer.Hash(File.ReadAllBytes(curfile)) != fs.hash)
                {
                    AddToNeedFiles(needfiles, fs);
                }
            }
            else
            {
                AddToNeedFiles(needfiles, fs);
            }
        }

        private int WriteAndCheck(int errcnt, SharedType.FileState fs, string curfile, string backup)
        {
            File.WriteAllBytes(curfile, fs.data);
            if (st.HASHer.Hash(File.ReadAllBytes(curfile)) != fs.hash)
            {
                File.Delete(curfile);
                Console.WriteLine("Сохраненный файл отличается от ожидаемого. Будет восстановлен исходный файл." + curfile);
                File.Move(backup, curfile);
                errcnt++;
            }
            else
            {
                updates++;
            }
            GC.Collect();
            return errcnt;
        }

        private static void AddToNeedFiles(List<SharedType.FileState> needfiles, SharedType.FileState fs)
        {
            SharedType.FileState stn = new SharedType.FileState();
            stn.hash = fs.hash;
            stn.filepath  = fs.filepath ;
            needfiles.Add(stn);
        }

        private static void CreateBackup(string curfile, string backup)
        {
            if (File.Exists(curfile))
            {
                if (File.Exists(backup))
                {
                    File.Delete(backup);
                }
                File.Move(curfile, backup);
                File.Delete(curfile);
            }
        }

        private string GetCurFile(SharedType.FileState fs)
        {
            string curfile = fs.filepath.Substring(fs.filepath.LastIndexOf(ProjDir));
            curfile = curfile.Substring(ProjDir.Length);
            curfile = curfile.Substring(curfile.IndexOf("\\"));
            curfile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + curfile;
            return curfile;
        }
    }
}
