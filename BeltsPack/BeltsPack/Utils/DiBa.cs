using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using BeltsPack.Models;
using BeltsPack.Views.Dialogs;
using CsvHelper;
using CsvHelper.Configuration;
using MaterialDesignThemes.Wpf;
using static BeltsPack.Models.Prodotto;

namespace BeltsPack.Utils
{
    public class DiBa
    {
        // Oggetti
        private Nastro _nastro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;
        private CassaInFerro _cassaInFerro;
        private int _numeroConfigurazione;
        private Imballi _imballi;
        private PdfUtils PdfUtils = new PdfUtils();

        // Variabili
        bool[] StatoCodici = new bool[15];
        bool allertCodiceMancante;
        IDictionary<int, string> mapCodici = new Dictionary<int, string>();
        public DiBa(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto, CassaInFerro cassaInFerro, int numeroConfigurazione, Imballi imballi)
        {
            // Oggetti che mi servono nella classe
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._cassaInFerro = cassaInFerro;
            this._numeroConfigurazione = numeroConfigurazione;
            this._imballi = imballi;

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
            mapCodici.Add(7, "Preparazione nastro");
            mapCodici.Add(8, "Fix");
            mapCodici.Add(9, "Blinkers");
            mapCodici.Add(10, "Applicazione blinkers");
            mapCodici.Add(11, "Giunzione");
            mapCodici.Add(12, "Articolo nastro sidewall");
            mapCodici.Add(13, "Imballo");
            mapCodici.Add(14, "Trasporto");
            mapCodici.Add(15, "Commissioni");
            mapCodici.Add(16, "Giunzione bordo");

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
                                   int altezza,
                                   string tipologia)
        {
            // Se il nastro è solo tazze
            string descrizione = "";
            if (tipologia == "Solo tazze")
            {
                descrizione = "cleated";
            }

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.PreparazioneSearchCommand(altezza, famiglia, sottogruppo, descrizione);
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
        public void SearchCodCommissioni(string nomeAgente, string codice)
        {
            // Determino la descrizione in base al nome dell'agente
            this._prodotto.DescrizioneCommissioni = "Commissioni " + this._prodotto.NomeAgente + " " + this._prodotto.CommissioniAgente + "%";

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CommissioniSearchCommand(codice);
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                this._prodotto.CodiceCommissioni = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._prodotto.UMCommissioni = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._prodotto.CodiceCommissioni, 11);
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
            string descrizione;
            descrizione = "";

            // In base alla tipologia del nastro scelgo il codice opportuno
            if (this._imballi.Tipologia == "Cassa")
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
        public void SearchCodGiunzioneBordi(string codice)
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
                this._bordo.CodiceGiunzione = reader.GetValue(reader.GetOrdinal("Cd_AR")).ToString();
                this._bordo.DescrizioneGiunzione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                this._bordo.UMGiunzione = reader.GetValue(reader.GetOrdinal("Unita_Misura_Pr")).ToString();

                break;
            }
            // Controllo che il codice del nastro sia presente
            this.PresenzaCodice(this._bordo.CodiceGiunzione, 16);
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
                  new Nastro // Giunzione bordi
                {
                    SpazioDiba = "",
                    Codice = bordo.CodiceGiunzione,
                    Descrizione = bordo.DescrizioneGiunzione,
                    QuantitaDiba = 1,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = bordo.UMBlk
                },
                  new Nastro // Commissioni
                {
                    SpazioDiba = "",
                    Codice = prodotto.CodiceCommissioni,
                    Descrizione = prodotto.DescrizioneCommissioni,
                    QuantitaDiba = 1,
                    SpazioDiba1 = "",
                    SpazioDiba2 = "",
                    UM = prodotto.UMCommissioni
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

            // Faccio comparire il menù per la scelta del logo
            List<Fornitore> fornitori = new List<Fornitore>();
            var selectedLogo = await DialogsHelper.ShowLoghiSelectionDialog(fornitori);

            // Creo la TDS
            this.PdfUtils.FillSchedaTDSSidewallsCleats(this._prodotto, path, this._nastro, this._bordo, selectedLogo, this._tazza);


            // Creo le note del nastro
            this.createTXTNastro(path, selectedLogo.Language);

            // Creo le note per l'imballo
            this.createTXTImballo(path, this._numeroConfigurazione);

            // Creo le note per il trasporto
            if (this._prodotto.Destinazione != "")
            {
                this.createTXTTrasporto(path);
            }

            // Creo note delle tazze
            this.createTXTTazze(path);

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

            // Apro la directory per visualizzare i file
            Process.Start(path);
        }
        public void createTXTNastro(string path, string language)
        {
            string fileName = "Caratteristiche_Nastro.txt";
            path = path + "\\" + fileName;
            FileInfo fi = new FileInfo(path);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                if (language == "English")
                {
                    // Create a new file     
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine("Base Belt type: " + this._nastro.Tipo + " " + this._nastro.Classe + "/" + this._nastro.NumTessuti + "+" +
                            this._nastro.NumTele + " " + this._nastro.SpessoreSup + "+" + this._nastro.SpessoreInf + " " + this._nastro.SiglaTrattamento);
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo bordi")
                        {
                            sw.WriteLine("Sidewall: HEF" + this._bordo.Altezza);
                        }
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            if (this._tazza.NumeroFile == 1)
                            {
                                sw.WriteLine("Cleats Type: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " + this._tazza.Lunghezza + " [mm]");
                            }
                            else
                            {
                                sw.WriteLine("Cleats Type: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " +
                                    this._tazza.Lunghezza + " [mm] x " + this._tazza.NumeroFile + " rows");
                            }
                        }
                        sw.WriteLine("Belt width: " + this._nastro.Larghezza + " [mm]");
                        sw.WriteLine("Free Lateral Space: " + this._prodotto.PistaLaterale + " [mm]");
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            sw.WriteLine("Cleats pitch: " + this._tazza.Passo + " [mm]");
                        }
                        if (this._nastro.Aperto)
                        {
                            sw.WriteLine("Open belt length: " + this._nastro.Lunghezza + " [mm]");
                        }
                        else
                        {
                            sw.WriteLine("Endless belt length: " + this._nastro.Lunghezza + " [mm]");
                        }
                        sw.WriteLine("-");
                    }
                }
                else if(language == "Italian")
                {
                    // Create a new file     
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine("Nastro base: " + this._nastro.Tipo + " " + this._nastro.Classe + "/" + this._nastro.NumTessuti + "+" +
                            this._nastro.NumTele + " " + this._nastro.SpessoreSup + "+" + this._nastro.SpessoreInf + " " + this._nastro.SiglaTrattamento);
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo bordi")
                        {
                            sw.WriteLine("Bordi: HEF" + this._bordo.Altezza);
                        }
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            if (this._tazza.NumeroFile == 1)
                            {
                                sw.WriteLine("Tipo tazze: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " + this._tazza.Lunghezza + " [mm]");
                            }
                            else
                            {
                                sw.WriteLine("Tipo tazze: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " +
                                    this._tazza.Lunghezza + " [mm] x " + this._tazza.NumeroFile + " file");
                            }
                        }
                        sw.WriteLine("Larghezza nastro: " + this._nastro.Larghezza + " [mm]");
                        sw.WriteLine("Piste laterali: " + this._prodotto.PistaLaterale + " [mm]");
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            sw.WriteLine("Passo tazze: " + this._tazza.Passo + " [mm]");
                        }
                        if (this._nastro.Aperto)
                        {
                            sw.WriteLine("Nastro aperto: " + this._nastro.Lunghezza + " [mm]");
                        }
                        else
                        {
                            sw.WriteLine("Nastro chiuso: " + this._nastro.Lunghezza + " [mm]");
                        }
                        sw.WriteLine("-");
                    }
                }
                else if (language == "German")
                {
                    // Create a new file     
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine("Basisgurt typ: " + this._nastro.Tipo + " " + this._nastro.Classe + "/" + this._nastro.NumTessuti + "+" +
                            this._nastro.NumTele + " " + this._nastro.SpessoreSup + "+" + this._nastro.SpessoreInf + " " + this._nastro.SiglaTrattamento);
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo bordi")
                        {
                            sw.WriteLine("Wellkanten typ: HEF" + this._bordo.Altezza);
                        }
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            if (this._tazza.NumeroFile == 1)
                            {
                                sw.WriteLine("Stollen typ: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " + this._tazza.Lunghezza + " [mm]");
                            }
                            else
                            {
                                sw.WriteLine("Stollen typ: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " +
                                    this._tazza.Lunghezza + " [mm] x " + this._tazza.NumeroFile + " Datei");
                            }
                        }
                        sw.WriteLine("Basisgurt breite: " + this._nastro.Larghezza + " [mm]");
                        sw.WriteLine("Randzone: " + this._prodotto.PistaLaterale + " [mm]");
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            sw.WriteLine("Stollenabstand : " + this._tazza.Passo + " [mm]");
                        }
                        if (this._nastro.Aperto)
                        {
                            sw.WriteLine("Basisgurtkörper geschlossen : " + this._nastro.Lunghezza + " [mm]");
                        }
                        else
                        {
                            sw.WriteLine("Basisgurtkörper offer: " + this._nastro.Lunghezza + " [mm]");
                        }
                        sw.WriteLine("-");
                    }
                }
                else if (language == "Spanish")
                {
                    // Create a new file     
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine("Tipo de banda: " + this._nastro.Tipo + " " + this._nastro.Classe + "/" + this._nastro.NumTessuti + "+" +
                            this._nastro.NumTele + " " + this._nastro.SpessoreSup + "+" + this._nastro.SpessoreInf + " " + this._nastro.SiglaTrattamento);
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo bordi")
                        {
                            sw.WriteLine("Tipo de borde contenciòn: HEF" + this._bordo.Altezza);
                        }
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            if (this._tazza.NumeroFile == 1)
                            {
                                sw.WriteLine("Tipo de taco: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " + this._tazza.Lunghezza + " [mm]");
                            }
                            else
                            {
                                sw.WriteLine("Tipo de taco: " + this._tazza.SiglaTele + "-" + this._tazza.Forma + this._tazza.Altezza + " x " +
                                    this._tazza.Lunghezza + " [mm] x " + this._tazza.NumeroFile + " filas");
                            }
                        }
                        sw.WriteLine("Ancho de banda: " + this._nastro.Larghezza + " [mm]");
                        sw.WriteLine("Playas libres laterales: " + this._prodotto.PistaLaterale + " [mm]");
                        if (this._prodotto.Tipologia == "Bordi e tazze" || this._prodotto.Tipologia == "Solo tazze")
                        {
                            sw.WriteLine("Paso entre taco: " + this._tazza.Passo + " [mm]");
                        }
                        if (this._nastro.Aperto)
                        {
                            sw.WriteLine("Banda abierta: " + this._nastro.Lunghezza + " [mm]");
                        }
                        else
                        {
                            sw.WriteLine("Banda cerrada: " + this._nastro.Lunghezza + " [mm]");
                        }
                        sw.WriteLine("-");
                    }
                }

            }
            catch
            {
                System.Windows.MessageBox.Show("C'è stato un problema nella creazione delle note del nastro.\nSe il problema persiste contattare l'assistenza.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void createTXTImballo(string path, int numeroConf)
        {
            string fileName = "Dimensioni_Imballo.txt";
            double grossWeight = Math.Round(this._prodotto.PesoTotaleNastro + this._cassaInFerro.PesoFinale[numeroConf]);

            path = path + "\\" + fileName;
            FileInfo fi = new FileInfo(path);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("L: " + this._imballi.Lunghezza[numeroConf] + " [mm]" + "  -  W:" + 
                        this._imballi.Larghezza[numeroConf] + " [mm]" + "  -  H:" +
                        this._imballi.Altezza[numeroConf] + " [mm]");
                    sw.WriteLine("Gross Weight: " + grossWeight + " [kg]");
                    sw.WriteLine("-");
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("C'è stato un problema nella creazione delle note dell'imballo.\nSe il problema persiste contattare l'assistenza.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void createTXTTazze(string path)
        {
            string fileName = "Dettagli_Tazze.txt";

            path = path + "\\" + fileName;
            FileInfo fi = new FileInfo(path);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("N° tazze: " + this._tazza.Numero);
                    sw.WriteLine("Lunghezza: " + this._tazza.Lunghezza);
                    sw.WriteLine("N° file: " + this._tazza.NumeroFile);
                    if (this._tazza.NumeroFile != 1)
                    {
                        sw.WriteLine("Spazio file: " + this._tazza.SpazioFileMultiple);
                    }
                    sw.WriteLine("-");
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("C'è stato un problema nella creazione delle note dell'imballo.\nSe il problema persiste contattare l'assistenza.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void createTXTTrasporto(string path)
        {
            string fileName = "Trasporto.txt";

            path = path + "\\" + fileName;
            FileInfo fi = new FileInfo(path);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("Destination: " + this._prodotto.Destinazione);
                    if (this._prodotto.TipoConsegna == "")
                    {
                        sw.WriteLine("Incoterms: not specified" );
                    }
                    else
                    {
                        sw.WriteLine("Incoterms: " + this._prodotto.TipoConsegna);
                    }
                    
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("C'è stato un problema nella creazione delle note del trasporto.\nSe il problema persiste contattare l'assistenza.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
