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

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Page
    {
        public AboutView()
        {
            this.DataContext = this;
            InitializeComponent();

            // Mostro la pagina del wiki
            webBrowser.Navigate("http://www.a2engineering.it/wp-content/uploads/WikiCBM/MANUALE_UTENTE.htm");
            this.ButtonBack.Visibility = Visibility.Visible;
        }

        private void EsportaExcel_Click(object sender, RoutedEventArgs e)
        {
            // Torno al manuale utente
            webBrowser.Navigate("http://www.a2engineering.it/wp-content/uploads/WikiCBM/MANUALE_UTENTE.htm");
        }


    }
}
