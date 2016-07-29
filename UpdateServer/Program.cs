using Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SharedType st = new SharedType();
            var t =st.getProjects();
            foreach (var item in t)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }
}
