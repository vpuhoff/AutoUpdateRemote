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
            Remote.Server Server = new Remote.Server(9999);
            Console.ReadLine();
        }
    }
}
