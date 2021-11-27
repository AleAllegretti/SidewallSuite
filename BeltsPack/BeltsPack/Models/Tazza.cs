﻿using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class Tazza
    {
        // Codice Applicazione
        public string CodiceApplicazione { get; set; }
        // Descrizione Applicazione
        public string DescrizioneApplicazione { get; set; }
        // Quantita Applicazione
        public string QuantitaApplicazione { get; set; }
        // UM Applicazione
        public string UMApplicazione { get; set; }
        // Codice raspatura
        public string CodiceRaspatura { get; set; }
        // Descrizione raspatura
        public string DescrizioneRaspatura { get; set; }
        // Quantita raspatura
        public string QuantitaRaspatura { get; set; }
        // UM raspatura
        public string UMRaspatura { get; set; }
        // peso totale
        public double PesoTotale { get; set; }
        // Lunghezza totale
        public int LunghezzaTotale { get; set; }
        // Peso
        public double Peso { get; set; }
        // Passo
        public int Passo { get; set; }
        // Numero tazze
        public int Numero { get; set; }

        // Lunghezza
        public int Lunghezza { get; set; }
        // Altezza
        private int altezza;

		public int Altezza
		{
			get { return altezza; }
			set { altezza = value; }
		}

		// Forma
		private string forma;

		public string Forma
		{
			get { return forma; }
			set { forma = value; }
		}

		// Larghezza base
		private int larghezza;

		public int Larghezza
		{
			get { return larghezza; }
			set { larghezza = value; }
		}

		public void NumeroTazzeTotali(double lunghezzaNastro, int passo)
        {
            // Calcolo il numero delle tazze totali
            try
            {
                this.Numero = Convert.ToInt32(Math.Round(lunghezzaNastro / passo));
            }
            catch
            {
                this.Numero = 0;
            }
        }
		public void CarattersticheTazza()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingTazzeCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Altezza"));
                var temp1 = reader.GetValue(reader.GetOrdinal("Forma" + this.Forma));
                if (Convert.ToInt32(temp.ToString()) == this.Altezza & temp1.ToString() == "x")
                {
                    this.Peso = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Peso" + this.Forma)));
                    this.Larghezza = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LarghezzaTazze" + this.Forma)));
                    break;
                }
            }
        }
        public void SetLunghezzaTotale(int larghezzaUtile)
        {
            this.LunghezzaTotale = this.Numero * larghezzaUtile;
        }
        public void SetPesoTotale()
        {
            // Peso in kg
            this.PesoTotale = (this.Peso * this.LunghezzaTotale * 0.001);
        }
	}
}
