using Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var linkTimeLocal = GetLinkerTime(Assembly.GetExecutingAssembly());
            Console.Title = "Сервер обновлений v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " от " + linkTimeLocal.ToShortDateString()+" " + linkTimeLocal.ToLongTimeString ();
            Remote.Server Server = new Remote.Server(9999);
            Console.WriteLine("Нажмите любую клавишу, чтобы выйти...");
            Console.ReadLine();
        }

        static public DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }
}
