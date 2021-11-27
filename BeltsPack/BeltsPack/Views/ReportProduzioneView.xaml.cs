using BeltsPack.Models;
using BeltsPack.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BeltsPack.ViewModels;
using System.Windows.Media.Imaging;
using BeltsPack.Utils;
using System.Data.SqlClient;
using System.Data;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for ReportProduzioneView.xaml
    /// </summary>
    public partial class ReportProduzioneView : Page
    {
        PdfUtils Pdfutils = new PdfUtils();
        private ProdottoSelezionato prodottoSelezionato = new ProdottoSelezionato();
        public ReportProduzioneModel reportProduzione { get; set; }
        public ObservableCollection<AttachedImage> AttachedImages { get; set; }
        public ReportProduzioneView()
        {
            // this enable the data binding for the page
            this.DataContext = this;

            this.reportProduzione = new ReportProduzioneModel();
            InitializeComponent();

            // Crea il wrapper del database
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Comando per mostrare la tabella con tutti gli imballi
            SqlCommand creacomando = databaseSQL.CreateDbProduzioneCommand("Inviato");

            // Riempie la tabella
            SqlDataAdapter dataAdapter = new SqlDataAdapter(creacomando);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            // Chiude la connessione
            databaseSQL.CloseConnection();

            // Controlla se sono presenti imballi da controllare     
            if (dataTable.Rows.Count != 0)
            {
                DatabaseTotale.DataContext = dataTable;
            }
            else
            {
                this.ConfermaSelezione.Visibility = Visibility.Collapsed;
                this.DatabaseTotale.Visibility = Visibility.Collapsed;
                this.Avviso.Visibility = Visibility.Visible;
            }
        }

        private async void ConfermaSelezione_Click(object sender, RoutedEventArgs e)
        {
            
            if (await CanAddImage())
            {
                // configure open file dialog box to show just images
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".jpg";
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // add the image to the Module08
                    this.reportProduzione.AddImage(dlg.FileName);
                }
            }
        }

        private async void OnRemoveImageClick(object sender, RoutedEventArgs e)
        {
            var confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro di voler rimuovere questa immagine?");

            if (confirmed == ConfirmDialogResult.Yes)
            {
                // we are sure that the sender is the Button so we can force the cast here
                var packIcon = (Button)sender;
                // for the same reason, we are sure that the DataContext contains the AttachedImage
                var imageToRemove = (AttachedImage)packIcon.DataContext;
                // so, we can now remove it from the Module's collection
                this.reportProduzione.RemoveImage(imageToRemove);
            }
        }

        private void AttachedImages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChange("AttachedImages");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task<bool> CanAddImage()
        {
            if (reportProduzione.AttachedImages.Count > 1)
            {
                var confirmed = await DialogsHelper.ShowConfirmDialog("Sono già presenti due immagini, vuoi sovrascrivere l'ultima?", ConfirmDialog.ButtonConf.YES_NO);

                if (confirmed == ConfirmDialogResult.Yes)
                {
                    // remove the 2nd image
                    this.reportProduzione.AttachedImages.RemoveAt(1);
                }
                else
                {
                    // the user does not want to delete the last image and we cannot provide more than 2 images
                    return false;
                }
            }

            return true;
        }

        private async void Mostradocumentazione_Click(object sender, RoutedEventArgs e)
        {
            string pdfpath;
            bool checkboxriempiti = true;
            object selectedItem;

            // Controllo che almeno un imballo sia stato selezionato
            if (this.DatabaseTotale.SelectedItems.Count != 0)
            {
                // Controllo checkbox
                if (this.Conforme.IsChecked == false & this.NonConforme.IsChecked == false)
                {
                    checkboxriempiti = false;
                }
                // Controllo che tutti i campi siano stati compilati
                if (this.prodottoSelezionato.LarghezzaCassaReale == 0 | this.prodottoSelezionato.LunghezzaCassaReale == 0 |
                    this.prodottoSelezionato.AltezzaCassaReale == 0 | this.prodottoSelezionato.Conformita == "" | checkboxriempiti == false)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che tutti i campi siano stati riempiti", ConfirmDialog.ButtonConf.OK_ONLY);
                }
                else
                {
                    try
                    {
                        // Oggetti utili
                        ProdottoSelezionato prodotto = new ProdottoSelezionato();

                        // Caratteristiche del prodotto selezionato
                        selectedItem = this.DatabaseTotale.SelectedItems[0];
                        var dataRow = (selectedItem as DataRowView).Row;
                        this.prodottoSelezionato.Cliente = dataRow["Cliente"].ToString();
                        this.prodottoSelezionato.Codice = dataRow["Codice"].ToString();
                        this.prodottoSelezionato.VersioneCodice = int.Parse(dataRow["Versione"].ToString());

                        // Vado a compilare il report tecnico
                        DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
                        dbSQL.OpenConnection();
                        SqlDataReader reader;
                        SqlCommand creaComando = dbSQL.CreateDbTotaleCommand();
                        reader = creaComando.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader.GetValue(reader.GetOrdinal("Codice")).ToString() == dataRow["Codice"].ToString() &
                                reader.GetValue(reader.GetOrdinal("Versione")).ToString() == dataRow["Versione"].ToString())
                            {
                                // Leggo dal db le grandezze calcolate
                                this.prodottoSelezionato.LunghezzaCassa = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LunghezzaImballo")));
                                this.prodottoSelezionato.LarghezzaCassa = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LarghezzaImballo")));
                                this.prodottoSelezionato.AltezzaCassa = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("AltezzaImballo")));
                                this.prodottoSelezionato.Data = DateTime.Now.ToString("d/M/yyyy");
                            }
                        }

                        // Note post produzione
                        this.prodottoSelezionato.NotePostProduzione = this.TBNote.Text;

                        // Riempie e salva nella cartella temporanea la scheda tecnica
                        pdfpath = this.Pdfutils.FillSchedaPostProduzione(this.prodottoSelezionato, reportProduzione);

                        // Salvo il pdf nel db
                        ResourceReader resourcereader = new ResourceReader();

                        // Scheda produzione
                        byte[] binarySchedaPostProduzione = null;
                        binarySchedaPostProduzione = resourcereader.PdfToBinary(pdfpath);
                        DatabaseSQL databaseSQL1 = DatabaseSQL.CreateDefault();
                        SqlCommand creacomando4 = databaseSQL1.WriteBinaryCommandSchedaPostProduzione(prodottoSelezionato.Codice, prodottoSelezionato.VersioneCodice, binarySchedaPostProduzione);
                        creacomando4.ExecuteNonQuery();

                        // Update dati posto produzione
                        creacomando4 = databaseSQL1.UpdateDatiPostProduzione(prodottoSelezionato.Codice, prodottoSelezionato.VersioneCodice, this.prodottoSelezionato);
                        creacomando4.ExecuteNonQuery();

                        // Messaggio che è stato tutto salvato correttamente
                        ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("I dati sono stati salvati correttamente a database", ConfirmDialog.ButtonConf.OK_ONLY);
                        // Navigo al database
                        this.NavigationService.Navigate(new DatabaseView());
                    }
                    catch (Exception ex)
                    {
                        // Messaggio che è stato tutto salvato correttamente
                        ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Abbiamo riscontrato un problema nel salvataggio, contattare l'assistenza. Errore: " + ex, ConfirmDialog.ButtonConf.OK_ONLY);
                        // Navigo al database
                        this.NavigationService.Navigate(new DatabaseView());
                    }
                }
                
            }
            else
            {
                await DialogsHelper.ShowConfirmDialog("Assicurati di aver selezionato almeno un imballo", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }            

        private async void TBAltezzaCassa_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.TBAltezzaCassa.Text, out int _))
            {
                this.prodottoSelezionato.AltezzaCassaReale = Convert.ToInt32(this.TBAltezzaCassa.Text);
            }
            else if (this.TBAltezzaCassa.Text == "")
            {
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private async void TBLunghezzaCassa_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.TBLunghezzaCassa.Text, out int _))
            {
                this.prodottoSelezionato.LunghezzaCassaReale = Convert.ToInt32(this.TBLunghezzaCassa.Text);
            }
            else if (this.TBLunghezzaCassa.Text == "")
            {
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private async void TBLarghezzaCassa_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.TBLarghezzaCassa.Text, out int _))
            {
                this.prodottoSelezionato.LarghezzaCassaReale = Convert.ToInt32(this.TBLarghezzaCassa.Text);
            }
            else if (this.TBLarghezzaCassa.Text == "")
            {
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private void Conforme_Checked(object sender, RoutedEventArgs e)
        {
            // Significa che l'imballo è conforme
            this.prodottoSelezionato.Conformita = "Conforme";
            this.NonConforme.IsChecked = false;
        }

        private void NonConforme_Checked(object sender, RoutedEventArgs e)
        {
            // Significa che l'imballo non è conforme
            this.prodottoSelezionato.Conformita = "Non conforme";
            this.Conforme.IsChecked = false;
        }


    }
}
