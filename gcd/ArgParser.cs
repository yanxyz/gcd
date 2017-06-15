using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcd
{
    class Options
    {
        public string Url;
        public bool WillRun = false;
        public bool Help = false;
        public bool Version = false;
    }

    static class ArgParser
    {
        public static Options Parse(string[] args)
        {
            var options = new Options();
            if (args.Length == 0) return options;

            foreach (var arg in args)
            {
                if (arg.StartsWith("--"))
                {
                                        
                }

                if (arg.StartsWith("-"))
                {

                }

                if (!arg.StartsWith("-"))
                {
                    options.Url = arg;
                    break;
                }

                if (arg == "-h" || arg == "--help")
                {
                    options.Help = true;
                    break;
                }

                if (arg == "-v" || arg == "--version")
                {
                    options.Version = true;
                    break;
                }
            }

            return options;
        }
    }
}
