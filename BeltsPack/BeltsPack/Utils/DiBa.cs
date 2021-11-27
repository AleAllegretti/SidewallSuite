﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using BeltsPack.Models;
using BeltsPack.Views.Dialogs;
using CsvHelper;
using CsvHelper.Configuration;

namespace BeltsPack.Utils
{
    public class DiBa
    {
        // Oggetti
        private Nastro _nastro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;

        // Variabili
        bool[] StatoCodici = new bool[15];
        bool allertCodiceMancante;
        IDictionary<int, string> mapCodici = new Dictionary<int, string>();
        public DiBa(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto)
        {
            // Oggetti che mi servono nella classe
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;

            // Inizializzo le variabili
            allertCodiceMancante = false;

            // Inizializzo la mappa dei codici
            mapCodici.Add(0, "Nastro base");
            mapCodici.Add(1, "Bordo");
            mapCodici.Add(2, "Raspatura bordo");
            mapCodici.Add(3, "Raspatura tazze");
            mapCodici.Add(4, "Attrezzaggio linea");
            mapCodici.Add(5, "Applicazione bordo");
            mapCodici.Add(6, "Applicazione tazze");
            mapCodici.Add(7, "Preparazione bordo");
            mapCodici.Add(8, "Fix");
            mapCodici.Add(9, "Blinkers");
            mapCodici.Add(10, "Applicazione blinkers");
            mapCodici.Add(11, "Giunzione");
            mapCodici.Add(12, "Articolo nastro sidewall");
            mapCodici.Add(13, "Imballo");
            mapCodici.Add(14, "Trasporto");

        }

        public void SearchCodnastro(string tipo,
                                    int classe,
                                    double larghezza,
                                    string trattamento,
                                    string famiglia = "NAS",
                                    string gruppo = "NAS",
                                    string sottogruppo = "NAS")
        {
            // Inizilizzo la larghezza massima
            double largh_min = 3;
            larghezza = larghezza * 0.001;

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ArticleSearchCommand(tipo, trattamento, famiglia, gruppo, sottogruppo, classe, larghezza);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                string temp_CdArt = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString().ToLower();
                double temp_Largh = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LarghezzaMKS")));
                
                if (temp_Largh >= larghezza &&
                    largh_min >= temp_Largh)
                {
                    // Prendo le caratteristiche del nastro
                    this._nastro.Codice = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                    this._nastro.Descrizione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                    this._nastro.UM = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                    // Per considerare la larghezza maggiore più vicina alla larghezza del nastro
                    largh_min = temp_Largh;
                }
               
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._nastro.Codice, 0);

        }

