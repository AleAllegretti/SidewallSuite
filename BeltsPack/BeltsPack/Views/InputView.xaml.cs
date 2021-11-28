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

            // Riempio il menù a tendina dell'altezza dei bordi
            this.ComboAltezzaBordi.ItemsSource = this.bordo.ListaAltezzeBordi().ToArray();
            this.altezzaBordo = this.prodotto.AltezzaBordo;

            // Riempio il combo con tutte le altezze disponibili in base a quella tazza
            if (!String.IsNullOrEmpty(this.tazza.Forma))
            {
                this.ComboAltezzaTazze.ItemsSource = this.ListaAltezzeTazze().ToArray();
            }          
            this.altezzaTazza = this.prodotto.AltezzaTazze;
            this.tipologiaNastro = this.prodotto.TipoNastro;
            this.classeNastro = this.prodotto.ClasseNastro;
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

            // Riempio il mneù dei clineti
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
            }
            else if (this.Larghezza.Text == "")
            {
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
        }

        private async void TBPistaLaterale_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.TBPistaLaterale.Text, out int _))
            {
                this.prodotto.PistaLaterale = Convert.ToInt32(this.TBPistaLaterale.Text);
                this.tazza.Lunghezza = this.nastro.Larghezza - (this.prodotto.PistaLaterale * 2);
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
        public List<int> ListaClassi(string tipoNastro)
        {
            List<int> Classi = new List<int>();
            if (tipoNastro == "TEXRIGID")
            {
                Classi.Add(315);
                Classi.Add(500);
                Classi.Add(630);
                Classi.Add(800);
                Classi.Add(1000);
                Classi.Add(1250);
                Classi.Add(1600);
            }
            else
            {
                Classi.Add(500);
                Classi.Add(630);
                Classi.Add(800);
                Classi.Add(1000);
                Classi.Add(1250);
                Classi.Add(1600);
                Classi.Add(2000);
            }

            return Classi;
        }

        private void ComboStrutturaNastro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno il tipo di nastro
            this.nastro.Tipo = this.ComboStrutturaNastro.SelectedValue.ToString();
            if (this.nastro.Tipo != "NON CODIFICATO")
            {
                // Popola la combo delle classi in base al tipo di nastro
                this.ComboClasseNastro.ItemsSource = this.ListaClassi(this.nastro.Tipo).ToArray();
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
                    ComboAltezzaTazze.SelectedItem == null | prodotto.PistaLaterale == null)
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
                if (nastro.Lunghezza == 0 | nastro.Larghezza == 0 | ComboAperto.SelectedItem == null | ComboAltezzaTazze.SelectedItem == null | prodotto.PistaLaterale == 0)
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
                if (nastro.Lunghezza == 0 | nastro.Larghezza == 0 | ComboAperto.SelectedItem == null | ComboAltezzaBordi.SelectedItem == null | prodotto.PistaLaterale == 0)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che tutti i campi siano stati riempiti", ConfirmDialog.ButtonConf.OK_ONLY);
                }
                else
                {
                    formfilled = true;
                }
            }

            // Check se il tipo di trasporto è stato specificato
            if (this.tipologiaTrasporto != "" || formfilled == false)
            {
                formfilled = true;
            }
            else
            {
                formfilled = false;
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati che il tipo di trasporto sia stato selezionato", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            

            if (formfilled == true)
            {
                // Larghezza utile
                this.nastro.SetLarghezzautile(this.bordo.Larghezza, this.prodotto.PistaLaterale);
                // Caratteristiche nastro
                this.nastro.SetCaratterisitche();
                // Calcolo il peso del nastro base
                this.nastro.SetPeso();
                // Determino i dettagli del cliente
                this.prodotto.SetDettagliCliente();

                if (this.prodotto.Tipologia == "Solo tazze" | this.prodotto.Tipologia == "Bordi e tazze")
                {
                    // Calcolo il numero e di tazze totali
                    this.tazza.NumeroTazzeTotali(this.nastro.Lunghezza, this.tazza.Passo);
                    // Lunghezza delle tazze
                    this.tazza.SetLunghezzaTotale(this.nastro.LarghezzaUtile);
                    // Caratteristiche
                    this.tazza.CarattersticheTazza();
                    // Peso totale tazze [kg]
                    this.tazza.SetPesoTotale();
                }
                if (this.prodotto.Tipologia == "Solo bordi" | this.prodotto.Tipologia == "Bordi e tazze")
                {
                    // Lunghezza bordo
                    this.bordo.SetLunghezzaTotaleBordo(this.nastro.Lunghezza);
                    // Peso bordo
                    this.bordo.SetPesoTotale();
                }
               
                // Calcolo il peso totale del nastro finito
                this.prodotto.SetPesoTotale(this.nastro.PesoTotale, this.tazza.PesoTotale, this.bordo.PesoTotale);
                // Calcolo le dimensioni dell'imballo
                this.CalcolaImballo();
            }
        }

        private void ComboFormaTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno la forma delle tazze
            this.tazza.Forma = this.ComboFormaTazze.SelectedValue.ToString();

            // Riempio il combo con tutte le altezze disponibili in base a quella tazza
            this.ComboAltezzaTazze.ItemsSource = this.ListaAltezzeTazze().ToArray();

            // Tazze a forma di C non fix e blinkers
            if(this.tazza.Forma == "C")
            {
                this.ComboBlk.IsEnabled = false;
                this.ComboFix.IsEnabled = false;
                this.ComboPassoFix.IsEnabled = false;
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

        private void ComboAltezzaTazze_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Assegno l'altezza della tazza
            tazza.Altezza = Convert.ToInt32(this.ComboAltezzaTazze.SelectedValue.ToString());
            // Determino le caratterstiche della tazza
            this.tazza.CarattersticheTazza();
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
            this.tazza.Passo = this.passoFix;
        }

        private void ComboClasseNastro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.nastro.Classe = this.classeNastro;
        }

        private void ComboAltezzaTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tazza.Altezza = this.altezzaTazza;
        }

        private void ComboBaseBordi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino la larghezza della base del bordo
            this.bordo.Larghezza = this.baseBordo;
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
            // Assegno il trattamento del nastro
            this.nastro.Trattamento = this.ComboQuality.SelectedValue.ToString();

            // Determino la sigla del trattamento
            this.nastro.SetTrattamentoSigla(this.nastro.Trattamento);
        }

        private void ComboQualityBordo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno il trattamento del nastro
            this.bordo.Trattamento = this.ComboQualityBordo.SelectedValue.ToString();

            // Determino la sigla del trattamento
            this.bordo.SetTrattamentoSigla(this.bordo.Trattamento);
        }

        private void TipologiaTrasporto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.TipologiaTrasporto = this.tipologiaTrasporto;
        }

        private void ComboNTazzexFila_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tazza.NumeroFile = this.ntazzeXFila;

            // Se ho solo una fila singola disabilito il textbox per lo spazio tra le varie file
            if (this.tazza.NumeroFile == 1)
            {
                this.SpazioTazzeFileMultiple.IsEnabled = false;
            }
            else
            {
                this.SpazioTazzeFileMultiple.IsEnabled = true;
            }
        }

        private void SpazioTazzeFileMultiple_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.spazioTazzeFileMultiple = this.tazza.SpazioFileMultiple;
        }

        private void ComboQualityTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegno il trattamento del nastro
            this.tazza.Trattamento = this.ComboQualityTazze.SelectedValue.ToString();

            // Determino la sigla del trattamento
            this.tazza.SetTrattamentoSigla(this.tazza.Trattamento);
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
        }
    }
}
