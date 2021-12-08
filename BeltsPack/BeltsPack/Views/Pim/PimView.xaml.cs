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
    /// Interaction logic for PimView.xaml
    /// </summary>
    public partial class PimView : Page
    {
        public PimView()
        {
            InitializeComponent();
        }

        private void ViewPim_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ImpostazioniPim_Click(object sender, RoutedEventArgs e)
        {
            // Navigo alla schermata di impostazioni del PIM
            this.NavigationService.Navigate(new PimSettingsView());
        }
    }
}
