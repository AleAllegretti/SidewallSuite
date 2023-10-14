using BeltsPack.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using BeltsPack.Views.Dialogs;
using System.Data.SqlClient;
using BeltsPack.Utils;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace BeltsPack.Views
{
    /// <summary>
    /// Logica di interazione per Calcoli.xaml
    /// </summary>
    public partial class CalcoliView : Page
    {
        List<string> Clienti = new List<string>();
        private Nastro nastro;
        private Tazza tazza;
        private Prodotto _prodotto;
        private Bordo bordo;
        private Materiale _materiale;
        private Rullo rullo;
        private Tamburo _tamburo;
        private Motore _motore;
        private CalcoliImpianto _calcoliImpianto;
        private Imballi _imballi;
        private CassaInFerro _cassainferro;

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
        public string tipologia_prodotto { get; set; }
        public int altezzaTazza { get; set; }
        public int altezzaBordo { get; set; }
        public int baseBordo { get; set; }
        public int passoFix { get; set; }
        public int passonoFix { get; set; }
        public int pistaLaterale { get; set; }
        public int classeNastro { get; set; }
        public int qty_prodotto { get; set; }
        public bool larghezzaValidity { get; set; }
        public UserControl ParentControl { get; set; }
        public double capacityReq { get; set; }
        public double fillingFactor { get; set; }
        public double velNastro { get; set; }
        public string formaNastro { get; set; }
        public int inclNastro { get; set; }
        public double elevNastro { get; set; }
        public double centDist { get; set; }
        public string nomeMat { get; set; }
        public double matDens { get; set; }
        public double surAngle { get; set; }
        public double dimSingolo { get; set; }
        public double edgeType { get; set; }
        public CalcoliView(Prodotto prodotto, Nastro nastro, Tazza tazza, Bordo bordo, Materiale materiale, Rullo rullo, Tamburo tamburo, Motore motore)
        {
            this._prodotto = prodotto;
            this.nastro = nastro;
            this.tazza = tazza;
            this.bordo = bordo;
            this._materiale = materiale;
            this.rullo = rullo;
            this._tamburo = tamburo;
            this._motore = motore;
           
            InitializeComponent();

            // Assegno le grandezze ai rispettivi campi
            this.cliente = this._prodotto.Cliente;
            this.commessa = this._prodotto.Codice;
            this.apertoChiuso = this._prodotto.Aperto;
            this.qty_prodotto = this._prodotto.Qty;
            this.lunghezzaNastro = this._prodotto.LunghezzaNastro;
            this.larghezzaNastro = this._prodotto.LarghezzaNastro;
            this.trattamentoNastro = this._prodotto.TrattamentoNastro;
            this.trattamentoBordo = this._prodotto.TrattamentoBordo;
            this.trattamentoTazza = this._prodotto.TrattamentoTazze;
            this.CBTipologiaNastro.ItemsSource = this.nastro.ListaTiplogieNastro().ToArray();
            this.CBTipologiaNastro.SelectedItem = this._prodotto.TipoNastro;
            this.ComboAltezzaBordi.ItemsSource = this.bordo.ListaAltezzeBordi().ToArray();
            this.altezzaBordo = this._prodotto.AltezzaBordo;
            this.ComboClasseNastro.ItemsSource = this.nastro.ListaClassiNastro(this.nastro.Tipo).ToArray();
            this.ComboClasseNastro.SelectedItem = this._prodotto.ClasseNastro.ToString();
            this.baseBordo = this._prodotto.LarghezzaBordo;
            this.pistaLaterale = this._prodotto.PistaLaterale;
            this.presenzaFix = this._prodotto.PresenzaFix;
            this.presenzaBlinkers = this._prodotto.PresenzaBlinkers;
            this.formaTazza = this._prodotto.FormaTazze;
            this.altezzaTazza = this._prodotto.AltezzaTazze;
            this.capacityReq = this.nastro.capacityRequired;
            this.fillingFactor = this._materiale.fillFactor;
            this.velNastro = this.nastro.speed;
            this.formaNastro = this.nastro.forma.ToString();
            this.formaNastro = this.nastro.forma;
            this.inclNastro = this.nastro.inclinazione;
            this.elevNastro = this.nastro.elevazione;
            this.centDist = this.nastro.centerDistance;
            this.nomeMat = materiale.Nome;
            this.matDens = materiale.density;
            this.surAngle = materiale.surchAngle;
            this.dimSingolo = materiale.DimSingolo;
            this.edgeType = nastro.edgetype;

            if (presenzaFix == "Si")
            {
                this.passoFix = this._prodotto.PassoTazze;
            }
            else
            {
                this.passonoFix = this._prodotto.PassoTazze;
            }

            if (!String.IsNullOrEmpty(this.tazza.Forma))
            {
                this.ComboAltezzaTazze.ItemsSource = this.ListaAltezzeTazze().ToArray();
            }
            this.altezzaTazza = this._prodotto.AltezzaTazze;

            // Riempio il menù dei clienti
            ComboClienti.ItemsSource = this._prodotto.ListaClienti().ToArray();

            // Abilito il data binding
            this.DataContext = this;

            
        }

        private void ComboClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._prodotto.Cliente = this.cliente;
            this._prodotto.OrigineCLiente();
        }

        private void ComboClienti_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                ComboClienti.ItemsSource = this._prodotto.ListaClienti().ToArray();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("C'è stato un problema: " + ex, "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TBCodiceArtiolo_TextChanged(object sender, TextChangedEventArgs e)
        {
            _prodotto.Codice = this.TBCodiceArtiolo.Text;
        }

        private void Quantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Quantity.SelectedItem != null)
            {
                this._prodotto.Qty = Convert.ToInt32(this.Quantity.SelectedValue);
            }
        }

        private void ComboAperto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboAperto.SelectedValue.ToString() == "Aperto")
            {
                this.nastro.Aperto = true;
            }
            else
            {
                this.nastro.Aperto = false;
            }
        }

        private async void CBTipologiaNastro_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void CBTipologiaNastro_DropDownOpened(object sender, EventArgs e)
        {
            // Riempio il menù a tendina
            this.CBTipologiaNastro.ItemsSource = this.nastro.ListaTiplogieNastro().ToArray();
        }

        private async void ComboClasseNastro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino la classe
            this.nastro.Classe = Convert.ToInt32(this.ComboClasseNastro.SelectedValue);
            // Determino le caratteristiche del nastro
            this.nastro.SetCaratterisitche();
            if (this.nastro.NumTele == 0)
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il nome del nastro non può essere completato perchè mancano il numero di breaker e/o di tele. \nPer aggiungere: Impostazioni -> Nastri.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            else
            {
                // Vado a completare il nome finale del nastro
                this.nastroSelezionato = this.nastro.Tipo + " " + this.nastro.Classe + "/" + this.nastro.NumTessuti + "+" + this.nastro.NumTele + "  " + this.nastro.SpessoreSup + "+" + this.nastro.SpessoreInf;
                // Mostro a schermo il nome intero del nastro selezionato
                //this.NastroSel.Text = "(" + this.nastroSelezionato + ")";
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(this.Lunghezza.Text, out int _))
            {
                this.nastro.Lunghezza = Convert.ToInt32(this.Lunghezza.Text);
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
                this.nastro.Larghezza = Convert.ToInt32(this.Larghezza.Text);

                // Controllo che la larghezza non sia troppo alta
                if (this.nastro.Larghezza > 2500 && this.larghezzaValidity == false)
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Sei sicuro che il valore che hai inserito sia corretto e non troppo alto?", ConfirmDialog.ButtonConf.OK_ONLY);
                    this.larghezzaValidity = true;
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

        private void ComboAltezzaBordi_DropDownOpened(object sender, EventArgs e)
        {
            // Riempio il menù a tendina
            this.ComboAltezzaBordi.ItemsSource = this.bordo.ListaAltezzeBordi().ToArray();
        }

        private void ComboBaseBordi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino la larghezza della base del bordo
            this.bordo.Larghezza = this.baseBordo;
            // Calcolo la larghezza di ogni singola tazza
            if (this.tazza.NumeroFile != 0)
            {
                if (this._prodotto.Tipologia == "Bordi e tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this._prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                    //this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else if (this._prodotto.Tipologia == "Solo tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this._prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
                    //this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else
                {
                    //this.LarghezzaTazza.Text = "0";
                }
                // Determino la lunghezza della tazza come se fosse una unica
                this.tazza.Lunghezza = this.larghezzaTazze * this.tazza.NumeroFile;
            }
            // Determino le grandezze principali del bordo
            this.bordo.GetInfoBordo();
            // Vado a riempire la lista dei passi con fix in base al passo onda del bordo
            this.ComboPassoFix.ItemsSource = this.ListaPassoFix().ToArray();
        }

        private async void TBPistaLaterale_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check se la stringa inserita è un intero
            if (int.TryParse(this.TBPistaLaterale.Text, out int _))
            {
                this._prodotto.PistaLaterale = Convert.ToInt32(this.TBPistaLaterale.Text);

                // Calcolo la larghezza di ogni singola tazza
                if (this.tazza.NumeroFile != 0)
                {
                    if (this._prodotto.Tipologia == "Bordi e tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this._prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                        //this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else if (this._prodotto.Tipologia == "Solo tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this._prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
                        //this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else
                    {
                        //this.LarghezzaTazza.Text = "0";
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

        private void ComboQualityBordo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboQualityBordo.SelectedValue != null)
            {
                // Assegno il trattamento del nastro
                this.bordo.Trattamento = this.ComboQualityBordo.SelectedValue.ToString();

                // Determino la sigla del trattamento
                this.bordo.SetTrattamentoSigla(this.bordo.Trattamento);
            }
        }

        private void ComboAltezzaTazze_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tazza.Altezza = this.altezzaTazza;
        }

        private void ComboFix_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._prodotto.PresenzaFix = this.ComboFix.SelectedValue.ToString();

            if (this.ComboFix.SelectedValue.ToString() == "Si")
            {
                // Attivo solo i campi che mi servono
                this.PassoSenzaFix.Visibility = Visibility.Collapsed;
                this.ComboPassoFix.Visibility = Visibility.Visible;
                this.ComboBlk.IsEnabled = true;

            }
            else
            {
                this.ComboBlk.IsEnabled = false;
                this.ComboPassoFix.Visibility = Visibility.Collapsed;
                this.PassoSenzaFix.Visibility = Visibility.Visible;
                this._prodotto.PresenzaBlinkers = "No";
                this.ComboFix.SelectedValue = "No";
            }
        }

        private void ComboBlk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino se ho i blinkers o meno
            this._prodotto.PresenzaBlinkers = this.ComboBlk.SelectedValue.ToString();
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
                if (temp.ToString() == "x" & Convert.ToInt32(temp1.ToString()) <= altezzaBordo & altezzaBordo != 0)
                {
                    Altezze.Add(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza"))));
                }
                else if (temp.ToString() == "x" & altezzaBordo == 0)
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

        public List<int> ListaPassoFix()
        {
            List<int> Altezze = new List<int>();
            int passo = this.bordo.PassoOnda * 3;
            while (passo < this.bordo.PassoOnda * 15)
            {
                Altezze.Add(passo);
                passo += this.bordo.PassoOnda;
            }

            return Altezze;
        }

        private void Calcola_Click(object sender, RoutedEventArgs e)
        {
            // Calcolo la larghezza utile del nastro
            this.nastro.SetLarghezzautile(this.bordo.Larghezza, this._prodotto.PistaLaterale);
            this.nastro.SetLengthCoeff();
            this.nastro.SetCaratterisitche();
            this.nastro.SetPeso();

            // Prendo le caratteristiche della tazza
            this.tazza.CarattersticheTazza();
            this.tazza.NumeroTazzeTotali(this.nastro.Lunghezza, this.tazza.Passo);
            this.tazza.SetLunghezzaTotale(this.nastro.LarghezzaUtile);
            this.tazza.SetPesoTotale();

            // Prendo le caratteristiche del bordo
            this.bordo.GetInfoBordo();
            this.bordo.SetLunghezzaTotaleBordo(this.nastro.Lunghezza, this.nastro.Aperto);
            this.bordo.SetPesoTotale();
            
            // Caratteristiche prodotto
            this._prodotto.Tipologia = "Bordi e tazze";

            // Calcolo il peso totale del nastro finito
            this._prodotto.SetPesoTotale(this.nastro.PesoTotale, this.tazza.PesoTotale, this.bordo.PesoTotale, this._prodotto.Tipologia);

            // Calcolo le quantità di output
            CalcoliImpianto calcoliImpianto = new CalcoliImpianto(this.nastro, this.tazza, this._prodotto, this.bordo, this._materiale, this.rullo, this._tamburo, this._motore);
            this._calcoliImpianto = calcoliImpianto;

            // Info nastro
            this.TBLarghUtile.Text = Convert.ToString(this.nastro.LarghezzaUtile);
            this.TBPesoNastro.Text = Convert.ToString(this.nastro.PesoTotale);

            // Ottengo i dati che mi danno come output la portata del nastro
            calcoliImpianto.GetCapacity();
            this.TBCapacityMcubi.Text = Convert.ToString(calcoliImpianto.Qeff);

            if (calcoliImpianto.Qeff <= (this.nastro.capacityRequired + 10) && calcoliImpianto.Qeff >= (this.nastro.capacityRequired - 10))
            {
                this.TBCapacityMcubi.Background = Brushes.LightGreen;
            }
            else if (calcoliImpianto.Qeff <= (this.nastro.capacityRequired + 50) && calcoliImpianto.Qeff >= (this.nastro.capacityRequired - 50))
            {
                this.TBCapacityMcubi.Background = Brushes.Yellow;
            }            
            else
            {
                this.TBCapacityMcubi.Background = Brushes.Red;
            }

            this.TBCapacityTon.Text = Convert.ToString(calcoliImpianto.Qteff);

            // Tensioni e fattori di sicurezza
            calcoliImpianto.TensionsCalculation();
            this.TBTensPulley.Text = Convert.ToString(calcoliImpianto.MaxWorkTens);
            this.TBTensPisteLat.Text = Convert.ToString(calcoliImpianto.MaxWorkTensLat);
            this.TBFattSicurezza.Text = Convert.ToString(calcoliImpianto.Sfactor);
            this.TBFattSicPisteLat.Text = Convert.ToString(calcoliImpianto.Sfactor_pista);

            // Metto un controllo visivo sui fattori di sicurezza - Fattore di sicurezza
            if (calcoliImpianto.Sfactor >= 10)
            {
                this.TBFattSicurezza.Background = Brushes.LightGreen;
            }
            else if (calcoliImpianto.Sfactor < 8)
            {
                this.TBFattSicurezza.Background = Brushes.Red;
            }
            else
            {
                this.TBFattSicurezza.Background = Brushes.Yellow;
            }

            // Fattore di sicurezza piste laterali
            if (calcoliImpianto.Sfactor_pista >= 10)
            {
                this.TBFattSicPisteLat.Background = Brushes.LightGreen;
            }
            else if (calcoliImpianto.Sfactor_pista < 8)
            {
                this.TBFattSicPisteLat.Background = Brushes.Red;
            }
            else
            {
                this.TBFattSicPisteLat.Background = Brushes.Yellow;
            }

            // Potenze
            this.TBTailTakeUp.Text = Convert.ToString(calcoliImpianto.TakeUpTail);
            this.TBPotRichiesta.Text = Convert.ToString(calcoliImpianto.Pa);
            this.TBPotMotSuggerito.Text = Convert.ToString(this._motore.motorPower);

            // Dimensioni pulegge
            this.TBMinWheelWidth.Text = Convert.ToString(this.bordo.MinWheelWidth);
            this.TBMinDiamDeflection.Text = Convert.ToString(this.bordo.MinWheelDiam);
            this.TBMinPulleyDiam.Text = Convert.ToString(this.bordo.MinPulleyDiam);
        }
       
        private void TBCMaterialType_TextChanged(object sender, TextChangedEventArgs e)
        {
            this._materiale.Nome = this.TBCMaterialType.Text;
        }
        private async void Salva_Click(object sender, RoutedEventArgs e)
        {
            // Prendo tutti i dati di input con binding
            this.nastro.capacityRequired = this.capacityReq;
            this._materiale.surchAngle = this.surAngle;
            this._materiale.fillFactor = this.fillingFactor;
            this._materiale.density = this.matDens;
            this.nastro.forma = this.formaNastro;
            this.nastro.edgetype = this.edgeType;
            this.nastro.elevazione = this.elevNastro;
            this.nastro.centerDistance = this.centDist;
            this.nastro.inclinazione = this.inclNastro;
            this.nastro.speed = this.velNastro;

            try
            {
                // Crea il wrapper del database
                DatabaseSQL database = DatabaseSQL.CreateDefault();

                // Verifico che il numero di offerta non sia presente a database
                this._prodotto.VersioneCodice = 1;

                SqlDataReader reader;
                SqlCommand creaComando = database.CreateDbInputCalcoliCommand();
                reader = creaComando.ExecuteReader();
                while (reader.Read())
                {
                    var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                    int temp1 = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Versione")));
                    if (temp.ToString() == this._prodotto.Codice.ToString() + "_" + this._prodotto.VersioneCodice && Convert.ToInt32(temp1) >= this._prodotto.VersioneCodice)
                    {
                        this._prodotto.VersioneCodice = temp1 + 1;
                    }
                }

                // Chiude la connessione
                database.CloseConnection();

                // Apre la connessione
                database.OpenConnection();

                // Scrivo le informazioni di input del nastro nel db totale
                SqlCommand sqlCommand = database.UpdateDbCommandInputDariNastroCalcoli(this.nastro, this.bordo, this.tazza, this._prodotto, this._materiale);
                sqlCommand.ExecuteNonQuery();

                // Crea il comando di scrittura sul db di input
                sqlCommand = database.UpdateDbCommandInputCalcoli(this.nastro, this.bordo, this.tazza, this._prodotto, this._materiale);
                sqlCommand.ExecuteNonQuery();

                // Crea il comando di scrittura sul db di output
                sqlCommand = database.UpdateDbCommandOutputCalcoli(this.nastro, this.bordo, this.tazza, this._prodotto, this._materiale, this._calcoliImpianto, this._motore);
                sqlCommand.ExecuteNonQuery();

                // Avviso che la configurazione è stata salvata con successo
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Configurazione salvata correttamente", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Ritorna va al database
            //this.NavigationService.Navigate(new DatabaseView());
        }
    }
}
