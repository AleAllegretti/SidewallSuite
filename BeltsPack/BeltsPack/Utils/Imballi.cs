using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeltsPack.Utils;
using System.Data.SqlClient;
using System.Data.OleDb;
using BeltsPack.Views.Dialogs;
using System.Diagnostics;
using System.Globalization;
using Syncfusion.XPS;
using BeltsPack.Views;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace BeltsPack.Models
{
    enum TipologiaImballo
    {
        Pedana,
            Cassa
    }

    public class Imballi
    {
        public bool AvvisoImballoCritico { get; set; }
        public double LimiteLunghezzaCassaDoppia { get; set; }
        public bool PriorityCassaDoppia { get; set; }
        public int Numerofile { get; set; }
        public int TolleranzaLunghezza { get; set; }
        public int TolleranzaCassaDoppia { get; set; }
        public int LimiteOpenTop { get; set; }
        public int LimiteCassaStd { get; set; }
        public int LimiteHighCube { get; set; }
        public int TolleranzaLarghezza { get; set; }
        public int LimiteLunghezzaNastroCassaLegno { get; set; }
        public string CodiceCorrugato { get; set; }
        public int LunghezzaCorrugato { get; set; }
        public int PrezzoCorrugato { get; set; }
        public int DiametroCorrugato { get; set; }
        public int DiametroPolistirolo { get; set; }
        public string CodicePolistirolo { get; set; }
        public int SpessorePolistirolo { get; set; }
        public double[,] Coordinate { get; set; }
        public bool SfalsamentoCurvaPolistirolo { get; set; }
        public double LunghezzaCurvaEsterna{ get; set; }
        public double LunghezzaCurvaInterna { get; set; }
        public double AParameter { get; set; }
        public string Tipologia { get; set; } = "";
        public double[] Lunghezza { get; set; }
        public double[] Larghezza { get; set; }
        public double[] Altezza { get; set; }
        public double[] Peso { get; set; }
        public bool[] Fattibilita { get; set; }
        public double[] Costo { get; set; }
        public int[] NumeroConfigurazione { get; set; }
        public double NumeroGiri { get; set; }
        public double AltezzaPedana { get; set; }
        public  bool ImballoCalcolabile { get; set; }
        public string[] Criticita { get; set; }
        public double[] NumeroCurvePolistirolo { get; set; }
        public double[] NumeroCurveCorrugati { get; set; }
        public string Note { get; set; }
        public string NotePaladini { get; set; }
        public int itrasporto { get; set; }
        public int cassaFattibile { get; set; }

        // Oggetti
        private Nastro _nastro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;
        private CassaInFerro _cassainferro;

        // Variabili
        double[,] Strati = new double[30, 3];
        int contatorestrati = 1;
        double altezzacorrugati = 0;        // Altezza della parte dei corrugati per il calcolo dell'altezza massima della cassa
        double altezzapolistirolo = 0;      // Altezza della parte di polistirolo per il calcolo dell'altezza massima della cassa
        double altezzanastroimballato = 0;  // Ci dice quanto è alto il nastro fino ad ora imballato
        int ContatoreConfigurazioni = 0;    // Ci serve nei vettori altezza, larghezza e lunghezza per tenere conto delle configurazioni
        int i;
        int j;
        double altezzaNastroImballatoFinale = 0;
        public Imballi(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto, CassaInFerro cassaInFerro)
        {
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._cassainferro = cassaInFerro;
            this._cassainferro.PesoDiagonali = new double[10];
            this._cassainferro.PrezzoDiagonali = new double[10];
            this._cassainferro.PesoFinale = new double[10];
            this._cassainferro.PesoPlastica = new double[10];
            this._cassainferro.PrezzoCassaFinale = new double[10];
            this._cassainferro.PrezzoCassaSenzaAcc = new double[10];
            this._cassainferro.PesoLongheroni = new double[10];
            this._cassainferro.PrezzoLongheroni = new double[10];
            this._cassainferro.PesoReteTamponatura = new double[10];
            this._cassainferro.PrezzoReteTamponatura = new double[10];
            this._cassainferro.PrezzoManodoperaDiagonali = new double[10];
            this._cassainferro.PrezzoManodoperaIncroci = new double[10];
            this._cassainferro.PesoReteTamponaturaBase = new double[10];
            this._cassainferro.PrezzoReteTamponaturaBase = new double[10];
            this._cassainferro.PesoRitti = new double[10];
            this._cassainferro.PrezzoRitti = new double[10];
            this._cassainferro.PesoTraversiniBase = new double[10];
            this._cassainferro.PrezzoTraversiniBase = new double[10];
            this._cassainferro.PesoTraversiniSuperiori = new double[10];
            this._cassainferro.PrezzoTraversiniSuperiori = new double[10];
            this._cassainferro.LunghezzaDiagonaleUltimaCampata = new double[10];
            this._cassainferro.LunghezzaDiagonali = new double[10];
            this._cassainferro.LunghezzaDiagonaleUltimaCampata = new double[10];
            this._cassainferro.PesoDiagonaliUltimaCampata = new double[10];
            this._cassainferro.PesoIncrocioUltimaCampata = new double[10];
            this._cassainferro.PrezzoDiagonaliUltimaCampata = new double[10];
            this._cassainferro.PrezzoIncrocioUltimaCampata = new double[10];
            this._cassainferro.LunghezzaUltimaCampata = new double[10];
            this._cassainferro.NumeroDiagonali = new int[10];
            this._cassainferro.NumeroRitti = new int[10];
            this._cassainferro.PesoIncroci = new double[10];
            this._cassainferro.PrezzoIncroci = new double[10];
            this._cassainferro.PrezzoVerniciatura = new double[10];
            this._cassainferro.PrezzoLongheroniRinforzo = new double[10];
            this._cassainferro.PesoLongheroniRinforzo = new double[10];
            this._cassainferro.PesoPluriballAlluminio = new double[10];
            this._cassainferro.PrezzoPluriballAlluminio = new double[10];
            this._cassainferro.PesoCorrugati = new double[10];
            this._cassainferro.PrezzoCorrugati = new double[10];
            this._cassainferro.PesoSubbiPolistirolo = new double[10];
            this._cassainferro.PrezzoSubbiPolistirolo = new double[10];
            this._cassainferro.NumeroTraversiniBase = new int[10];
            this._cassainferro.LimiteAltezza = new double[10];
            this._cassainferro.LimiteLarghezza = new double[10];
            this._cassainferro.LimiteLunghezza = new double[10];
            this._cassainferro.PrezzoGestioneCassa = new double[10];
            this._cassainferro.TipoTrasporto = new string[10];
            this._cassainferro.FattibilitaTrasporto = new bool[10];
            this._cassainferro.FattibilitaCamion = new bool[10];
            this._cassainferro.FattibilitaNave = new bool[10];
            this._cassainferro.TrasportoDefault = new string[10];
            this._cassainferro.TipologiaCassa = new string[10];
            this._cassainferro.PresenzaGanci = new string[10];
            this._cassainferro.IncrociSpalle = new string[10];
            this._cassainferro.PresenzaSoloRitti = new string[10];
            this._cassainferro.PrezzoPannelliSandwich = new double[10];
            this._cassainferro.PesoPannelliSandwich = new double[10];

            this.Lunghezza = new double[10];
            this.Larghezza = new double[10];
            this.Altezza = new double[10];
            this.Fattibilita = new bool[10];
            this.Peso = new double[10];
            this.Costo = new double[10];
            this.NumeroConfigurazione = new int[10];
            this.Criticita = new string[10];
            this._nastro.Spessore = 15;
            this.NumeroCurvePolistirolo = new double[10];
            this.NumeroCurveCorrugati = new double[10];
            this.PriorityCassaDoppia = false;

            // Calcola altezza anstro
            prodotto.Altezza=  this.CalcoloAltezzaNastro(bordo.Altezza, tazza.Altezza, _nastro.Spessore);

            // Calcola lunghezza spirale
            this.SpiraleArchimedea(prodotto.Altezza);

            // Stabilisco tutte le tolleranze e impostazioni
            this.ImpostazioniImballi();

            // Calcola dimensioni imballo
            this.DimensioniImballo();

            // Mi serve per determinare se ho già avvisato che l'imballo è critico
            this.AvvisoImballoCritico = false;
        }
        public void ImpostazioniImballi()
        {
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingParametriCommand();
            reader = creaComando.ExecuteReader();

            // Costo e peso dei longheroni e traversini
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Elemento"));
                if (temp.ToString() == "TolleranzaLarghezza")
                {
                    this.TolleranzaLarghezza = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                else if (temp.ToString() == "MargineDiSicurezzaLunghezza")
                {
                    this.TolleranzaLunghezza = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                else if (temp.ToString() == "ArrotondamentoAltezza")
                {
                    this.LimiteCassaStd = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                else if(temp.ToString() == "ArrotondamentoAltezzaOpenTop")
                {
                    this.LimiteOpenTop = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                else if (temp.ToString() == "SpazioTraFileCassaDoppia")
                {
                    this.TolleranzaCassaDoppia = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                else if (temp.ToString() == "ArrotondamentoHighCube")
                {
                    this.LimiteHighCube = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                else if (temp.ToString() == "LimitePedanaLegno")
                {
                    this.LimiteLunghezzaNastroCassaLegno = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
            }

            reader.Close();
        }
        public void SpiraleArchimedea(double AltezzaNastro)
        {
             this.CalcoloAParameter(AltezzaNastro);
            double LunghezzaSpirale =0;
            double Tolleranza = 0;

            // Calcolo la lunghezza della spirale
            while (_nastro.Lunghezza + Tolleranza >= LunghezzaSpirale)
            {
                LunghezzaSpirale = 0.5 * this.AParameter * (2 * NumeroGiri * Math.PI * Math.Sqrt(1 + Math.Pow(2 * Math.PI * NumeroGiri, 2)) + Math.Log(2 * Math.PI * NumeroGiri + Math.Sqrt(1 + Math.Pow(2 * Math.PI * NumeroGiri, 2))));
                this.NumeroGiri += 0.1;
            }
        }

        public void CalcoloAParameter(double AltezzaNastro)
        {

            // Calcolo del parametro A della spirale
            if (this._nastro.Aperto)
            {
                this.AParameter = (1.6 * AltezzaNastro) / (2 * Math.PI);
            }
            else
            {
                this.AParameter = (AltezzaNastro) / (2 * Math.PI);
            } 

        }

        public double CalcoloAltezzaNastro(int AltezzaBordo, int AltezzaTazze, int SpessoreNastro)
        {
            double AltezzaNastro = 0;

            // Considero il massimo tra altezza bordo e tazze
            AltezzaNastro = Math.Max(AltezzaBordo, AltezzaTazze) + SpessoreNastro;
            
            // Se non ci sono bordi e tazze, considero lo spessore del nastro
            if (AltezzaNastro == 0) 
            {
                AltezzaNastro = SpessoreNastro+20;
            }

            // Output
            return AltezzaNastro;
        }

        public void DimensioniImballo()
        {

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Dimensioni della pedana
            this.Larghezza[0] = (this.NumeroGiri*1.15) * CalcoloAltezzaNastro(this._bordo.Altezza, this._tazza.Altezza, this._nastro.Spessore) * 2;

            // Stabilisco il limite in base al fatto che il nastro sia aperto o chiuso
            double limiteLarghezza = 2400;
            if (this._nastro.Aperto == false)
            {
                limiteLarghezza = 2400 / 1.15;
            }
            if (this.Larghezza[0] <= limiteLarghezza && this.LimiteLunghezzaNastroCassaLegno >= this._nastro.Lunghezza)
            {

                this.Lunghezza[0] = this.Larghezza[0];
                this.Altezza[0] = this._nastro.Larghezza + this.AltezzaPedana;

                // Crea il comando SQL
                SqlDataReader reader;
                SqlCommand creaComando = dbSQL.CreateImballiLegnoCommand();
                reader = creaComando.ExecuteReader();
                while (reader.Read())
                {
                    var temp = reader.GetValue(reader.GetOrdinal("Larghezza"));
                    if (Convert.ToInt32(temp) >= this.Larghezza[0])
                    {
                        this.Larghezza[0] = Convert.ToInt32(temp);
                        this.Lunghezza[0] = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Lunghezza")));
                        this.AltezzaPedana = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza")));
                        this.Peso[0] = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Peso")));
                        this.Costo[0] = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Costo")));
                        this.Altezza[0] = _nastro.Larghezza + this.AltezzaPedana;
                        this.Tipologia = TipologiaImballo.Pedana.ToString();
                        break;
                    }
                }
            }
            else
            {
                // Inizializzo il numero di file
                this.Numerofile = 1;

                // Diametro disco polistirolo e corrugato
                this.GetDiametroCorrugato_Polistirolo();

                // Lunghezza curva esterna
                this.CalcolaLunghezzaCurvaEsterna();

                // Lunghezza curva interna
                this.CalcolaLunghezzaCurvaInterna();

                // Sfalsamento curva interna
                this.SfalsamentoCurvaInterna();

                // Stabilisce il tipo di imballo
                this.Tipologia = TipologiaImballo.Cassa.ToString();

                // Calcola le dimensioni della cassa in ferro
                this.CalcolaDimensioniCassaInFerro();
            }

        }

        public string GetTipologiaImballo()
        {
            return TipologiaImballo.Cassa.ToString();
        }

        public void GetDiametroCorrugato_Polistirolo()
        {
            if (this._bordo.Altezza <= 120)
            {
                this.DiametroCorrugato = 250;
                this.CodiceCorrugato = "TUBO250";
                this.DiametroPolistirolo = 350;
                this.SpessorePolistirolo = 60;
                this.CodicePolistirolo = "POLISTIROLO350";
            }
            else if (this._bordo.Altezza == 160 || this._bordo.Altezza == 140)
            {
                this.DiametroCorrugato = 315;
                this.CodiceCorrugato = "TUBO315";
                this.DiametroPolistirolo = 450;
                this.SpessorePolistirolo = 80;
                this.CodicePolistirolo = "POLISTIROLO450";
            }
            else if (this._bordo.Altezza >= 200 && this._bordo.Altezza <= 250)
            {
                this.DiametroCorrugato = 400;
                this.CodiceCorrugato = "TUBO400";
                this.DiametroPolistirolo = 650;
                this.SpessorePolistirolo = 100;
                this.CodicePolistirolo = "POLISTIROLO650";
            }
            else if (this._bordo.Altezza >= 260 && this._bordo.Altezza <= 300)
            {
                this.DiametroCorrugato = 630;
                this.CodiceCorrugato = "TUBO630";
                this.DiametroPolistirolo = 900;
                this.SpessorePolistirolo = 120;
                this.CodicePolistirolo = "POLISTIROLO900";
            }
            else if (this._bordo.Altezza == 400)
            {
                this.DiametroCorrugato = 800;
                this.CodiceCorrugato = "TUBO800";
                this.DiametroPolistirolo = 1100;
                this.SpessorePolistirolo = 150;
                this.CodicePolistirolo = "POLISTIROLO1100";
            }
        }

        public void CalcolaLunghezzaCurvaEsterna()
        {
            // Nella curva esterna utilizziamo i 3/4 di circonferenza
            this.LunghezzaCurvaEsterna = this.DiametroCorrugato * Math.PI * 3 / 4;
        }

        public void CalcolaLunghezzaCurvaInterna()
        {
            this.LunghezzaCurvaInterna = this.DiametroPolistirolo * Math.PI * 0.5;
        }

        public void StabilisceLimitiImballo()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            int i = 0;

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingLimitiTrasportoCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._cassainferro.LimiteAltezza[i] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Altezza"))) * 1000;
                this._cassainferro.LimiteLarghezza[i] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Larghezza"))) * 1000;
                this._cassainferro.LimiteLunghezza[i] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Lunghezza"))) * 1000;
                this._cassainferro.TipoTrasporto[i] = reader.GetValue(reader.GetOrdinal("Trasporto")).ToString();

                if (Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Camion"))) == 1 && 
                    Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Nave"))) == 1)
                {
                    this._cassainferro.FattibilitaCamion[i] = true;
                    this._cassainferro.FattibilitaNave[i] = true;
                }
                else 
                {
                    this._cassainferro.FattibilitaCamion[i] = true;
                    this._cassainferro.FattibilitaNave[i] = false;
                }

                i++;
            }
        }

        public void CalcolaDimensioniCassaInFerro()
        {
            // Fattore per il calcolo dell'altezza del nastro.
            // Se il nastro è solo tazze o solo bordi metto i nastri uno dentro l'altro
            // quindi l'altezza si dimezza e il fattore è uguale a due,
            // Se il nastro è solo bordi con piste laterali pari a zero aumento la larghezza di 2 volte la larghezza del bordo.
            if (this._tazza.Altezza == 0 || this._bordo.Altezza == 0 || this._tazza.Forma == "TCW" || this._tazza.Forma == "TW")
            {
                this._prodotto.FattoreAltezza = 2;
            }
            else
            {
                this._prodotto.FattoreAltezza = 1;
            }

            // Se il nastro è solo bordi devo considerare una tolleranza di larghezza perchè
            // vado a mettere gli strati uno dentro l'altro
            if (this._tazza.Altezza == 0)
            {
                this._prodotto.LarghezzaAggSoloBordi = this._bordo.Larghezza * 2;
            }
            else
            {
                this._prodotto.LarghezzaAggSoloBordi = 0;
            }
            
            // Calcolo l'altezza delle applicazioni
            this._prodotto.AltezzaApplicazioni = Math.Max(this._bordo.Altezza, this._tazza.Altezza);

            // Stabilisce la lunghezza iniziale per il calcolo
            this._cassainferro.LunghezzaIniziale = 3000;

            // Calcola le dimensioni della cassa
            this._nastro.LunghezzaImballato = 0;

            // Stabilisce i limiti dell'imballo
            this.StabilisceLimitiImballo();

            // Verifico se è possibile disporre il nastro in doppia fila
            this.PossibilitaCassaDoppia();

            if (this._prodotto.AltezzaApplicazioni == 160 || 
                this._prodotto.AltezzaApplicazioni == 120 ||
                this._prodotto.AltezzaApplicazioni == 80 ||
                this._prodotto.AltezzaApplicazioni == 140)
            {
                // CONFGURAZIONE 7 - Configurazione principale               
                this.CalcolaImballoConfigurazione7();

            }
           else if (this._prodotto.AltezzaApplicazioni <= 120)
            {
                // CONFIGURAZIONE 1
                this.CalcolaImballoConfigurazione1();
            }
            else
            {
                // CONFIGURAZIONE 6
                this.CalcolaImballoConfigurazione6();
            }

            // Controlla se c'è almeno una configurazione valida
            this.ControllaFattibilitaTotale();
        }

        private double[,] CalcolaCoordinatePunti(double[,] strati)
        {
            // Coordinate dei punti [Numero punto, x, y]
            double[,] Coordinate = new double[100, 3];

            // Coordinate origine se il nastro è aperto
            Coordinate[0, 0] = 0;
            Coordinate[0, 1] = 0;
            Coordinate[0, 2] = 0;

            // Contatori
            int j = 0;

            // Gap tra uno strato e l'altro
            double GapStrati = strati[0, 1] - strati[1, 1];

            // Inizializza il vettore coordinate
            Coordinate[1, 0] = 1;
            Coordinate[1, 1] = strati[0, 1];
            Coordinate[1, 2] = strati[0, 2];

            // Riempie il vettore coordinate
            for (int i = 2; i < strati.Length*2; i++)
            {
                // Stabilisce il numero del punto
                Coordinate[i, j] = i;
                j += 1;

                // Stabilisce l'ascissa
                if(i!=1 && i % 2!=0)
                {
                    Coordinate[i, j] = Coordinate[i-2,j];
                }
                else
                {
                    Coordinate[i, j] = GapStrati*(i/2);
                }
           
                j += 1;

                // Stabilisce l'ordinata
                if(i%2==0)
                {
                    Coordinate[i, j] = this._bordo.Altezza *(i);
                }
                else
                {
                    Coordinate[i, j] = Coordinate[i - 1, j]+80;
                }

                // Resetta il contatore delle colonne
                j = 0;
            }
            // Output
            return Coordinate;
        }
        private void SfalsamentoCurvaInterna()
        {
            // Ci dice se tra uno strato e l'altro la curva interna viene sfalsata o vengono poste in verticale
            if ((this.DiametroPolistirolo - this._prodotto.AltezzaApplicazioni * 2)<300)
            {
                this.SfalsamentoCurvaPolistirolo = false;
            }
            else
            {
                this.SfalsamentoCurvaPolistirolo = true;
            }
        }
        private void CalcolaPesoCassa()
        {
            // Variabili locali
            double pesoLongh = 0;
            double prezzoLongh = 0;

            // Calcolo lunghezza dell'ultima campata ipotizzando le altre lunghe 2 metri
            this.LunghezzaUltimaCampata();

            // Calcola il peso dei due longheroni principali
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CostoCassaFerroCommand();
            reader = creaComando.ExecuteReader();

            // Capisco il tipo di cassa che sto considerando
            string codiceTubolare = "TUBOLARE80504";
            if(this._cassainferro.TipologiaCassa[ContatoreConfigurazioni] != null)
            {
                if (this._cassainferro.TipologiaCassa[ContatoreConfigurazioni].Substring(5) != "8"
                || this._cassainferro.TipologiaCassa[ContatoreConfigurazioni].Substring(5) != "11")
                {
                    codiceTubolare = "TUBOLARE80504";
                }
                else
                {
                    codiceTubolare = "TUBOLARE501205";
                }
            }
            
            // LONGHERONI E TRAVERSINI
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == codiceTubolare)
                {
                    // Altezza longherone
                    this._cassainferro.AltezzaLongherone = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza")));

                    // Peso e prezzo dei longheroni superiori ed inferiori
                    int numeroLongheroni = 4;
                    if (this.Numerofile == 2)
                    {
                        numeroLongheroni = 6;
                    }

                    // Capisco il peso ed il prezzo dei longheroni
                    pesoLongh = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro")));
                    prezzoLongh = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo")));

                    this._cassainferro.PesoLongheroni[ContatoreConfigurazioni] = pesoLongh * numeroLongheroni * this.Lunghezza[ContatoreConfigurazioni] * 0.001;
                    this._cassainferro.PrezzoLongheroni[ContatoreConfigurazioni] = prezzoLongh * numeroLongheroni * this.Lunghezza[ContatoreConfigurazioni] * 0.001;
                
                    // Peso e prezzo dei traversini alla base
                    this._cassainferro.NumeroTraversiniBase[ContatoreConfigurazioni] =Convert.ToInt32( Math.Round(this.Lunghezza[ContatoreConfigurazioni] / 1000, 0) + 1);
                    this._cassainferro.PesoTraversiniBase[ContatoreConfigurazioni] = pesoLongh * (Math.Round(this.Lunghezza[ContatoreConfigurazioni] / 1000, 0) + 1) * this.Larghezza[ContatoreConfigurazioni] * 0.001;
                    this._cassainferro.PrezzoTraversiniBase[ContatoreConfigurazioni] = prezzoLongh * (Math.Round(this.Lunghezza[ContatoreConfigurazioni] / 1000, 0) + 1) * this.Larghezza[ContatoreConfigurazioni] * 0.001;

                    break;
                }
            }

            // LONGHERONI DI RINFORZO
            reader.Close();
            creaComando = dbSQL.CreateSettingCasseCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                if(reader.GetValue(reader.GetOrdinal("Nome_Cassa")).ToString() == this._cassainferro.TipologiaCassa[ContatoreConfigurazioni])
                {
                    this._cassainferro.PesoLongheroniRinforzo[ContatoreConfigurazioni] = pesoLongh * Convert.ToDouble(reader.GetValue(reader.GetOrdinal("L_Rinforzo_Longherone"))) * 4;
                    this._cassainferro.PrezzoLongheroniRinforzo[ContatoreConfigurazioni] = prezzoLongh * Convert.ToDouble(reader.GetValue(reader.GetOrdinal("L_Rinforzo_Longherone"))) * 4;

                    // Capisco la presenza dei ganci
                    this._cassainferro.PresenzaGanci[ContatoreConfigurazioni] = reader.GetValue(reader.GetOrdinal("Presenza_Ganci")).ToString();

                    // Capisco la presenza degli incroci sulle spalle
                    this._cassainferro.IncrociSpalle[ContatoreConfigurazioni] = reader.GetValue(reader.GetOrdinal("Incroci_Spalle")).ToString();

                    // Capisco se la cssa può essere fatta con solo i ritti
                    this._cassainferro.PresenzaSoloRitti[ContatoreConfigurazioni] = reader.GetValue(reader.GetOrdinal("Solo_Ritti")).ToString();
                    if (this._cassainferro.PresenzaSoloRitti[ContatoreConfigurazioni] == "Si")
                    {
                        this._cassainferro.PesoLongheroni[ContatoreConfigurazioni] = this._cassainferro.PesoLongheroni[ContatoreConfigurazioni] / 2;
                        this._cassainferro.PrezzoLongheroni[ContatoreConfigurazioni] = this._cassainferro.PrezzoLongheroni[ContatoreConfigurazioni] / 2;
                    }

                    break;
                }             
            }
                
            // Costo e peso dei ritti e dei traversini superiori
            reader.Close();
            creaComando = dbSQL.CostoCassaFerroCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == "TUBOLARE40403")
                {
                    // Numero ritti
                    this._cassainferro.NumeroRitti[ContatoreConfigurazioni] = Convert.ToInt32((Math.Floor(this.Lunghezza[ContatoreConfigurazioni] / 2 * 0.001) + 1) * 
                        (this.Numerofile + 1));

                    // Peso e prezzo dei ritti su entrambi i lati
                    this._cassainferro.PesoRitti[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * 
                        (this.Altezza[ContatoreConfigurazioni] - this._cassainferro.AltezzaLongherone * 3) * this._cassainferro.NumeroRitti[ContatoreConfigurazioni] * 0.001;
                    this._cassainferro.PrezzoRitti[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * 
                        (this.Altezza[ContatoreConfigurazioni] - this._cassainferro.AltezzaLongherone * 3) * this._cassainferro.NumeroRitti[ContatoreConfigurazioni] * 0.001;

                    // Peso e prezzo dei traversini superiori
                    if (this._cassainferro.PresenzaSoloRitti[ContatoreConfigurazioni] == "No")
                    {
                        this._cassainferro.PesoTraversiniSuperiori[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) *
                        (Math.Round(this.Lunghezza[ContatoreConfigurazioni] / 2000, 0) + 1) * this.Larghezza[ContatoreConfigurazioni] * 0.001;
                        this._cassainferro.PrezzoTraversiniSuperiori[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) *
                            (Math.Round(this.Lunghezza[ContatoreConfigurazioni] / 2000, 0) + 1) * this.Larghezza[ContatoreConfigurazioni] * 0.001;
                    }
                    break;
                }
            }

            // Costo e peso della rete di tamponatura
            reader.Close();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == "RETETAMPONATURA20001000")
                {
                    // Peso e prezzo della rete di tamponatura sui fianchi
                    this._cassainferro.PesoReteTamponatura[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Altezza[ContatoreConfigurazioni] * 0.001 * 2;
                    this._cassainferro.PrezzoReteTamponatura[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Altezza[ContatoreConfigurazioni] * 0.001 * 2;

                    // Peso e prezzo della rete di tamponatura alla base
                    this._cassainferro.PesoReteTamponaturaBase[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Larghezza[ContatoreConfigurazioni] * 0.001;
                    this._cassainferro.PrezzoReteTamponaturaBase[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Larghezza[ContatoreConfigurazioni] * 0.001;

                    break;
                }
            }

            // Peso e prezzo pluriball in alluminio
            reader.Close();
            creaComando = dbSQL.CreateSettingAccessoriCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == "COPERTURAALU")
                {
                    // Peso e prezzo della rete di tamponatura sui fianchi
                    this._cassainferro.PesoPluriballAlluminio[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso"))) * (this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Altezza[ContatoreConfigurazioni] * 0.001 * 2 +
                        this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Larghezza[ContatoreConfigurazioni] * 0.001 * 2);
                    this._cassainferro.PrezzoPluriballAlluminio[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * (this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Altezza[ContatoreConfigurazioni] * 0.001 * 2 +
                        this.Lunghezza[ContatoreConfigurazioni] * 0.001 * this.Larghezza[ContatoreConfigurazioni] * 0.001 * 2);

                    break;
                }
            }

            // Peso e prezzo pluriball in alluminio
            int Id = 0;
            if (this.Lunghezza[ContatoreConfigurazioni] <= 4000)
            {
                Id = 2;
            }
            else
            {
                Id = 3;
            }

            reader.Close();
            creaComando = dbSQL.CreateSettingCostiGestioneCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("ID"));
                if (temp.ToString() == Id.ToString())
                {
                    // Peso e prezzo della rete di tamponatura sui fianchi
                    this._cassainferro.PrezzoGestioneCassa[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo")));
                    
                    break;
                }
            }

            // Prezzo gestione pannelli sandwich
            reader.Close();
            creaComando = dbSQL.CreateSettingCostiGestioneCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("ID"));
                if (temp.ToString() == "4")
                {
                    // Peso e prezzo della rete di tamponatura sui fianchi
                    this._cassainferro.PrezzoManodoperaPannelliSandwich = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo")));

                    break;
                }
            }

            // Peso e prezzo dei subbi in polistirolo
            reader.Close();
            creaComando = dbSQL.CreateSettingAccessoriCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == this.CodicePolistirolo)
                {
                    // Peso e prezzo della rete di tamponatura sui fianchi
                    this._cassainferro.PesoSubbiPolistirolo[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso"))) * this.NumeroCurvePolistirolo[ContatoreConfigurazioni];
                    this._cassainferro.PrezzoSubbiPolistirolo[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * this.NumeroCurvePolistirolo[ContatoreConfigurazioni];

                    break;
                }
            }

            // Peso e prezzo corrugati
            reader.Close();
            creaComando = dbSQL.CreateSettingAccessoriCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (temp.ToString() == this.CodiceCorrugato)
                {
                    // Peso e prezzo della rete di tamponatura sui fianchi
                    this._cassainferro.PesoCorrugati[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso"))) * 
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] * this.Larghezza[ContatoreConfigurazioni] * 0.001;
                    this._cassainferro.PrezzoCorrugati[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * +
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] * this.Larghezza[ContatoreConfigurazioni] * 0.001;

                    break;
                }
            }

            // Diagonali e incroci se la cassa è maggiore di o uguale di 8 metri
            creaComando = dbSQL.CreateSettingCostiFerroCommand();
            if (this._cassainferro.IncrociSpalle[ContatoreConfigurazioni] == "Si")
            {
                reader.Close();
                reader = creaComando.ExecuteReader();
                while (reader.Read())
                {
                    var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                    if (temp.ToString() == "TUBOLARE40403") 
                    {
                        // Peso e prezzo delle diagonali su entrambi i lati
                        this._cassainferro.PesoDiagonali[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(2000,2)) * Math.Floor(this.Lunghezza[ContatoreConfigurazioni] / 1000) * 0.001;
                        this._cassainferro.PrezzoDiagonali[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) *Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(2000, 2)) * Math.Floor(this.Lunghezza[ContatoreConfigurazioni] / 1000) * 0.001;

                        // Peso e prezzo diagonali ultima campata
                        this._cassainferro.PesoDiagonaliUltimaCampata[ContatoreConfigurazioni] =  Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(this._cassainferro.LunghezzaUltimaCampata[ContatoreConfigurazioni],2)) * 2 * 0.001;
                        this._cassainferro.PrezzoDiagonaliUltimaCampata[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(this._cassainferro.LunghezzaUltimaCampata[ContatoreConfigurazioni], 2)) * 2 * 0.001;

                        // Calcolo la lunghezza delle diagonali
                        this._cassainferro.LunghezzaDiagonali[ContatoreConfigurazioni] = Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(2000, 2));
                        this._cassainferro.LunghezzaDiagonaleUltimaCampata[ContatoreConfigurazioni] = Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(this._cassainferro.LunghezzaUltimaCampata[ContatoreConfigurazioni], 2));

                        // Calcolo il numero di diagonali
                        this._cassainferro.NumeroDiagonali[ContatoreConfigurazioni] = Convert.ToInt32((Math.Floor(this.Lunghezza[ContatoreConfigurazioni] / 2 * 0.001) + 1) * 2);
                        break;
                    }
                }

                reader.Close();
                reader = creaComando.ExecuteReader();
                while (reader.Read())
                {
                    var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                    if (temp.ToString() == "TUBOLAREDIAM20")
                    {
                        // Peso e prezzo delle diagonali su entrambi i lati
                        this._cassainferro.PesoIncroci[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(2000, 2)) * Math.Floor(this.Lunghezza[ContatoreConfigurazioni] / 1000) * 0.001;
                        this._cassainferro.PrezzoIncroci[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(2000, 2)) * Math.Floor(this.Lunghezza[ContatoreConfigurazioni] / 1000) * 0.001;

                        // Aggiungo il peso e il prezzo dell'ultima campata
                        this._cassainferro.PesoIncrocioUltimaCampata[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMetro"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(this._cassainferro.LunghezzaUltimaCampata[ContatoreConfigurazioni], 2)) * 2 * 0.001;
                        this._cassainferro.PrezzoIncrocioUltimaCampata[ContatoreConfigurazioni] = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Prezzo"))) * Math.Sqrt(Math.Pow(this.Altezza[ContatoreConfigurazioni], 2) + Math.Pow(this._cassainferro.LunghezzaUltimaCampata[ContatoreConfigurazioni], 2)) * 2 * 0.001;

                        break;
                    }
                }                
            }

            // Aumento il contatore delle configurazioni
            ContatoreConfigurazioni += 1;
        }
        private void LunghezzaUltimaCampata()
        {
            this._cassainferro.LunghezzaUltimaCampata[ContatoreConfigurazioni] = (this.Lunghezza[ContatoreConfigurazioni]*0.001 - (Math.Floor(this.Lunghezza[ContatoreConfigurazioni]*0.001 / 2) * 2))*Math.Pow(10,3);
        }
        private void CalcolaImballoConfigurazione1()
        {
            // PER NASTRI CON BORDI DA 120MM E 160MM

            // Scorro le varie tipologie di trasporto x vedere qual'è la migliore per questo imballo
            for (int counter = 0; counter < this._cassainferro.TipoTrasporto.Length - 1; counter++)
            {
                // Inizializza variabili
                this.InizializzaVariabili(counter);

                while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this._cassainferro.FattibilitaTrasporto[counter] == true)
                {
                    // Calcola la lunghezza di ciascun strato
                    Strati[i, j] = contatorestrati;
                    j += 1;

                    if (contatorestrati == 1)
                    {
                        // Calcola la lunghezza del primo strato
                        Strati[i, j] = Math.Abs(this._cassainferro.LunghezzaIniziale) - this.DiametroPolistirolo / 2;
                    }
                    else if (contatorestrati % 2 == 0) // Strati pari
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato * 0.5 - this._prodotto.AltezzaApplicazioni;
                    }
                    else if (contatorestrati % 2 != 0) // Strati dispari
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato * 0.5 - this.DiametroPolistirolo / 2;
                    }

                    // Calcola la lunghezza del nastro fino ad ora imballato
                    if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;
                    }
                    else if (Strati[i, j] % 2 != 0)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;
                    }

                    // Calcola l'oridinata del punto
                    j += 1;
                    if (contatorestrati != 1)
                    {
                        Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                    }
                    else
                    {
                        Strati[i, j] = 0;
                    }

                    // Controlla che l'altezza della cassa sia dentro i limiti
                    // Se il numero di strati è di numero pari moltiplico la metà del numero degli strati per il diametro
                    // del polistirolo e trovo l'altezza massima della parte in polistirolo. Se il numero di strati è dispari,
                    // sottraggo 1 al numero di strati, lo divido per due e lo moltiplico per il diametro del corrugato
                    if (contatorestrati % 2 == 0)
                    {
                        altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurvePolistirolo[ContatoreConfigurazioni] = contatorestrati / 2;
                    }
                    else
                    {
                        altezzacorrugati = (contatorestrati + 1) * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza + 0.5 * this.DiametroCorrugato;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                    }

                    // Controllo che siamo dentro i limiti - Ho stabilito 200 [mm] di tolleranza
                    this.ControllaLimitiImballo(counter);

                    // Se il nastro non ci sta nell'attuale trasporto (fattibilita = -1) passo al tipo di trasporto successivo
                    if (this.cassaFattibile == -1 || this.cassaFattibile == 1)
                    {
                        break;
                    }
                }

                // Assegna le dimensioni al vettore solo se la cassa è fattibile
                if (this.cassaFattibile == 1)
                {
                    ContatoreConfigurazioni = counter;
                    this.AssegnaDimensioni();
                    this.NumeroConfigurazione[ContatoreConfigurazioni] = 1;
                    this._cassainferro.Configurazione = 1;

                    // Calcola la criticità dell'imballo
                    this.CalcoloCriticitaImballo();

                    // Calcola peso e prezzo della configurazione
                    this.CalcolaPesoCassa();
                } 
            }  
        }
        private void CalcolaImballoConfigurazione2()
        {
            // Inizializza variabili
            //this.InizializzaVariabili(counter);

            // Resetta i limiti dell'imballo
            this.StabilisceLimitiImballo();

            while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this.Fattibilita[ContatoreConfigurazioni] == true)
            {
                // Calcola la lunghezza di ciascun strato
                Strati[i, j] = contatorestrati;
                j += 1;

                if (contatorestrati == 1)
                {
                    // Calcola la lunghezza del primo strato
                    Strati[i, j] = this._cassainferro.LunghezzaIniziale - this.DiametroPolistirolo / 2;
                }
                else if ( contatorestrati % 2 == 0) // Strati pari 
                {
                    Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                }
                else if (contatorestrati % 2 != 0 && contatorestrati != 1) // Strati dispari
                {
                    Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato / 2 - (this.DiametroPolistirolo * 1.5);
                }

                // Calcola la lunghezza del nastro fino ad ora imballato
                if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                {
                    this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;
                }
                else if (Strati[i, j] % 2 != 0)
                {
                    this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;
                }

                // Calcola l'oridinata del punto
                j += 1;
                if (contatorestrati != 1)
                {
                    Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                }
                else
                {
                    Strati[i, j] = 0;
                }

                // Controlla che l'altezza della cassa sia dentro i limiti
                // Se il numero di strati è di numero pari moltiplico la metà del numero degli strati per il diametro
                // del polistirolo e trovo l'altezza massima della parte in polistirolo. Se il numero di strati è dispari,
                // sottraggo 1 al numero di strati, lo divido per due e lo moltiplico per il diametro del corrugato
                if (contatorestrati % 2 == 0)
                {
                    altezzapolistirolo = (contatorestrati - 2) * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza + this.DiametroPolistirolo;

                    // Ci dice quanti strati di polistirolo ha l'imballo
                    this.NumeroCurvePolistirolo[ContatoreConfigurazioni] = contatorestrati / 2;
                }
                else
                {
                    altezzacorrugati = this.DiametroCorrugato + contatorestrati * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza;

                    // Ci dice quanti strati di polistirolo ha l'imballo
                    this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                }

                // Controllo che siamo dentro i limiti
                //this.ControllaLimitiImballo();
            }
            // Assegna le dimensioni al vettore
            this.AssegnaDimensioni();

            // Assegna il numero di configurazione
            if (this.Fattibilita[ContatoreConfigurazioni] == true)
            {
                this.NumeroConfigurazione[ContatoreConfigurazioni] = 2;
            }

            // Calcola la criticità dell'imballo
            this.CalcoloCriticitaImballo();

            // Calcola peso e prezzo della configurazione
            this.CalcolaPesoCassa();
        }
        private void CalcolaImballoConfigurazione3()
        {
            // Inizializza variabili
            //this.InizializzaVariabili(counter);

            // Resetta i limiti dell'imballo
            this.StabilisceLimitiImballo();

            while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this.Fattibilita[ContatoreConfigurazioni] == true)
            {
                // Calcola la lunghezza di ciascun strato
                Strati[i, j] = contatorestrati;
                j += 1;

                if (contatorestrati == 1)
                {
                    // Calcola la lunghezza del primo strato
                    Strati[i, j] = this._cassainferro.LunghezzaIniziale - this.DiametroPolistirolo / 2;
                }
                else if (contatorestrati == 2)
                {
                    Strati[i, j] = Strati[0, 1] - this.DiametroCorrugato / 2;
                }
                else if (contatorestrati == 6 || contatorestrati == 7)
                {
                    Strati[i, j] = Strati[2, j];
                }
                else
                {
                    Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato * 0.5;
                }

                // Calcola la lunghezza del nastro fino ad ora imballato
                if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                {
                    this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;
                }
                else if (Strati[i, j] % 2 != 0)
                {
                    this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;
                }

                // Calcola l'oridinata del punto
                j += 1;
                if (contatorestrati != 1)
                {
                    Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                }
                else
                {
                    Strati[i, j] = 0;
                }

                // Controlla che l'altezza della cassa sia dentro i limiti
                // Se il numero di strati è di numero pari moltiplico la metà del numero degli strati per il diametro
                // del polistirolo e trovo l'altezza massima della parte in polistirolo. Se il numero di strati è dispari,
                // sottraggo 1 al numero di strati, lo divido per due e lo moltiplico per il diametro del corrugato
                if (contatorestrati % 2 == 0)
                {
                    altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo;

                    // Ci dice quanti strati di polistirolo ha l'imballo
                    this.NumeroCurvePolistirolo[ContatoreConfigurazioni] = contatorestrati / 2;
                }
                else
                {
                    altezzacorrugati = this.DiametroCorrugato + contatorestrati * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza;

                    // Ci dice quanti strati di polistirolo ha l'imballo
                    this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                }

                // Controllo che siamo dentro i limiti - Ho stabilito 200 [mm] di tolleranza
                //this.ControllaLimitiImballo();
            }
            // Assegna le dimensioni al vettore
            this.AssegnaDimensioni();

            // Assegna il numero di configurazione
            if (this.Fattibilita[ContatoreConfigurazioni] == true)
            {
                this.NumeroConfigurazione[ContatoreConfigurazioni] = 3;
            }

            // Calcola la criticità dell'imballo
            this.CalcoloCriticitaImballo();

            // Calcola peso e prezzo della configurazione
            this.CalcolaPesoCassa();
        }
        private void CalcolaImballoConfigurazione4()
        {
            // Inizializza variabili
            //this.InizializzaVariabili(counter);

            // Resetta i limiti dell'imballo
            this.StabilisceLimitiImballo();

            while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this.Fattibilita[ContatoreConfigurazioni] == true)
                {
                    // Calcola la lunghezza di ciascun strato
                    Strati[i, j] = contatorestrati;
                    j += 1;

                    if (contatorestrati == 1)
                    {
                        // Calcola la lunghezza del primo strato
                        Strati[i, j] = this._cassainferro.LunghezzaIniziale - this.DiametroPolistirolo / 2;
                    }
                    else if (contatorestrati == 2)
                    {
                        Strati[i, j] = Strati[0, 1] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                    }
                    else if (contatorestrati == 4) // Strati pari (4, 6 ecc...)
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                    }
                    else if (contatorestrati % 2 != 0 && contatorestrati != 1 && contatorestrati != 7) // Strati dispari (3, 5 ecc...)
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato / 2 - (this.DiametroPolistirolo / 2 * 1.7);
                    }
                    else if (contatorestrati == 6 || contatorestrati == 7)
                    {
                        Strati[i, j] = Strati[2, j];
                    }

                    // Calcola la lunghezza del nastro fino ad ora imballato
                    if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;
                    }
                    else if (Strati[i, j] % 2 != 0)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;
                    }

                    // Calcola l'oridinata del punto
                    j += 1;
                    if (contatorestrati != 1)
                    {
                        Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                    }
                    else
                    {
                        Strati[i, j] = 0;
                    }

                    // Controlla che l'altezza della cassa sia dentro i limiti
                    // Se il numero di strati è di numero pari moltiplico la metà del numero degli strati per il diametro
                    // del polistirolo e trovo l'altezza massima della parte in polistirolo. Se il numero di strati è dispari,
                    // sottraggo 1 al numero di strati, lo divido per due e lo moltiplico per il diametro del corrugato
                    if (contatorestrati % 2 == 0)
                    {
                        altezzapolistirolo = (contatorestrati - 2) * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza + this.DiametroPolistirolo;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurvePolistirolo[ContatoreConfigurazioni] = contatorestrati / 2;
                }
                    else if (contatorestrati == 3 || contatorestrati == 5)
                    {
                        altezzacorrugati = this.DiametroCorrugato + contatorestrati * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                    }
                    else if (contatorestrati == 7)
                    {
                        altezzacorrugati =  this.DiametroCorrugato + contatorestrati * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                    }

                    // Controllo che siamo dentro i limiti - Ho stabilito 200 [mm] di tolleranza
                    //this.ControllaLimitiImballo();
                }
            // Assegna le dimensioni al vettore
            this.AssegnaDimensioni();

            // Assegna il numero di configurazione
            if (this.Fattibilita[ContatoreConfigurazioni] == true)
            {
                this.NumeroConfigurazione[ContatoreConfigurazioni] = 4;
            }

            // Calcola la criticità dell'imballo
            this.CalcoloCriticitaImballo();

            // Calcola peso e prezzo della configurazione
            this.CalcolaPesoCassa();
        }
        private void CalcolaImballoConfigurazione5()
        {
            // Inizializza variabili
            //this.InizializzaVariabili(counter);

            // Resetta i limiti dell'imballo
            this.StabilisceLimitiImballo();

            while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this.Fattibilita[ContatoreConfigurazioni] == true)
                {
                    // Calcola la lunghezza di ciascun strato
                    Strati[i, j] = contatorestrati;
                    j += 1;

                    if (contatorestrati == 1)
                    {
                        // Calcola la lunghezza del primo strato
                        Strati[i, j] = this._cassainferro.LunghezzaIniziale - this.DiametroPolistirolo / 2;
                    }
                    else if (contatorestrati == 2)
                    {
                        Strati[i, j] = Strati[0, 1] - this.DiametroCorrugato / 2;
                    }
                    else if (contatorestrati == 6 || contatorestrati == 7)
                    {
                        Strati[i, j] = Strati[2, j];
                    }
                    else if(contatorestrati == 8 || contatorestrati == 9)
                    {
                     Strati[i, j] = Strati[0, j] - 2 * this.DiametroCorrugato;
                    }
                    else if (contatorestrati == 10 || contatorestrati == 11)
                    {
                        Strati[i, j] = Strati[4, j];
                    }
                    else if (contatorestrati == 12 || contatorestrati == 13)
                    {
                        Strati[i, j] = Strati[8, j]- this.DiametroCorrugato;
                    }
                    else
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato * 0.5;
                    }

                    // Calcola la lunghezza del nastro fino ad ora imballato
                    if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;
                    }
                    else if (Strati[i, j] % 2 != 0)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;
                    }

                    // Calcola l'oridinata del punto
                    j += 1;
                    if (contatorestrati != 1)
                    {
                        Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                    }
                    else
                    {
                        Strati[i, j] = 0;
                    }

                    // Controlla che l'altezza della cassa sia dentro i limiti
                    // Se il numero di strati è di numero pari moltiplico la metà del numero degli strati per il diametro
                    // del polistirolo e trovo l'altezza massima della parte in polistirolo. Se il numero di strati è dispari,
                    // sottraggo 1 al numero di strati, lo divido per due e lo moltiplico per il diametro del corrugato
                    if (contatorestrati % 2 == 0)
                    {
                        altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurvePolistirolo[ContatoreConfigurazioni] = contatorestrati / 2;
                    }
                    else if (contatorestrati == 3 || contatorestrati == 5 || contatorestrati == 9 || contatorestrati == 11 || contatorestrati == 13)
                    {
                        altezzacorrugati = this.DiametroCorrugato + contatorestrati * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                     }
                    else if (contatorestrati == 7)
                    {
                        altezzacorrugati = this.DiametroCorrugato * 2 + (contatorestrati - 2) * this._prodotto.AltezzaApplicazioni / this._prodotto.FattoreAltezza;

                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                    }

                // Controllo che siamo dentro i limiti - Ho stabilito 200 [mm] di tolleranza
                //this.ControllaLimitiImballo();

            }
            // Assegna le dimensioni al vettore
            this.AssegnaDimensioni();

            // Assegna il numero di configurazione
            if (this.Fattibilita[ContatoreConfigurazioni] == true)
            {
                this.NumeroConfigurazione[ContatoreConfigurazioni] = 5;
            }

            // Calcola la criticità dell'imballo
            this.CalcoloCriticitaImballo();

            // Calcola peso e prezzo della configurazione
            this.CalcolaPesoCassa();
        }
        private void CalcolaImballoConfigurazione6()
            {
            // PER NASTRI SOLO BORDI E SOLO TAZZE

            // Scorro le varie tipologie di trasporto x vedere qual'è la migliore per questo imballo
            for (int counter = 0; counter < this._cassainferro.TipoTrasporto.Length-1; counter++)
            {
                // Inizializza variabili
                this.InizializzaVariabili(counter);

                while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this._cassainferro.FattibilitaTrasporto[counter] == true)
                {
                    // Calcola la lunghezza di ciascun strato
                    Strati[i, j] = contatorestrati;
                    j += 1;

                    if (contatorestrati == 1)
                    {
                        Strati[i, j] = this._cassainferro.LunghezzaIniziale - this.DiametroPolistirolo / 2;
                    }
                    else if (contatorestrati == 2)
                    {
                        Strati[i, j] = Strati[i - 1, 1] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                        // Altezza polistirolo
                        altezzapolistirolo = this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 3)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * contatorestrati + DiametroCorrugato + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 4)
                    {
                        Strati[i, j] = Strati[i - 1, 1] - 1.5 * this.DiametroCorrugato - this._prodotto.AltezzaApplicazioni * 2;
                        // Altezza polistirolo
                        altezzapolistirolo = 2 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 5)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * contatorestrati + DiametroCorrugato + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 6)
                    {
                        Strati[i, j] = Strati[i - 1, j] - 1.5 * this.DiametroCorrugato - this._prodotto.AltezzaApplicazioni * 2;
                        // Altezza polistirolo
                        altezzapolistirolo = 3 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 7)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = contatorestrati * this._prodotto.AltezzaApplicazioni + DiametroCorrugato + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 8)
                    {
                        Strati[i, j] = Strati[i - 3, j] - 1.5 * this.DiametroCorrugato - this._prodotto.AltezzaApplicazioni * 2;
                        // Altezza polistirolo
                        altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 9)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = contatorestrati * this._prodotto.AltezzaApplicazioni + DiametroCorrugato + this._nastro.Spessore * (contatorestrati +1);
                    }
                    else if (contatorestrati == 10)
                    {
                        Strati[i, j] = Strati[i - 8, j] - 1.5 * this.DiametroCorrugato - this._prodotto.AltezzaApplicazioni * 2;
                        // Altezza polistirolo
                        altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo + this._nastro.Spessore* contatorestrati;
                    }
                    else if (contatorestrati % 2 == 0)
                    {
                        Strati[i, j] = Strati[i - 1, j] - 2 * this.DiametroCorrugato;
                        // Altezza polistirolo
                        altezzapolistirolo = contatorestrati * this.altezzapolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else
                    {
                        Strati[i, j] = Strati[i - 1, j] - 2 * this.DiametroCorrugato;
                        // Altezza corrugato
                        altezzacorrugati = contatorestrati * this.altezzacorrugati;
                    }

                    // Calcola la lunghezza del nastro fino ad ora imballato
                    if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;
                    }
                    else if (Strati[i, j] % 2 != 0)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;
                    }

                    // Calcola l'oridinata del punto
                    j += 1;
                    if (contatorestrati != 1)
                    {
                        Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                    }
                    else
                    {
                        Strati[i, j] = 0;
                    }

                    // Controllo che siamo dentro i limiti - Ho stabilito 200 [mm] di tolleranza
                    this.ControllaLimitiImballo(counter);

                    // Se il nastro non ci sta nell'attuale trasporto (fattibilita = -1) passo al tipo di trasporto successivo
                    if (this.cassaFattibile == -1 || this.cassaFattibile == 1)
                    {
                        break;
                    }

                }
                // Assegna le dimensioni al vettore
                if (this.cassaFattibile == 1)
                {
                    ContatoreConfigurazioni = counter;
                    this.AssegnaDimensioni();
                    this.NumeroConfigurazione[ContatoreConfigurazioni] = 6;
                    this._cassainferro.Configurazione = 6;

                    // Calcola la criticità dell'imballo
                    this.CalcoloCriticitaImballo();

                    // Calcola peso e prezzo della configurazione
                    this.CalcolaPesoCassa();

                    // Riazzero la fattibilità della cassa
                    this.cassaFattibile = 0;
                }             
            }
        }
        private void CalcolaImballoConfigurazione7()
        {
            // Il nastro viene disposto scalando sia il polistirolo che il corrugato
            // Prendere come riferimento l'imballo della commessa: 5501780338

            // Scorro le varie tipologie di trasporto x vedere qual'è la migliore per questo imballo
            for (int counter = 0; counter < this._cassainferro.TipoTrasporto.Length -1; counter++)
            {
                // Inizializza variabili
                this.InizializzaVariabili(counter);

                while (this._nastro.LunghezzaImballato < this._nastro.Lunghezza && this._cassainferro.FattibilitaTrasporto[counter] == true)
                {
                    // Calcola la lunghezza di ciascun strato
                    Strati[i, j] = contatorestrati;
                    j += 1;

                    if (contatorestrati == 1)
                    {
                        Strati[i, j] = this._cassainferro.LunghezzaIniziale - this.DiametroPolistirolo / 2;
                    }
                    else if (contatorestrati == 2)
                    {
                        Strati[i, j] = Strati[i - 1, 1] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                        // Altezza polistirolo
                        altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo;
                    }
                    else if (contatorestrati == 3)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * contatorestrati + DiametroCorrugato + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 4)
                    {
                        Strati[i, j] = Strati[i - 1, 1] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                        // Altezza polistirolo
                        altezzapolistirolo = contatorestrati / 2 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 5)
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroPolistirolo;
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * contatorestrati + DiametroCorrugato + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 6)
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato /2 -  this._prodotto.AltezzaApplicazioni;
                        // Altezza polistirolo
                        altezzapolistirolo = 2.5 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 7)
                    {
                        Strati[i, j] = Strati[i - 1, j] + this.DiametroPolistirolo * 1.5;
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * 5 + DiametroCorrugato * 2 + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 8)
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                        // Altezza polistirolo
                        altezzapolistirolo = (contatorestrati - 2) / 2 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 9)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * contatorestrati + this.DiametroCorrugato + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 10)
                    {
                        Strati[i, j] = Strati[i - 1, j] - this.DiametroCorrugato / 2 - this._prodotto.AltezzaApplicazioni;
                        // Altezza polistirolo
                        altezzapolistirolo = 4 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 11)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * 7 + DiametroCorrugato * 3 + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 12)
                    {
                        Strati[i, j] = Strati[i - 1, j];
                        // Altezza polistirolo
                        altezzapolistirolo = 4.8 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati == 13)
                    {
                        Strati[i, j] = Strati[i - 4, j];
                        // Altezza corrugato
                        altezzacorrugati = this._prodotto.AltezzaApplicazioni * 13 + DiametroCorrugato * 2 + this._nastro.Spessore * (contatorestrati + 1);
                    }
                    else if (contatorestrati == 14)
                    {
                        Strati[i, j] = Strati[i - 1, j] - 3 * this.DiametroCorrugato;
                        // Altezza polistirolo
                        altezzapolistirolo = 4 * this.DiametroPolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else if (contatorestrati % 2 == 0)
                    {
                        Strati[i, j] = Strati[i - 1, j] - 2 * this.DiametroCorrugato;
                        // Altezza polistirolo
                        altezzapolistirolo = contatorestrati * this.altezzapolistirolo + this._nastro.Spessore * contatorestrati;
                    }
                    else
                    {
                        Strati[i, j] = Strati[i - 1, j] - 2 * this.DiametroCorrugato;
                        // Altezza corrugato
                        altezzacorrugati = contatorestrati * this.altezzacorrugati + this._nastro.Spessore * (contatorestrati + 1);
                    }

                    // Calcola la lunghezza del nastro fino ad ora imballato
                    if (Strati[i, j] % 2 == 0 && Strati[i, j] != 2)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaEsterna) * this.Numerofile;

                    }
                    else if (Strati[i, j] % 2 != 0)
                    {
                        this._nastro.LunghezzaImballato = Math.Abs(this._nastro.LunghezzaImballato) + (Strati[i, j] + this.LunghezzaCurvaInterna) * this.Numerofile;

                    }

                    // Calcola l'ordinata del punto
                    j += 1;
                    if (contatorestrati != 1)
                    {
                        Strati[i, j] = this._prodotto.AltezzaApplicazioni * (contatorestrati) / this._prodotto.FattoreAltezza;
                    }
                    else
                    {
                        Strati[i, j] = 0;
                    }

                    if (contatorestrati % 2 == 0)
                    {
                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurvePolistirolo[ContatoreConfigurazioni] = contatorestrati / 2;
                    }
                    else
                    {
                        // Ci dice quanti strati di polistirolo ha l'imballo
                        this.NumeroCurveCorrugati[ContatoreConfigurazioni] = (contatorestrati - 1) / 2;
                    }


                    // Controllo che siamo dentro i limiti - Ho stabilito 200 [mm] di tolleranza
                    this.ControllaLimitiImballo(counter);

                    // Se il nastro non ci sta nell'attuale trasporto (fattibilita = -1) passo al tipo di trasporto successivo
                    if (this.cassaFattibile == -1 || this.cassaFattibile == 1)
                    {
                        break;
                    }

                }
                // Assegna le dimensioni al vettore
                if (this.cassaFattibile == 1)
                {
                    ContatoreConfigurazioni = counter;
                    this.AssegnaDimensioni();
                    this.NumeroConfigurazione[ContatoreConfigurazioni] = 7;
                    this._cassainferro.Configurazione = 7;

                    // Calcolo numero subbi e corrugati
                    this.CalcoloNumeroSubbiCorrugati();

                    // Calcola la criticità dell'imballo
                    this.CalcoloCriticitaImballo();

                    // Calcola peso e prezzo della configurazione
                    this.CalcolaPesoCassa();
                }               
            }
        }
        private void ControllaLimitiImballo(int contTrasporto)
        {
            // Calcolo l'altezza dell'imballo in base al massimo tra polistirolo e corrugati
            altezzanastroimballato = Math.Max(altezzacorrugati, altezzapolistirolo);

            // Stabilisco la tolleranza: altezza longheroni inf + altezza longh sup + traversini base = 240 [mm]
            // Se il nastro è chiuso aggiungo anche l'altezza del nastro alla tolleranza
            double tolleranza = 80;

            // Mi dice se la cassa è ancora trasportabile con il tipo di trasporto selezionato
            // -1 --> Infattibile
            // 0 --> C'è ancora possibilità
            // 1 --> Soluzione

            // Inizializzo il contatore per il tipo di trasporto
            this.itrasporto = contTrasporto;
            
            // Stabilisco la tolleranza in base al fatto che il nastro sia aperto o chiuso
            if (this._nastro.Aperto == false)
            {
                tolleranza = tolleranza + this._prodotto.Altezza;
            }

            // Stabilisco se questo nastro ha la priorità della cassa singola o doppia
            if (this._cassainferro.LunghezzaIniziale < 5000)
            {
                this.GetPriorityCassaDoppia(this.itrasporto);
            }

            // Capisco se posso passare allo strato successivo mantenendo
            // COSTANTE sia l'altezza che la lunghezza
            if (altezzanastroimballato + tolleranza <= this._cassainferro.LimiteAltezza[this.itrasporto]
                        && this._nastro.LunghezzaImballato < this._nastro.Lunghezza 
                        && this.contatorestrati <= 12)
            {
                // Passiamo allo strato successivo
                j = 0;
                i += 1;
                contatorestrati += 1;

                // Ancora non so se la cassa sia fattibile o meno
                cassaFattibile = 0;
            }
            // Nell'attuale cassa il nastro non ci sta
            // Azzero tutto ed AUMENTO LA LUNGHEZZA INIZIALE della cassa
            else if ((this._cassainferro.LimiteLunghezza[this.itrasporto] >= this._cassainferro.LunghezzaIniziale + 100
                        && altezzanastroimballato + tolleranza > this._cassainferro.LimiteAltezza[this.itrasporto])
                        || this.contatorestrati > 12)
            {
                // Aumenta la lunghezza della cassa di 10 cm                   
                this._cassainferro.LunghezzaIniziale = this._cassainferro.LunghezzaIniziale + 100;

                // Inizializzo contatori
                this.InizializzaContatori();
            }

            // Provo a disporre, se possibile il nastro in doppia fila
            else if (this.Numerofile != 2 && (this._nastro.Larghezza * this.Numerofile + this.TolleranzaLarghezza + this.TolleranzaCassaDoppia +
                        this._prodotto.LarghezzaAggSoloBordi * 2) < this._cassainferro.LimiteLarghezza[this.itrasporto] &&
                        this._cassainferro.LunghezzaIniziale >= this._cassainferro.LimiteLunghezza[this.itrasporto]
                        && this._nastro.LunghezzaImballato < this._nastro.Lunghezza)
            {
                // Stabilisco il numero di file
                this.Numerofile = 2;
                this.CalcoloNumeroSubbiCorrugati();

                // Inizializzo contatori
                this.InizializzaVariabili(this.itrasporto);

            }
            // Se entro qui significa l'altezza è nei limiti e anche la lunghezza è nei limiti dell'imballo, quindi la disposizione è valida
            // inoltre in questo caso il nastro è disposto su una fila
            else if ((this._cassainferro.LimiteLunghezza[this.itrasporto] >= this._cassainferro.LunghezzaIniziale + 100
                        && altezzanastroimballato + tolleranza <= this._cassainferro.LimiteAltezza[this.itrasporto])
                        && this.Numerofile == 1
                        && (this._nastro.Larghezza * this.Numerofile + this.TolleranzaLarghezza) < this._cassainferro.LimiteLarghezza[this.itrasporto])
            {
                // La configurazione è valida su questo imballo
                this._cassainferro.FattibilitaTrasporto[this.itrasporto] = true;
                this.cassaFattibile = 1;
                altezzaNastroImballatoFinale = altezzanastroimballato;
                this.CalcoloNumeroSubbiCorrugati();

                // Inizializzo contatori
                this.InizializzaContatori();
            }
            // Se entro qui significa l'altezza è nei limiti e anche la lunghezza è nei limiti dell'imballo, quindi la disposizione è valida
            // inoltre in questo caso il nastro è disposto su due file
            else if ((this._cassainferro.LimiteLunghezza[this.itrasporto] >= this._cassainferro.LunghezzaIniziale + 100
                        && altezzanastroimballato + tolleranza <= this._cassainferro.LimiteAltezza[this.itrasporto])
                        && this.Numerofile == 2
                        && (this._nastro.Larghezza * this.Numerofile + this.TolleranzaLarghezza + this.TolleranzaCassaDoppia +
                        this._prodotto.LarghezzaAggSoloBordi * 2) < this._cassainferro.LimiteLarghezza[this.itrasporto])
            {
                // La configurazione è valida su questo imballo
                this._cassainferro.FattibilitaTrasporto[this.itrasporto] = true;
                this.cassaFattibile = 1;
                altezzaNastroImballatoFinale = altezzanastroimballato;
                this.CalcoloNumeroSubbiCorrugati();

                // Inizializzo contatori
                this.InizializzaContatori();
            }
            
            else
            {
                // Su questo trasporto il nastro non ci sta
                this.cassaFattibile = -1;

                // Comunico che il nastro non può stare su questo trasporto
                this._cassainferro.FattibilitaTrasporto[this.itrasporto] = false;

                // Inizializzo contatori
                this.InizializzaContatori();
            }
            
        }
        private void InizializzaVariabili(int counter)
        {
            this._cassainferro.FattibilitaTrasporto[counter] = true;
            this._nastro.LunghezzaImballato = 0;
            Strati = new double[30, 3];
            i = 0;
            j = 0;
            contatorestrati = 1;
            altezzacorrugati = 0;
            altezzapolistirolo = 0;
            this._cassainferro.LunghezzaIniziale = 3000;
        }
        private void AssegnaDimensioni()
        {

            if (this._cassainferro.FattibilitaTrasporto[ContatoreConfigurazioni])
            {
                // Arrotondamento lunghezza x container 11.8m nel caso c'è il margine di sicurezza
                if (this._cassainferro.LunghezzaIniziale >= 11000 & this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] == 11800 &
                    this._cassainferro.LunghezzaIniziale <= this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] - this.TolleranzaLunghezza)
                {
                    this.Lunghezza[ContatoreConfigurazioni] = 11800;
                }
                // Arrotondamento lunghezza x container 11.8m nel caso in cui non c'è il margine di sicurezza in lunghezza
                else if (this._cassainferro.LunghezzaIniziale >= 11000 & this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] == 11800 & 
                    this._cassainferro.LunghezzaIniziale >= this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] - this.TolleranzaLunghezza)
                {
                    this.Lunghezza[ContatoreConfigurazioni] = 11800;
                    if(this.AvvisoImballoCritico == false)
                    {
                        // Faccio comparire l'avviso che l'imballo è critico perchè non c'è il margine di sicurezza
                        //System.Windows.MessageBox.Show("ATTENZIONE! Imballo critico, non ci sono i 30cm di margine.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        this.AvvisoImballoCritico = true;
                    }                 
                }
                else if(this._cassainferro.LunghezzaIniziale >= 12000 & this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] == 13600 &
                    this._cassainferro.LunghezzaIniziale <= this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] - this.TolleranzaLunghezza)
                {
                    this.Lunghezza[ContatoreConfigurazioni] = 13600;
                }
                else if (this._cassainferro.LunghezzaIniziale >= 12000 & this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] == 13600 &
                    this._cassainferro.LunghezzaIniziale >= this._cassainferro.LimiteLunghezza[ContatoreConfigurazioni] - this.TolleranzaLunghezza)
                {
                    this.Lunghezza[ContatoreConfigurazioni] = 13600;
                    if (this.AvvisoImballoCritico == false)
                    {
                        // Faccio comparire l'avviso che l'imballo è critico perchè non c'è il margine di sicurezza
                        //System.Windows.MessageBox.Show("ATTENZIONE! Imballo critico, non ci sono i 30cm di margine.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        this.AvvisoImballoCritico = true;
                    }
                }
                else if (this._cassainferro.LunghezzaIniziale <= 5800 & this._cassainferro.LunghezzaIniziale >= 4800 &
                    this._cassainferro.LunghezzaIniziale <= 5800 - this.TolleranzaLunghezza)
                {
                    this.Lunghezza[ContatoreConfigurazioni] = 5800;
                }
                else if (this._cassainferro.LunghezzaIniziale <= 5800 & this._cassainferro.LunghezzaIniziale >= 4800 &
                    this._cassainferro.LunghezzaIniziale >= 5800 - this.TolleranzaLunghezza)
                {
                    this.Lunghezza[ContatoreConfigurazioni] = 5800;
                    if (this.AvvisoImballoCritico == false)
                    {
                        // Faccio comparire l'avviso che l'imballo è critico perchè non c'è il margine di sicurezza
                        //System.Windows.MessageBox.Show("ATTENZIONE! Imballo critico, non ci sono i 30cm di margine.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        this.AvvisoImballoCritico = true;
                    }
                }
                else
                {
                    this.Lunghezza[ContatoreConfigurazioni] = this._cassainferro.LunghezzaIniziale + this.TolleranzaLunghezza;
                }
                
                // Arrotondamento altezza
                if (altezzaNastroImballatoFinale >= this.LimiteCassaStd & altezzaNastroImballatoFinale <= this.LimiteHighCube)
                {
                    this.Altezza[ContatoreConfigurazioni] = 2240;
                }
                else if (altezzaNastroImballatoFinale >= this.LimiteHighCube & altezzaNastroImballatoFinale <= this.LimiteOpenTop)
                {
                    this.Altezza[ContatoreConfigurazioni] = 2530;
                }
                else if (altezzaNastroImballatoFinale >= this.LimiteOpenTop)
                {
                    this.Altezza[ContatoreConfigurazioni] = 2800;
                }
                else
                {
                    this.Altezza[ContatoreConfigurazioni] = altezzaNastroImballatoFinale + 240;
                }     
                
                // Calcolo la larghezza in base al numero di file
                if (this.Numerofile == 1)
                {
                    this.Larghezza[ContatoreConfigurazioni] = this._nastro.Larghezza + this.TolleranzaLarghezza + this._prodotto.LarghezzaAggSoloBordi;
                }
                else
                {
                    this.Larghezza[ContatoreConfigurazioni] = this._nastro.Larghezza * 2 + this.TolleranzaLarghezza + this.TolleranzaCassaDoppia + this._prodotto.LarghezzaAggSoloBordi * 2;
                }

                // Stabilisco la tipologia della cassa
                this.GetCassaType();
            }
        }

        public void GetCassaType()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            int i = 0;

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingCasseCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                if (this.Lunghezza[ContatoreConfigurazioni] <= Convert.ToDouble(reader.GetValue(reader.GetOrdinal("L_Max")))
                    && this.Lunghezza[ContatoreConfigurazioni] >= Convert.ToDouble(reader.GetValue(reader.GetOrdinal("L_Min")))
                    && this._prodotto.PesoTotaleNastro <= Convert.ToDouble(reader.GetValue(reader.GetOrdinal("P_Max")))
                    && this._prodotto.PesoTotaleNastro >= Convert.ToDouble(reader.GetValue(reader.GetOrdinal("P_Min"))))
                {
                    this._cassainferro.TipologiaCassa[ContatoreConfigurazioni] = reader.GetValue(reader.GetOrdinal("Nome_Cassa")).ToString();
                }
            }
        }
        private async void ControllaFattibilitaTotale()
        {
            // Controlla se c'è almeno una configurazione fattibile
            for (int i = 0; i <= this._cassainferro.FattibilitaTrasporto.Length - 1; i++)
            {
                if (this._cassainferro.FattibilitaTrasporto[i] == true)
                {
                    this.ImballoCalcolabile = true;
                }
            }

            // Se nessuna configurazione è fattibile metto un avviso e non faccio andare avanti
            if (this.ImballoCalcolabile == false)
            {
                await DialogsHelper.ShowConfirmDialog("Non riesco a calcolare le dimensioni dell'imballo, troppo grande.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }
        private void CalcoloCriticitaImballo()
        {
            // Stabilisco la criticità dell'imballo
            if (this.Lunghezza[ContatoreConfigurazioni] > 11000)
            {
                this.Criticita[ContatoreConfigurazioni] = "Alta";
            }
            else
            {
                this.Criticita[ContatoreConfigurazioni] = "Media";
            }
        }
        private void CalcoloNumeroSubbiCorrugati()
        {
            // Ci dice quanti strati di polistirolo ha l'imballo
            this.NumeroCurveCorrugati[ContatoreConfigurazioni + 1] = (contatorestrati - 1) / 2;
            // Ci dice quanti strati di polistirolo ha l'imballo
            this.NumeroCurvePolistirolo[ContatoreConfigurazioni + 1] = contatorestrati / 2;
        }
        private void PossibilitaCassaDoppia()
        {
            // Controllo se la doppia fila mi ci sta in termini di larghezza
            if (this._nastro.Larghezza * 2 + this.TolleranzaCassaDoppia + this.TolleranzaLarghezza < this._cassainferro.LimiteLarghezza[i])
            {
                this._cassainferro.DoppiaFila = true;
            }
            else
            {
                this._cassainferro.DoppiaFila = false;
            }
            
        }
        private void GetPriorityCassaDoppia(int itrasporto)
        {
            // Se la alrghezza del nastro è <600 do priorità alla cassa doppia
            if (this._nastro.Larghezza <= 600)
            {
                this.PriorityCassaDoppia = true;
                this.LimiteLunghezzaCassaDoppia = this._cassainferro.LimiteLunghezza[itrasporto];
            }
            else
            {
                this.LimiteLunghezzaCassaDoppia = this._cassainferro.LimiteLunghezza[itrasporto];
                this.PriorityCassaDoppia = false;
            }

        }
        private void InizializzaContatori()
        {
            this._nastro.LunghezzaImballato = 0;
            if (this.itrasporto < this._cassainferro.FattibilitaTrasporto.Length - 1)
            {
               this._cassainferro.FattibilitaTrasporto[this.itrasporto + 1] = true;
            }
            
            i = 0;
            j = 0;
            contatorestrati = 1;
            altezzacorrugati = 0;
            altezzapolistirolo = 0;
            altezzanastroimballato = 0;
        }
        public List<string> ListaTipologieTrasporto(bool camion)
        {
            List<string> TipologieTrasporto = new List<string>();

            // Mi dice se l'imballo suggerito è già stato comunicato
            bool suggerito = false;

            for (int counter = 0; counter < this._cassainferro.TipoTrasporto.Length; counter++)
            {
                if (camion == true)
                {
                    if (this._cassainferro.FattibilitaTrasporto[counter] == true &&
                        this._cassainferro.FattibilitaCamion[counter] == true )
                    {
                        // Se è il primo, allora suggerisco quello perchè il più conveniente
                        if (this._cassainferro.FattibilitaNave[counter] == false && suggerito == false)
                        {
                            TipologieTrasporto.Add(this._cassainferro.TipoTrasporto[counter] + " (Sugg.)");
                            suggerito = true;
                        }
                        else
                        {
                            TipologieTrasporto.Add(this._cassainferro.TipoTrasporto[counter]);
                        }                      
                    }
                }
                // Vuol dire che ho selezionato la nave come trasporto
                else
                {
                    if (this._cassainferro.FattibilitaTrasporto[counter] == true &&
                        this._cassainferro.FattibilitaNave[counter] == true)
                    {
                        // Se è il primo, allora suggerisco quello perchè il più conveniente
                        if (suggerito == false)
                        {
                            TipologieTrasporto.Add(this._cassainferro.TipoTrasporto[counter] + " (Sugg.)");
                            suggerito = true;
                        }
                        else
                        {
                            TipologieTrasporto.Add(this._cassainferro.TipoTrasporto[counter]);
                        }
                    }
                }

            }
            return TipologieTrasporto;
        }
    }
}
