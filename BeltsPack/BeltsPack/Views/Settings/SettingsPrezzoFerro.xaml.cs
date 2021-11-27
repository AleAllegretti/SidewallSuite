using System.Windows.Controls;
using System.Data.SqlClient;
using BeltsPack.Utils;
using System.Data;
using System;
using BeltsPack.Views.Dialogs;
using System.Windows;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for SettingsPrezzoFerro.xaml
    /// </summary>
    public partial class SettingsPrezzoFerro : Page
    {
        SqlDataAdapter dataAdapter;
        DataSet DataSet;
        Boolean TabellaModificata = false;
        public SettingsPrezzoFerro()
        {
            InitializeComponent();
            // Crea il wrapper del database
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Comando per mostrare la tabella con tutti i parametri degli imballi in legno
            SqlCommand creacomando = databaseSQL.CreateSettingCostiFerroCommand();

            // Riempie la tabella
            DataSet = new DataSet();
            dataAdapter = new SqlDataAdapter(creacomando);
            SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(DataSet);

            dataGrid.DataContext = DataSet.Tables[0];

            // Chiude la connessione
            databaseSQL.CloseConnection();
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

        private async void ModificaDimensioni_Click(object sender, RoutedEventArgs e)
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

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Ci dice che la tabela è stata modificata
            this.TabellaModificata = true;

            // Durante la modifica della tabella il bottone viene disabilitato
            ModificaDimensioni.IsEnabled = false;
        }

        private async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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

                // Controlla che i valori inseriti siano conformi con il database
                if (e.Column.Header.ToString() == "Prezzo" || e.Column.Header.ToString() == "Spessore" ||
                    e.Column.Header.ToString() == "Larghezza" || e.Column.Header.ToString() == "Altezza" ||
                    e.Column.Header.ToString() == "PesoMetro")
                {
                    if (int.TryParse(((System.Windows.Controls.TextBox)e.EditingElement).Text.ToString(), out int _))
                    {

                        // Abilita il tasto di salvataggio
                        ModificaDimensioni.IsEnabled = true;

                        // Aggiorno il database con l'ultima data di aggiornamento
                        dataRow["DataUltimoAggiornamento"] = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Il valore che hai inserito non è corretto", "Avviso", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

            }
        }
    }
}
