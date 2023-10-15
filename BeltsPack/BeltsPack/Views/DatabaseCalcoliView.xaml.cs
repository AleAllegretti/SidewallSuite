using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using System.Data;
using Syncfusion.UI.Xaml.Grid;
using BeltsPack.Models;
using MaterialDesignThemes.Wpf;
using System;

namespace BeltsPack.Views
{
    /// <summary>
    /// Logica di interazione per DatabaseCalcoliView.xaml
    /// </summary>
    public partial class DatabaseCalcoliView : Page
    {
        public DatabaseCalcoliView()
        {
            this.DataContext = this;

            InitializeComponent();

            SfDataGrid dataGrid = new SfDataGrid();

            // Crea il wrapper del database
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand creacomando = databaseSQL.CreateCalcoliInputCommand();

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Chiude la connessione
            databaseSQL.CloseConnection();

            // Popola la tabella           
            DatabaseTotaleCalcoli.DataContext = dataTable;
        }

        private void DatabaseTotaleCalcoli_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {

        }

        private async void CopiaImballo_Click(object sender, RoutedEventArgs e)
        {
          
            // Assegno le caratteristiche dell'imballo sselezionato
            Prodotto prodotto = new Prodotto();
            Nastro nastro = new Nastro();
            Bordo bordo = new Bordo();
            Tazza tazza = new Tazza();
            Materiale materiale = new Materiale();
            Motore motore = new Motore();
            Rullo rullo = new Rullo();
            Tamburo tamburo = new Tamburo();
            

            object selectedItem;

            if (this.DatabaseTotaleCalcoli.SelectedItems.Count != 0)
            {
                selectedItem = this.DatabaseTotaleCalcoli.SelectedItems[0];
                var dataRow = (selectedItem as DataRowView).Row;
                string codice = dataRow["Codice"].ToString();

                // Assegno le caratteristiche del prodotto
                try
                {
                    prodotto.Cliente = dataRow["Cliente"].ToString();
                    prodotto.Codice = dataRow["Codice"].ToString();
                    prodotto.Aperto = dataRow["ApertoChiuso"].ToString();
                    prodotto.LunghezzaNastro = Convert.ToInt32(dataRow["LunghezzaNastro"].ToString());
                    prodotto.LarghezzaNastro = Convert.ToInt32(dataRow["LarghezzaNastro"].ToString());
                    prodotto.AltezzaBordo = Convert.ToInt32(dataRow["AltezzaBordo"].ToString());
                    prodotto.AltezzaTazze = Convert.ToInt32(dataRow["AltezzaTazze"].ToString());
                    prodotto.PresenzaFix = dataRow["PresenzaFix"].ToString();
                    prodotto.PresenzaBlinkers = dataRow["PresenzaBlinkers"].ToString();
                    prodotto.TipoNastro = dataRow["TipoNastro"].ToString();
                    prodotto.PistaLaterale = Convert.ToInt32(dataRow["PistaLaterale"].ToString());
                    prodotto.PassoTazze = Convert.ToInt32(dataRow["PassoTazze"].ToString());
                    prodotto.LarghezzaBordo = Convert.ToInt32(dataRow["BaseBordo"].ToString());
                    prodotto.ClasseNastro = Convert.ToInt32(dataRow["ClasseNastro"].ToString());
                    prodotto.FormaTazze = dataRow["FormaTazze"].ToString();
                    prodotto.TrattamentoNastro = dataRow["TrattamentoNastro"].ToString();
                    prodotto.TrattamentoBordo = dataRow["TrattamentoBordo"].ToString();
                    prodotto.TrattamentoTazze = dataRow["TrattamentoTazze"].ToString();
                    prodotto.TazzeTelate = dataRow["TazzeTelate"].ToString();
                    prodotto.Qty = Convert.ToInt32(dataRow["Qty"].ToString());
                    nastro.capacityRequired = Convert.ToDouble(dataRow["Capacity"].ToString());
                    materiale.fillFactor = Convert.ToDouble(dataRow["FillingFactor"].ToString());
                    nastro.speed = Convert.ToDouble(dataRow["VelocitaNastro"].ToString());
                    nastro.forma = dataRow["Forma"].ToString();
                    nastro.inclinazione = Convert.ToInt32(dataRow["PendenzaMax"].ToString());
                    nastro.elevazione = Convert.ToDouble(dataRow["Elevazione"].ToString());
                    nastro.centerDistance = Convert.ToDouble(dataRow["DistDalCentro"].ToString());
                    materiale.Nome = dataRow["NomeMateriale"].ToString();
                    materiale.density = Convert.ToDouble(dataRow["DensitaMateriale"]);
                    materiale.surchAngle = Convert.ToDouble(dataRow["AngoloCarico"]);
                    materiale.DimSingolo = Convert.ToDouble(dataRow["DimensioneSingolo"].ToString());
                    nastro.edgetype = Convert.ToDouble(dataRow["EdgeType"].ToString());

                    // Inizializza il risultato del dialog
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro di voler copiare l'imballo della commessa " + codice + " ?", ConfirmDialog.ButtonConf.YES_NO);
                    //Naviga al menù principale o resta sulla pagina
                    if (confirmed.ToString() == "Yes")
                    {
                        //DialogHost.CloseDialogCommand.Execute(null, null);
                        this.NavigationService.Navigate(new CalcoliView(prodotto, nastro, tazza, bordo, materiale, rullo, tamburo, motore));
                        //DialogHost.CloseDialogCommand.Execute(new object(), null);
                    }
                }
                catch
                {
                    //ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("L'imballo selezionato appartiene ad una versione vecchia del programma e non tutti i campi verranno compilati. \nVuoi comunque procedere?", ConfirmDialog.ButtonConf.YES_NO);
                    // Naviga al menù principale o resta sulla pagina
                    this.NavigationService.Navigate(new CalcoliView(prodotto, nastro, tazza, bordo, materiale, rullo, tamburo, motore));
                    //if (confirmed.ToString() == "Yes")
                    //{
                        
                    //}
                }
            }
            else
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'imballo da copiare.");
            }
        }

        private async void EliminaRiga_Click(object sender, RoutedEventArgs e)
        {
            string cellValue;
            int versionevalue;
            try
            {
                var selectedItem = this.DatabaseTotaleCalcoli.SelectedItems[0];
                var dataRow = (selectedItem as DataRowView).Row;
                cellValue = dataRow["Codice"].ToString();
                versionevalue = Convert.ToInt32(dataRow["Versione"]);
            }
            catch
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'imballo da eliminare");
                return;
            }

            var confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro di voler cancellare questo imballo?", ConfirmDialog.ButtonConf.YES_NO);

            // Create the Database wrapper
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Creates the command to retrieve this check
            SqlCommand createCommand = databaseSQL.DeleteRowDBTotaleCommand(cellValue, versionevalue);
            createCommand.ExecuteNonQuery();

            createCommand = databaseSQL.DeleteRowDBInputCommand(cellValue, versionevalue);
            createCommand.ExecuteNonQuery();

            createCommand = databaseSQL.DeleteRowDBOutputCommand(cellValue, versionevalue);
            createCommand.ExecuteNonQuery();

            //  The article's been eliminated correctly
            await DialogsHelper.ShowMessageDialog("Imballo eliminato correttamente");

            // Comando per mostrare la tabella con tutti gli imballi
            createCommand = databaseSQL.CreateCalcoliInputCommand();

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(createCommand);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Popola la tabella
            this.DatabaseTotaleCalcoli.DataContext = dataTable;
        }
    }
}
