using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CurrentTheHomework2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _pwd.Text = ShrinkPath(__pwd.FullName);
            _ChangeDisk.ItemsSource = System.IO.DriveInfo.GetDrives();
            _cmd.Text = "echo ready;g++ b* -o ./b.exe ; ./b.exe; rm ./b.exe ;echo complete ; read -n 1 key";
            Ls();
        }

        DirectoryInfo AppDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        DirectoryInfo __pwd = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        DirectoryInfo Pwd
        {
            get
            {
                return __pwd;
            }
            set
            {
                __pwd = value;
                _pwd.Text = ShrinkPath(value.FullName);
            }
        }

        void Ls()
        {
            try
            {
                _ls.ItemsSource =
                    Pwd.EnumerateDirectories().Select(x => new DirItem() { DirectoryInfo = x }).Concat(
                        Pwd.EnumerateFiles().Select(x => new DirItem() { FileInfo = x }));
                var files = Pwd.EnumerateFiles("*.cpp");
                if (files.Count() > 0)
                {
                    catPath = files.ElementAt(0).FullName;
                    cat();
                }
            }
            catch (Exception) { }

        }

        private void CdSpaceDotDOt_Click(object sender, RoutedEventArgs e)
        {
            if (Pwd.Parent != null)
            {
                Pwd = Pwd.Parent;
                Ls();
            }
        }
        private void CdSpaceMinus1_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CdSpacePlus1_Click(object sender, RoutedEventArgs e)
        {

        }

        
        private void Ls_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var list = (sender as ListView);
            var item = (list.SelectedItem as DirItem);
            if (item.IsDirectory)
            {
                Pwd = item.DirectoryInfo;
                Ls();
            }
            if (item.IsFile)
            {
                catPath = item.FileInfo.FullName;
                cat();
            }

        }

        Regex Reg_PathNode = new Regex(@"\\[^\\:]+");
        private string ShrinkPath(string path)
        {
            string disk = path.Substring(0, 2);
            var matches = Reg_PathNode.Matches(path);
            for(int i = 0; i < matches.Count-1; i ++)
            {
                disk += matches[i].Value.Substring(0, 2);
            }
            if (matches.Count >= 1)
            {
                disk += matches[matches.Count - 1].Value;
            }
            else
            {
                disk += "\\";
            }

            return disk;
        }

        private void ChangeDisk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ComboBox).SelectedItem as DriveInfo;
            Pwd = item.RootDirectory;
            Ls();
        }


        void ExecProgram(string path, string args = "")
        {
            Console.WriteLine(path+" "+args);
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, File.ReadAllText("./input.txt"));
            ExecProgram(@"C:\Program Files\Git\bin\bash",
                $@"-c '" +
                $@"cd ""{Pwd.FullName}"" ;" +
                $@"{_cmd.Text}'");

        }

        Encoding encoding = Encoding.GetEncoding(950);
        string catPath = "";
        private void cat()
        {
            Source.Text = File.ReadAllText(catPath, encoding);
        }

        private void ChangeToBig5_Click(object sender, RoutedEventArgs e)
        {
            encoding = Encoding.GetEncoding(950);
            if (!String.IsNullOrWhiteSpace(catPath))
                cat();
        }
        private void ChangeToUtf8_Click(object sender, RoutedEventArgs e)
        {
            encoding = Encoding.UTF8;
            if (!String.IsNullOrWhiteSpace(catPath))
                cat();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Pwd.FullName + "\\b.cpp", Source.Text);
        }
        private void SaveHeader_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Pwd.FullName + "\\bclass.h", Source.Text);
        }
        private void SaveClass_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Pwd.FullName + "\\bclass.cpp", Source.Text);
        }
    }

    public class DirItem
    {
        public bool IsFile => FileInfo != null;
        public bool IsDirectory => DirectoryInfo != null;
        public DirectoryInfo DirectoryInfo { get; set; }
        public FileInfo FileInfo { get; set; }
        public override string ToString()
        {
            if(IsDirectory)
                return DirectoryInfo.ToString();
            if (IsFile)
                return FileInfo.ToString();
            return "Empty";
        }
    }
}
