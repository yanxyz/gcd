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
        public MainWindow()
        {
            InitializeComponent();
            InputLanguageManager.SetInputLanguage(this, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
        }

        private string Parse(string url)
        {
            var repo = new Parser(url);
            if (repo.Dir == null)
                throw new Exception("Cannot get dir");

            var cmd = $"git clone --depth=1 {repo.Url} {repo.Dir}";
            if (repo.Branch != null) cmd += $" --branch \"{repo.Branch}\"";
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
                Arguments = $"/k {arguments}"
            };

            Process.Start(startInfo);
            this.Close();
        }

        private void tbUrl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbUrl.IsKeyboardFocusWithin)
            {
                tbUrl.SelectAll();
                e.Handled = true;
                tbUrl.Focus();
            }
        }
    }
}
