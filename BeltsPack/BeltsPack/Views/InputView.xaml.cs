using BeltsPack.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using BeltsPack.Views.Dialogs;
using System.Data.SqlClient;
using BeltsPack.Utils;
using System.Net;
using System.Reflection;
using System.IO;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Globalization;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for InputView.xaml
    /// </summary>
    public partial class InputView : Page
    {
        List<string> Clienti = new List<string>();
        Nastro nastro = new Nastro();
        Prodotto prodotto = new Prodotto();
        Bordo bordo = new Bordo();
        Tazza tazza = new Tazza();
        CassaInFerro cassaInFerro = new CassaInFerro();
        public string tazzeTelate { get; set; }
        public string trattamentoNastro { get; set; }
        public string trattamentoBordo { get; set; }
        public string trattamentoTazza { get; set; }
        public string tipologiaTrasporto { get; set; }
        public string formaTazza { get; set; }
        public string presenzaFix { get; set; }
        public string presenzaBlinkers { get; set; }
        public string tipologiaNastro { get; set; }
        public int ntazzeXFila { get; set; }
        public int spazioTazzeFileMultiple { get; set; }
        public int lunghezzaNastro { get; set; }
        public string nastroSelezionato { get; set; }
        public int larghezzaTazze { get; set; }
        public int larghezzaNastro { get; set; }
        public string cliente { get; set; }
        public string apertoChiuso { get; set; }
        public string commessa { get; set; }
        public string tipologiaProdotto { get; set; }
        public int altezzaTazza { get; set; }
        public int altezzaBordo { get; set; }
        public int baseBordo { get; set; }
        public int passoFix { get; set; }
        public int passonoFix { get; set; }
        public int pistaLaterale { get; set; }
        public int classeNastro { get; set; }
        public int qtyProdotto { get; set; }
        public bool larghezzaValidity { get; set; }
        public UserControl ParentControl { get; set; }
        public InputView(Prodotto prodotto)
        {
            this.prodotto = prodotto;
          
            InitializeComponent();

            // Assegno le grandezze nel caso in cui l'imballo sia stato copiato
            this.cliente = this.prodotto.Cliente;
            this.commessa = this.prodotto.Codice;
            this.tipologiaTrasporto = this.prodotto.TipologiaTrasporto;
            this.tipologiaProdotto = this.prodotto.Tipologia;
            this.apertoChiuso = this.prodotto.Aperto;
            this.lunghezzaNastro = this.prodotto.LunghezzaNastro;
            this.larghezzaNastro = this.prodotto.LarghezzaNastro;
            this.trattamentoNastro = this.prodotto.TrattamentoNastro;
            this.trattamentoBordo = this.prodotto.TrattamentoBordo;
            this.trattamentoTazza = this.prodotto.TrattamentoTazze;
            this.ntazzeXFila = this.prodotto.NumeroTazzexFila;
            this.spazioTazzeFileMultiple = this.prodotto.SpazioFile;
            this.tazzeTelate = this.prodotto.TazzeTelate;
            this.qtyProdotto = this.prodotto.Qty;

            // Azzero i controllori
            this.larghezzaValidity = false;

            // Riempio il menù a tendina dell'altezza dei bordi
            this.ComboAltezzaBordi.ItemsSource = this.bordo.ListaAltezzeBordi().ToArray();
            this.altezzaBordo = this.prodotto.AltezzaBordo;

            // Riempio il combo con tutte le altezze disponibili in base a quella tazza
            if (!String.IsNullOrEmpty(this.tazza.Forma))
            {
                this.ComboAltezzaTazze.ItemsSource = this.ListaAltezzeTazze().ToArray();
            }          
            this.altezzaTazza = this.prodotto.AltezzaTazze;

            // Riempio il combo con tutte le tipologie di nastro
            this.CBTipologiaNastro.ItemsSource = this.nastro.ListaTiplogieNastro().ToArray();
            this.CBTipologiaNastro.SelectedItem = this.prodotto.TipoNastro;
            
            this.ComboClasseNastro.SelectedItem = this.prodotto.ClasseNastro;
            this.baseBordo = this.prodotto.LarghezzaBordo;
            this.pistaLaterale = this.prodotto.PistaLaterale;
            this.presenzaFix = this.prodotto.PresenzaFix;
            this.presenzaBlinkers = this.prodotto.PresenzaBlinkers;
            this.formaTazza = this.prodotto.FormaTazze;

            if (presenzaFix == "Si")
            {
                this.passoFix = this.prodotto.PassoTazze;
            }
            else
            {
                this.passonoFix = this.prodotto.PassoTazze;
            }

            // Riempio il mneù dei clienti
            ComboClienti.ItemsSource = this.prodotto.ListaClienti().ToArray();

            // Abilito il data binding
            this.DataContext = this;

        }

        public string GetcurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private async void CheckForUpdates()
        {
            // Controllo se ci sono aggiornamenti
            {
                var client = new WebClient();
                client.Headers.Add("User-Agent", "C# console program");

                string url = "http://www.a2engineering.it/wp-content/uploads/updatesconveyorbeltsmanager/CheckUpdates/Aggiornamento.htm";
                try
                {
                    string content = client.DownloadString(url);
                    string version = this.GetcurrentVersion();
                    if (content.Contains(version) == false)
                    {
                        // Inizializza il risultato del dialog
                        ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("E' disponibile un aggiornamento del software, vuoi aggiornarlo ora?", ConfirmDialog.ButtonConf.YES_NO);

                        // Aggiorno o resta sulla pagina
                        if (confirmed.ToString() == "Yes")
                        {
                            System.Diagnostics.Process.Start("http://www.a2engineering.it/wp-content/uploads/updatesconveyorbeltsmanager/publish.htm");
                        }
                    }

                }
                catch
                {

                }

            }
        }
        private void TipologiaNastro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Assegnazione caratteristiche nastro
            prodotto.Tipologia = this.TipologiaNastro.SelectedValue.ToString();

            if (prodotto.Tipologia == "Nastro liscio")
            {
                this.StackTazze.IsEnabled = false;
                this.StackBordi.IsEnabled = false;
            }
            else if (prodotto.Tipologia == "Solo tazze")
            {
                this.StackTazze.IsEnabled = true;
                this.ComboFix.IsEnabled = false;
                this.ComboBlk.IsEnabled = false;
                this.ComboBaseBordi.IsEnabled = false;
                this.ComboAltezzaBordi.IsEnabled = false;
                this.ComboPassoFix.IsEnabled = false;
                this.TBPistaLaterale.IsEnabled = true;
                this.PassoSenzaFix.IsEnabled = true;
                this.ComboAltezzaBordi.SelectedItem = null;
                this.ComboBaseBordi.SelectedItem = null;
                this.bordo.Altezza = 0;
                this.ComboQualityBordo.SelectedItem = null;
                this.ComboQualityBordo.IsEnabled = false;
                this.ComboQualityTazze.IsEnabled = true;
                this.ComboTeleTazze.SelectedItem = null;
                this.ComboTeleTazze.IsEnabled = true;
                this.ComboNTazzexFila.IsEnabled = true;
                this.ComboNTazzexFila.SelectedItem = null;
                this.SpazioTazzeFileMultiple.IsEnabled = true;
            }
            else if (prodotto.Tipologia == "Solo bordi")
            {
                this.StackTazze.IsEnabled = false;
                this.StackBordi.IsEnabled = true;
                this.ComboBaseBordi.IsEnabled = true;
                this.ComboAltezzaBordi.IsEnabled = true;
                this.ComboAltezzaTazze.SelectedItem = null;
                this.ComboFormaTazze.SelectedItem = null;
                this.tazza.Altezza = 0;
                this.ComboTeleTazze.SelectedItem = null;
                this.ComboTeleTazze.IsEnabled = false;
                this.ComboQualityTazze.SelectedItem = null;
                this.ComboQualityTazze.IsEnabled = false;
                this.ComboQualityBordo.IsEnabled = true;
                this.ComboNTazzexFila.IsEnabled = false;
                this.ComboNTazzexFila.SelectedItem = null;
                this.SpazioTazzeFileMultiple.IsEnabled = false;
            }
            else if (prodotto.Tipologia == "Bordi e tazze")
            {
                this.ComboAltezzaBordi.IsEnabled = true;
                this.StackTazze.IsEnabled = true;
                this.StackBordi.IsEnabled = true;
                this.ComboFix.IsEnabled = true;
                this.ComboBlk.IsEnabled = true;
                this.ComboPassoFix.IsEnabled = true;
                this.ComboBaseBordi.IsEnabled = true;
                this.ComboQualityBordo.IsEnabled = true;
                this.ComboQualityTazze.IsEnabled = true;
                this.ComboTeleTazze.SelectedItem = null;
                this.ComboTeleTazze.IsEnabled = true;
                this.ComboNTazzexFila.IsEnabled = true;
                this.ComboNTazzexFila.SelectedItem = null;
                this.SpazioTazzeFileMultiple.IsEnabled = true;
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(this.Lunghezza.Text, out int _))
            {
                nastro.Lunghezza = Convert.ToInt32(this.Lunghezza.Text);
            }
            else if (this.Lunghezza.Text == "")
            {
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private async void Larghezza_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.Larghezza.Text, out int _))
            {
                nastro.Larghezza = Convert.ToInt32(this.Larghezza.Text);

                // Controllo che la larghezza non sia troppo alta
                if (nastro.Larghezza > 2500 && this.larghezzaValidity == false)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro che il valore che hai inserito sia corretto e non troppo alto?", ConfirmDialog.ButtonConf.OK_ONLY);
                    this.larghezzaValidity = true;
                }

                // Calcolo la larghezza di ogni singola tazza
                if (this.tazza.NumeroFile != 0)
                {
                    if (this.prodotto.Tipologia == "Bordi e tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                        this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else if (this.prodotto.Tipologia == "Solo tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
                        this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else
                    {
                        this.LarghezzaTazza.Text = "0";
                    }
                    // Determino la lunghezza della tazza come se fosse una unica
                    this.tazza.Lunghezza = this.larghezzaTazze * this.tazza.NumeroFile;
                }
            }
            else if (this.Larghezza.Text == "")
            {
            }
            else if (Convert.ToInt32(this.Larghezza.Text) > 2000)
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro che il valore che hai inserito sia corretto e non troppo alto?", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private void ComboAperto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboAperto.SelectedValue.ToString() == "Aperto")
            {
                nastro.Aperto = true;
            }
            else
            {
                nastro.Aperto = false; 
            }
        }

        private void ComboAltezzaBordi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno l'altezza del bordo
            bordo.Altezza = this.altezzaBordo;
            // Creo la lista delle larghezze delle basi disponibili
            this.ComboBaseBordi.ItemsSource = this.bordo.ListaBasiBordo().ToArray();
            // Azzero la parte delle tazze
            this.ComboAltezzaTazze.Text = "";
            this.ComboFormaTazze.Text = "";
        }

        private async void TBPistaLaterale_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.TBPistaLaterale.Text, out int _))
            {
                this.prodotto.PistaLaterale = Convert.ToInt32(this.TBPistaLaterale.Text);

                // Calcolo la larghezza di ogni singola tazza
                if (this.tazza.NumeroFile != 0)
                {
                    if (this.prodotto.Tipologia == "Bordi e tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                        this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else if (this.prodotto.Tipologia == "Solo tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
                        this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else
                    {
                        this.LarghezzaTazza.Text = "0";
                    }
                    // Determino la lunghezza della tazza come se fosse una unica
                    this.tazza.Lunghezza = this.larghezzaTazze * this.tazza.NumeroFile;
                }
            }
            else if (this.TBPistaLaterale.Text == "")
            {
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        public void TBNomeCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Controllo se ci sono aggiornamenti
            this.CheckForUpdates();
            //prodotto.Cliente = this.TBNomeCliente.Text;

            // Primo input per velocizzare il processo
            bool rootcreated = false;

            // Verifico che la cartella con le credenziali esiste, altrimenti faccio comparire il form per la registrazione
            string root = @"C:\ASquared\Info.xml";
            if (rootcreated == false)
            {
                if (File.Exists(root) == false)
                {
                    // pulisco il campo
                    //this.TBNomeCliente.Text = "";

                    // Creo la directory
                    Directory.CreateDirectory(@"C:\ASquared");

                    // Mostro lo userform per la registrazione
                    this.NavigationService.Navigate(new LoginView());

                    // Prendo il nome utente
                    prodotto.Utente = this.prodotto.DeterminoNomeUtente(@"C:\ASquared\Info.xml");

                    // Confermo che la root è stata creata
                    rootcreated = true;
                }
                else
                {
                    // Confermo che la root è stata creata
                    rootcreated = true;
                }
            }

        }

        private void TBCodiceArtiolo_TextChanged(object sender, TextChangedEventArgs e)
        {
            prodotto.Codice = this.TBCodiceArtiolo.Text;
        }

        private void CalcolaImballo()
        {
            // Calcola le dimensioni dell'imballo
            Imballi imballiBobina = new Imballi(nastro, bordo, tazza, prodotto, cassaInFerro);

            // Naviga nella finestra di output
            if (imballiBobina.Tipologia.ToString() == TipologiaImballo.Pedana.ToString())
            {
               
                // Navigo nella schermata di output
                this.NavigationService.Navigate(new OutputView(nastro, imballiBobina, this, prodotto, this.bordo, this.tazza));
            }
            else if (imballiBobina.ImballoCalcolabile == true)
            {
                // Navigo nella schermata degli accessori in ferro              
                this.NavigationService.Navigate(new AccessoriCasseFerro(imballiBobina, this, this.nastro, cassaInFerro, bordo, prodotto, tazza));
            }
        }

        private async void TipologiaNastro_DropDownOpened(object sender, EventArgs e)
        {
            if (prodotto.Codice == null | prodotto.Cliente == null)
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Mancano dei dati nell'anagrafica", ConfirmDialog.ButtonConf.OK_ONLY);
            }

        }

        private async void Calcola_Click_1(object sender, RoutedEventArgs e)
        {
            // Ci dice se il form è stato completato
            bool formfilled = false;

            //Controlla se tutti i campi sono stati riempiti
            if (prodotto.Tipologia == "Nastro liscio")
            {
                if (nastro.Lunghezza == 0 | nastro.Larghezza == 0 | ComboAperto.SelectedItem == null | nastro.Tipo == "")
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che tutti i campi siano stati riempiti", ConfirmDialog.ButtonConf.OK_ONLY);
                    formfilled = false;
                }
                else
                {
                    formfilled = true;
                }

            }
            else if (prodotto.Tipologia == "Bordi e tazze")
            {
                if (nastro.Lunghezza == 0 | nastro.Larghezza == 0 | ComboAperto.SelectedItem == null | ComboAltezzaBordi.SelectedItem == null | 
                    ComboAltezzaTazze.SelectedItem == null)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che tutti i campi siano stati riempiti", ConfirmDialog.ButtonConf.OK_ONLY);
                    formfilled = false;
                }
                else
                {
                    formfilled = true;
                }
            }
            else if (prodotto.Tipologia == "Solo tazze")
            {
                if (nastro.Lunghezza == 0 | nastro.Larghezza == 0 | ComboAperto.SelectedItem == null | ComboAltezzaTazze.SelectedItem == null)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che tutti i campi siano stati riempiti", ConfirmDialog.ButtonConf.OK_ONLY);
                    formfilled = false;
                }
                else
                {
                    formfilled = true;
                }
            }
            else if (prodotto.Tipologia == "Solo bordi")
            {
                if (nastro.Lunghezza == 0 | nastro.Larghezza == 0 | ComboAperto.SelectedItem == null | ComboAltezzaBordi.SelectedItem == null)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che tutti i campi siano stati riempiti", ConfirmDialog.ButtonConf.OK_ONLY);
                }
                else
                {
                    formfilled = true;
                }
            }

            // Check se il tipo di trasporto è stato specificato
            //if (this.tipologiaTrasporto != "" || formfilled == false)
            //{
            //    formfilled = true;
            //}
            //else
            //{
            //    formfilled = false;
            //    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che il tipo di trasporto sia stato selezionato", ConfirmDialog.ButtonConf.OK_ONLY);
            //}
            

            if (formfilled == true)
            {
                // Larghezza utile
                this.nastro.SetLarghezzautile(this.bordo.Larghezza, this.prodotto.PistaLaterale);
                // Calcolo il peso del nastro base
                this.nastro.SetPeso();
                // Determino i dettagli del cliente
                this.prodotto.SetDettagliCliente();

                if (this.prodotto.Tipologia == "Solo tazze" | this.prodotto.Tipologia == "Bordi e tazze")
                {
                    // Calcolo il numero e di tazze totali
                    this.tazza.NumeroTazzeTotali(this.nastro.Lunghezza, this.tazza.Passo);
                    // Lunghezza delle tazze
                    this.tazza.SetLunghezzaTotale(this.tazza.Lunghezza);
                    // Caratteristiche
                    try
                    {
                        this.tazza.CarattersticheTazza();
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Non sono riuscito a determinare tutte le caratteristiche della tazza, quindi alcune grandezze potrebbero essere errate. \nAssicurarsi che il dB sia aggiornato.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    // Peso totale tazze [kg]
                    this.tazza.SetPesoTotale();
                }
                if (this.prodotto.Tipologia == "Solo bordi" | this.prodotto.Tipologia == "Bordi e tazze")
                {
                    // Lunghezza bordo
                    this.bordo.SetLunghezzaTotaleBordo(this.nastro.Lunghezza, this.nastro.Aperto);
                    // Peso bordo
                    this.bordo.SetPesoTotale();
                }
               
                // Calcolo il peso totale del nastro finito
                this.prodotto.SetPesoTotale(this.nastro.PesoTotale, this.tazza.PesoTotale, this.bordo.PesoTotale, this.prodotto.Tipologia);

                // Calcolo le dimensioni dell'imballo
                this.CalcolaImballo();
            }
        }

        private void ComboFormaTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno la forma delle tazze
            try
            {
                if (this.ComboFormaTazze.SelectedValue != null)
                {
                    this.tazza.Forma = this.ComboFormaTazze.SelectedValue.ToString();

                    // Riempio il combo con tutte le altezze disponibili in base a quella tazza
                    this.ComboAltezzaTazze.ItemsSource = this.ListaAltezzeTazze().ToArray();

                    // Tazze a forma di C non fix e blinkers
                    if (this.tazza.Forma == "C")
                    {
                        this.ComboBlk.IsEnabled = false;
                        this.ComboFix.IsEnabled = false;
                        this.ComboPassoFix.IsEnabled = false;
                        this.ComboFix.Text = "No";
                        this.ComboBlk.Text = "No";
                    }
                    else if (this.tazza.Forma == "T" || this.tazza.Forma == "TW")
                    {
                        this.ComboBlk.IsEnabled = false;
                    }
                    else
                    {
                        this.ComboBlk.IsEnabled = true;
                        this.ComboFix.IsEnabled = true;
                        this.ComboPassoFix.IsEnabled = true;
                    }
                }
                
            }            
            catch
            {
                return;
            }
        }
        public List<int> ListaAltezzeTazze()
        {
            List<int> Altezze = new List<int>();

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingTazzeCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Forma" + this.tazza.Forma));
                var temp1 = reader.GetValue(reader.GetOrdinal("Altezza"));
                if (temp.ToString() == "x" & Convert.ToInt32(temp1.ToString()) <= altezzaBordo & altezzaBordo !=0)
                {
                    Altezze.Add(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza"))));
                }
                else if(temp.ToString() == "x" &  altezzaBordo == 0)
                {
                    Altezze.Add(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza"))));
                }
            }

            // Metto gli elementi della lista in ordine crescente
            GFG gg = new GFG();
            Altezze.Sort(gg);

            return Altezze;
        }
        public class GFG : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x == 0 || y == 0)
                {
                    return 0;
                }

                // CompareTo() method 
                return x.CompareTo(y);

            }
        }

        private void ComboFix_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.PresenzaFix = this.ComboFix.SelectedValue.ToString();

            if (this.ComboFix.SelectedValue.ToString() == "Si")
            {
                // Attivo solo i campi che mi servono
                this.PassoSenzaFix.IsEnabled = false;
                this.ComboPassoFix.IsEnabled = true;
                this.ComboBlk.IsEnabled = true;
                
            }
            else
            {
                this.ComboBlk.IsEnabled = false;
                this.ComboPassoFix.IsEnabled = false;
                this.PassoSenzaFix.IsEnabled = true;
                this.prodotto.PresenzaBlinkers = "No";
                this.ComboFix.SelectedValue = "No";
            }
        }
        private void ComboAltezzaBordi_DropDownOpened(object sender, EventArgs e)
        {
            // Riempio il menù a tendina
            this.ComboAltezzaBordi.ItemsSource = this.bordo.ListaAltezzeBordi().ToArray();
        }
        public List<int> ListaPassoFix()
        {
            List<int> Altezze = new List<int>();
            int passo = this.bordo.PassoOnda * 3;
            while(passo < this.bordo.PassoOnda*15)
            {
                Altezze.Add(passo);
                passo += this.bordo.PassoOnda;
            }

            return Altezze;
        }

        private async void PassoSenzaFix_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.PassoSenzaFix.Text, out int _))
            {
                this.tazza.Passo = Convert.ToInt32(this.PassoSenzaFix.Text);
            }
            else if (this.TBPistaLaterale.Text == "")
            {
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il valore inserito non è valido", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private void ComboPassoFix_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.tazza.Passo = Convert.ToInt32(this.ComboPassoFix.SelectedItem);
            }
            catch 
            { }   
        }

        private async void ComboClasseNastro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino la classe
            this.nastro.Classe = Convert.ToInt32(this.ComboClasseNastro.SelectedValue);
            // Determino le caratteristiche del nastro
            this.nastro.SetCaratterisitche();
            if(this.nastro.NumTessuti == 0 || this.nastro.NumTele == 0)
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il nome del nastro non può essere completato perchè mancano il numero di breaker e/o di tele. \nPer aggiungere: Impostazioni -> Nastri.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            else
            {
                // Vado a completare il nome finale del nastro
                this.nastroSelezionato = this.nastro.Tipo + " " + this.nastro.Classe + "/" + this.nastro.NumTessuti + "+" + this.nastro.NumTele + "  " + this.nastro.SpessoreSup + "+" + this.nastro.SpessoreInf ;
                // Mostro a schermo il nome intero del nastro selezionato
                this.NastroSel.Text = "(" + this.nastroSelezionato + ")";
            }         
            
        }

        private void ComboAltezzaTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tazza.Altezza = this.altezzaTazza;
        }

        private void ComboBaseBordi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino la larghezza della base del bordo
            this.bordo.Larghezza = this.baseBordo;
            // Calcolo la larghezza di ogni singola tazza
            if (this.tazza.NumeroFile != 0)
            {
                if (this.prodotto.Tipologia == "Bordi e tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                    this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else if (this.prodotto.Tipologia == "Solo tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
                    this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else
                {
                    this.LarghezzaTazza.Text = "0";
                }
                // Determino la lunghezza della tazza come se fosse una unica
                this.tazza.Lunghezza = this.larghezzaTazze * this.tazza.NumeroFile;
            }
            // Determino le grandezze principali del bordo
            this.bordo.GetInfoBordo();
            // Vado a riempire la lista dei passi con fix in base al passo onda del bordo
            this.ComboPassoFix.ItemsSource = this.ListaPassoFix().ToArray();
        }

        private void ComboBlk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino se ho i blinkers o meno
            this.prodotto.PresenzaBlinkers = this.ComboBlk.SelectedValue.ToString();
        }

        private void ComboQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboQuality.SelectedValue != null)
            {
                // Assegno il trattamento del nastro
                this.nastro.Trattamento = this.ComboQuality.SelectedValue.ToString();

                // Determino la sigla del trattamento
                this.nastro.SetTrattamentoSigla(this.nastro.Trattamento);
            }
            
        }

        private void ComboQualityBordo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboQualityBordo.SelectedValue !=null)
            {
                // Assegno il trattamento del nastro
                this.bordo.Trattamento = this.ComboQualityBordo.SelectedValue.ToString();

                // Determino la sigla del trattamento
                this.bordo.SetTrattamentoSigla(this.bordo.Trattamento);
            }
            
        }

        private void TipologiaTrasporto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.TipologiaTrasporto = this.tipologiaTrasporto;
        }

        private void ComboNTazzexFila_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno il numero di tazze
            this.tazza.NumeroFile = Convert.ToInt32(this.ComboNTazzexFila.SelectedItem);

            // Calcolo la larghezza di ogni singola tazza
           if (this.tazza.NumeroFile != 0)
            {
                if (this.prodotto.Tipologia == "Bordi e tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                    this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else if (this.prodotto.Tipologia == "Solo tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
                    this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else
                {
                    this.LarghezzaTazza.Text = "0";
                }
                
            }

            // Determino la lunghezza della tazza come se fosse una unica
            this.tazza.Lunghezza = this.larghezzaTazze * this.tazza.NumeroFile;
        }

        private void SpazioTazzeFileMultiple_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Assegno lo spazio
            if (this.SpazioTazzeFileMultiple.Text != "")
            {
                this.tazza.SpazioFileMultiple = Convert.ToInt32(this.SpazioTazzeFileMultiple.Text);
            }
            
            // Se lo spazio è diverso da zero, calcolo quante tazze x fila posso mettere
            int maxTazzeXFila = 2;
            double temp = (this.nastro.Larghezza - this.pistaLaterale * 2 - this.tazza.SpazioFileMultiple * (maxTazzeXFila - 1)) / maxTazzeXFila;
            if (this.tazza.SpazioFileMultiple != 0)
            {
                this.ComboNTazzexFila.Items.Clear();
                this.ComboNTazzexFila.IsEnabled = true;
                while (Math.Round(temp,0) >= this.tazza.Altezza)
                {
                    this.ComboNTazzexFila.Items.Add(maxTazzeXFila);
                    maxTazzeXFila++;
                    temp = (this.nastro.Larghezza - this.pistaLaterale * 2 - this.tazza.SpazioFileMultiple * (maxTazzeXFila - 1)) / maxTazzeXFila;
                }
            }
            else
            {
                this.ComboNTazzexFila.Items.Clear();
                this.ComboNTazzexFila.Items.Add(1);
                this.ComboNTazzexFila.SelectedItem = this.ComboNTazzexFila.Items[0];
                this.ComboNTazzexFila.IsEnabled = false;
            }
        }

        private void ComboQualityTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno il trattamento del nastro
            if (this.ComboQualityTazze.SelectedValue != null)
            {
                this.tazza.Trattamento = this.ComboQualityTazze.SelectedValue.ToString();

                // Determino la sigla del trattamento
                this.tazza.SetTrattamentoSigla(this.tazza.Trattamento);
            }
            
        }

        private void ComboTeleTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tazza.Telata = this.tazzeTelate;

            // Stabilisco la sigla delle tele
            if(this.tazza.Telata == "Si")
            {
                this.tazza.SiglaTele = "HBF";
            }
            else
            {
                this.tazza.SiglaTele = "HBL";
            }
        }

        private void ComboClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.Cliente = this.cliente;

            // Capisco la nazionalità del cliente
            DatabaseSQL dbSQL = DatabaseSQL.CreateARCF();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ClienteSearchCommand(this.cliente);
            reader = creaComando.ExecuteReader();

            // Trovo la sigla della nazione di provenienza del cliente
            while (reader.Read())
            {
                this.prodotto.ProvenienzaClienteSigla = reader.GetValue(reader.GetOrdinal("Cd_Nazione")).ToString();
            }

            reader.Close();

            // In base alla sigla del cliente mi trovo le info della nazione
            try
            {
                RegionInfo info = new RegionInfo(this.prodotto.ProvenienzaClienteSigla);
                this.prodotto.ProvenienzaClienteNazione = info.EnglishName;
            }
            catch (ArgumentException argEx)
            {
                // The code was not a valid country code
            }

            // Capisco se il paese è in europa o meno
            if (this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("russia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("germany") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("united kingdom") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("france") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("italy") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("spain") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("ukraine") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("poland") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("netherlands") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("belgium") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("czech republic") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("greece") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("portugal") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("sweden") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("hungary") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("belarus") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("austria") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("switzerland") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("bulgaria") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("denmark") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("finland") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("slovakia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("norway") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("ireland") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("croatia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("moldova") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("bosnia and herzegovina") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("albania") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("lithuania") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("north macedonia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("slovenia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("latvia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("estonia") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("montenegro") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("luxembourg") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("malta") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("iceland") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("andorra") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("monaco") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("liechtestein") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("san marino") ||
                this.prodotto.ProvenienzaClienteNazione.ToLower().Contains("holy see"))
            {
                this.prodotto.ProvenienzaClienteContinente = "EU";
            }
            else
            {
                this.prodotto.ProvenienzaClienteContinente = "EXTRA-EU";
            }
        }

        private void Quantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.Qty = qtyProdotto;
        }

        private void ComboClienti_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                ComboClienti.ItemsSource = this.prodotto.ListaClienti().ToArray();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("C'è stato un problema: " + ex, "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            
        }

        private void CBTipologiaNastro_DropDownOpened_1(object sender, EventArgs e)
        {
            // Riempio il menù a tendina
            this.CBTipologiaNastro.ItemsSource = this.nastro.ListaTiplogieNastro().ToArray();
        }

        private async void CBTipologiaNastro_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Assegno la tipologia di nastro
            this.nastro.Tipo = this.CBTipologiaNastro.SelectedItem.ToString();

            // Assegno il tipo di nastro
            if (this.CBTipologiaNastro.SelectedItem.ToString() != "NON CODIFICATO")
            {
                // Popola la combo delle classi in base al tipo di nastro
                this.ComboClasseNastro.ItemsSource = this.nastro.ListaClassiNastro(this.nastro.Tipo).ToArray();
                // Se il menù è vuoto, blocco il flusso e mando al menù impostazioni per inserire la classe
                if (this.ComboClasseNastro.Items[0].ToString() == "")
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Per questo nastro non sono ancora state inserite le classi. \nPer aggiungere: Impostazioni -> Nastri.", ConfirmDialog.ButtonConf.OK_ONLY);
                }

                // Abilito la combo della classe nel caso in cui fosse stata disabilitata
                this.ComboClasseNastro.IsEnabled = true;
            }
            else
            {
                var view = new NastroNonCodDialog(this.nastro);
                DialogHost.Show(view);
                // Diasibilito la combo della classe
                this.ComboClasseNastro.IsEnabled = false;
            }
        }
    }
}
