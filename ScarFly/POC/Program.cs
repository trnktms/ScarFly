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
            string LevelName = "level_1.level";
            List<string> rows = new List<string>();
            if (File.Exists(LevelName))
            {
                using (StreamReader reader = new StreamReader(LevelName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null) { rows.Add(line); }
                }
            }

            for (int i = 0; i < rows.Count; i++)
            {
                Console.WriteLine("{0} - {1}", i, rows[i]);
            }

            Console.ReadLine();
        }
    }
}
