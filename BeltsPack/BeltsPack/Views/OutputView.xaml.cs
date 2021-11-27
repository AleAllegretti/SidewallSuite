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

            // Il salvataggio è avvenuto correttamente
            await DialogsHelper.ShowConfirmDialog("L'imballo è stato salvato correttamente nella versione " + this._prodotto.VersioneCodice + ".", ConfirmDialog.ButtonConf.OK_ONLY);

            // Ritorna va al database
            this.NavigationService.Navigate(new DatabaseView());
        }
    }
}
