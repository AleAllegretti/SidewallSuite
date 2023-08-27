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
    /// Logica di interazione per Calcoli.xaml
    /// </summary>
    public partial class CalcoliView : Page
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
        public CalcoliView(Prodotto prodotto)
        {
            this.prodotto = prodotto;

            InitializeComponent();

            // Abilito il data binding
            this.DataContext = this;

            // Riempio il menù dei clienti
            // ComboClienti.ItemsSource = this.prodotto.ListaClienti().ToArray();
        }

        private void ComboClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.Cliente = this.cliente;
            this.prodotto.OrigineCLiente();
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

        private void TBCodiceArtiolo_TextChanged(object sender, TextChangedEventArgs e)
        {
            prodotto.Codice = this.TBCodiceArtiolo.Text;
        }

        private void Quantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.prodotto.Qty = qtyProdotto;
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
                if (this.prodotto.Tipologia == "Bordi e tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                    //this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                }
                else if (this.prodotto.Tipologia == "Solo tazze")
                {
                    this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
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
                this.prodotto.PistaLaterale = Convert.ToInt32(this.TBPistaLaterale.Text);

                // Calcolo la larghezza di ogni singola tazza
                if (this.tazza.NumeroFile != 0)
                {
                    if (this.prodotto.Tipologia == "Bordi e tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1) - 2 * this.bordo.Larghezza) / this.tazza.NumeroFile;
                        //this.LarghezzaTazza.Text = this.larghezzaTazze.ToString();
                    }
                    else if (this.prodotto.Tipologia == "Solo tazze")
                    {
                        this.larghezzaTazze = (this.nastro.Larghezza - this.prodotto.PistaLaterale * 2 - this.spazioTazzeFileMultiple * (this.tazza.NumeroFile - 1)) / this.tazza.NumeroFile;
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
            this.prodotto.PresenzaFix = this.ComboFix.SelectedValue.ToString();

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
                this.prodotto.PresenzaBlinkers = "No";
                this.ComboFix.SelectedValue = "No";
            }
        }

        private void ComboBlk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determino se ho i blinkers o meno
            this.prodotto.PresenzaBlinkers = this.ComboBlk.SelectedValue.ToString();
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
    }
}
