using BeltsPack.Models;
using BeltsPack.ViewModels;
using BeltsPack.Views;
using BeltsPack.Views.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Net;
using System.IO;
using System.Xml;

namespace BeltsPack 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string _nomeutente { get; set; }
        Prodotto prodotto = new Prodotto();
        public string currentVersion { get; set; }
        public MainWindow()
        {
            this.DataContext = this;
            // Prendo il nome utente
            this._nomeutente = "Ciao, " + this.prodotto.DeterminoNomeUtente(@"C:\ASquared\Info.xml");
            this.prodotto.Utente = this.prodotto.DeterminoNomeUtente(@"C:\ASquared\Info.xml");

            // Versione software
            this.currentVersion = "Versione: " + this.GetcurrentVersion();
            InitializeComponent();
            _mainFrame.Navigate(new InputView(prodotto));

            // Permette di far vedere la barra delle applicazioni quando l'applicazione è a tutto schermo
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

        }

        
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
                    }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

      
        private void ButtonMaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            ButtonMaximizeWindow.Visibility = Visibility.Collapsed;
            ButtonRestoreWindow.Visibility = Visibility.Visible;
        }

        private async void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            // Avviso se si è sicuri di chiudere il programma
            ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro di voler uscire dal programma?", ConfirmDialog.ButtonConf.YES_NO);

            // Esce dal programma
            if (confirmed.ToString() == "Yes")
            {
                this.Close();
            }
            
        }

        private void ButtonRestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            ButtonMaximizeWindow.Visibility = Visibility.Visible;
            ButtonRestoreWindow.Visibility = Visibility.Collapsed;
        }

        private void ButtonMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ListViewItem_Selected_1(object sender, RoutedEventArgs e)
        {
            this._mainFrame.NavigationService.Navigate(new DatabaseView());
        }

        private void Home_Selected(object sender, RoutedEventArgs e)
        {
            this.Lista.SelectedItem = null;
            this._mainFrame.NavigationService.Navigate(new InputView(prodotto));
        }

        private void Impostazioni_Selected(object sender, RoutedEventArgs e)
        {
            this._mainFrame.NavigationService.Navigate(new SettingsMenu());
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ListViewItem_Selected_2(object sender, RoutedEventArgs e)
        {
            // Vado alla schermata del diagramma di Gantt
            this._mainFrame.NavigationService.Navigate(new GanttView());      
        }

        private void ListViewItem_Selected_3(object sender, RoutedEventArgs e)
        {
            // Vado alla schermata della produzione
            this._mainFrame.NavigationService.Navigate(new ReportProduzioneView());
        }

        private void About_Selected(object sender, RoutedEventArgs e)
        {
            // Vado alla schermata about
            this._mainFrame.NavigationService.Navigate(new AboutView());
        }
        public string GetcurrentVersion()
        {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Login_Selected(object sender, RoutedEventArgs e)
        {
            // Vado alla schermata di login
            this._mainFrame.NavigationService.Navigate(new LoginView());
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Pim_Selected(object sender, RoutedEventArgs e)
        {
            // Navigo alla schermata del PIM
            this._mainFrame.NavigationService.Navigate(new PimView());
        }

        private void Calcoli_Selected(object sender, RoutedEventArgs e)
        {
            // Navigo alla schermata dei calcoli
            this._mainFrame.NavigationService.Navigate(new CalcoliView(prodotto));
        }
    }
}
