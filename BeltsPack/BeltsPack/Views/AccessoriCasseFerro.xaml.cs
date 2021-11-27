using BeltsPack.Models;
using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for AccessoriCasseFerro.xaml
    /// </summary>
    public partial class AccessoriCasseFerro : Page
    {
        private Nastro _nastro;
        private InputView _inputView;
        private Imballi _imballi;
        private CassaInFerro _cassaInFerro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;

        public AccessoriCasseFerro(Imballi imballi, InputView inputView, Nastro nastro, CassaInFerro cassaInFerro, Bordo bordo, Prodotto prodotto, Tazza tazza)
        {
            this._nastro = nastro;
            this._inputView = inputView;
            this._tazza = tazza;
            this._bordo = bordo;
            this._imballi = imballi;
            this._cassaInFerro = cassaInFerro;
            this._prodotto = prodotto;

            this.DataContext = this;
            // Inizializza
            InitializeComponent();

            // 
            if(this._imballi.Lunghezza.Max() > 6000)
            {
                this.SoloRitti.IsEnabled = false;
                this.CardRitti.Background = Brushes.LightGray;
            }
        }

        private async void Calcola_Click(object sender, RoutedEventArgs e)
        {
            // Dichiarazione delle variabili per i prezzi
            double prezzoincrocio = 0;
            double prezzotamponatura = 0;
            double prezzovernicitura = 0;
            double prezzoetichetteganci = 0;
            double prezzodiagonali = 0;

            // Calcolo il prezzo delle etichette da mettere su ogni gancio
            prezzoetichetteganci = this.InterrogaListinoAccessori("ETICHETTEGANCI", true);
            
            // Diagonali singole se la cassa è maggiore o uguale di 8 metri
            if (this._imballi.Lunghezza[0] >= 8000)
            {
                prezzodiagonali = this.InterrogaListinoPaladini("DIAGONALI", true);
            }

            //Diagonali ad incrocio
            if (this.DiagonaliIncrocio.IsChecked == true)
            {
                // Calcolo il prezzo della manodopera dal listino di paladini
                prezzoincrocio = this.InterrogaListinoPaladini("INCROCIO", true);

                // Confermo la presenza delle diagonali nella cassa
                this._cassaInFerro.DiagonaliIncrocio = true;
            }
            else
            {
                // Confermo l'assenza di diagonali
                this._cassaInFerro.DiagonaliIncrocio = false;
            }

            // Tamponatura con rete fianchi
            if (this.TamponaturaConRete.IsChecked == true)
            {
                prezzotamponatura = this.InterrogaListinoPaladini("RETE20001000", true);
                this._cassaInFerro.TamponaturaConRete = true;
            }
            else
            {
                prezzotamponatura = 0;
                int i = 0;
                while(this._cassaInFerro.PrezzoReteTamponatura[i] != 0)
                {
                    this._cassaInFerro.PrezzoReteTamponatura[i] = 0;
                    this._cassaInFerro.PesoReteTamponatura[i] = 0;
                    i += 1;
                }
                this._cassaInFerro.TamponaturaConRete = false;
            }

            // Verniciatura
            if (this.Verniciatura.IsChecked == true & this._imballi.Lunghezza[0] <= 9000)
            {
                int i = 0;
                while (this._imballi.Lunghezza[i] != 0)
                {
                    this._cassaInFerro.PrezzoVerniciatura[i] = this.InterrogaListinoPaladini("VERNICIATURAFINO9", true);
                    this._cassaInFerro.Verniciatura = true;
                    i += 1;
                }
            }
            else if (this.Verniciatura.IsChecked == true & this._imballi.Lunghezza[0] > 9000)
            {
                int i = 0;
                while (this._imballi.Lunghezza[i] != 0)
                {
                    this._cassaInFerro.PrezzoVerniciatura[i] = this.InterrogaListinoPaladini("VERNICIATURAOLTRE9", true);
                    this._cassaInFerro.Verniciatura = true;
                    i += 1;
                }
            }
            else
            {
                this._cassaInFerro.Verniciatura = false;
            }

            // Fondo in lamiera
            if (this.FondoInLamiera.IsChecked == true)
            {
                this._cassaInFerro.FondoLamiera = true;
            }
            else
            {
                this._cassaInFerro.FondoLamiera = false;
            }

            // Solo ritti
            if (this.SoloRitti.IsChecked == true)
            {
                int i = 0;
                while (this._cassaInFerro.PrezzoReteTamponatura[i] != 0)
                {
                    this._cassaInFerro.PrezzoLongheroni[i] = this._cassaInFerro.PrezzoLongheroni[i] / 2;
                    this._cassaInFerro.PesoLongheroni[i] = this._cassaInFerro.PesoLongheroni[i] / 2;
                    i += 1;
                }
                    
                this._cassaInFerro.SoloRitti = true;
            }
            else
            {
                this._cassaInFerro.SoloRitti = false;
            }

            // Check completamento
            bool checkcompletamento = false;

            // Determino il tipo di personalizzazione
            if(this.RadioSidewall.IsChecked.Value == true)
            {
                // Assegna la personalizzazione
                this._cassaInFerro.Personalizzazione = "Sidewall";
                checkcompletamento = true;
            }
            else if(this.RadioSig.IsChecked.Value == true)
            {
                // Assegna la personalizzazione
                this._cassaInFerro.Personalizzazione = "SIG";
                checkcompletamento = true;
            }
            else if(this.RadioPersonalizzato.IsChecked.Value == true)
            {
                // Assegna la personalizzazione
                this._cassaInFerro.Personalizzazione = "Personalizzato";
                checkcompletamento = true;
            }
            else if (this.RadioAnonimo.IsChecked.Value == true)
            {
                // Assegna la personalizzazione
                this._cassaInFerro.Personalizzazione = "Anonimo";
                checkcompletamento = true;
            }
            else 
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Assicurati di aver inserito il tipo di personalizazione", ConfirmDialog.ButtonConf.OK_ONLY);
                checkcompletamento = false;
            }
            
            if(checkcompletamento)
            {
                // Calcolo il prezzo finale della cassa
                this.CalcoloPrezzoFinale(prezzoincrocio, prezzotamponatura, prezzovernicitura, prezzoetichetteganci, prezzodiagonali);

                // Naviga alla finestra di output
                this.NavigationService.Navigate(new OutputViewCasseInFerro(this._imballi, this._cassaInFerro, this._inputView, this._nastro, this._bordo, this._tazza, this._prodotto));
            }
            
        }

        private async void CalcoloPrezzoFinale(double prezzoincrocio, double prezzotamponatura, double prezzoverniciatura, double prezzoetichetteganci,
            double prezzodiagonali)
        {
            // Variabili
            string CodiceGabbia = "";
            double prezzocassasenzacc = 0;

            for (int i = 0; i < this._imballi.Lunghezza.Length; i++)
            {
                // Definisco il codice della cassa
                if (this._imballi.Lunghezza[i] <= 4000 && this._cassaInFerro.SoloRitti == true)
                {
                    CodiceGabbia = "CASSA24RITTI";
                }
                else if (this._imballi.Lunghezza[i] <= 4000 && this._cassaInFerro.SoloRitti == false)
                {
                    CodiceGabbia = "CASSA24SPALLE";
                }
                else if (this._imballi.Lunghezza[i] > 4000 && this._imballi.Lunghezza[i] <= 6000)
                {
                    CodiceGabbia = "CASSA46SPALLE";
                }
                else if (this._imballi.Lunghezza[i] > 6000 && this._imballi.Lunghezza[i] <= 9000)
                {
                    CodiceGabbia = "CASSA69SPALLE";
                }
                else if (this._imballi.Lunghezza[i] > 9000 && this._imballi.Lunghezza[i] <= 13600)
                {
                    CodiceGabbia = "CASSA69SPALLE";
                }
                else
                {
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("La cassa in ferro non rientra in nessuna categoria, conttatare l'assistenza.", ConfirmDialog.ButtonConf.OK_ONLY);
                }

                // Calcolo il prezzo base della cassa
                prezzocassasenzacc = this.InterrogaListinoPaladini(CodiceGabbia, true);

                // Calcolo il prezzo dei ganci
                this._cassaInFerro.PrezzoGanci = this.InterrogaListinoPaladini("GANCI", true);

                // Calcolo il peso dei ganci
                this._cassaInFerro.PesoGanci = this.InterrogaListinoPaladini("GANCI", false) * 4;

                // Calcolo il prezzo delle etichette dei ganci
                this._cassaInFerro.PrezzoEtichetteGanci = this.InterrogaListinoAccessori("ETICHETTEGANCI", true);

                // Calcolo il peso delle etichette dei ganci
                this._cassaInFerro.PesoEtichetteGanci = this.InterrogaListinoAccessori("ETICHETTEGANCI", false) * 4;

                if (this._imballi.Lunghezza[i] != 0)
                {
                    // Sommo tutti i costi - Manodopera + materia prima
                    this._cassaInFerro.PrezzoCassaFinale[i] = Math.Round(prezzocassasenzacc +
                        prezzoincrocio +                            // Manodopera x incrocio
                        prezzotamponatura +                         // Manodopera x tamponatura
                        this._cassaInFerro.PrezzoVerniciatura[0] +  // Manodopera x verniciatura
                        this._cassaInFerro.PrezzoGanci +            // Prezzo x 4 ganci
                        this._cassaInFerro.PrezzoEtichetteGanci +   // Prezzo x 4 etichette ganci
                        prezzodiagonali +                           // Manodopera x diagonali
                        this._cassaInFerro.PrezzoLongheroni[i] +
                        this._cassaInFerro.PrezzoLongheroniRinforzo[i] +
                        this._cassaInFerro.PrezzoTraversiniBase[i] +
                        this._cassaInFerro.PrezzoRitti[i] +
                        this._cassaInFerro.PrezzoReteTamponaturaBase[i] +
                        this._cassaInFerro.PrezzoDiagonali[i] +
                        this._cassaInFerro.PrezzoDiagonaliUltimaCampata[i] +
                        this._cassaInFerro.PrezzoTraversiniSuperiori[i] +
                        this._cassaInFerro.PrezzoReteTamponatura[i] +
                        this._cassaInFerro.PrezzoPluriballAlluminio[i] +
                        this._cassaInFerro.PrezzoIncroci[i] +
                        this._cassaInFerro.PrezzoIncrocioUltimaCampata[i], 2);

                    // Sommo tutti i pesi
                    this._cassaInFerro.PesoFinale[i] = Math.Round((this._cassaInFerro.PesoLongheroni[i] +
                        this._cassaInFerro.PesoLongheroniRinforzo[i] +
                        this._cassaInFerro.PesoDiagonali[i] +
                        this._cassaInFerro.PesoReteTamponatura[i] +
                        this._cassaInFerro.PesoReteTamponaturaBase[i] +
                        this._cassaInFerro.PesoRitti[i] +
                        this._cassaInFerro.PesoTraversiniBase[i] +
                        this._cassaInFerro.PesoTraversiniSuperiori[i] +
                        this._cassaInFerro.PesoGanci +
                        this._cassaInFerro.PesoEtichetteGanci +
                        this._cassaInFerro.PesoPluriballAlluminio[i] +
                        this._cassaInFerro.PesoIncroci[i] +
                        this._cassaInFerro.PesoIncrocioUltimaCampata[i] +
                        this._cassaInFerro.PesoDiagonaliUltimaCampata[i]) * 0.001, 2);
                }                
            }
            // Trovo la configurazione con peso minore
           this.GetConfigurazioneConveniente();
        }

        private void GetConfigurazioneConveniente()
        {
            double min = this._cassaInFerro.PrezzoCassaFinale.Max();

            for (int i = 0; i < this._cassaInFerro.PrezzoCassaFinale.Length; i++)
            { 
                if(this._cassaInFerro.PrezzoCassaFinale[i] != 0 & this._cassaInFerro.PrezzoCassaFinale[i] <= min)
                {
                    this._cassaInFerro.IndiceConfConveniente = i;
                    min = this._cassaInFerro.PrezzoCassaFinale[i];
                }
            }

           
        }
        private double  InterrogaListinoPaladini(string CodiceAccessorio, bool PresenzaPrezzo)
        {
            double Output = 0;

            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ConsultaListinoPaladini();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == CodiceAccessorio)
                {
                    if (PresenzaPrezzo == true)
                    {
                        // Prezzo
                        Output = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo")));
                    }
                    else
                    {
                        // Peso
                        Output = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso")));
                    }
                }
            }

            // Output
            return Output;
        }

        public double InterrogaListinoAccessori(string CodiceAccessorio, bool PresenzaPrezzo)
        {
            double Prezzo = 0;

            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingAccessoriCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == CodiceAccessorio)
                {
                    if (PresenzaPrezzo == true)
                    {
                        // Prezzo
                        Prezzo = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo")));
                    }
                    else
                    {
                        // Peso
                        Prezzo = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso")));
                    }
                }
            }

            // Output
            return Prezzo;
        }

        private async void SoloRitti_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this._imballi.Lunghezza.Length; i++)
            {
                if (this.SoloRitti.IsChecked == true && this._imballi.Lunghezza[i] > 4000)
                {
                    // Mostra il messaggio di avviso
                    ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("La cassa con 'solo i ritti' è prevista per lunghezze minori o uguali di 4 [m].", ConfirmDialog.ButtonConf.OK_ONLY);

                    // Cambia lo stato della presenza dei ritti a false
                    this.SoloRitti.IsChecked = false;
                    this._cassaInFerro.SoloRitti = false;
                }
            }
                     
        }

        private void RadioSidewall_Checked(object sender, RoutedEventArgs e)
        {
            this.logo.Source = new BitmapImage(new Uri(@"\Assets\Images\LOGO_SIDEWALL.png", UriKind.Relative));
        }

        private void RadioSig_Checked(object sender, RoutedEventArgs e)
        {
            this.logo.Source = new BitmapImage(new Uri(@"\Assets\Images\Logo_Sig.png", UriKind.Relative));
        }

        private void RadioPersonalizzato_Checked(object sender, RoutedEventArgs e)
        {
            this.logo.Source = new BitmapImage(new Uri(@"\Assets\Images\Customization_Logo.png", UriKind.Relative));
        }

        private void RadioAnonimo_Checked(object sender, RoutedEventArgs e)
        {
            this.logo.Source = new BitmapImage(new Uri(@"\Assets\Images\Anonimo_Logo.png", UriKind.Relative));
        }
    }
}
