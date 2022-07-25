using BeltsPack.Views.Dialogs;
using BeltsPack.Views.Settings;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for SettingsMenu.xaml
    /// </summary>
    public partial class SettingsMenu : Page
    {
        public SettingsMenu()
        {
            InitializeComponent();
        }

        private void ImpostazioniPedaneLegno_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsView());
        }

        private void ListinoPaladini_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsListinoPaladini());
        }

        private void Ferro_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsPrezzoFerro());
        }

        private void Accessori_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingPrezzoAccessori());
        }

        private void Gestione_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingManagementCosts());
        }

        private async void Vari_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsVariousCosts());
        }

        private void Bordi_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsBordi());
        }

        private void Nastri_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsNastri());
        }

        private void Tazze_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsTazze());
        }

        private void Parametri_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsParameters());
        }

        private void Tipo_Casse_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsTipologiaCasse());
        }
    }
}
