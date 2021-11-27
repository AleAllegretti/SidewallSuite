using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BeltsPack.Views;
using static BeltsPack.Views.InputView;
using CsvHelper.Configuration;

namespace BeltsPack.Models
{
    public class BordiInfoMap : ClassMap<Bordo>
    {

        public BordiInfoMap()
        {
            Map(Bordo => Bordo.Codice).Name("codice");
            Map(Bordo => Bordo.Descrizione).Name("descrizione");
            Map(Bordo => Bordo.QuantitaDiba).Name("lunghezza");
            Map(Bordo => Bordo.UM).Name("um");
        }
    }
    public class Bordo
    {
        // Codice Applicazione Blk
        public string CodiceApplicazionexBlk { get; set; }
        // Descrizione Applicazione Blk
        public string DescrizioneApplicazioneBlk { get; set; }
        // Quantita Applicazione Blk
        public string QuantitaApplicazioneBlk { get; set; }
        // UM Applicazione Blk
        public string UMApplicazioneBlk { get; set; }
        // Sigla trattamento
        public string SiglaTrattamento { get; set; }
        // Codice Blk
        public string CodiceBlk { get; set; }
        // Descrizione Blk
        public string DescrizioneBlk { get; set; }
        // Quantita Blk
        public string QuantitaBlk { get; set; }
        // UM Blk
        public string UMBlk { get; set; }
        // Codice Fix
        public string CodiceFix { get; set; }
        // Descrizione Fix
        public string DescrizioneFix { get; set; }
        // Quantita Fix
        public string QuantitaFix { get; set; }
        // UM Fix
        public string UMFix { get; set; }
        // Codice Applicazione
        public string CodiceApplicazione { get; set; }
        // Descrizione Applicazione
        public string DescrizioneApplicazione { get; set; }
        // Quantita Applicazione
        public string QuantitaApplicazione { get; set; }
        // UM Applicazione
        public string UMApplicazione { get; set; }
        // Codice Attrezzaggio
        public string CodiceAttrezzaggio { get; set; }
        // Descrizione Attrezzaggio
        public string DescrizioneAttrezzaggio { get; set; }
        // Quantita Attrezzaggio
        public string QuantitaAttrezzaggio { get; set; }
        // UM Attrezzaggio
        public string UMAttrezzaggio { get; set; }
        // Codice raspatura
        public string CodiceRaspatura { get; set; }
        // Descrizione raspatura
        public string DescrizioneRaspatura { get; set; }
        // Quantita raspatura
        public string QuantitaRaspatura { get; set; }
        // UM raspatura
        public string UMRaspatura { get; set; }
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
        // Peso totale
        public double PesoTotale { get; set; }
        // Lunghezza totale
        public double LunghezzaTotale { get; set; }
        // Peso
        public double Peso { get; set; }
        // Passo onda
        public int PassoOnda { get; set; }
        // Altezza
        private int altezza;

		public int Altezza
		{
			get { return altezza; }
			set { altezza = value; }
		}

		// Larghezza
		private int larghezza;

		public int Larghezza
		{
			get { return larghezza; }
			set { larghezza = value; }
		}
		public void SetLunghezzaTotaleBordo(int lunghezzanastro)
        {
			this.LunghezzaTotale = lunghezzanastro * 2;
        }
		public void SetPesoTotale()
        {
			this.PesoTotale = this.LunghezzaTotale * 0.001 * this.Peso;
        }

        public void GetInfoBordo()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingBordiCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp1 = reader.GetValue(reader.GetOrdinal("LarghezzaBase"));
                var temp = reader.GetValue(reader.GetOrdinal("Altezza"));
                if (Convert.ToInt32(temp.ToString()) == this.Altezza & Convert.ToInt32(temp1.ToString()) == this.Larghezza)
                {
                    this.Peso = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso")));
                    this.PassoOnda = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("PassoOnda")));
                    break;
                }
            }

        }
        public List<int> ListaBasiBordo()
        {
            List<int> Basi = new List<int>();

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingBordiCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Altezza"));
                if (Convert.ToInt32(temp) == this.Altezza)
                {
                    Basi.Add(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LarghezzaBase"))));
                }
                
            }

            // Metto gli elementi della lista in ordine crescente
            GFG gg = new GFG();
            Basi.Sort(gg);

            return Basi;
        }
        public List<int> ListaAltezzeBordi()
        {
            List<int> Altezze = new List<int>();

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingBordiCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Altezza"));
                if (temp.ToString() != null)
                {
                    Altezze.Add(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza"))));
                }
            }

            // Metto gli elementi della lista in ordine crescente
            GFG gg = new GFG();
            Altezze.Sort(gg);

            return Altezze;
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
        }
    }
}
