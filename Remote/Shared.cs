using HashTableHashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remote
{
    public interface ISharedTypeInterface
    {
        string GetStatus();
        string[] getProjects();
        List<Remote.SharedType.FileState> GetProjectState(string projectname);
        List<Remote.SharedType.FileState> GetProjectData(string projectname);
        List<Remote.SharedType.FileState> GetProjectData(List<Remote.SharedType.FileState> needfiles, string projectname);
    }

    public class SharedType : MarshalByRefObject, ISharedTypeInterface
    {
        public SuperFastHashSimple HASHer = new SuperFastHashSimple();

        [Serializable ]
        public class FileState
        {
            public UInt32 hash;
            //public FileInfo Info;
            public string filepath;
            public byte[] data;
        }

        public List<FileState> GetProjectState(string projectname)
        {
            try
            {
                Console.WriteLine("GetProjectState:" + projectname);
                List<FileState> filesInfo = new List<FileState>();
                string project = projDir + projectname;
                var files = Directory.GetFiles(project, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    filesInfo.Add(GetFileState(file, new FileInfo(file)));
                    GC.Collect();
                }
                return filesInfo;
            }
            catch (Exception e)
            {
                throw NewThrow(e);
            }
        }

        private static Exception NewThrow(Exception e)
        {
            Console.WriteLine(e.Message + e.Source + e.StackTrace);
            return new Exception(e.Message + e.Source + e.StackTrace);
        }
        public List<FileState> GetProjectData(string projectname)
        {
            try
            {
                Console.WriteLine("GetProjectData:" + projectname);
                List<FileState> filesInfo = new List<FileState>();
                string project = projDir + projectname;
                var files = Directory.GetFiles(project, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    filesInfo.Add(GetFileStateWithData(file, new FileInfo(file)));
                    GC.Collect();
                }
                return filesInfo;
            }
            catch (Exception e)
            {
                throw NewThrow(e);
            }
        }
        public List<FileState> GetProjectData(List<FileState> needfiles, string projectname)
        {
            try
            {
                Console.WriteLine("GetProjectData:" + projectname);
                string project = projDir + projectname;
                foreach (var file in needfiles)
                {
                    if (File.Exists(file.filepath ))
                    {
                        file.data = File.ReadAllBytes(file.filepath);
                    }
                }
                return needfiles;
            }
            catch (Exception e)
            {
                throw NewThrow(e);
            }
        }

        private FileState GetFileState(string file, FileInfo f)
        {
            try
            {
                Console.WriteLine("GetFileState:" + file);
                FileState fs = new FileState();
                fs.filepath = f.FullName ;
                var data = File.ReadAllBytes(file);
                fs.hash = HASHer.Hash(data);
                data = null;
                return fs;
            }
            catch (Exception e)
            {
                throw NewThrow(e);
            }
        }

        private FileState GetFileStateWithData(string file, FileInfo f)
        {
            try
            {
                Console.WriteLine("GetFileStateWithData:" + file);
                FileState fs = new FileState();
                fs.filepath  = f.FullName ;
                var data = File.ReadAllBytes(file);
                fs.hash = HASHer.Hash(data);
                fs.data = data;
                return fs;
            }
            catch (Exception e)
            {
                throw NewThrow(e);
            }
        }

        string projDir = ".\\Projects\\";
        public string[] getProjects()
        {
            if (!Directory.Exists(projDir))
            {
                Directory.CreateDirectory(projDir);
            }
            return Directory.GetDirectories(projDir);
        }

        
        public string GetStatus()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ":Server status: OK" );
            return "OK";
        }

    }
}
