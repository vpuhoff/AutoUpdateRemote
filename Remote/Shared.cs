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
        List<FileInfo> GetProjectState(string projectname);
    }

    public class SharedType : MarshalByRefObject, ISharedTypeInterface
    {

        public List<FileInfo> GetProjectState(string projectname)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string project = projDir + projectname;
            var files = Directory.GetFiles(project, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                FileInfo f = new FileInfo(file);
                filesInfo.Add(f);
            }
            return filesInfo;
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
