using Remote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LabelStreamWriter lbsw = new LabelStreamWriter(label1);
            var args= Environment.GetCommandLineArgs();
            Console.SetOut(lbsw);
            string host = args[1];
            int port = int.Parse(args[2]);
            string project = args[3];
            string exe = args[4];
            var linkTimeLocal = GetLinkerTime(Assembly.GetExecutingAssembly());
            this.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString() + " от " + linkTimeLocal.ToShortDateString();
            new Thread(call =>
            {
                try
                {
                    Client Updater = new Client(host, port);
                    List<Process> processes = GetCorrectProcesses(exe);
                    if (Updater.GetUpdates(project))
                    {
                        if (processes.Count() > 0)
                        {
                            foreach (var item in processes)
                            {
                                item.Kill();
                            }
                        }
                        Process.Start(exe);
                    }
                    else
                    {
                        if (processes.Count() == 0)
                        {
                            Process.Start(exe);
                        }
                    }
                    Application.Exit();
                }
                catch (Exception)
                {
                    List<Process> processes = GetCorrectProcesses(exe);
                    if (processes.Count() > 0)
                    {

                    }
                    else
                    {
                        Process.Start(exe);
                    }
                    Application.Exit();
                }
                
            }).Start();
        }

        private static List<Process> GetCorrectProcesses(string exe)
        {
            var processess = Process.GetProcesses();
            List<Process> processes = GetProcess(exe, processess);
            return processes;
        }

        private static List<Process> GetProcess(string exe, Process[] processess)
        {
            List<Process> processes = new List<Process>();
            foreach (var item in processess)
            {
                try
                {
                    if (Path.GetFileName(item.MainModule.FileName) == exe)
                    {
                        processes.Add(item);
                    }
                }
                catch (Exception)
                {

                }
            }
            return processes;
        }

        class LabelStreamWriter : TextWriter
        {
            Label  _output = null;
            public LabelStreamWriter(Label output)
            {
                _output = output;
            }
            string s = "";
            public override void Write(char value)
            {
                base.Write(value);
                if (value == '\r')
                {
                    if (_output.InvokeRequired )
                    {
                        _output.Invoke(new Action(() =>
                        {
                            if (s!="")
                            {
                                _output.Text = s;
                            }
                        }));
                    }
                    s = "";
                }
                else
                {
                    s += value.ToString();
                }
            }
            public override Encoding Encoding
            {
                get { return Encoding.Default; }
            }
        }
        public DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
