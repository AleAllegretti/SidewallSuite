using BeltsPack.Models;
using iTextSharp.text;
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Utils
{
    public class CalcoliImpianto
    {

        // CAPACITY OF THE BELT
        // OUTPUT
        // Potenza motore
        public double MotorPower { get; set; }
        // Required take-up at tail
        public double TakeUpTail { get; set; }
        // Max working tension
        public double MaxWorkTens { get; set; }
        // Max tension at lateral spaces
        public double MaxWorkTensLat { get; set; }
        // Coefficiente di sicurezza nastro
        public double Sfactor { get; set; }
        // Coefficiente di sicurezza piste laterali
        public double Sfactor_pista { get; set; }
        // Minimum motor power - [kW]
        public double Pmin { get; set; }
        // Required power - [kW]
        public double Pa { get; set; }
        // Fvmin
        public double Fvmin { get; set; }
        // Fv
        public double Fv { get; set; }
        // Take 1
        public double Tvn { get; set; }
        // Take up 2
        public double Tv1 { get; set; }
        // Take up 3
        public double Tv2 { get; set; }
        // Take up 4
        public double Tv3 { get; set; }
        // Minima tensione ammissibile nel lato della portata
        public double Tsup { get; set; }
        // Minima tensione ammissibile nel lato inferiore
        public double Tinf { get; set; }
        // Parametro per l'extra take-up
        public double extratakeup { get; set; }
        // Tensione in testa al tamburo - in
        public double TH1n { get; set; }
        // Tensione in testa al tamburo - out
        public double TH11n { get; set; }
        // Tensione totale in testa al tamburo - in
        public double TTH1n { get; set; }
        // Tensione totale in testa al tamburo - out
        public double TTH11n { get; set; }
        // Coefficiente di tensione tight-side
        public double T1n { get; set; }
        // Coefficiente di tensione slack-side
        public double T2n { get; set; }
        // Fattore di attrito
        public double mu { get; set; }
        // Coefficiente di avvolgimento
        public double k { get; set; }
        // Totale forze periferiche
        public double totForcePeriphe { get; set; }
        // Totale forza resistente lato ritorno
        public double totPeripheForceRet { get; set; }
        // Totale forza resistente lato portante
        public double totPeripheForceCar { get; set; }
        // Forza resistente speciale portata
        public double peripheForce5 { get; set; }

        // Forza resistente speciale portata
        public double peripheForce6 { get; set; }
        // Energia potenziale materiale
        public double matPotEnergy { get; set; }
        // Material inertia
        public double matInertia { get; set; }
        // Totale forza resistente
        public double totForzaResistante { get; set; }
        // Forza periferica secondaria
        public double peripheForce3 { get; set; }
        // Forza periferica secondaria - 2
        public double peripheForce4 { get; set; }
        // Forza pèeriferica secondaria nastro vuoto
        public double peripheForce2 { get; set; }

        // Forza periferica nastro vuoto
        public double peripheForce { get; set; }
        // Altezza utile listelli - l
        public int HLuse { get; set; }
        // Sezione listello - Sa
        public double Sezlis { get; set; }
        // Filling height - H1
        public double FillingHeight { get; set; }
        // inclinazione nastro in radianti - Alpha
        public double AlphaRad { get; set; }
        // Sezione di riempimento - Sb
        public double Sb { get; set; }
        // Friction height. Altezza del materiale dovuta al fatto che non scivola per l'inclinazione - H2
        public double FriHeight { get; set; }
        // Lunghezza friction section - l2
        public double FricLength { get; set; }
        // Lunghezza superiore di frizione - l1
        public double FricLength2 { get; set; }
        // Altezza sezione di frizione - H3
        public double FricHeight3 { get; set; }
        // Sezione di frizione
        public double FricSection { get; set; }
        // Capacità di ogni listello
        public double CleatCapacity { get; set; }
        // Sezione d'interferenza
        public double PitchSection { get; set; }
        // Interferenza - Su
        public double CleatInterference { get; set; }
        // Volume di materiale contenuto in ciascun listello
        public double CleatVolume { get; set; }
        // Capacità del nastro totale
        public double Q { get; set; }
        // Capacità effettiva del nastro
        public double Qeff { get; set; }
        // Fattore di riempimento
        public double fillFactor { get; set; }
        // Capacità massica
        public double Qteff { get; set; }

        // Oggetti
        private Nastro _nastro;
        private Tazza _tazza;
        private Prodotto _prodotto;
        private Bordo _bordo;
        private Materiale _material;
        private Rullo _rullo;
        private Tamburo _tamburo;
        private Motore _motore;

        // Costruttore
        public CalcoliImpianto(Nastro nastro, Tazza tazza, Prodotto prodotto, Bordo bordo, 
            Materiale material, Rullo rullo, Tamburo tamburo, Motore motore)
        {
            this._nastro = nastro; ;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._bordo = bordo;
            this._material = material;
            this._rullo = rullo;
            this._tamburo = tamburo;
            this._motore = motore;
        }

        public void GetCapacity()
        {
            // Definisco le prorpietà del motore
            this._motore.GetCarattersticheMotore();

            // Definisco le caratteristiche del tamburo
            this._tamburo.GetCarattersticheTamburo();

            // Definisco le caratteristiche del rullo
            this._rullo.GetCarattersticheRullo();

            //Calcolo l'altezza di riempimento. Cioè fino che punto il materiale sarà contenuto tra i listelli [mm]
            this.FillingHeight = this._tazza.AltezzaUtile * Math.Tan(Math.PI / 2 - this.FromDegToRad(this._nastro.inclinazione));

            // Calcolo la sezione di riempimento del materiale [dm2]
            this.Sb = Math.Round((this._tazza.AltezzaUtile * this.FillingHeight) / 20000, 3);

            // Altezza relativa al fatto che il materiale non scivola per l'inclinazione
            this.FriHeight = this._tazza.AltezzaUtile * Math.Tan(Math.PI / 2 - this.FromDegToRad(this._nastro.inclinazione) + this.FromDegToRad(this._material.surchAngle));

            // Lunghezza frizione materiale
            this.FricLength = (this._tazza.AltezzaUtile) / Math.Cos(Math.PI / 2 - this.FromDegToRad(this._nastro.inclinazione));

            // Lunghezza superiore di frizione
            this.FricLength2 = (this._tazza.AltezzaUtile) / Math.Cos(Math.PI / 2 - this.FromDegToRad(this._nastro.inclinazione) + this.FromDegToRad(this._material.surchAngle));

            // Altezza sezione di frizione
            this.FricHeight3 = this.FricLength * Math.Sin(this.FromDegToRad(this._material.surchAngle));

            // Sezione di frizione - dm2
            this.FricSection = Math.Round((this.FricHeight3 * this.FricLength2) / 20000, 3);

            // Calcolo la capacità di ciascun listello
            this.CleatCapacity = this._tazza.Sezione + this.Sb + this.FricSection;

            // Calcolo l'interferenza
            this.CleatInterference = this._tazza.Spessore + this.FriHeight - this._tazza.Passo;

            // A seconda che l'interferenza sia negativa o pisitiva calcolo la sezione effettiva
            if (this.CleatInterference <= 0)
            {
                this.PitchSection = this.CleatCapacity;
            }
            else
            {
                this.PitchSection = Math.Round(this.CleatCapacity - Math.Pow(this.CleatInterference, 2) * Math.Tan(this.FromDegToRad(this._nastro.inclinazione) - this.FromDegToRad(this._material.surchAngle)) / 20000, 3);
            }

            // Faccio l'eccezione nel caso in cui l'angolo di carico sia maggiore dell'inclinazione del nastro
            if (this._material.surchAngle >= this._nastro.inclinazione)
            {
                this.PitchSection = Math.Round(this._tazza.Sezione + this._tazza.AltezzaUtile * (this._tazza.Passo - this._tazza.Spessore) / 10000, 3);
            }

            // Calcolo il volume di materiale che ogni tazza può contenere [dm3]
            this.CleatVolume = this.PitchSection * this._nastro.LarghezzaUtile / 100;

            // Capacità totale del nastro senza considerare il filling factor - m3/h
            if (this._tazza.Passo > 0)
            {
                this.Q = Math.Round(this.CleatVolume / this._tazza.Passo * this._nastro.speed * 3600, 3);
            }

            // Capacità effettiva considerando il fattore di riempimento -m3/h
            this.Qeff = Math.Round(this.Q * this._material.fillFactor, 1);

            // Capacità massica - ton/h
            this.Qteff = Math.Round(Qeff * this._material.density, 1);
        }

        public double FromDegToRad(double deg)
        {
            double rad;
            rad = deg * Math.PI / 180;
            return rad;
        }

        public void TensionsCalculation()
        {
            // Trasformo la capacità richiesta in m3/h
            this._nastro.capacityRequired = this._nastro.capacityRequiredTon / this._material.density;

            // Calcolo la lunghezza del tratto di carico del nastro e il carico extra
            this._nastro.SetLunghezzaTrattodiCarico();

            // Calcolo il peso specifico in larghezza
            this._nastro.SetSpecWeightWidth();

            // Setto l'incilinazione media del nastro
            this._nastro.SetLunghOriz();
            this._nastro.SetAngoloMedio();

            // Calcolo la forza periferica
            double G = 9.80665;

            // Calcolo il peso al metro lineare del prodotto
            this._prodotto.PesoM2 = this._prodotto.PesoTotaleNastro / (this._nastro.Lunghezza*Math.Pow(10,-3) * this._nastro.Larghezza * Math.Pow(10, -3));

            // Calcoli il peso del rullo
            this._rullo.GetRulloWeight(this._nastro.Larghezza);

            this.peripheForce = G * (this._nastro.lengthCoeff * this._rullo.coeffAttrito * this._nastro.centerDistance *
                (this._prodotto.PesoM2 * this._nastro.Larghezza / 1000 * Math.Cos(this._nastro.inclinazioneMed) +
                this._rullo.peso / this._rullo.passo) + this._nastro.centerDistance * this._prodotto.PesoM2 * this._nastro.Larghezza / 1000 *
                Math.Sin(this._nastro.inclinazioneMed));

            this.peripheForce2 = G * (this._nastro.lengthCoeff * this._rullo.coeffAttrito * this._nastro.centerDistance *
                (this._prodotto.PesoM2 * this._nastro.Larghezza / 1000 * Math.Cos(this._nastro.inclinazioneMed) +
                this._rullo.peso / this._rullo.passo) - this._nastro.centerDistance * this._prodotto.PesoM2 * this._nastro.Larghezza / 1000 *
                Math.Sin(this._nastro.inclinazioneMed));

            // Calcolo il totale della forza resistente del nastro a vuoto [N]
            this.totForzaResistante = this.peripheForce + this.peripheForce2;

            // Calcolo l'ineria del materiale
            if (this._nastro.speed != 0)
            {
                this.matInertia = (G * this._nastro.lengthCoeff * this._rullo.coeffAttrito * this._nastro.centerDistance *
                this._nastro.capacityRequired * Math.Cos(this._nastro.inclinazioneMed)) / (3.6 * this._nastro.speed);

                this.matPotEnergy = (G * this._nastro.capacityRequired * this._nastro.elevazione) / (3.6 * this._nastro.speed);
            }

            // Calcolo le restanti forze resistenti
            this.peripheForce3 = G * 9 * this._nastro.lunghTrattoCarico;
            this.peripheForce4 = G * 21 * this._nastro.lunghTrattoCarico;

            // Calcolo le ulteriori resistenze sia all'andata che al ritorno
            this.peripheForce5 = this.totForzaResistante / 10;
            this.peripheForce6 = this.totForzaResistante / 10;

            // Totale forza resistente lato portante
            this.totPeripheForceCar = this.peripheForce + this.matInertia + this.matPotEnergy + this.peripheForce3 + this.peripheForce4 + this.peripheForce5;

            // Totale forza resistente lato ritorno
            this.totPeripheForceRet = this.peripheForce2 + this.peripheForce6;

            // Totale forza resistente al tamburo
            this.totForcePeriphe = this.totPeripheForceCar + this.totPeripheForceRet;

            // Altre variabili utili
            double dTv, T2, T1, TH1, TH11, TT1, TT11, TT1n, TT11n, F_v, Tmax, fsl, Nu, Plut, WheelWidth;
            TT1n = 0;
            TT11n = 0;
            Nu = 0.85;

            if (this._nastro.centerDistance > 0)
            {
                // Calcolo il coefficiente di avvolgimento
                this.k = 1 / (Math.Pow(2.718, (this._nastro.effPuleggia * this._nastro.alpha)) - 1);

                // Primo coefficiente di tensione - Tight-side
                T1n = Math.Abs(this.totForcePeriphe) * (this.k + 1);

                // Secondo coefficinte di tensione - slack-side
                T2n = Math.Abs(this.totForcePeriphe) * this.k;

                if (totForcePeriphe >= 0)
                {
                    // Tensione testa del tamburo
                    this.TH1n = this.T1n;
                    this.TH11n = this.T2n;
                    TT1n = this.T2n + this.totPeripheForceRet;
                    TT11n = this.T2n + this.totPeripheForceRet;
                }
                else
                {
                    this.TH1n = 0;
                    this.TH11n= 0;
                    this.TTH1n = 0;
                }
            }

            if (this._nastro.centerDistance > 0)
            {
                this.Tvn = this.T2n + this.totPeripheForceRet;
            }

            this.Tv1 = this.Tvn * (this._nastro.caricoExtra);

            if (this._nastro.speed > 0)
            {
                this.Tsup = (this._rullo.passo / (8 * this._nastro.s1)) * G * (this._prodotto.PesoM2 * this._nastro.Larghezza / 1000 + this._nastro.capacityRequired / (3.6 * this._nastro.speed));
                this.Tinf = (this._rullo.passo / (8 * this._nastro.S2)) * G * this._prodotto.PesoM2 * this._nastro.Larghezza / 1000;
            }

            this.Tv2 = this.Tsup;
            this.Tv3 = this.Tinf;

            Fvmin = Math.Max(Tv1, Tv2);
            Fvmin = 2 * Math.Max(Tv3, Fvmin);

            F_v = Fvmin;

            // required take-up at tail
            this.TakeUpTail = Math.Round(F_v / G,1);

            dTv = F_v / 2 - Tvn;

            T2 = T2n + dTv;
            T1 = T1n + dTv;
            TH1 = TH1n + dTv;
            TH11 = TH11n + dTv;
            TT1 = TT1n + dTv;
            TT11 = TT11n + dTv;

            Tmax = Math.Max(Math.Max(T1, TH1), Math.Max(TH11, Math.Max(TH1, TT11)));

            // Max working tension at pulley
            if (this._nastro.Larghezza > 0)
            {
                this.MaxWorkTens = Math.Round(Tmax / this._nastro.Larghezza,1);
            }

            if (this.MaxWorkTens > 0)
            {
                fsl = this._nastro.Classe / this.MaxWorkTens;
            }

            // Motor power calculation
            this.Pa = Math.Round(this.totForcePeriphe * this._nastro.speed / 1000,1);
            this.Pmin = Math.Round(Pa / Nu,1);
            this._motore.GetMotorPower(Pmin);

            // Calcolo la larghezza minima della puleggia
            if (this._prodotto.PistaLaterale > 0)
            {
                this._bordo.MinWheelWidth = Math.Max(0.8 * this._prodotto.PistaLaterale, this._prodotto.PistaLaterale - 40);
            }
            else
            {
                this._bordo.MinWheelWidth = 0;
            }

            // Belt class and safety factors
            if (this._nastro.Larghezza > 0)
            {
                this.MaxWorkTens = Math.Round(T1 / this._nastro.Larghezza,1);
            }

            Plut = Math.Max(0.8 * this._prodotto.PistaLaterale, this._prodotto.PistaLaterale - 40);
            Plut = Plut - this._nastro.edgetype;

            // Max working tension at free lateral spaces
            if (this._prodotto.PistaLaterale > 0)
            {
                WheelWidth = Plut;
                this._bordo.GetInfoBordo();

                if (this._nastro.forma == "S-Shape")
                {
                    this.MaxWorkTensLat = Math.Round(T2 / (2 * Plut));

                }
                else if (this._nastro.forma == "L-Shape")
                {
                    this.MaxWorkTensLat = Math.Round(TT1 / (2 * Plut));
                }
                else
                {
                    this.MaxWorkTensLat = 0;
                    WheelWidth = 0;
                    this._bordo.MinWheelDiam = 0;
                }
            }

            if (this.MaxWorkTens > 0)
            {
                this.Sfactor = Math.Round(this._nastro.Classe / this.MaxWorkTens,1);
            }

            if (this.MaxWorkTensLat > 0)
            {
                this.Sfactor_pista = Math.Round(this._nastro.Classe / this.MaxWorkTensLat,1);
            }
            else
            {
                this.Sfactor_pista = 0;
            }
        }

    }

}
