using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace gcd
{
    static class App
    {
        public static Assembly Reference = typeof(App).Assembly; 

        public static void ShowHelp()
        {
            var name = "gcd.Usage.txt";
            using (var stream = Reference.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Console.Write(result);
            }
        }

        public static void ShowVersion()
        {
            Console.WriteLine(Reference.GetName().Version);
        }
    }
}
