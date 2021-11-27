﻿using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CsvHelper.Configuration;

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
        public  double QuantitaDiba { get; set; }
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
        public void SetCaratterisitche()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

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
                    break;
                }
            }
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
        }
    }

    
    enum ApertoChiuso
    {
        Aperto,
        Chiuso
    }

}