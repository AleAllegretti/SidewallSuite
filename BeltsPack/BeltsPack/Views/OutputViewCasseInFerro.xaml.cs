using BeltsPack.Models;
using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using LiveCharts;
using LiveCharts.Defaults;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for OutputViewCasseInFerro.xaml
    /// </summary>
    public partial class OutputViewCasseInFerro : Page
    {
        private Nastro _nastro;
        private InputView _inputView;
        private Imballi _imballi;
        private CassaInFerro _cassaInFerro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;
        int NumeroConfigurazione = 0;
        public string configurazioneconveniente { get; set; }
        public ChartValues<ObservablePoint> NastroImballato { get; set; }
        public OutputViewCasseInFerro(Imballi imballi, CassaInFerro cassaInFerro, InputView inputView, Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto)
        {
            this._tazza = tazza;
            this._bordo = bordo;
            this._imballi = imballi;
            this._nastro = nastro;
            this._inputView = inputView;
            this._cassaInFerro = cassaInFerro;
            this._prodotto = prodotto;

            this.DataContext = this;

            // Configurazione più conveniente
            this.configurazioneconveniente = "N° configurazione più ottimizzata: " + this._imballi.NumeroConfigurazione[this._cassaInFerro.IndiceConfConveniente];

            // Inizializza la schermata
            InitializeComponent();

            // Riempie la combobox con il numero delle configurazioni
            int i = 0;
            for (i = 0; i <= this._imballi.NumeroConfigurazione.Length -1; i++)
            {
                if (this._imballi.Fattibilita[i] == true)
                {
                    this.ComboNumeroConfigurazioni.Items.Add(this._imballi.NumeroConfigurazione[i].ToString());
                }
            }

            // Assegna il numero di configurazione in base all'altezza del bordo
            int confPrincipale;
            int confSecondaria;
            int indexConf = 100;
            if (this._bordo.Altezza != 0 && this._tazza.Altezza != 0)
            {
                if (this._bordo.Altezza <= 160)
                {
                    confPrincipale = 1;
                    confSecondaria = 5;
                }
                else
                {
                    confPrincipale = 7;
                    confSecondaria = 6;
                }
            }
            else
            {
                confPrincipale = 6;
                confSecondaria = 3;
            }


            // Controllo se è presente la configurazione principale
            for (i = 0; i <= this._imballi.NumeroConfigurazione.Length - 1; i++)
            {
                if (this._imballi.NumeroConfigurazione[i] == confPrincipale)
                {
                    indexConf = i;
                }
                else if (this._imballi.NumeroConfigurazione[i] == confSecondaria && indexConf == 100)
                {
                    indexConf = i;
                }
            }
            this.ComboNumeroConfigurazioni.Text = this._imballi.NumeroConfigurazione[indexConf].ToString();
            this._cassaInFerro.Configurazione = int.Parse(this.ComboNumeroConfigurazioni.Text.ToString());

            // Assegno il peso del nastro
            this.TBPesoNastro.Text = this._prodotto.PesoTotaleNastro.ToString();

            // Chekko l'immagine del nastro
            this.CKNastro.IsChecked = true;
           }

        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            // Ritorna alla home
            this.NavigationService.Navigate(this._inputView);
        }

        private async void Show2D_Click(object sender, RoutedEventArgs e)
        {
            // Per plottare i dati
            double coordinatatemporanea=0;

            // Discretizza la curva
            double fattorecurva = 1.5;

            // Mostra il pdf solo se il nastro è aperto
            if (this._nastro.Aperto == true)
            {
                // Crea schema pdf
                NastroImballato = new ChartValues<ObservablePoint>();
                for (int i = 0; i<20; i++)
                {
                    // Plot del primo punto di ogni strato
                    NastroImballato.Add(new ObservablePoint(this._imballi.Coordinate[i,1], this._imballi.Coordinate[i,2]));

                    // Plot dei punti intermedi
                    if (i%2==0)
                    {
                        coordinatatemporanea = this._imballi.Coordinate[i, 1] + 150;
                        while (coordinatatemporanea < this._imballi.Coordinate[i + 1, 1])
                        {
                            NastroImballato.Add(new ObservablePoint(coordinatatemporanea, this._imballi.Coordinate[i, 2]));
                            coordinatatemporanea += 150;
                        }
                    }
                    else
                    {
                        coordinatatemporanea = this._imballi.Coordinate[i, 1] - 150;
                        while (coordinatatemporanea > this._imballi.Coordinate[i + 1, 1])
                        {
                            NastroImballato.Add(new ObservablePoint(coordinatatemporanea, this._imballi.Coordinate[i, 2]));
                            coordinatatemporanea -= 150;
                        }
                    }
                    
                    // Plot della curva di polistirolo
                    if (i%2!=0)
                    {
                        while (fattorecurva < 3)
                        {
                            NastroImballato.Add(new ObservablePoint(this._imballi.Coordinate[i, 1] + this._imballi.DiametroPolistirolo / 2 * Math.Cos(Math.PI * fattorecurva),
                                this._imballi.Coordinate[i, 2] + this._imballi.DiametroPolistirolo/2 + this._imballi.DiametroPolistirolo / 2 * Math.Sin(Math.PI * fattorecurva)));

                            // Incrementa il fattore curva
                            fattorecurva += 0.1;
                        }                    
                    }
                    // Plot della curva con corrugato
                    else if (i!=0)
                    {
                        while (fattorecurva > -0.1)
                        {
                            NastroImballato.Add(new ObservablePoint(this._imballi.Coordinate[i, 1] + this._imballi.DiametroCorrugato / 2 * Math.Cos(Math.PI * fattorecurva),
                                this._imballi.Coordinate[i, 2] + this._imballi.DiametroCorrugato/2 + this._imballi.DiametroCorrugato / 2 * Math.Sin(Math.PI * fattorecurva)));

                            // Incrementa il fattore curva
                            fattorecurva -= 0.1;
                        }
                    }

                    // Azzera il fattore curva
                    fattorecurva = 1.5;
                }

            }
            else
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Per i nastri chiusi ad anello l'anteprima pdf non è disponibile.", ConfirmDialog.ButtonConf.OK_ONLY);
            }

        }

        private void TBCostoImballo_Loaded(object sender, RoutedEventArgs e)
        {
            int NumeroConf = 0;

            // Riempie la combobox con il numero delle configurazioni
            for (int i = 0; i <= this._imballi.NumeroConfigurazione.Length - 1; i++)
            {
                if (this._imballi.Fattibilita[i] == true)
                {
                    NumeroConf += 1;
                }
            }

            //// Ci da l'avviso se sono disponibili più configurazioni
            //if (this._imballi.NumeroConfigurazione.Length > 1)
            //{
            //    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Per questo imballo sono disponibili " + NumeroConf + " diverse configurazioni", ConfirmDialog.ButtonConf.OK_ONLY);
            //}
        }

        private void ComboNumeroConfigurazioni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Assegna il numero di configurazione
            this._cassaInFerro.Configurazione = int.Parse(this.ComboNumeroConfigurazioni.SelectedItem.ToString());

            for (int i = 0; i < this._imballi.Lunghezza.Length; i++)
            {
                if (this._imballi.Fattibilita[i] == true && this._imballi.NumeroConfigurazione[i] == int.Parse(this.ComboNumeroConfigurazioni.SelectedItem.ToString()))
                {
                    NumeroConfigurazione = i;
                    // Quando cambio il numero della configurazione vado a cambiare anche tutti i valori calcolati per quella configurazione
                    this.TBTipologiaImballo.Text = this._imballi.Tipologia;
                    this.TBLunghezzaImballo.Text = Convert.ToString(this._imballi.Lunghezza[i]);
                    this.TBLarghezzaImballo.Text = Convert.ToString(this._imballi.Larghezza[i]);
                    this.TBAltezzaImballo.Text = Convert.ToString(this._imballi.Altezza[i]);
                    this.TBCostoImballo.Text = Convert.ToString(this._cassaInFerro.PrezzoCassaFinale[i]);
                    this.TBPesoImballo.Text = Convert.ToString(this._cassaInFerro.PesoFinale[i]);
                    this.TBPesoTotale.Text = Convert.ToString(this._cassaInFerro.PesoFinale[i] + this._prodotto.PesoTotaleNastro);

                    // Cambia l'immagine a seconda della configurazione selezionata
                    this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\Configurazione" + this.ComboNumeroConfigurazioni.SelectedItem.ToString() + ".png", UriKind.Relative));
                    
                    // Se la cassa è maggiore di 6 metri, devo dividerla in 2
                    if (this._imballi.Lunghezza[i] > 6000)
                    {
                        this.TBNotePaladini.Text = "Da dividere in 2 parti";
                    }
                }            
            }

            // Mostro la nota della cassa su fila doppia
            if (this._imballi.Numerofile == 2)
            {
                this.LBCassaDoppia.Visibility = Visibility.Visible;
                this.TBNote.Text = "Cassa doppia";
            }

            // Assegna il numero di configurazione
            this._cassaInFerro.Configurazione = int.Parse(this.ComboNumeroConfigurazioni.SelectedItem.ToString());

        }

        private async void buttonSalvaDb_Click(object sender, RoutedEventArgs e)
        {
            // Crea il wrapper del database
            DatabaseSQL database = DatabaseSQL.CreateDefault();
            SqlCommand creacomando = database.CreateDbTotaleCommand();

            // Salvo il campo note
            this._imballi.Note = this.TBNote.Text;

            // Salvo il campo note per Paladini
            this._imballi.NotePaladini = this.TBNotePaladini.Text;

            // Setto lo stato dell'imballo su offerta
            this._cassaInFerro.Stato = "Offerta";

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

            // Salvo l'immagine della disposizione del nastro a database
            ResourceReader resourcereader = new ResourceReader();
            byte[] binaryDisposizioneNastro = null;
            Bitmap result = new Bitmap(@"Assets\Images\Configurazione" + this._cassaInFerro.Configurazione + ".png");
            string root = @"C:\tmp\out.png";
            if (File.Exists(root) == false)
            {
                // Creo la directory
                Directory.CreateDirectory(@"C:\tmp");
            }
            result.Save(@"C:\tmp\out.png");
            binaryDisposizioneNastro = resourcereader.ImageToBinary(System.Drawing.Image.FromFile(@"C:\tmp\out.png"));
            result.Dispose();

            // Crea il comando di scrittura
            SqlCommand sqlCommand = database.UpdateDbCommandFerro(this._nastro, this._bordo, this._tazza, this._prodotto, this._imballi, NumeroConfigurazione, this._cassaInFerro);
            sqlCommand.ExecuteNonQuery();

            // Scrive il binary dell'immagine
            SqlCommand creacomando4 = database.WriteBinaryCommandImage(this._prodotto.Codice, this._prodotto.VersioneCodice, binaryDisposizioneNastro);
            creacomando4.ExecuteNonQuery();

            // Genero la distinta base e i relativi documenti
            ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Vuoi generare anche l'offerta?", ConfirmDialog.ButtonConf.YES_NO);

            // Naviga al menù principale o resta sulla pagina
            if (confirmed.ToString() == "Yes")
            {
                // Creo la classe distinta
                DiBa distinta = new DiBa(this._nastro, this._bordo, this._tazza, this._prodotto, this._cassaInFerro, NumeroConfigurazione, this._imballi);

                // Larghezza utile
                this._nastro.SetLarghezzautile(this._bordo.Larghezza, this._prodotto.PistaLaterale);
                // Caratteristiche nastro
                this._nastro.SetCaratterisitche();

                // Codice Nastro
                distinta.SearchCodnastro(this._nastro.Tipo, this._nastro.Classe, this._nastro.Larghezza, this._nastro.SiglaTrattamento);
                if (_prodotto.Tipologia == "Bordi e tazze" || _prodotto.Tipologia == "Solo bordi")
                {
                    // Lunghezza bordo
                    this._bordo.SetLunghezzaTotaleBordo(this._nastro.Lunghezza);

                    // Codice Bordo
                    distinta.searchCodBordo(this._bordo.Altezza, this._bordo.Larghezza, this._nastro.SiglaTrattamento);

                    // Raspatura bordo
                    distinta.searchCodRaspaturaBordo("RAB", this._bordo.Altezza, this._bordo.SiglaTrattamento);

                    // Attrezzaggio bordo
                    distinta.SearchCodAttAppBor("ATR", "LAV", _bordo.Altezza);

                    // Applicazione bordo
                    distinta.SearchCodAttAppBor("APB", "LAV", _bordo.Altezza);

                }
                if (this._prodotto.Tipologia == "Solo tazze" | this._prodotto.Tipologia == "Bordi e tazze")
                {
                    // Calcolo il numero e di tazze totali
                    this._tazza.NumeroTazzeTotali(this._nastro.Lunghezza, this._tazza.Passo);
                    // Lunghezza delle tazze
                    this._tazza.SetLunghezzaTotale(this._nastro.LarghezzaUtile);
                    // Caratteristiche
                    this._tazza.CarattersticheTazza();
                    // Raspatura tazze
                    distinta.searchCodRaspaturaTazze("RAT", this._tazza.Altezza, this._bordo.Trattamento, this._tazza.Forma);

                    // Applicazione tazze
                    distinta.searchCodApplicazioneTazze("LAV", "APT", "APT", this._tazza.Altezza, this._tazza.Forma);
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
                        distinta.SearchCodBlk("LIS", "LIS", this._bordo.Altezza, this._bordo.SiglaTrattamento);
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
                    distinta.SearchCodGiunzione("GIU-OFF");
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

        private void buttonMaterialeUtilizzato_Click(object sender, RoutedEventArgs e)
        {
            var view = new CostWeightSummary(this._imballi, this._cassaInFerro, NumeroConfigurazione, this._nastro, this._bordo, this._tazza, this._prodotto);
            DialogHost.Show(view);
        }

        private void GoToDb_Click(object sender, RoutedEventArgs e)
        {
            // Ritorna va al database
            this.NavigationService.Navigate(new DatabaseView());
        }

        private void CKNastro_Checked(object sender, RoutedEventArgs e)
        {
            // Mostra e nasconde le icone
            this.IconaNastro.Kind = PackIconKind.Show;
            this.IconaCassa.Kind = PackIconKind.Hide;
            this.CKCassa.IsChecked = false;

            // Carico l'immagine del nastro
            this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\Configurazione" + this.ComboNumeroConfigurazioni.SelectedItem.ToString() + ".png", UriKind.Relative));

        }

        private void CKCassa_Checked(object sender, RoutedEventArgs e)
        {
            // Mostra e nasconde le icone
            this.IconaCassa.Kind = PackIconKind.Show;
            this.IconaNastro.Kind = PackIconKind.Hide;
            this.CKNastro.IsChecked = false;

            // Carico l'immagine della cassa
            double deltalarghezza;
            deltalarghezza = Convert.ToInt32(this.TBLarghezzaImballo.Text) - this._nastro.Larghezza * 2;

            if (Convert.ToInt32(this.TBLunghezzaImballo.Text) < 8000 & deltalarghezza < 0)
            {
                this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\NoRinforziNoreteLaterale.png", UriKind.Relative));
            }
            else if (Convert.ToInt32(this.TBLunghezzaImballo.Text) > 8000 & this._cassaInFerro.DiagonaliIncrocio == true & deltalarghezza < 0)
            {
                this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\IncrociRinforzi8metri.png", UriKind.Relative));
            }
            else if (Convert.ToInt32(this.TBLunghezzaImballo.Text) > 8000 & this._cassaInFerro.DiagonaliIncrocio == false & deltalarghezza < 0)
            {
                this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\Cassa8MetriBase.jpg", UriKind.Relative));
            }
            else if (Convert.ToInt32(this.TBLunghezzaImballo.Text) < 8000 & deltalarghezza > 0)
            {
                this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\Cassa_Doppia.png", UriKind.Relative));
            }
            else if (Convert.ToInt32(this.TBLunghezzaImballo.Text) > 8000 & deltalarghezza > 0)
            {
                this.imagePack.Source = new BitmapImage(new Uri(@"\Assets\Images\Cassa_Doppia_8_Metri.png", UriKind.Relative));
            }
            

        }
    }
}
