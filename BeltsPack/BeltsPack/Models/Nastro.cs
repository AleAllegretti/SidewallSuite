using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CsvHelper.Configuration;
using System.Windows;
using iTextSharp.text;

namespace BeltsPack.Models
{
    public class NastriInfoMap : ClassMap<Nastro>
    {

        public NastriInfoMap()
        {
            Map(Nastro => Nastro.SpazioDiba).Name("");
            Map(Nastro => Nastro.Codice).Name("");
            Map(Nastro => Nastro.Descrizione).Name("");
            Map(Nastro => Nastro.QuantitaDiba).Name("");
            Map(Nastro => Nastro.SpazioDiba1).Name("");
            Map(Nastro => Nastro.SpazioDiba2).Name("");
            Map(Nastro => Nastro.UM).Name("");
        }
    }
    public class Nastro
    {
        // Massimo sag al top - Valore fisso
        public double S2 { get; set; }
        // Massimo sag at bottom- Valore fisso
        public double s1 { get; set; }
        // Angolo avvoglimento nastro su tamburo
        public double alpha { get; set; }
        // Efficienza puleggia
        public double effPuleggia { get; set; }
        // Capacità richiesta - ton/h
        public double capacityRequiredTon { get; set; }
        // Capacità richiesta - m3/h
        public double capacityRequired { get; set; }
        // Tipo di forma nastro
        public string forma { get; set; }
        // Tipo di edge
        public double edgetype  { get; set; }
        // Peso specifico in larghezza
        public double pesoSpecLargh { get; set; }
        // Angolo d'inclinazione medio
        public double inclinazioneMed { get; set; }
        // Lunghezza orizzontale per nastro inclinato che consideriamo nei calcoli
        public double lunghOriz { get; set; }
        // Coefficiente di lunghezza
        public double lengthCoeff { get; set; }
        // Percentuale di carico extra
        public double caricoExtra { get; set; }
        // Lunghezza tratto di carico
        public double lunghTrattoCarico { get; set; }
        // Elevazione - [m]
        public double elevazione { get; set; }
        // Centro delle distance - [m]
        public double centerDistance { get; set; }
        // Inclincazione - [deg]
        public int inclinazione { get; set; }
        // Lunghezza gradino giunta
        public int LunghGradinoGiunta { get; set; }
        // Range temperatura
        public string RangeTemperatura { get; set; }
        // Spessore superiore
        public int SpessoreSup { get; set; }
        // Spessore inferiore
        public int SpessoreInf { get; set; }
        // Numero tele
        public int NumTele { get; set; }
        // Numero tessuti
        public int NumTessuti { get; set; }
        // Codice Giunzione
        public string CodiceGiunzione { get; set; }
        // Descrizione Giunzione
        public string DescrizioneGiunzione { get; set; }
        // Quantita Giunzione
        public string QuantitaGiunzione { get; set; }
        // UM Giunzione
        public string UMGiunzione { get; set; }
        // Codice Preparazione
        public string CodicePreparazione { get; set; }
        // Descrizione Preparazione
        public string DescrizionePreparazione { get; set; }
        // Quantita Preparazione
        public string QuantitaPreparazione { get; set; }
        // UM Preparazione
        public string UMPreparazione { get; set; }
        // Prova diba
        public string SpazioDiba { get; set; }
        // Prova diba
        public string SpazioDiba1 { get; set; }
        // Prova diba
        public string SpazioDiba2 { get; set; }
        // Codice
        public string Codice { get; set; }
        // Descrizione
        public string Descrizione { get; set; }
        // Unità di misura
        public string UM { get; set; }
        // Trattamento
        public string Trattamento { get; set; }
        // Quantità DiBa
        public double QuantitaDiba { get; set; }
        // Sigla trattamento
        public string SiglaTrattamento { get; set; }
        // Peso totale
        public double PesoTotale { get; set; }
        // Larghezza utile
        public int LarghezzaUtile { get; set; }
        // Peso del nastro
        public double Peso { get; set; }
        // Tipo di nastro
        public string Tipo { get; set; }
        // Sigla tipo di nastro
        public string SiglaTipo { get; set; }
        // Classe del nastro
        public int Classe { get; set; }
        // Spessore nastro
        public int Spessore { get; set; } = 0;
        // Lunghezza nastro
        public int Lunghezza { get; set; }
        // Larghezza nastro
        public int Larghezza { get; set; }
        // Nastro aperto
        public bool Aperto { get; set; }
        // Lunghezza nastro imballato
        private double lunghezzaimballato;
        // Lunghezza giunta
        public double lunghezzaGiunta { get; set; }
        // Tolleranza lunghezza giunta
        public double tollLunghezzaGiunta { get; set; }
        // Velocità nastro - [m/s]
        public double speed { get; set; }

