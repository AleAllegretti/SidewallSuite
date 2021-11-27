using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for SettingsTazze.xaml
    /// </summary>
    public partial class SettingsTazze : Page
    {
        SqlDataAdapter dataAdapter;
        DataSet DataSet;
        Boolean TabellaModificata = false;
        public SettingsTazze()
        {
            InitializeComponent();
            // Crea il wrapper del database
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Comando per mostrare la tabella con tutti i parametri degli imballi in legno
            SqlCommand creacomando = databaseSQL.CreateSettingTazzeCommand();

            // Riempie la tabella
            DataSet = new DataSet();
            dataAdapter = new SqlDataAdapter(creacomando);
            SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(DataSet);

            dataGrid.DataContext = DataSet.Tables[0];

            // Chiude la connessione
            databaseSQL.CloseConnection();
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
                confirmed = await DialogsHelper.ShowConfirmDialog("Vuoi procedere senza salvare le modifiche?", ConfirmDialog.ButtonConf.YES_NO);
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
            databaseSQL.EliminaRiga(this.dataGrid.SelectedItems[0], databaseSQL.TABELLA_TAZZE);
            // Ricarico il db
            DatabaseSQL databaseSQL1 = DatabaseSQL.CreateDefault();
            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand createCommand = databaseSQL1.CreateSettingTazzeCommand();
            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(createCommand);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.DataContext = dataTable;
        }
    }
}
