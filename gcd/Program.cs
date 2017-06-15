using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gcd
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Help();
                return;
            }

            Parser repo;

            try
            {
                repo = new Parser(args[0]);
            }
            catch (Exception)
            {
                Warn();
                return;
            }
            
            if (repo.Dir == null)
            {
                Warn();
                return;
            }

            var cmd = $"git clone --depth=1 {repo.Url} {repo.Dir}";
            if (repo.Branch != null) cmd += $" --branch \"{repo.Branch}\"";
            Console.WriteLine(cmd);

            try
            {
                // 默认 1s 后才能抛出错误，这里提速
                Clipboard.SetDataObject(cmd, true, 2, 10);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                Console.Error.WriteLine("gcd: 复制到剪贴板失败，剪贴板被占用，比如下载工具监视剪贴板。");
            }
        }

        static void Help()
        {
            Console.WriteLine("gcd <repository_url>");
            Console.WriteLine("https://github.com/yanxyz/gcd#readme");
        }

        static void Warn()
        {
            Console.Error.WriteLine("invalid repository url");
        }
    }
}
