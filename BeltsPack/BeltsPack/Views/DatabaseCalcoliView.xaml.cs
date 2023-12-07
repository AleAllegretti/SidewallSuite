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
using System.Windows.Media.Media3D;
using Microsoft.Office.Interop.Outlook;

namespace BeltsPack.Views
{
    /// <summary>
    /// Logica di interazione per DatabaseCalcoliView.xaml
    /// </summary>
    public partial class DatabaseCalcoliView : Page
    {
        private PdfUtils PdfUtils = new PdfUtils();
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

            if (this.DatabaseTotaleCalcoli.SelectedItems.Count == 1)
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
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro di voler copiare i dati del nastro della commessa " + codice + " ?", ConfirmDialog.ButtonConf.YES_NO);
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
            else if (this.DatabaseTotaleCalcoli.SelectedItems.Count > 1)
            {
                await DialogsHelper.ShowMessageDialog("E' possibile copiare solamente un nastro alla volta.");
            }
            else
            {
                await DialogsHelper.ShowMessageDialog("Prima devi selezionare il nastro da copiare.");
            }
        }

        private async void EliminaRiga_Click(object sender, RoutedEventArgs e)
        {
            string cellValue;
            int versionevalue;

            if (this.DatabaseTotaleCalcoli.SelectedItems.Count == 1)
            {
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
            else
            {
                await DialogsHelper.ShowMessageDialog("E' possibile eliminare solo un imballo alla volta.");
                return;
            }

        }

        private async void CreaPdf_Click(object sender, RoutedEventArgs e)
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
            CalcoliImpianto calcoliImpianto = new CalcoliImpianto(nastro, tazza, prodotto, bordo, materiale, rullo, tamburo, motore);

            object selectedItem;
            string control;

            // Metto il controllo che al massimo posso selezionare 2 imballi
            if (this.DatabaseTotaleCalcoli.SelectedItems.Count <= 2 && this.DatabaseTotaleCalcoli.SelectedItems.Count > 0)
            {
                for (int i = 0; i < this.DatabaseTotaleCalcoli.SelectedItems.Count; i ++)
                {
                    selectedItem = this.DatabaseTotaleCalcoli.SelectedItems[i];
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
                        calcoliImpianto.Qteff = Convert.ToDouble(dataRow["CapacitaTonOra"].ToString());
                        prodotto.PesoM2 = Convert.ToDouble(dataRow["PesoNastro"].ToString());
                        calcoliImpianto.MaxWorkTens = Convert.ToDouble(dataRow["MaxTensPulley"].ToString());
                        calcoliImpianto.MaxWorkTensLat = Convert.ToDouble(dataRow["MaxTensLaterale"].ToString());
                        calcoliImpianto.Sfactor = Convert.ToDouble(dataRow["FattSicurezza"].ToString());
                        calcoliImpianto.Sfactor_pista = Convert.ToDouble(dataRow["FattSicurezzaPiste"].ToString());
                        calcoliImpianto.TakeUpTail = Convert.ToDouble(dataRow["TailTakeUp"].ToString());
                        calcoliImpianto.Pa= Convert.ToDouble(dataRow["PotRichiesta"].ToString());
                        motore.motorPower = Convert.ToDouble(dataRow["PotSuggerita"].ToString());
                        bordo.MinPulleyDiam= Convert.ToInt32(dataRow["MinPulleyDiameter"].ToString());
                        bordo.MinWheelDiam = Convert.ToInt32(dataRow["MinDeflectionWheel"].ToString());
                        bordo.MinWheelWidth = Convert.ToInt32(dataRow["MinWheelWidth"].ToString());
                        calcoliImpianto.Qeff = Math.Round(calcoliImpianto.Qteff / materiale.density,1);
                        bordo.Larghezza = Convert.ToInt32(dataRow["BaseBordo"].ToString());
                        nastro.Trattamento = dataRow["FormaTazze"].ToString();
                        nastro.Tipo = prodotto.TipoNastro;
                        nastro.Classe = prodotto.ClasseNastro;
                        nastro.Larghezza = prodotto.LarghezzaNastro;
                        bordo.Altezza = prodotto.AltezzaBordo;
                        bordo.SiglaTele = "HEF";
                        tazza.Passo = prodotto.PassoTazze;
                        tazza.Forma = prodotto.FormaTazze;
                        tazza.Altezza = prodotto.AltezzaTazze;
                        nastro.Trattamento = prodotto.TrattamentoNastro;
                        nastro.NumTessuti = Convert.ToInt32(dataRow["NumTessuti"].ToString());
                        nastro.NumTele = Convert.ToInt32(dataRow["NumTele"].ToString());
                        nastro.SpessoreInf = Convert.ToInt32(dataRow["SpessoreInf"].ToString());
                        nastro.SpessoreSup = Convert.ToInt32(dataRow["SpessoreSup"].ToString());
                        nastro.LarghezzaUtile = Convert.ToInt32(dataRow["LarghezzaUtile"].ToString());

                        // Stampo il pdf
                        var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string FileName;
                        if (i > 0)
                        {
                            FileName = path + "\\" + "Sidewall_Calculations" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf";
                        }
                        else
                        {
                            FileName = "";
                        }
                       
                        control = this.PdfUtils.FillSidewallCalculations(prodotto, path, nastro, bordo, tazza, materiale, calcoliImpianto, motore, i + 1, FileName);

                        if (control != "" && i == this.DatabaseTotaleCalcoli.SelectedItems.Count -1 && FileName != "")
                        {
                            // Avviso che il pdf è sato stampato correttamente sul desktiop
                            ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("PDF salvato correttamente sul desktop.", ConfirmDialog.ButtonConf.OK_ONLY);
                            // Aper il file
                            System.Diagnostics.Process.Start(FileName);
                        }
                        else if(control != "" && i == this.DatabaseTotaleCalcoli.SelectedItems.Count - 1 && FileName == "")
                        {
                            // Avviso che il pdf è sato stampato correttamente sul desktiop
                            ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("PDF salvato correttamente sul desktop.", ConfirmDialog.ButtonConf.OK_ONLY);
                            FileName = path + "\\" + "Sidewall_Calculations" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf";
                            // Aper il file
                            System.Diagnostics.Process.Start(FileName);
                        }

                        

                    }
                    catch (System.Exception ex) { }
                }
                
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Prima di generare il pdf deviselezionare un codice.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }
    }

}
