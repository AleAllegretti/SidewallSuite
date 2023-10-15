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

        public AccessoriCasseFerro(Imballi imballi, Nastro nastro, CassaInFerro cassaInFerro, Bordo bordo, Prodotto prodotto, Tazza tazza)
        {
            this._nastro = nastro;
            this._tazza = tazza;
            this._bordo = bordo;
            this._imballi = imballi;
            this._cassaInFerro = cassaInFerro;
            this._prodotto = prodotto;

            this.DataContext = this;

            // Inizializza
            InitializeComponent();

            // Abilito l'opzione dei solo ritti
            if (this._imballi.Lunghezza.Max() <= 6000
                && this._prodotto.ProvenienzaClienteContinente == "EU"
                && this._prodotto.Cliente.ToLower().Contains("italiana gomma") == false)
            {
                // Abilito il checkbox
                this.SoloRitti.IsEnabled = true;
            }

                // Se la SIG seleziono il logo della SIG
                if (this._prodotto.Cliente.ToString().ToLower().Contains("italiana gomma"))
            {
                this.RadioSig.IsChecked = true;
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
            int i = 0;

            // Calcolo il prezzo delle etichette da mettere su ogni gancio
            prezzoetichetteganci = this.InterrogaListinoAccessori("ETICHETTEGANCI", true);
            
            while (i < this._cassaInFerro.IncrociSpalle.Length)
            {
                if (this._cassaInFerro.IncrociSpalle[i] == "Si")
                {
                    this._cassaInFerro.PrezzoManodoperaDiagonali[i] = this.InterrogaListinoPaladini("DIAGONALI", true);
                    this._cassaInFerro.PrezzoManodoperaIncroci[i] = this.InterrogaListinoPaladini("INCROCIO", true);
                }
                else
                {
                    this._cassaInFerro.PrezzoManodoperaDiagonali[i] = 0;
                    this._cassaInFerro.PrezzoManodoperaIncroci[i] = 0;
                }
                i += 1;
            }

            // Tamponatura con rete fianchi - la metto solo se il cliente è la SIG
            if (this._prodotto.Cliente.ToString().ToLower().Contains("italiana gomma"))
            {
                prezzotamponatura = this.InterrogaListinoPaladini("RETE20001000", true);
                this._cassaInFerro.TamponaturaConRete = true;
            }
            else
            {
                prezzotamponatura = 0;
                i = 0;
                while(i < this._cassaInFerro.PrezzoReteTamponatura.Length)
                {
                    this._cassaInFerro.PrezzoReteTamponatura[i] = 0;
                    this._cassaInFerro.PesoReteTamponatura[i] = 0;
                    i += 1;
                }
                this._cassaInFerro.TamponaturaConRete = false;
            }

            // Verniciatura
            if (this._prodotto.Cliente.ToString().ToLower().Contains("italiana gomma") & this._imballi.Lunghezza[0] <= 9000)
            {
                i= 0;
                while (this._imballi.Lunghezza[i] != 0)
                {
                    this._cassaInFerro.PrezzoVerniciatura[i] = this.InterrogaListinoPaladini("VERNICIATURAFINO9", true);
                    this._cassaInFerro.Verniciatura = true;
                    i += 1;
                }
            }
            else if (this._prodotto.Cliente.ToString().ToLower().Contains("italiana gomma") & this._imballi.Lunghezza[0] > 9000)
            {
                i = 0;
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

            // Pannelli sandwich
            if (this.PannelliSandwich.IsChecked == true)
            {
                this._cassaInFerro.PannelliSandwich = true;
 
                // Calcolo il prezzo ed il peso
                i = 0;
                while (this._imballi.Lunghezza.Length - 1 >= i)
                {
                    // Prezzo
                    this._cassaInFerro.PrezzoPannelliSandwich[i] = 
                        Math.Round(this.InterrogaListinoAccessori("PANNSANDWICH", true) * ((this._imballi.Lunghezza[i] * this._imballi.Altezza[i] * 2 + this._imballi.Larghezza[i] * this._imballi.Altezza[i] * 2) * Math.Pow(10, -6)),1) +
                        Math.Round((this.InterrogaListinoPaladini("PANSANDWICH", true) * (this._imballi.Lunghezza[i] * 3 +  this._imballi.Larghezza[i] * 2) * Math.Pow(10, -3)),1) +
                        this.InterrogaListinoCostiGestione("PANSANDWICH", true);

                    // Peso
                    this._cassaInFerro.PesoPannelliSandwich[i] = Math.Round(this.InterrogaListinoAccessori("PANNSANDWICH", false) * 
                        ((this._imballi.Lunghezza[i] * this._imballi.Altezza[i] * 2 + this._imballi.Larghezza[i] * this._imballi.Altezza[i] * 2) * Math.Pow(10, -6)),1);
                    i += 1;
                }
            }
            else
            {
                // Comunico che i pannelli sandwich non sono presenti
                this._cassaInFerro.PannelliSandwich = false;
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


            // Comunico gli accessori nel caso il cliente sia la SIG
            if (this._prodotto.Cliente.ToString().ToLower().Contains("italiana gomma"))
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Verniciatura e tamponatura laterale sono state inserite in automatico per la SIG.", ConfirmDialog.ButtonConf.OK_ONLY);
            }

            if (checkcompletamento)
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
                if(this._imballi.Lunghezza[i] != 0)
                {
                    // Definisco il codice della cassa
                    if (this._imballi.Lunghezza[i] <= 4000 && this._cassaInFerro.PresenzaSoloRitti[i] == "Si")
                    {
                        CodiceGabbia = "CASSA24RITTI";
                    }
                    else if (this._imballi.Lunghezza[i] <= 4000 && this._cassaInFerro.PresenzaSoloRitti[i] == "No")
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
                        ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("La cassa in ferro non rientra in nessuna categoria, contattare l'assistenza.", ConfirmDialog.ButtonConf.OK_ONLY);
                        break;
                    }

                    // Calcolo il prezzo base della cassa
                    this._cassaInFerro.PrezzoCassaSenzaAcc[i] = this.InterrogaListinoPaladini(CodiceGabbia, true);

                    if (this._cassaInFerro.PresenzaGanci[i] == "Si")
                    {
                        // Calcolo il peso dei ganci
                        this._cassaInFerro.PesoGanci = this.InterrogaListinoPaladini("GANCI", false) * 4;

                        // Calcolo il prezzo delle etichette dei ganci
                        this._cassaInFerro.PrezzoEtichetteGanci = this.InterrogaListinoAccessori("ETICHETTEGANCI", true);

                        // Calcolo il peso delle etichette dei ganci
                        this._cassaInFerro.PesoEtichetteGanci = this.InterrogaListinoAccessori("ETICHETTEGANCI", false) * 4;
                    }


                    if (this._imballi.Lunghezza[i] != 0)
                    {
                        // Sommo tutti i costi - Manodopera + materia prima
                        this._cassaInFerro.PrezzoCassaFinale[i] = Math.Round(this._cassaInFerro.PrezzoCassaSenzaAcc[i] +
                            this._cassaInFerro.PrezzoGestioneCassa[i] +
                            this._cassaInFerro.PrezzoManodoperaIncroci[i] +     // Manodopera x incrocio
                            prezzotamponatura +                                 // Manodopera x tamponatura
                            this._cassaInFerro.PrezzoVerniciatura[0] +          // Manodopera x verniciatura
                            this._cassaInFerro.PrezzoGanci +                    // Prezzo x 4 ganci
                            this._cassaInFerro.PrezzoEtichetteGanci +           // Prezzo x 4 etichette ganci
                            this._cassaInFerro.PrezzoManodoperaDiagonali[i] +   // Manodopera x diagonali
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
                            this._cassaInFerro.PrezzoCorrugati[i] +
                            this._cassaInFerro.PrezzoSubbiPolistirolo[i] +
                            this._cassaInFerro.PrezzoIncroci[i] +
                            this._cassaInFerro.PrezzoIncrocioUltimaCampata[i] +
                            this._cassaInFerro.PrezzoPannelliSandwich[i], 2);

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
                            this._cassaInFerro.PesoCorrugati[i] +
                            this._cassaInFerro.PesoSubbiPolistirolo[i] +
                            this._cassaInFerro.PesoIncroci[i] +
                            this._cassaInFerro.PesoIncrocioUltimaCampata[i] +
                            this._cassaInFerro.PesoDiagonaliUltimaCampata[i] +
                            this._cassaInFerro.PesoPannelliSandwich[i] * 1000) * 0.001, 2);

                        // Sommo i pesi dei componenti in plastica
                        this._cassaInFerro.PesoPlastica[i] = this._cassaInFerro.PesoPluriballAlluminio[i] +
                            this._cassaInFerro.PesoCorrugati[i] +
                            this._cassaInFerro.PesoSubbiPolistirolo[i] +
                            this._cassaInFerro.PesoPannelliSandwich[i];
                    }
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

        public double InterrogaListinoCostiGestione(string CodiceAccessorio, bool PresenzaPrezzo)
        {
            double Prezzo = 0;

            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingCostiGestioneCommand();
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
