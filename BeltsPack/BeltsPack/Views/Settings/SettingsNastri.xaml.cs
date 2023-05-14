using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for SettingsNastri.xaml
    /// </summary>
    public partial class SettingsNastri : Page
    {

        SqlDataAdapter dataAdapter;
        DataSet DataSet;
        Boolean TabellaModificata = false;
        public SettingsNastri()
        {

            InitializeComponent();

            // Crea il wrapper del database
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Comando per mostrare la tabella con tutti i parametri degli imballi in legno
            SqlCommand creacomando = databaseSQL.CreateSettingNastriCommand();

            // Riempie la tabella
            DataSet = new DataSet();
            dataAdapter = new SqlDataAdapter(creacomando);
            SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(DataSet);          
            dataGrid.DataContext = DataSet.Tables[0];

            // Chiude la connessione
            databaseSQL.CloseConnection();

        }
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
            switch (e.Column.Header.ToString())
            {
                case "NomeNastro":
                    e.Column.Header = "Nome nastro";
                    break;

                case "SiglaNastro":
                    e.Column.Header = "Sigla Nastro Arca";
                    break;

                case "Classe":
                    e.Column.Header = "Classe [N/mm]";
                    break;

                case "PesoMQ":
                    e.Column.Header = "Peso [kg/m2]";
                    break;

                case "SpessoreSup":
                    e.Column.Header = "Spessore sup. [mm]";
                    break;

                case "SpessoreInf":
                    e.Column.Header = "Spessore inf. [mm]";
                    break;

                case "NumeroTele":
                    e.Column.Header = "N. breaker";
                    break;

                case "NumeroTessuti":
                    e.Column.Header = "N. tele";
                    break;

                case "MinimoDiametroPulley":
                    e.Column.Header = "Min. diam. pulley [mm]";
                    break;

                case "LunghezzaGradinoGiunta":
                    e.Column.Header = "Lungh. gradino giunta [mm]";
                    break;

                case "DataUltimoAggiornamento":
                    e.Column.Header = "Ultimo aggiornamento";
                    break;

            }
        }
        public async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string ID;
            var selectedItem = this.dataGrid.SelectedItem;

            if (selectedItem == null)
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'articolo da visualizzare");
                return;
            }
            else
            {
                var dataRow = (selectedItem as DataRowView).Row;
                ID = dataRow["ID"].ToString();

                // Abilita il tasto di salvataggio
                ModificaDimensioni.IsEnabled = true;

                // Aggiorno il database con l'ultima data di aggiornamento
                dataRow["DataUltimoAggiornamento"] = DateTime.Now.ToString("dd/MM/yyyy");
            }

        }
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Ci dice che la tabela è stata modificata
            this.TabellaModificata = true;

            // Durante la modifica della tabella il bottone viene disabilitato
            ModificaDimensioni.IsEnabled = false;
        }
        private async void Indietro_Click(object sender, RoutedEventArgs e)
        {
            // Inizializza il risultato del dialog
            ConfirmDialogResult confirmed = ConfirmDialogResult.Yes;

            // Avvisa che si sta procedendo senza salvare
            if (this.TabellaModificata == true)
            {
                try
                {
                    confirmed = await DialogsHelper.ShowConfirmDialog("Vuoi procedere senza salvare le modifiche?", ConfirmDialog.ButtonConf.YES_NO);
                }
                catch
                {

                }
                
            }

            // Naviga al menù principale o resta sulla pagina
            if (confirmed.ToString() == "Yes")
            {
                this.NavigationService.Navigate(new SettingsMenu());
            }

        }

        private async void ModificaDimensioni_Click_1(object sender, RoutedEventArgs e)
        {
            // Aggiorna il database in base alle modifiche effettuate
            try
            {
                dataAdapter.Update(DataSet.Tables[0]);
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il database è stato aggiornato correttamente!", ConfirmDialog.ButtonConf.OK_ONLY);
                this.NavigationService.Navigate(new SettingsMenu());
            }
            catch (Exception ex)
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog(ex.Message, ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private void EliminaRiga_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();
            // Popola la tabella con la riga eliminata
            databaseSQL.EliminaRiga(this.dataGrid.SelectedItems[0], databaseSQL.TABELLA_NASTRI);
            // Ricarico il db
            DatabaseSQL databaseSQL1 = DatabaseSQL.CreateDefault();
            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand createCommand = databaseSQL1.CreateSettingNastriCommand();
            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(createCommand);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.DataContext = dataTable;
        }

        // This snippet can be used if you can be sure that every
        // member will be decorated with a [DisplayNameAttribute]
        

    }

    public class Client
    {
        [DisplayName("Name")]
        public String name { set; get; }

        [DisplayName("Claim Number")]
        public String claim_number { set; get; }
    }

}
