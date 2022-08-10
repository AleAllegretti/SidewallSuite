using BeltsPack.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using System.Data.SqlClient;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for OutputView.xaml
    /// </summary>
    public partial class OutputView : Page
    {
        public ChartValues<ObservablePoint> NastroImballato { get; set; }
        public string QuotaImballoOrizzontale { get; set; }
        public string QuotaImballoVerticale { get; set; }

        private Nastro _nastro;
        private Imballi _imballi;
        private InputView _inputView;
        private Prodotto _prodotto;
        private Bordo _bordo;
        private Tazza _tazza;
        private CassaInFerro _cassaInFerro;
        public OutputView(Nastro nastro, Imballi imballi, InputView inputView, Prodotto prodotto, Bordo bordo, Tazza tazza)
        {
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._nastro = nastro;
            this._imballi = imballi;
            this._inputView = inputView;

            InitializeComponent();

            // Mostra dimensioni imballo a schermo
            this.TBTipologiaImballo.Text = imballi.Tipologia;
            this.TBLunghezzaImballo.Text = Convert.ToString(imballi.Lunghezza[0]);
            this.TBLarghezzaImballo.Text = Convert.ToString(imballi.Larghezza[0]);
            this.TBPesoImballo.Text = Convert.ToString(imballi.Peso[0]);
            this.TBCostoImballo.Text = Convert.ToString(imballi.Costo[0]);
            this.TBAltezzaImballo.Text = Convert.ToString(imballi.Altezza[0]);
            this.TBPesoNastro.Text = this._prodotto.PesoTotaleNastro.ToString();
            this.TBPesoTotale.Text = Convert.ToString(this._prodotto.PesoTotaleNastro + imballi.Peso[0]);

            // Quote per schema 2D
            QuotaImballoOrizzontale = Convert.ToString(imballi.Lunghezza[0]) + " [mm]";
            QuotaImballoVerticale = Convert.ToString(imballi.Larghezza[0]) + " [mm]";

            // Crea schema pdf
            int Angolo = 0;
            NastroImballato = new ChartValues<ObservablePoint>();
            while (this._imballi.NumeroGiri > Angolo / 360)
            {
                NastroImballato.Add(new ObservablePoint(this._imballi.AParameter * Angolo * Math.PI / 180 * Math.Cos(Angolo * Math.PI / 180), this._imballi.AParameter * Angolo * Math.PI / 180 * Math.Sin(Angolo * Math.PI / 180)));
                Angolo += 5;
            }

            // Mostra il plot
            this.SchemaBobina.Visibility = Visibility.Visible;
            this.QuotaVerticale.Visibility = Visibility.Visible;
            DataContext = this;

            // Mostra le quote
            this.LabelQuotaOrizzontale.Visibility = Visibility.Visible;
            this.LabelQuotaVerticale.Visibility = Visibility.Visible;
            this.QuotaOrizzontale.Visibility = Visibility.Visible;
            this.QuotaVerticale.Visibility = Visibility.Visible;
        }

        private async void Show2D_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Mostra il pdf solo se il nastro è aperto
            if (this._nastro.Aperto == true)
            {
                // Crea schema pdf
                int Angolo = 0;
                NastroImballato = new ChartValues<ObservablePoint>();
                while (this._imballi.NumeroGiri > Angolo / 360)
                {
                    NastroImballato.Add(new ObservablePoint(this._imballi.AParameter * Angolo * Math.PI / 180 * Math.Cos(Angolo * Math.PI / 180), this._imballi.AParameter * Angolo * Math.PI / 180 * Math.Sin(Angolo * Math.PI / 180)));
                    Angolo += 5;
                }

                // Mostra il plot
                this.SchemaBobina.Visibility = Visibility.Visible;
                this.QuotaVerticale.Visibility = Visibility.Visible;
                DataContext = this;

                // Mostra le quote
                this.LabelQuotaOrizzontale.Visibility = Visibility.Visible;
                this.LabelQuotaVerticale.Visibility = Visibility.Visible;
                this.QuotaOrizzontale.Visibility = Visibility.Visible;
                this.QuotaVerticale.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Per i nastri chiusi ad anello l'anteprima pdf non è disponibile.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            
        }

        private async void CreaPDF_Click(object sender, RoutedEventArgs e)
        {
           if (this._imballi.Tipologia == TipologiaImballo.Pedana.ToString())
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Per le pedane in legno il report PDF non è necessario", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }

        private async void buttonSalvaDb_Click(object sender, RoutedEventArgs e)
        {
            // Memorizzo le note che sono state inserite
            this._imballi.Note = this.TBNote.Text;

            // Crea il wrapper del database
            DatabaseSQL database = DatabaseSQL.CreateDefault();

            // Verifico che il numero di offerta non sia presente a database
            this._prodotto.VersioneCodice = 1;
            SqlDataReader reader;
            SqlCommand creaComando = database.CreateDbTotaleCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                int temp1 = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Versione")));
                if (temp.ToString() == this._prodotto.Codice.ToString() && Convert.ToInt32(temp1) >= this._prodotto.VersioneCodice)
                {
                    this._prodotto.VersioneCodice = temp1 + 1;
                }
            }

            // Chiude la connessione
            database.CloseConnection();

            // Apre la connessione
            database.OpenConnection();

            // Crea il comando di scrittura
            SqlCommand sqlCommand = database.UpdateDbCommand(this._nastro, this._bordo, this._tazza, this._prodotto, this._imballi, 0);
            sqlCommand.ExecuteNonQuery();

            // Genero la distinta base e i relativi documenti
            ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Vuoi generare anche l'offerta?", ConfirmDialog.ButtonConf.YES_NO);

            // Naviga al menù principale o resta sulla pagina
            if (confirmed.ToString() == "Yes")
            {
                // Creo la classe distinta
                DiBa distinta = new DiBa(this._nastro, this._bordo, this._tazza, this._prodotto, this._cassaInFerro, 0, this._imballi);

                // Larghezza utile
                this._nastro.SetLarghezzautile(this._bordo.Larghezza, this._prodotto.PistaLaterale);
                // Caratteristiche nastro
                this._nastro.SetCaratterisitche();

                // Codice Nastro
                distinta.SearchCodnastro(this._nastro.Tipo, this._nastro.Classe, this._nastro.Larghezza, 
                    this._nastro.SpessoreSup, "NAS", this._nastro.SiglaTipo, this._nastro.SiglaTrattamento, this._nastro.SpessoreInf,
                    this._nastro.NumTele, this._nastro.NumTessuti);
                if (_prodotto.Tipologia == "Bordi e tazze" || _prodotto.Tipologia == "Solo bordi")
                {
                    // Lunghezza bordo
                    this._bordo.SetLunghezzaTotaleBordo(this._nastro.Lunghezza);

                    // Codice Bordo
                    distinta.searchCodBordo(this._bordo.Altezza, this._bordo.Larghezza, "BOR", this._bordo.SiglaTele, this._bordo.SiglaTrattamento);

                    // Raspatura bordo
                    distinta.searchCodRaspaturaBordo("RAB", this._bordo.Altezza, this._bordo.SiglaTrattamento);

                    // Attrezzaggio bordo
                    distinta.SearchCodAttAppBor("ATT", "LAV", _bordo.Altezza, _prodotto.Tipologia);

                    // Applicazione bordo
                    distinta.SearchCodAppBor("APB", "BOR", _bordo.Altezza, _prodotto.Tipologia);

                }
                if (this._prodotto.Tipologia == "Solo tazze" | this._prodotto.Tipologia == "Bordi e tazze")
                {
                    // Calcolo il numero e di tazze totali
                    this._tazza.NumeroTazzeTotali(this._nastro.Lunghezza, this._tazza.Passo);
                    // Lunghezza delle tazze
                    this._tazza.SetLunghezzaTotale(this._nastro.LarghezzaUtile);
                    // Caratteristiche
                    this._tazza.CarattersticheTazza();
                    // Tazze
                    distinta.searchCodTazza(this._tazza.Altezza, this._nastro.LarghezzaUtile,
                            this._tazza.SiglaTrattamento, this._tazza.SiglaTele, this._tazza.Forma, "LIS");
                    // Raspatura tazze
                    distinta.searchCodRaspaturaTazze("RAL", this._tazza.Altezza, this._bordo.Trattamento, this._tazza.Forma);
                    // Applicazione tazze
                    distinta.searchCodApplicazioneTazze("APP", "LIS", this._tazza.Altezza, this._tazza.Forma, this._tazza.Lunghezza);
                }
                if (this._prodotto.Tipologia == "Solo tazze" | this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo bordi")
                {
                    // Preparazione nastro
                    distinta.SearchCodPrepNastro("NAS", "LAV", this._bordo.Altezza, this._prodotto.Tipologia);
                }
                if (this._prodotto.Tipologia == "Bordi e tazze")
                {
                    if (this._prodotto.PresenzaFix == "Si")
                    {
                        // Fix
                        distinta.SearchCodFix("FIX", "LAV", this._bordo.Altezza);
                    }

                    if (this._prodotto.PresenzaBlinkers == "Si")
                    {
                        // Blk
                        distinta.SearchCodBlk("BLK", this._bordo.Altezza, this._bordo.SiglaTrattamento);
                        // Applicazione blinkers
                        distinta.SearchCodApplicazioneBlk("APPLI-BLINKERS");
                    }

                }

                // Giunzione
                if (this._nastro.Aperto == false)
                {
                    if (this._prodotto.Tipologia == "Solo bordi")
                    {
                        // Aggiungo la giunzione dei bordi
                        distinta.SearchCodGiunzioneBordi("GIUNZIONEBORDI");
                    }

                    // Codice giunzione
                    distinta.SearchCodGiunzione("LAV", "GIU", this._prodotto.AltezzaApplicazioni, this._nastro.Larghezza);
                }

                // Commissioni
                if (this._prodotto.NomeAgente != "")
                {
                    distinta.SearchCodCommissioni(this._prodotto.NomeAgente, "SPESE EXTRA");
                }

                // Inserisco la movimentazione se il nastro è più alto di 280
                if (this._prodotto.AltezzaApplicazioni >= 280)
                {
                    distinta.SearchCodMovimentazione("SPESE EXTRA");
                }

                // Prodotto finito
                distinta.SearchCodProdotto(this._bordo.Altezza, this._prodotto.Tipologia);
                // Imballo
                distinta.SearchCodImballo(this._nastro.Lunghezza);
                // Trasporto
                distinta.SearchCodTrasporto("Trasporto");

                // Crea CSV
                distinta.creaCSV();
            }

            // Ritorna va al database
            this.NavigationService.Navigate(new DatabaseView());
        }
    }
}
