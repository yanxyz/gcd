using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace gcd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isCloning = false;
        private int pid;
        private string cloneDir;

        public MainWindow()
        {
            InitializeComponent();
            tbUrl.Text = "https://github.com/yanxyz/test.git";
            InputLanguageManager.SetInputLanguage(this, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
        }

        private string Parse(string url)
        {
            var repo = new Parser(url);
            if (repo.Dir == null)
                throw new Exception("Cannot get dir");

            var cmd = $"git clone --depth=1 {repo.Url} {repo.Dir}";
            if (repo.Branch != null) cmd += $" --branch \"{repo.Branch}\"";
            cloneDir = repo.Dir;
            return cmd;
        }

        private void tbUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            var url = tbUrl.Text.Trim();
            if (url.Length == 0)
            {
                tbCmd.Text = "";
                tbLog.Text = "";
                return;
            }

            try
            {
                var cmd = Parse(url);
                tbCmd.Text = cmd;
                tbLog.Text = "";
            }
            catch (Exception ex)
            {
                tbLog.Text = ex.Message;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var arguments = Prepare();
            if (String.IsNullOrEmpty(arguments)) return;

            arguments = arguments.Substring(3);
            var progressOption = " --progress";
            if (arguments.IndexOf(progressOption) == -1)
                arguments += progressOption;

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git.exe",
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.Exited += new EventHandler(Process_Exited);
            
            btnOk.IsEnabled = false;
            tbLog.Text = "";
            try
            {
                process.Start();
                pid = process.Id;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Could not find git.exe, please make sure it is added to Path.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string Prepare()
        {
            var arguments = tbCmd.Text.Trim();

            if (!arguments.StartsWith("git clone"))
            {
                tbLog.Text = "Not 'git clone' command.";
                return null;
            }

            return arguments;
        }

        private void After()
        {
            btnOk.IsEnabled = true;

            if (isCloning)
            {
                btnStop.Visibility = Visibility.Hidden;
                isCloning = false;
            }
        }

        private void clearFolder(string FolderName)
        {
            var dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

        private void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            var line = e.Data;
            Debug.WriteLine($"xxx {line}");
            if (line == null) return;
            if (isCloning && line.IndexOf(" done.") == -1) return;
            if (line.StartsWith("Cloning into ")) isCloning = true;

            this.Dispatcher.Invoke(() =>
            {
                if (isCloning && btnStop.Visibility == Visibility.Hidden)
                {
                    btnStop.Visibility = Visibility.Visible;
                }
                tbLog.AppendText(line + "\n");
            });
        }

        private void Process_Exited(object sender, System.EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                After();
            });
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (pid == 0) return;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = $"/t /f /pid {pid}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();
            if (proc.ExitCode == 0)
            {
                After();
                tbLog.AppendText("Canceled");
                if (Directory.Exists(cloneDir))
                    clearFolder(cloneDir);
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/yanxyz/gcd/");
        }

        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            var arguments = Prepare();
            if (String.IsNullOrEmpty(arguments)) return;

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k {arguments}",
            };

            Process.Start(startInfo);
        }
    }
}