        public double LunghezzaImballato
        {
            get { return lunghezzaimballato; }
            set { lunghezzaimballato = value; }
        }

        public string TipologiaNastro()
        {
            if (this.Aperto == true)
            {
                return ApertoChiuso.Aperto.ToString();
            }
            else
            {
                return ApertoChiuso.Chiuso.ToString();
            }
        }
        public void SetLarghezzautile(int larghezzaBordo, int pistaLaterale)
        {
            this.LarghezzaUtile = this.Larghezza - (larghezzaBordo * 2) - (pistaLaterale * 2);
        }

        public void SetLunghezzaGiunta()
        {
            // Vado a leggere la tolleranza
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingParametriCommand();
            reader = creaComando.ExecuteReader();

            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Elemento"));
                if (temp.ToString() == "TolleranzaGiunte")
                {
                    this.tollLunghezzaGiunta = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Valore")));
                }
            }

            reader.Close();

            // Calcolo la lunghezza della giunta
            this.lunghezzaGiunta = this.Larghezza * 0.3 + this.tollLunghezzaGiunta + this.LunghGradinoGiunta * (this.NumTessuti - 1);
        }

        public void SetCaratterisitche()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            try
            {
                // Crea il comando SQL
                SqlDataReader reader;
                SqlCommand creaComando = dbSQL.CreateSettingNastriCommand();
                reader = creaComando.ExecuteReader();
                while (reader.Read())
                {
                    var temp = reader.GetValue(reader.GetOrdinal("NomeNastro"));
                    var temp1 = reader.GetValue(reader.GetOrdinal("Classe"));
                    if (temp.ToString() == this.Tipo & Convert.ToInt32(temp1.ToString()) == this.Classe)
                    {
                        this.Peso = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("PesoMQ")));
                        this.SpessoreSup = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("SpessoreSup")));
                        this.SpessoreInf = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("SpessoreInf")));
                        this.NumTele = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("NumeroTele")));
                        this.NumTessuti = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("NumeroTessuti")));
                        this.SiglaTipo = reader.GetValue(reader.GetOrdinal("SiglaNastro")).ToString();
                        this.LunghGradinoGiunta = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LunghezzaGradinoGiunta")));
                        break;
                    }
                }
            }
            catch
            {
                //System.Windows.MessageBox.Show("Assicurati che il nastro che hai scelto abbia tutte le CARATTERISTICHE (peso, costo ecc...) nel menù impostazioni.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Calcolo la lunghezza della giunta
            this.SetLunghezzaGiunta();
        }
        public void SetPeso()
        {
            this.PesoTotale = this.Larghezza * this.Lunghezza * Math.Pow(10, -6) * this.Peso;
        }

        public void SetTrattamentoSigla(string key)
        {
            IDictionary<string, string> siglaTrattamento = new Dictionary<string, string>();
            siglaTrattamento.Add("AY Abrasion Resistant", "AY"); //adding a key/value using the Add() method
            siglaTrattamento.Add("AW Extra Abr. Resistant", "AW");
            siglaTrattamento.Add("HR 130", "HR");
            siglaTrattamento.Add("HR 150", "MX");
            siglaTrattamento.Add("OR Oil Resistant", "OR");
            siglaTrattamento.Add("FRK Self-Extinguish", "FRK");
            siglaTrattamento.Add("ORK Oil Res. and Self-Ext.", "ORK");

            // Determino la sigla del trattamento
            this.SiglaTrattamento = siglaTrattamento[key];

            this.SetTemperatureRange(key);
        }
        public void SetTemperatureRange(string key)
        {
            IDictionary<string, string> tempTrattamento = new Dictionary<string, string>();
            tempTrattamento.Add("AY Abrasion Resistant", "-20° ÷  60°"); //adding a key/value using the Add() method
            tempTrattamento.Add("AW Extra Abr. Resistant", "-50° ÷  60°");
            tempTrattamento.Add("HR 130", "Up to 130°");
            tempTrattamento.Add("HR 150", "Up to 150°");
            tempTrattamento.Add("OR Oil Resistant", "-20° ÷  60°");
            tempTrattamento.Add("FRK Self-Extinguish", "-20° ÷  60°");
            tempTrattamento.Add("ORK Oil Res. and Self-Ext.", "-20° ÷  60°");

            // Determino la sigla del trattamento
            this.RangeTemperatura = tempTrattamento[key];
        }
        public List<string> ListaClassiNastro(string NomeNastro)
        {
            List<string> ClassiNastro = new List<string>();

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingDistinctClasseCommand(NomeNastro);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Classe"));
                if (temp.ToString() != null)
                {
                    ClassiNastro.Add(reader.GetValue(reader.GetOrdinal("Classe")).ToString());
                }
            }

            // Metto gli elementi della lista in ordine crescente

            return ClassiNastro;
        }
        public List<string> ListaTiplogieNastro()
        {
            List<string> TipologieNastro = new List<string>();

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingDistinctNastriCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("NomeNastro"));
                if (temp.ToString() != null)
                {
                    TipologieNastro.Add(reader.GetValue(reader.GetOrdinal("NomeNastro")).ToString());
                }
            }

            // Metto gli elementi della lista in ordine crescente

            return TipologieNastro;
    }
        public void SetLengthCoeff()
        {
            if(this.centerDistance == 0)
            {
                this.lengthCoeff = 0;
            }
            else if (this.centerDistance < 55)
            {
                this.lengthCoeff = 2.4 + 2.8 * (Math.Pow(1.8, (-0.22 * (this.centerDistance - 16))));
            }
            else if (this.centerDistance >= 251)
            {
                this.lengthCoeff = 1.03 + (Math.Pow(8.31, -0.001 * (this.centerDistance + 325)));
            }
            else
            {
                this.lengthCoeff = 1.38 + 0.92 * (Math.Pow(80, -0.004 * (this.centerDistance - 52)));
            }
            this.lengthCoeff = Math.Round(this.lengthCoeff, 2);
        }
        public void SetLunghOriz()
        {
            if((Math.Pow(this.centerDistance,2) - Math.Pow(this.elevazione,2))>0)
            {
                this.lunghOriz = Math.Pow((Math.Pow(this.centerDistance, 2) - Math.Pow(this.elevazione, 2)), 0.5);
            }
        }
        public void SetAngoloMedio()
        {
            if(this.lunghOriz > 0)
            {
                this.inclinazioneMed = Math.Atan(this.elevazione / this.lunghOriz);
            }
        }
        public void SetSpecWeightWidth()
        {
            this.pesoSpecLargh = this.Peso * 1000 / this.Larghezza;
        }

        public void SetLunghezzaDaCalcoli(int wheelDiam, int pulleyDiam, double centDist, string shape, bool aperto)
        {
               switch (shape)
                {
                    case "S-Shape":
                        this.Lunghezza = Convert.ToInt32(Math.Round(1.5 * Math.PI * pulleyDiam + Math.PI * wheelDiam  * 0.5 + centDist * 2 * 1000,0));
                        break;

                    case "L-Shape":
                        this.Lunghezza = Convert.ToInt32(Math.Round(1.25 * Math.PI * pulleyDiam  * 1.25 + Math.PI * wheelDiam * 0.25 + centDist * 2 * 1000));
                        break;

                    case "Dritto":
                        this.Lunghezza = Convert.ToInt32(Math.Round(Math.PI * pulleyDiam + centDist * 2 * 1000));
                        break;
                }
        }
        public void SetLunghezzaTrattodiCarico()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingsCalcoliCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("ID"));
                if (temp.ToString() == "6")
                {
                    this.lunghTrattoCarico = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                if (temp.ToString() == "7")
                {
                    this.caricoExtra = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                if (temp.ToString() == "8")
                {
                    this.effPuleggia = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                if (temp.ToString() == "9")
                {
                    this.s1 = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                if (temp.ToString() == "10")
                {
                    this.S2 = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
            }

            // Angolo avvolgimento nastro su tamburo
            this.alpha = Math.PI;
        }
    }
    
    enum ApertoChiuso
    {
        Aperto,
        Chiuso
    }

}
