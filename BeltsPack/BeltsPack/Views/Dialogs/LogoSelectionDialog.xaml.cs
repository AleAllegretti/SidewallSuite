using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static BeltsPack.Models.Prodotto;

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for LogoSelectionDialog.xaml
    /// </summary>
    public partial class LogoSelectionDialog : UserControl
    {
        public bool noteEnglish { get; set; }
        public bool noteGerman { get; set; }
        public bool noteSpanish { get; set; }
        public bool noteItalian { get; set; }

        public LogoSelectionDialog()
        {
            // Abilito il data binding
            this.DataContext = this;

            // Inizializzo le note
            this.noteEnglish = true;
            this.noteGerman = false;
            this.noteItalian = false;
            this.noteSpanish = false;

            InitializeComponent();

            List<Fornitore> listOfVendors = new List<Fornitore>();

            listOfVendors.Add(new Fornitore()
            {
                Name = "Vuoto",
                ImagePath = "/Assets/Images/Empty_Logo.png",
                ImageLocalPath = "",
                Height = 1024,
                Width = 1024,
                Language = ""
                
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Rik",
                ImagePath = "/Assets/Images/Logo_Rik.png",
                ImageLocalPath = @"Assets\Images\Logo_Rik.png",
                Height = 60,
                Width = 49,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Buddh",
                ImagePath = "/Assets/Images/Logo_Bud.png",
                ImageLocalPath = @"Assets\Images\Logo_Bud.png",
                Height = 65,
                Width = 57,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Bull",
                ImagePath = "/Assets/Images/Logo_Bull.png",
                ImageLocalPath = @"Assets\Images\Logo_Bull.png",
                Height = 129,
                Width = 30,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Centrobelt",
                ImagePath = "/Assets/Images/Logo_Centrobelt.png",
                ImageLocalPath = @"Assets\Images\Logo_Centrobelt.png",
                Height = 108,
                Width = 21
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Ersa",
                ImagePath = "/Assets/Images/Logo_Ersa.png",
                ImageLocalPath = @"Assets\Images\Logo_Ersa.png",
                Height = 119,
                Width = 30,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Lutze",
                ImagePath = "/Assets/Images/Logo_Lutze.png",
                ImageLocalPath = @"Assets\Images\Logo_Lutze.png",
                Height = 126,
                Width = 22,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Martial",
                ImagePath = "/Assets/Images/Logo_Martial.png",
                ImageLocalPath = @"Assets\Images\Logo_Martial.png",
                Height = 111,
                Width = 30,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "MBS",
                ImagePath = "/Assets/Images/Logo_MBS.png",
                ImageLocalPath = @"Assets\Images\Logo_MBS.png",
                Height = 119,
                Width = 22,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Primogum",
                ImagePath = "/Assets/Images/Logo_Primogum.png",
                ImageLocalPath = @"Assets\Images\Logo_Primogum.png",
                Height = 119,
                Width = 27,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Rema",
                ImagePath = "/Assets/Images/Logo_Rema.png",
                ImageLocalPath = @"Assets\Images\Logo_Rema.png",
                Height = 118,
                Width = 21,
                Language = ""
            });
            listOfVendors.Add(new Fornitore()
            {
                Name = "Sig",
                ImagePath = "/Assets/Images/Logo_Sig.png",
                ImageLocalPath = @"Assets\Images\Logo_Sig.png",
                Height = 136,
                Width = 68,
                Language = ""
            });

            this.listVendors.ItemsSource = listOfVendors;

            // Seleziono il logo vuoto
            this.listVendors.SelectedValue = "Vuoto";
        }

        private void BtnNessunLogo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void ConfermaLogo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Fornitore selectedItem = (Fornitore)this.listVendors.SelectedItem;
            if (selectedItem != null)
            {
                // Capisco quale lingguaggio è stato selezionato
                if (this.Italian.IsChecked == true) { selectedItem.Language = "Italian"; }
                if (this.German.IsChecked == true) { selectedItem.Language = "German"; }
                if (this.English.IsChecked == true) { selectedItem.Language = "English"; }
                if (this.Spanish.IsChecked == true) { selectedItem.Language = "Spanish"; }

                // close the dialog with the selected item as result
                DialogHost.CloseDialogCommand.Execute(selectedItem, null);
            }
            else
            {
                System.Windows.MessageBox.Show("Attenzione, non hai selezionato il logo.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
    }
}
