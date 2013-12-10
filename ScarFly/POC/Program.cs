using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10000; i++)
            {
                Guid g1 = Guid.NewGuid();
                Guid g2 = Guid.NewGuid();
                Console.Write(g1.CompareTo(g2) + ";");
            }
            Console.ReadLine();
        }
    }
}
