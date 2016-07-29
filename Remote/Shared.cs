using HashTableHashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            public FileInfo Info;
            public byte[] data;
        }

        public List<FileState> GetProjectState(string projectname)
        {
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
        public List<FileState> GetProjectData(string projectname)
        {
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
        public List<FileState> GetProjectData(List<FileState> needfiles, string projectname)
        {
            string project = projDir + projectname;
            foreach (var file in needfiles)
            {
                if (File.Exists(file.Info.FullName))
                {
                    file.data = File.ReadAllBytes(file.Info.FullName);
                }
            }
            return needfiles;
        }

        private FileState GetFileState(string file, FileInfo f)
        {
            FileState fs = new FileState();
            fs.Info = f;
            var data = File.ReadAllBytes(file);
            fs.hash = HASHer.Hash(data);
            data = null;
            return fs;
        }

        private FileState GetFileStateWithData(string file, FileInfo f)
        {
            FileState fs = new FileState();
            fs.Info = f;
            var data = File.ReadAllBytes(file);
            fs.hash = HASHer.Hash(data);
            fs.data = data;
            return fs;
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
            return "OK";
        }

    }
}
