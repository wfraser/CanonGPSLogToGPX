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

using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace CanonGPSLogWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            var inputPath = InputPath.Text;

            if (!File.Exists(inputPath))
            {
                MessageBox.Show("The input file you selected does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new OpenFileDialog();
            dialog.Filter = "GPX File|*.gpx";
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(inputPath);
            dialog.CheckFileExists = false;

            bool? ok = dialog.ShowDialog();
            if (!ok.HasValue || !ok.Value)
            {
                return;
            }

            string outputPath = dialog.FileName;

            if (File.Exists(outputPath))
            {
                MessageBoxResult confirm = MessageBox.Show("The selected file already exists and will be overwritten. Continue?", "Confirm Overwrite", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
                if (confirm == MessageBoxResult.Cancel)
                    return;
            }

            try
            {
                CanonGPSLog.Converter.LogToGPX(inputPath, outputPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MessageBoxResult result = MessageBox.Show("File successfully converted. Show resulting file?", "Success", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                OpenExplorerAtFile(outputPath);
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Canon GPS Track|*.LOG";

            bool? ok = dialog.ShowDialog();
            if (ok.HasValue && ok.Value)
            {
                InputPath.Text = dialog.FileName;
            }
        }

        private void OpenExplorerAtFile(string path)
        {
            IntPtr pidlList = NativeMethods.ILCreateFromPathW(path);
            if (pidlList != IntPtr.Zero)
            {
                try
                {
                    int hr = NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                    Marshal.ThrowExceptionForHR(hr);
                }
                finally
                {
                    NativeMethods.ILFree(pidlList);
                }
            }
        }
    }
}
