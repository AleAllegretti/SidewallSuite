using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using System.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.Windows.Forms;
using System.IO;
using BeltsPack.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using static BeltsPack.Views.Dialogs.AttachmentSelectionDialog;
using MaterialDesignThemes.Wpf;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    
    public partial class DatabaseView : Page
    {
        public DateTime _startDate { get; set; }
        MailManager mailManager = new MailManager();
        ProdottoSelezionato prodottoSelezionato = new ProdottoSelezionato();
        PdfUtils PdfUtils = new PdfUtils();
        public DatabaseView()
        {
            // Definisce la data di oggi
            this._startDate = DateTime.Now.AddDays(15);
            this.DataContext = this;

            InitializeComponent();

            SfDataGrid dataGrid = new SfDataGrid();

            // Crea il wrapper del database
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand creacomando = databaseSQL.CreateDbTotaleCommand();

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Chiude la connessione
            databaseSQL.CloseConnection();

            // Popola la tabella           
            DatabaseTotale.DataContext = dataTable;
        }

        private async void EsportaExcel_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = DatabaseTotale.ExportToExcel(DatabaseTotale.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            // Abilita i filtri nel foglio Excel
            workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

            // Abilita i bordi
            workBook.Worksheets[0].UsedRange.BorderInside(ExcelLineStyle.Medium, ExcelKnownColors.Black);
            workBook.Worksheets[0].UsedRange.BorderAround(ExcelLineStyle.Medium, ExcelKnownColors.Black);

            // Setta la larghezza della prima colonna
            workBook.Worksheets[0].SetColumnWidth(1, 11);

            // Stile dell'intestazione
            workBook.Worksheets[0].Range["A1:AV1"].CellStyle.Font.Bold = true;
            workBook.Worksheets[0].Range["A1:AV1"].CellStyle.Color = System.Drawing.Color.LightGray;
            workBook.Worksheets[0].Range["A1:AV1"].CellStyle.Font.Size = 10;
            
            // Impostazioni del saving dialog
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "EstrazioneDatabaseImballi_" + DateTime.Now.ToString("dd_MM_yyyy"),
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = sfd.OpenFile())
                {

                    if (sfd.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;

                    else if (sfd.FilterIndex == 2)
                        workBook.Version = ExcelVersion.Excel2010;

                    else
                        workBook.Version = ExcelVersion.Excel2013;
                    workBook.SaveAs(stream);
                }

                // Inizializza il risultato del dialog
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Vuoi aprire il file Excel?", ConfirmDialog.ButtonConf.YES_NO);
                
                // Naviga al menù principale o resta sulla pagina
                if (confirmed.ToString() == "Yes")
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }

            }
        }

        private async void EliminaRiga_Click(object sender, RoutedEventArgs e)
        {
            string cellValue;
            int versionevalue;
            try
            {
                var selectedItem = this.DatabaseTotale.SelectedItems[0];
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

            //  The article's been eliminated correctly
            await DialogsHelper.ShowMessageDialog("Imballo eliminato correttamente");

            // Comando per mostrare la tabella con tutti gli imballi
            createCommand = databaseSQL.CreateDbTotaleCommand();

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(createCommand);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Popola la tabella
            DatabaseTotale.DataContext = dataTable;
        }

        private async void Creadocumentazione_Click(object sender, RoutedEventArgs e)
        {
            string cellValue;
            object selectedItem;
            string pdfpath;
            string pdfpathPaladini = "";

            // Genera la scheda pdf
            try
            {
                selectedItem = this.DatabaseTotale.SelectedItems[0];
                var dataRow = (selectedItem as DataRowView).Row;
                cellValue = dataRow["Codice"].ToString();

                // Aggiorna le note e la data nel db
                string note;
                int versione;
                string codice;
                string notePaladini;
                string dataconsegna;

                codice = cellValue;
                versione = int.Parse(dataRow["Versione"].ToString());
                note = this.TBNote.Text;
                notePaladini = this.TBNotePaladini.Text;
                dataconsegna = this._startDate.ToString("d/M/yyyy");

                DatabaseSQL database = DatabaseSQL.CreateDefault();
                SqlCommand creacomando = database.UpdateNoteDbTotaleCommand(codice, note, versione);
                creacomando.ExecuteNonQuery();
                creacomando = database.UpdateNotePaladiniDbTotaleCommand(codice, notePaladini, versione);
                creacomando.ExecuteNonQuery();
                creacomando = database.UpdateDataConsegnaCassaCommand(codice, dataconsegna, versione);
                creacomando.ExecuteNonQuery();

                if (dataRow["Stato"].ToString() != "Completato" & dataRow["Stato"].ToString() != "Inviato")
                {
                    

                    // Riempie il modulo pdf della scheda di produzione
                    if (dataRow["TipologiaImballo"].ToString() == "Pedana")
                    {
                        // Memorizzo i dati dell'imballo selezionato
                        this.GetRowElementLegno(selectedItem);
                        // Riempio la scheda per la produzione
                        pdfpath = this.PdfUtils.FillSchedaProduzioneLegno(prodottoSelezionato);
                    }   
                    else
                    {
                        // Memorizzo i dati dell'imballo selezionato
                        this.GetRowElements(selectedItem);
                        // Riempio la scheda per la produzione
                        pdfpath = this.PdfUtils.FillSchedaProduzione(prodottoSelezionato);
                        // Riempie la scheda per Paladini
                        pdfpathPaladini = this.PdfUtils.FillSchedaPaladini(prodottoSelezionato, this.PdfUtils.SAVING_PATH);
                    }

                    // sending the email is a long task, so we move it in a background task while we show a progress bar
                    DialogsHelper.ShowProgress(async (dialog) =>
                    {
                        // send the email in a background thread
                        await Task.Run(() =>
                        {

                            // Prepara l'email
                            var mail = this.mailManager.PrepareEmail("");
                            mail = this.mailManager.PrepareEmail("Imballo per commessa N°" + this.prodottoSelezionato.Codice + " - " + this.prodottoSelezionato.Cliente, "g.marchini@morinat.com");
                            mail.Body = "Schede tecniche imballo " + this.prodottoSelezionato.Cliente + " commessa N° " + this.prodottoSelezionato.Codice + " in allegato";
                            mail.AddAttachment(pdfpath, "Scheda_Produzione" + "_" + prodottoSelezionato.Cliente + "_" + prodottoSelezionato.Codice + ".pdf");                         
                            if (dataRow["TipologiaImballo"].ToString() != "Pedana")
                            {
                                mail.AddAttachment(pdfpathPaladini, "Scheda_Tecnica_Paladini" + "_" + prodottoSelezionato.Cliente + "_" + prodottoSelezionato.Codice + ".pdf");
                            }

                            // Mostra outlook
                            try
                            {
                                mail.Show();
                            }
                            catch
                            {

                                    DialogsHelper.ShowMessageDialog("In questo momento non è possibile inviare l'email; l'imballo è stato comunque registrato nel sistema.");
                              
                            }
                        });

                        // when done close the dialog
                        dialog.Close();

                    }, "Sto preparando l'email...");

                    // Salvo il pdf nel db
                    ResourceReader resourcereader = new ResourceReader();

                    // Scheda produzione
                    byte[] binarySchedaProduzione = null;
                    binarySchedaProduzione = resourcereader.PdfToBinary(pdfpath);                   
                    DatabaseSQL databaseSQL1 = DatabaseSQL.CreateDefault();
                    SqlCommand creacomando4 = databaseSQL1.WriteBinaryCommandPDF(codice, versione, binarySchedaProduzione);
                    creacomando4.ExecuteNonQuery();

                    // Scheda paladini
                    byte[] binarySchedaPaladini = null;
                    if (dataRow["TipologiaImballo"].ToString() != "Pedana")
                    {
                        binarySchedaPaladini = resourcereader.PdfToBinary(pdfpathPaladini);
                        creacomando4 = databaseSQL1.WriteBinaryCommandPDFPaladini(codice, versione, binarySchedaPaladini);
                        creacomando4.ExecuteNonQuery();
                    }

                    // Ricarica il db con le note nuove
                    // Comando per mostrare la tabella con tutti gli imballi
                    // Crea il wrapper del database
                    DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

                    // Comando per mostrare la tabella con tutti gli imballi
                    SqlCommand creacomando3 = databaseSQL.CreateDbTotaleCommand();

                    // Riempie la tabella
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando3);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Popola la tabella
                    DatabaseTotale.DataContext = dataTable;
                }
                else
                {
                    if(dataRow["TipologiaImballo"].ToString() == "Pedana")
                    {
                      await DialogsHelper.ShowMessageDialog("Per le pedane in legno non è prevista nessuna documentazione.");
                    }
                    else
                    {
                      await DialogsHelper.ShowMessageDialog("La documentazione di questo imballo è già stata generata.");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                   await DialogsHelper.ShowMessageDialog("Abbiamo riscontrato il seguente errore: " + ex);
                }
                else
                {
                    DialogsHelper.ShowMessageDialog("Devi prima selezionare l'articolo da visualizzare");
                }
                return;
            }

            
        }
        private void GetRowElements(object selectedItem)
        {
            var dataRow = (selectedItem as DataRowView).Row;
            this.prodottoSelezionato.Cliente = dataRow["Cliente"].ToString();
            this.prodottoSelezionato.Codice = dataRow["Codice"].ToString();
            this.prodottoSelezionato.Data = dataRow["Data"].ToString();
            this.prodottoSelezionato.LunghezzaNastro = int.Parse(dataRow["LunghezzaNastro"].ToString());
            this.prodottoSelezionato.LarghezzaNastro = int.Parse(dataRow["LarghezzaNastro"].ToString());
            this.prodottoSelezionato.AltezzaBordo = int.Parse(dataRow["AltezzaBordo"].ToString());
            this.prodottoSelezionato.Aperto = dataRow["ApertoChiuso"].ToString();
            this.prodottoSelezionato.Peso = int.Parse(dataRow["PesoImballo"].ToString());
            this.prodottoSelezionato.LarghezzaCassa = int.Parse(dataRow["LarghezzaImballo"].ToString());
            this.prodottoSelezionato.LunghezzaCassa = int.Parse(dataRow["LunghezzaImballo"].ToString());
            this.prodottoSelezionato.AltezzaCassa = int.Parse(dataRow["AltezzaImballo"].ToString());
            this.prodottoSelezionato.Configurazione = int.Parse(dataRow["Configurazione"].ToString());
            this.prodottoSelezionato.Personalizzazione = dataRow["Personalizzazione"].ToString();
            this.prodottoSelezionato.Criticita = dataRow["Criticita"].ToString();
            this.prodottoSelezionato.TipoTrasporto = dataRow["TipologiaTrasporto"].ToString();
            this.prodottoSelezionato.DiametroCorrugato = int.Parse(dataRow["DiametroCorrugati"].ToString());
            this.prodottoSelezionato.DiametroSubbio = int.Parse(dataRow["DiametroSubbi"].ToString());
            this.prodottoSelezionato.NumeroSubbi = int.Parse(dataRow["NumeroSubbi"].ToString());
            this.prodottoSelezionato.LunghezzaSingoloCorrugato = int.Parse(dataRow["LunghezzaCorrugati"].ToString());
            this.prodottoSelezionato.NumeroCorrugati = int.Parse(dataRow["NumeroCorrugati"].ToString());
            this.prodottoSelezionato.Note = this.TBNote.Text.ToString();
            this.prodottoSelezionato.Configurazione = int.Parse(dataRow["Configurazione"].ToString());
            this.prodottoSelezionato.DataConsegna = this._startDate.ToString("d/M/yyyy");
            this.prodottoSelezionato.NotePaladini = this.TBNotePaladini.Text.ToString();
            this.prodottoSelezionato.PresenzaIncroci = dataRow["PresenzaIncroci"].ToString();
            this.prodottoSelezionato.PresenzaGanci = dataRow["PresenzaGanci"].ToString();
            this.prodottoSelezionato.PresenzaReteLaterale = dataRow["PresenzaReteLaterale"].ToString();
            this.prodottoSelezionato.CassaVerniciata = dataRow["CassaVerniciata"].ToString();
            this.prodottoSelezionato.PresenzaLamieraBase = dataRow["PresenzaLamieraBase"].ToString();
            this.prodottoSelezionato.PesoTotaleNastro =double.Parse(dataRow["PesoTotaleNastro"].ToString());

            // Cambio lo stato dell'imballo
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();
            SqlCommand creacomando = databaseSQL.UpdateDbTotaleCommand(dataRow["Codice"].ToString(), int.Parse(dataRow["Versione"].ToString()));
            creacomando.ExecuteNonQuery();

            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand creacomando1 = databaseSQL.CreateDbTotaleCommand();

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando1);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Chiude la connessione
            databaseSQL.CloseConnection();

            // Prende i campi del prodotto selezionato
            // this.prodottoSelezionato.Commessa = selectedItem.
        }

        private void GetRowElementLegno(object selectedItem)
        {
            var dataRow = (selectedItem as DataRowView).Row;
            this.prodottoSelezionato.Cliente = dataRow["Cliente"].ToString();
            this.prodottoSelezionato.Codice = dataRow["Codice"].ToString();
            this.prodottoSelezionato.Data = dataRow["Data"].ToString();
            this.prodottoSelezionato.LunghezzaNastro = int.Parse(dataRow["LunghezzaNastro"].ToString());
            this.prodottoSelezionato.LarghezzaNastro = int.Parse(dataRow["LarghezzaNastro"].ToString());
            this.prodottoSelezionato.AltezzaBordo = int.Parse(dataRow["AltezzaBordo"].ToString());
            this.prodottoSelezionato.Aperto = dataRow["ApertoChiuso"].ToString();
            this.prodottoSelezionato.Peso = int.Parse(dataRow["PesoImballo"].ToString());
            this.prodottoSelezionato.LarghezzaCassa = int.Parse(dataRow["LarghezzaImballo"].ToString());
            this.prodottoSelezionato.LunghezzaCassa = int.Parse(dataRow["LunghezzaImballo"].ToString());
            this.prodottoSelezionato.AltezzaCassa = int.Parse(dataRow["AltezzaImballo"].ToString());
            this.prodottoSelezionato.Criticita = dataRow["Criticita"].ToString();
            this.prodottoSelezionato.TipoTrasporto = dataRow["TipologiaTrasporto"].ToString();            
            this.prodottoSelezionato.Note = this.TBNote.Text.ToString();           
            this.prodottoSelezionato.PesoTotaleNastro = double.Parse(dataRow["PesoTotaleNastro"].ToString());

            // Cambio lo stato dell'imballo
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();
            SqlCommand creacomando = databaseSQL.UpdateDbTotaleCommand(dataRow["Codice"].ToString(), int.Parse(dataRow["Versione"].ToString()));
            creacomando.ExecuteNonQuery();

            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand creacomando1 = databaseSQL.CreateDbTotaleCommand();

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando1);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Chiude la connessione
            databaseSQL.CloseConnection();

            // Prende i campi del prodotto selezionato
            // this.prodottoSelezionato.Commessa = selectedItem.
        }

        private void DatabaseTotale_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            object selectedItem;

            // Mostra il campo note
            if (this.DatabaseTotale.SelectedItems.Count == 1)
            {
                selectedItem = this.DatabaseTotale.SelectedItems[0];
                var dataRow1 = (selectedItem as DataRowView).Row;

                // Disabilito le note per Paladini se l'imballo è in legno
                if(dataRow1["TipologiaImballo"].ToString() == "Pedana")
                {
                    this.TBNotePaladini.IsEnabled = false;
                }
                else
                {
                    this.TBNotePaladini.IsEnabled = true;
                }
                this.TBNote.Text = dataRow1["Note"].ToString();
                this.TBNotePaladini.Text = dataRow1["NotePaladini"].ToString();
            }
            
        }

        private async void DatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Controlla che la data selezionata non sia inferiore ad oggi
            if(this._startDate < DateTime.Now)
            {
              await DialogsHelper.ShowConfirmDialog("Non puoi selezonare una data precedente ad oggi: (" + DateTime.Now.ToString("d/M/yyyy") + ").", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private async void Mostradocumentazione_Click_1(object sender, RoutedEventArgs e)
        {
            string codice;
            object selectedItem;
            int versione;

            // Genera la scheda pdf
            selectedItem = this.DatabaseTotale.SelectedItems[0];
            var dataRow = (selectedItem as DataRowView).Row;
            codice = dataRow["Codice"].ToString();
            versione = int.Parse(dataRow["Versione"].ToString());

            // Create the Database wrapper
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();

            // Creo il comando per ottenere l'imballo selezionato
            SqlCommand createCommand = dbSQL.ReadDBTotaleCommand(codice, versione);
            createCommand.ExecuteNonQuery();

            SqlDataReader rdr = createCommand.ExecuteReader();
            rdr.Read();

            // Popolo dinamicamente la lista degli allegati
            List<Attachements> attachements = new List<Attachements>();

            if (!rdr.IsDBNull(rdr.GetOrdinal("BinarySchedaProduzione"))) { attachements.Add(new Attachements("BinarySchedaProduzione", "Scheda tecnica produzione", true)); }
            if (!rdr.IsDBNull(rdr.GetOrdinal("BinarySchedaPaladini"))) { attachements.Add(new Attachements("BinarySchedaPaladini", "Scheda tecnica paladini", true)); }
            if (!rdr.IsDBNull(rdr.GetOrdinal("BinarySchedaPostProduzione"))) { attachements.Add(new Attachements("BinarySchedaPostProduzione", "Scheda tecnica post-produzione", true)); }
            if (!rdr.IsDBNull(rdr.GetOrdinal("BinayDisposizioneNastro"))) { attachements.Add(new Attachements("BinayDisposizioneNastro", "Schema disposizione nastro", false, (byte[])rdr["BinayDisposizioneNastro"])); }

            if (attachements.Count == 0)
            {
                await DialogsHelper.ShowMessageDialog("Non esistono allegati da visualizzare per questo articolo");
                return;
            }
            var selectedAttachment = await DialogsHelper.ShowAttachmentSelectionDialog(attachements);

            // based on the selected item opens the attachement
            if (selectedAttachment != null)
            {
                // gets the binary of the file
                byte[] binaryData = (byte[])rdr[selectedAttachment.DbColumn];

                // create a temporary filename according to the attachment name and type
                string filepath = Path.GetTempPath() + codice + "_" + selectedAttachment.Label;
                filepath += selectedAttachment.IsPdf ? ".pdf" : ".jpeg";
                File.WriteAllBytes(filepath, binaryData);

                // then open it with system default app
                System.Diagnostics.Process.Start(filepath);
            }

            dbSQL.CloseConnection();
        }

        private async void Mostradocumentazione_Click(object sender, RoutedEventArgs e)
        {
            string cellValue;
            object selectedItem;

            if (this.DatabaseTotale.SelectedItems.Count != 0)
            {
                selectedItem = this.DatabaseTotale.SelectedItems[0];
                var dataRow = (selectedItem as DataRowView).Row;
                cellValue = dataRow["Stato"].ToString();
                string codice = dataRow["Codice"].ToString(); ;
                int versione = Convert.ToInt32(dataRow["Versione"].ToString());

                if (cellValue == "Inviato")
                {
                    string dataconsegna;
                    dataconsegna = dataRow["DataConsegnaCassa"].ToString();
                    var view = new AggDataConsegnaDialog(codice, versione, dataconsegna, dataRow["Cliente"].ToString());
                    await DialogHost.Show(view);

                    DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

                    // Comando per mostrare la tabella con tutti gli imballi
                    SqlCommand creacomando3 = databaseSQL.CreateDbTotaleCommand();

                    // Riempie la tabella
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando3);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Popola la tabella
                    DatabaseTotale.DataContext = dataTable;
                }
                else
                {
                    await DialogsHelper.ShowMessageDialog("Per gli imballi in stato '" + cellValue + "' non è possibile aggiornare la data");
                }
            }
            else
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'imballo.");
            }
            
        }

        private async void Riepilogo_Click(object sender, RoutedEventArgs e)
        {
            string cliente;
            object selectedItem;

            if (this.DatabaseTotale.SelectedItems.Count != 0)
            {
                selectedItem = this.DatabaseTotale.SelectedItems[0];
                var dataRow = (selectedItem as DataRowView).Row;
                cliente = dataRow["Cliente"].ToString();
                string codice = dataRow["Codice"].ToString(); ;
                int lunghezza = Convert.ToInt32(dataRow["LunghezzaImballo"].ToString());
                int altezza = Convert.ToInt32(dataRow["AltezzaImballo"].ToString());
                int larghezza = Convert.ToInt32(dataRow["LarghezzaImballo"].ToString());
                int pesoImballo = Convert.ToInt32(dataRow["PesoImballo"].ToString());
                double costoImballo = Convert.ToDouble(dataRow["CostoImballo"].ToString().Trim('€'));
                int pesoTotaleNastro = Convert.ToInt32(dataRow["PesoTotaleNastro"].ToString());

                // Apro il dialog
                var view = new DialogSummary(cliente, codice, lunghezza, altezza, larghezza, pesoImballo, costoImballo, pesoTotaleNastro);
                await DialogHost.Show(view);
            }
            else
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'imballo.");
            }
        }

        private async void CopiaImballo_Click(object sender, RoutedEventArgs e)
        {
            // Assegno le caratteristiche dell'imballo sselezionato
            Prodotto prodotto = new Prodotto();
            
            object selectedItem;

            if (this.DatabaseTotale.SelectedItems.Count != 0)
            {
                selectedItem = this.DatabaseTotale.SelectedItems[0];
                var dataRow = (selectedItem as DataRowView).Row;
                string codice = dataRow["Codice"].ToString();

                // Assegno le caratteristiche del prodotto
                try
                {
                    prodotto.Cliente = dataRow["Cliente"].ToString();
                    prodotto.Codice = dataRow["Codice"].ToString();
                    prodotto.TipologiaTrasporto = dataRow["TipologiaTrasporto"].ToString();
                    if (Convert.ToInt32(dataRow["AltezzaBordo"].ToString()) == 0 && Convert.ToInt32(dataRow["AltezzaTazze"].ToString()) == 0)
                    {
                        prodotto.Tipologia = "Nastro liscio";
                    }
                    else if (Convert.ToInt32(dataRow["AltezzaBordo"].ToString()) != 0 && Convert.ToInt32(dataRow["AltezzaTazze"].ToString()) == 0)
                    {
                        prodotto.Tipologia = "Solo bordi";
                    }
                    else if (Convert.ToInt32(dataRow["AltezzaBordo"].ToString()) == 0 && Convert.ToInt32(dataRow["AltezzaTazze"].ToString()) != 0)
                    {
                        prodotto.Tipologia = "Solo tazze";
                    }
                    else
                    {
                        prodotto.Tipologia = "Bordi e tazze";
                    }
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
                    prodotto.NumeroTazzexFila = Convert.ToInt32(dataRow["NumeroTazzexFila"].ToString());
                    prodotto.SpazioFile = Convert.ToInt32(dataRow["SpazioFile"].ToString());
                    prodotto.TrattamentoNastro = dataRow["TrattamentoNastro"].ToString();
                    prodotto.TrattamentoBordo = dataRow["TrattamentoBordo"].ToString();
                    prodotto.TrattamentoTazze = dataRow["TrattamentoTazze"].ToString();
                    prodotto.TazzeTelate = dataRow["TazzeTelate"].ToString();
                    prodotto.Qty = Convert.ToInt32(dataRow["Qty"].ToString());

                    // Inizializza il risultato del dialog
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro di voler copiare l'imballo della commessa " + codice + " ?", ConfirmDialog.ButtonConf.YES_NO);
                    // Naviga al menù principale o resta sulla pagina
                    if (confirmed.ToString() == "Yes")
                    {
                        this.NavigationService.Navigate(new InputView(prodotto));
                    }
                }
               catch
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("L'imballo selezionato appartiene ad una versione vecchia del programma e non tutti i campi verranno compilati. \nVuoi comunque procedere?", ConfirmDialog.ButtonConf.YES_NO);
                    // Naviga al menù principale o resta sulla pagina
                    if (confirmed.ToString() == "Yes")
                    {
                        this.NavigationService.Navigate(new InputView(prodotto));
                    }
                }              
            }
            else
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'imballo da copiare.");
            }
            
        }
    }
}
