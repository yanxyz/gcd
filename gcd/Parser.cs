using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcd
{
    public class Parser
    {
        public string Url { get; private set; }
        public string Branch { get; private set; }
        public string Dir { get; private set; }

        public Parser(string urlString)
        {
            Url = urlString.TrimEnd('/');

            if (Url.StartsWith("git@"))
            {
                ParseSSH();
            }
            else if (Url.StartsWith("https://"))
            {
                var uri = new Uri(Url);
                switch (uri.Host)
                {
                    case "github.com":
                        ParseGithub(uri);
                        break;
                    case "git.coding.net":
                        ParseCodingHttps(uri);
                        break;
                    case "coding.net":
                        ParseCoding(uri);
                        break;
                    default:
                        throw new Exception("Only Github url is supported.");
                }
            }
            else
            {
                throw new Exception("Invalid url.");
            }
        }

        private void ParseSSH()
        {
            var i = Url.IndexOf(":");
            if (Url.EndsWith(".git"))
            {
                ++i;
                var path = Url.Substring(i, Url.Length - i - 4);
                Dir = path.Replace('/', '-');
            }
        }

        private void ParseGithub(Uri uri)
        {
            var parts = uri.AbsolutePath.Split('/');

            if (parts.Length == 3)
            {
                var p = parts[2];
                if (p.EndsWith(".git"))
                {
                    p = p.Substring(0, p.Length - 4);
                    Dir = parts[1] + "-" + p;
                }
                else
                {
                    Dir = parts[1] + "-" + p;
                    var path = String.Join("/", parts);
                    Url = new Uri(uri, path).AbsoluteUri;
                }
            }
            else if (parts.Length > 4)
            {
                var s = parts[3];
                if (s == "blob" || s == "tree")
                {
                    if (parts[4].Length > 39)
                        throw new Exception("Cannot get branch");

                    Branch = parts[4];

                    var owner = parts[1];
                    var repo = parts[2];
                    Dir = owner + "-" + repo;

                    var path = $"/{owner}/{repo}";
                    Url = new Uri(uri, path).AbsoluteUri;
                }
            }
        }

        private void ParseCodingHttps(Uri uri)
        {
            var path = uri.AbsolutePath;
            if (path.EndsWith(".git"))
            {
                Dir = path.Substring(1, path.Length - 5).Replace('/', '-');
            }
        }

        private void ParseCoding(Uri uri)
        {
            var parts = uri.AbsolutePath.Split('/');
            var length = parts.Length;
            if (length < 6 || parts[1] != "u" || parts[3] != "p") return;

            var u = parts[2];
            var p = parts[4];
            Dir = u + "-" + p;
            Url = $"https://git.coding.net/{u}/{p}";

            if (length > 6)
            {
                var s = parts[6];
                if (s == "blob" || s == "tree") Branch = parts[7];
            }
        }
    }
}