        // Controllo la presenza del codice
        private void PresenzaCodice(string codice, int indice)
        {
            if (codice == null)
            {
                StatoCodici[indice] = true;

                // Comunico che manca almeno un codice
                allertCodiceMancante = true;
            }
            else
            {
                StatoCodici[indice] = false;

            }
        }
        public void searchCodBordo(int altezza,
                                    double larghezza,
                                    string trattamento,
                                    string tipo = "BORDO",
                                    string famiglia = "BOR",
                                    string gruppo = "BOR",
                                    string sottogruppo = "BOR")
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.BordoSearchCommand(tipo, altezza, larghezza * 0.001, trattamento, famiglia, gruppo, sottogruppo);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                // Prendo le caratteristiche del nastro
                this._bordo.Codice = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._bordo.Descrizione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._bordo.UM = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._bordo.Codice, 1);
        }

        public void searchCodRaspaturaBordo(string gruppo,
                                    int altezza,
                                    string trattamento)
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Controllo il trattamento
            if (trattamento != "HR")
            {
                trattamento = "STD";
            }

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.RaspaturaBordoSearchCommand(gruppo, altezza, trattamento);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                    // Prendo le caratteristiche del nastro
                    this._bordo.CodiceRaspatura = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                    this._bordo.DescrizioneRaspatura = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                    this._bordo.UMRaspatura = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                    break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._bordo.CodiceRaspatura, 2);
        }

        public void searchCodRaspaturaTazze(string gruppo,
                                    int altezza,
                                    string trattamento,
                                    string forma)
    {
        // Crea il wrapper del database
        DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
        dbSQL.OpenConnection();

        // Controllo il trattamento
        if (trattamento != "HR")
        {
            trattamento = "STD";
        }

        // Crea il comando SQL
        SqlDataReader reader;
        SqlCommand creaComando = dbSQL.RaspaturaTazzeSearchCommand(gruppo, altezza, trattamento, forma);
        reader = creaComando.ExecuteReader();
        while (reader.Read())
        {
            this._tazza.CodiceRaspatura = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
            this._tazza.DescrizioneRaspatura = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
            this._tazza.UMRaspatura = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();           

            break;
        }
        // Controllo che il codice del nastro sia presente
        this.PresenzaCodice(this._tazza.CodiceRaspatura, 3);
        }
        public void SearchCodAttAppBor(string sottogruppo,
                                    string famiglia,
                                    int altezza)
        {

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.AttrezzaggioSearchCommand(altezza, famiglia, sottogruppo);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                if (sottogruppo == "ATR")
                {
                    this._bordo.CodiceAttrezzaggio = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                    this._bordo.DescrizioneAttrezzaggio = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                    this._bordo.UMAttrezzaggio = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                    
                }
                else
                {
                    this._bordo.CodiceApplicazione = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                    this._bordo.DescrizioneApplicazione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                    this._bordo.UMApplicazione = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                    
                }
            }
            if (sottogruppo == "ATR")
            {
                // Controllo che il codice del nastro sia presente
                this.PresenzaCodice(this._bordo.CodiceAttrezzaggio, 4);
            }
            else
            {
                // Controllo che il codice del nastro sia presente
                this.PresenzaCodice(this._bordo.CodiceApplicazione, 5);
            }    
        }

        public void searchCodApplicazioneTazze(string famiglia,
                                   string gruppo,
                                   string sottogruppo,
                                   int altezza,
                                   string forma)
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Considero la descrizione in locale
            string lunghezzaTazza;
            int lunghezzaTazzaNum;
            int lunghezzaMin = 3000;

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ApplicazioneTazzaSearchCommand(altezza, famiglia, sottogruppo, forma);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                // Prendo la descrizione
                this._tazza.DescrizioneApplicazione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();

                // Considero solo l'ultima parte della descrizione per vedere la lunghezza
                lunghezzaTazza = this._tazza.DescrizioneApplicazione.Substring(25);

                // Elimino gli spazi vuoti
                lunghezzaTazza = lunghezzaTazza.Replace(" ", "");
                lunghezzaTazza = lunghezzaTazza.Replace("x", "");
                lunghezzaTazza = lunghezzaTazza.Replace("X", "");

                // Converto la string in numero per vederne la lunghezza corretta
                lunghezzaTazzaNum = Convert.ToInt32(lunghezzaTazza);

                // Vedo se la lunghezza della mia tazza è minore di quella che sto leggendo a dB
                if (lunghezzaTazzaNum >= this._tazza.Lunghezza && lunghezzaTazzaNum <lunghezzaMin)
                {
                    // Mi tengo buono questo valore
                    lunghezzaMin = lunghezzaTazzaNum;

                    // Prendo le caratteristiche del nastro
                    this._tazza.CodiceApplicazione = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                    this._tazza.UMApplicazione = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                    break;
                }
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._tazza.CodiceApplicazione, 6);
        }

        public void SearchCodFix(string sottogruppo,
                                    string famiglia,
                                    int altezza)
        {
            // Capisco che tipologia di fix devo utilizzare in base all'altezza del bordo
            string descrizioneFix;
            descrizioneFix = "";

            if (altezza <= 160)
            {
                descrizioneFix = "160";
            }
            else if (altezza > 160 && altezza <= 240)
            {
                descrizioneFix = "240";
            }
            else if (altezza == 300)
            {
                descrizioneFix = "300";
            }
            else if (altezza > 300 && altezza <= 630)
            {
                descrizioneFix = "630";
            }

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.FixSearchCommand(descrizioneFix, famiglia, sottogruppo);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._bordo.CodiceFix = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._bordo.DescrizioneFix = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._bordo.UMFix = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._bordo.CodiceFix, 8);
        }
        public void SearchCodBlk(string sottogruppo,
                                    string famiglia,
                                    int altezza,
                                    string trattamento)
        {
            // Capisco che tipologia di fix devo utilizzare in base all'altezza del bordo
            string descrizioneBlk;
            descrizioneBlk = "";

            if (altezza <= 140)
            {
                descrizioneBlk = "140";
            }
            else if (altezza > 140 && altezza <= 220)
            {
                descrizioneBlk = "220";
            }
            else if (altezza == 280)
            {
                descrizioneBlk = "280";
            }

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.BlkSearchCommand(descrizioneBlk, famiglia, sottogruppo, trattamento);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._bordo.CodiceBlk = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._bordo.DescrizioneBlk = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._bordo.UMBlk = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._bordo.CodiceBlk, 9);
        }
        public void SearchCodPrepNastro(string sottogruppo,
                                   string famiglia,
                                   int altezza)
        {

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.PreparazioneSearchCommand(altezza, famiglia, sottogruppo);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._nastro.CodicePreparazione = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._nastro.DescrizionePreparazione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._nastro.UMPreparazione = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._nastro.CodicePreparazione, 7);
        }
        public void SearchCodGiunzione(string codice)
        {

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.GiunzioneSearchCommand(codice);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._nastro.CodiceGiunzione = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._nastro.DescrizioneGiunzione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._nastro.UMGiunzione = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._nastro.CodiceGiunzione, 11);
        }
        public void SearchCodApplicazioneBlk(string codice)
        {

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ApplicazioneBlkSearchCommand(codice);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._bordo.CodiceApplicazionexBlk = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._bordo.DescrizioneApplicazioneBlk = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._bordo.UMApplicazioneBlk = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._bordo.CodiceApplicazionexBlk, 10);
        }
        public void SearchCodProdotto(int altezza,
                                    string tipologiaProdotto)
        {
            // In base alla tipologia del nastro scelgo il codice opportuno
            string descrizione;
            descrizione = "";

            if (tipologiaProdotto == "Solo tazza")
            {
                descrizione = "Cleated belt";
            }

            if (tipologiaProdotto == "Bordi e tazze" || tipologiaProdotto == "Solo bordi")
            {
                descrizione = "Nastro trasportatore tipo Sidewall " + altezza;
            }

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ProdottoSearchCommand(descrizione);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._prodotto.CodiceProdotto = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._prodotto.DescrizioneProdotto = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._prodotto.UMProdotto = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._prodotto.CodiceProdotto, 12);
        }
        public void SearchCodImballo(int lunghezza)
        {
            // In base alla tipologia del nastro scelgo il codice opportuno
            string descrizione;
            descrizione = "";

            if (lunghezza > 30)
            {
                descrizione = "Gabbia in ferro";
            }
            else
            {
                descrizione = "Pedana";
            }

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ImballoSearchCommand(descrizione);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._prodotto.CodiceImballo = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._prodotto.DescrizioneImballo = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._prodotto.UMImballo = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._prodotto.CodiceImballo, 13);
        }
        public void SearchCodTrasporto(string descrizione)
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.ImballoSearchCommand(descrizione);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._prodotto.CodiceTrasporto = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._prodotto.DescrizioneTrasporto = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._prodotto.UMTrasporto = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._prodotto.CodiceTrasporto, 14);
        }
        public static List<Nastro> GetNastri(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto)
        {

            return new List<Nastro>
            {
                new Nastro // Nastro base
                {
                    SpazioDiba = "",
                    Codice = nastro.Codice,
                    Descrizione = nastro.Descrizione,
                    QuantitaDiba = nastro.Lunghezza * 0.001,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = nastro.UM
                },
                   
                new Nastro // Bordo
                {
                SpazioDiba = "",
                    Codice = bordo.Codice,
                    Descrizione = bordo.Descrizione,
                    QuantitaDiba = nastro.Lunghezza * 2 * 0.001,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UM
                },
                
            new Nastro // Raspatura bordo
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceRaspatura,
                    Descrizione = bordo.DescrizioneRaspatura,
                    QuantitaDiba = nastro.Lunghezza * 2 * 0.001,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMRaspatura
                },
                 new Nastro // Raspatura tazze
                {
                    SpazioDiba = "",
                    Codice = tazza.CodiceRaspatura,
                    Descrizione = tazza.DescrizioneRaspatura,
                    QuantitaDiba = tazza.LunghezzaTotale * 0.001,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = tazza.UMRaspatura
                },
                 new Nastro // Attrezzaggio
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceAttrezzaggio,
                    Descrizione = bordo.DescrizioneAttrezzaggio,
                    QuantitaDiba = 1,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMAttrezzaggio
                },
                  new Nastro // Applicazione bordo
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceApplicazione,
                    Descrizione = bordo.DescrizioneApplicazione,
                    QuantitaDiba = bordo.LunghezzaTotale * 0.001,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMApplicazione
                },
                  new Nastro // Applicazione tazze
                {
                    SpazioDiba = "",
                    Codice = tazza.CodiceApplicazione,
                    Descrizione = tazza.DescrizioneApplicazione,
                    QuantitaDiba = tazza.Numero,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = tazza.UMApplicazione
                },
                  new Nastro // Preparazione nastro
                {
                    SpazioDiba = "",
                    Codice = nastro.CodicePreparazione,
                    Descrizione = nastro.DescrizionePreparazione,
                    QuantitaDiba = nastro.Larghezza * 0.001 * nastro.Lunghezza * 0.001,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = nastro.UMPreparazione
                },
                  new Nastro // Fix
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceFix,
                    Descrizione = bordo.DescrizioneFix,
                    QuantitaDiba = tazza.Numero * 2,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMFix
                },
                  new Nastro // Blinkers
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceBlk,
                    Descrizione = bordo.DescrizioneBlk,
                    QuantitaDiba = tazza.Numero * 2,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMBlk
                },
                  new Nastro // Applicazione blinkers
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceApplicazionexBlk,
                    Descrizione = bordo.DescrizioneApplicazioneBlk,
                    QuantitaDiba = tazza.Numero * 2,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMApplicazioneBlk
                },
                  new Nastro // Giunzione
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceBlk,
                    Descrizione = bordo.DescrizioneBlk,
                    QuantitaDiba = tazza.Numero * 2,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMBlk
                },
                  new Nastro // Articolo nastro con bordi
                {
                    SpazioDiba = "",
                    Codice = prodotto.CodiceProdotto,
                    Descrizione = prodotto.DescrizioneProdotto,
                    QuantitaDiba = 1,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = prodotto.UMProdotto
                },
                   new Nastro // Imballo
                {
                    SpazioDiba = "",
                    Codice = prodotto.CodiceImballo,
                    Descrizione = prodotto.DescrizioneImballo,
                    QuantitaDiba = 1,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = prodotto.UMImballo
                },
                   new Nastro // Trasporto
                {
                    SpazioDiba = "",
                    Codice = prodotto.CodiceTrasporto,
                    Descrizione = prodotto.DescrizioneTrasporto,
                    QuantitaDiba = 1,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = prodotto.UMTrasporto
                }
            };
        }

        public async void creaCSV()
        {
            // Path salvataggio csv
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + this._prodotto.Cliente + "_" + this._prodotto.Codice ;
            try
            {
                // Se la directory non esiste la creo
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("C'è stato un problema nella creazione della cartella di salvataggio.\nSe il problema persiste contattare l'assistenza.", ConfirmDialog.ButtonConf.OK_ONLY);
            }

            // Path di salvataggio del CSV
            var csvPath = path + "\\Distinta.csv";

            try
            {
                // Scrivo il csv
                using (var streaWriter = new StreamWriter(csvPath))
                {
                    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";"
                    };

                    using (var csvWriter = new CsvWriter(streaWriter, csvConfig))
                    {
                        var nastro = GetNastri(this._nastro, this._bordo, this._tazza, this._prodotto);
                        csvWriter.Context.RegisterClassMap<NastriInfoMap>();
                        csvWriter.WriteRecords(nastro);
                    }

                }
            }
            catch
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("C'è stato un problema nella creazione del file .CSV.\nSe il problema persiste contattare l'assistenza.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            

            // Avviso quali codici sono mancanti
            if (allertCodiceMancante == false)
            {
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("File CSV è stato creato sul Destop. Tutti i codici sono stati trovati.", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            else
            {
                string codiceMancanteTemp;
                string codiciMancanti;
                codiciMancanti = "";
                for (int i = 0; i <= StatoCodici.Length - 1; i++)
                {
                    if (StatoCodici[i] == true)
                    {
                        mapCodici.TryGetValue(i, out codiceMancanteTemp);
                        codiciMancanti = codiciMancanti +  "\n" + " - " + codiceMancanteTemp;
                    }
                }
                // Faccio l'elenco dei codici mancanti
                ConfirmDialogResult confirmed = await DialogsHelper.ShowConfirmDialog("Il CSV è stato creato sul Desktop, ma i seguenti codici non sono stati trovati:" + "\n" +
                    codiciMancanti, ConfirmDialog.ButtonConf.OK_ONLY);
            }
            
        }
    }
}
