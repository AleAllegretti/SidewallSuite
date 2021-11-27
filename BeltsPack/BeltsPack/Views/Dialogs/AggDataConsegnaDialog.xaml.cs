using BeltsPack.Utils;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AggDataConsegnaDialog.xaml
    /// </summary>
    public partial class AggDataConsegnaDialog : UserControl
    {
        MailManager mailManager = new MailManager();
        public string _datavecchia { get; set; }
        public DateTime _startDate { get; set; }
        public string _codice { get; set; }
        public string _cliente { get; set; }
        public int _versione { get; set; }
        public AggDataConsegnaDialog(string codice, int versione, string dataconsegna, string cliente)
        {
            this.DataContext = this;

            // Inizializzo la data
            this._startDate = DateTime.Now.AddDays(15);
            // Faccio comparire la data vecchia
            this._datavecchia = dataconsegna;

            this._cliente = cliente;
            this._codice = codice;
            this._versione = versione;
            InitializeComponent();
        }

        private void BtnConferma_Click(object sender, RoutedEventArgs e)
        {
            if(this.Invia.IsChecked == false & this.NonInviare.IsChecked == false)
            {               
               System.Windows.MessageBox.Show("Prima di procedere, devi decidere se inviare la notifica di aggiornamento in produzione.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Aggiorno la data di consegna
                try
                {
                    DatabaseSQL database = DatabaseSQL.CreateDefault();
                    SqlCommand creacomando = database.UpdateDataConsegnaCassaCommand(this._codice, this._startDate.ToString("d/M/yyyy"), this._versione);
                    creacomando.ExecuteNonQuery();

                    if (this.Invia.IsChecked == true)
                    {
                        // Prepara l'email
                        var mail = this.mailManager.PrepareEmail("");
                        mail = this.mailManager.PrepareEmail("Aggiornamento data per commessa N°" + this._codice + " - " + this._cliente, "g.marchini@morinat.com");
                        mail.Body = "E' stata aggiornata la data di consegna dell'imballo in oggetto. La nuova data di consegna è: " + this._startDate.ToString("d/M/yyyy");

                        // Chiudo il dialog
                        DialogHost.CloseDialogCommand.Execute(null, null);

                        // Mostra outlook
                        try
                        {
                            mail.Show();
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("In questo momento non è possibile inviare l'email.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        // Chiudo il dialog
                        DialogHost.CloseDialogCommand.Execute(null, null);
                    }
                }
                catch
                {

                }
            }
            
        }

        private void Invia_Checked(object sender, RoutedEventArgs e)
        {
            this.NonInviare.IsChecked = false;
        }

        private void NonInviare_Checked(object sender, RoutedEventArgs e)
        {
            this.Invia.IsChecked = false;
        }
    }
}
