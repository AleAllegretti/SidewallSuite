using Squirrel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace CheckUpdates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            using (var manager = new UpdateManager(@"C:\Temp\Releases"))
            {
                await manager.UpdateApp();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            {
                var client = new WebClient();
                client.Headers.Add("User-Agent", "C# console program");

                string url = "http://www.a2engineering.it/updatesconveyorbeltspack/";
                try
                {
                    string content = client.DownloadString(url);
                    string version = "Versione:" + "1.0.0.1";
                    if (content.Contains(version))
                    {
                        MessageBox.Show(version);
                    }
                    else
                    {
                        MessageBox.Show("Devi aggiornare");
                    }
                }
                catch (Exception)
                {

                }

            }
        }
    }
}
