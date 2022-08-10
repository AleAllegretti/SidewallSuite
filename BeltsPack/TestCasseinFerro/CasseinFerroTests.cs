using NUnit.Framework;
using BeltsPack;
using BeltsPack.Models;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using BeltsPack.Utils;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TestCasseinFerro
{
    public class CasseinFerroTests
    {

        [Test]
        public void Test1()
        {
            // DATABASE
            var databasesql = new DatabaseSQL("Server=DELLALE\\SQLEXPRESS;Database=DBSidewall;Trusted_Connection=True;  Min Pool Size=10; Max Pool Size=1000");

            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            int i = 0;

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateDbTestCommand();
            reader = creaComando.ExecuteReader();

            // Prestabilisco alcune caratteristiche del nastro che non sono influenti per l'imballo
            var _nastro = new Nastro();
            var _bordo = new Bordo();
            var _tazza = new Tazza();
            var _prodotto = new Prodotto();
            var _cassainferro = new CassaInFerro();
            var _imballi = new Imballi(_nastro, _bordo, _tazza, _prodotto, _cassainferro);
            int realCassaLength;
            int realcassaheight;
            int deltaLength;
            int deltaheight;
            int k = 0;

            // CARATTERISTICHE NASTRO
            _nastro.Larghezza = 650;
            _nastro.Classe = 500;
            _nastro.Tipo = "TEXRIGID";
            _nastro.SpessoreInf = 0;
            _nastro.SpessoreSup = 0;
            _nastro.NumTele = 0;
            _nastro.NumTessuti = 0;
            _nastro.SiglaTrattamento = "AY";
            _nastro.Aperto = true;
            _nastro.LarghezzaUtile = 700;
            _nastro.SetCaratterisitche();         
           

            // CARATTERISTICHE BORDO
            _bordo.Larghezza = 50;
            _bordo.Altezza = 120;
            _bordo.SiglaTrattamento = "AW";
            _bordo.GetInfoBordo();

            // CARATTERISTICHE TAZZA
            _tazza.Altezza = 110;
            _tazza.Forma = "T";
            _tazza.SiglaTrattamento = "AW";
            _tazza.SiglaTele = "HBL";
            _tazza.CarattersticheTazza();

            // CARATTERISTICHE PRODOTTO
            _prodotto.Tipologia = "Bordi e tazze";
            _prodotto.Cliente = "Bevcon Wayors Pvt. Ltd.";
            _prodotto.SetDettagliCliente();
            _prodotto.AltezzaApplicazioni = Math.Max(_tazza.Altezza, _bordo.Altezza);

            // Per il display dei risultati
            List<string> TestResults = new List<string>();

            // TEST CHE LA DISTINTA FUNZIONI CORRETTAMENTE
            DiBa distinta = new DiBa(_nastro, _bordo, _tazza, _prodotto, _cassainferro, 6, _imballi);

            // Codice nastro
            distinta.SearchCodnastro(_nastro.Tipo, _nastro.Classe, _nastro.Larghezza,
                   _nastro.SpessoreSup, "NAS", _nastro.SiglaTipo, _nastro.SiglaTrattamento, _nastro.SpessoreInf,
                   _nastro.NumTele, _nastro.NumTessuti);

            // Codice bordo
            distinta.searchCodBordo(_bordo.Altezza, _bordo.Larghezza, "BOR", _bordo.SiglaTele, _bordo.SiglaTrattamento);

            // Codice raspatura bordo
            distinta.searchCodRaspaturaBordo("RAB", _bordo.Altezza, _bordo.SiglaTrattamento);

            // Codice tazze
            distinta.searchCodTazza(_tazza.Altezza, _nastro.LarghezzaUtile,
                        _tazza.SiglaTrattamento, _tazza.SiglaTele, _tazza.Forma, "LIS");

            // Codice raspatura tazze
            distinta.searchCodRaspaturaTazze("RAL", _tazza.Altezza, _tazza.SiglaTrattamento, _tazza.Forma);

            // Codice attrezzaggio linea
            distinta.SearchCodAttAppBor("ATT", "LAV", _bordo.Altezza, _prodotto.Tipologia);

            // Codice applicazione bordo
            distinta.SearchCodAppBor("APP", "BOR", _bordo.Altezza, _prodotto.Tipologia);

            // Codice applicazione listelli
            distinta.searchCodApplicazioneTazze("APP", "LIS", _tazza.Altezza, _tazza.Forma, _nastro.LarghezzaUtile);

            // Codice preparazione nastro
            distinta.SearchCodPrepNastro("NAS", "LAV", _bordo.Altezza, _prodotto.Tipologia);

            // Codice fix
            distinta.SearchCodFix("FIX", "LAV", _bordo.Altezza);

            // COdice blinkers
            distinta.SearchCodBlk("BLK", _bordo.Altezza, _bordo.SiglaTrattamento);

            // Applicazione blinkers
            distinta.SearchCodApplicazioneBlk("APPLI-BLINKERS");

            // Codice giunzione
            distinta.SearchCodGiunzione("LAV", "GIU", _prodotto.AltezzaApplicazioni, _nastro.Larghezza);

            // Articolo sidewall
            distinta.SearchCodProdotto(_bordo.Altezza, _prodotto.Tipologia);

            // Imballo
            distinta.SearchCodImballo(_nastro.Lunghezza);

            // Trasporto
            distinta.SearchCodTrasporto("Trasporto");

            // Commissioni
            distinta.SearchCodCommissioni(_prodotto.NomeAgente, "SPESE EXTRA");

            // Movimentazione
            distinta.SearchCodMovimentazione("SPESE EXTRA");

            // Controllo che gli articoli siano stati pescati correttamente
            Console.WriteLine("CODICI");
            Console.WriteLine("1. Nastro:" + _nastro.Codice);                           // TEXRIGID500/800
            Console.WriteLine("2. Bordo:" + _bordo.Codice);                             // BORDO200/75HR
            Console.WriteLine("3. Raspatura bordo:" + _bordo.CodiceRaspatura);          // RASBORDO200HR
            Console.WriteLine("4. Tazze:" + _tazza.Codice);                             // 
            Console.WriteLine("5. Raspatura tazze:" + _tazza.CodiceRaspatura);          //
            Console.WriteLine("6. Attrezzaggio linea:" + _bordo.CodiceAttrezzaggio);    //
            Console.WriteLine("7. Applicazione bordo:" + _bordo.CodiceApplicazione);    // APPLB200
            Console.WriteLine("8. Applicazione listelli:" + _tazza.CodiceApplicazione); //
            Console.WriteLine("9. Fix:" + _bordo.CodiceFix);                            //
            Console.WriteLine("10. Blinkers:" + _bordo.CodiceBlk);                      //
            Console.WriteLine("11. Appl. blinkers:" + _bordo.CodiceApplicazionexBlk);   //
            Console.WriteLine("12. Giunzione:" + _bordo.CodiceApplicazionexBlk);        //
            Console.WriteLine("13. Prodotto finito:" + _prodotto.CodiceProdotto);       //
            Console.WriteLine("14. Imballo:" + _prodotto.CodiceImballo);                //
            Console.WriteLine("15. Trasporto:" + _prodotto.CodiceTrasporto);            //
            Console.WriteLine("16. Commissioni:" + _prodotto.CodiceCommissioni);        //
            Console.WriteLine("17. Movimentazione:" + _prodotto.CodiceMovimentazione);  //

            // Scorro tra gli imballi presenti a db
            while (reader.Read() & reader.GetValue(reader.GetOrdinal("F6")).ToString() != "")
            {
                var temp = reader.GetValue(reader.GetOrdinal("F6"));
                if (temp.ToString() != null &&
                    reader.GetValue(reader.GetOrdinal("F12")).ToString() == "Cassa in ferro")
                {
                    // NASTRO
                    _nastro.Lunghezza = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F3")));
                    _nastro.Larghezza = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F4")));

                    // BORDO           
                    _bordo.Altezza = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F5")));

                    // PRODOTTO
                    _prodotto.Aperto = reader.GetValue(reader.GetOrdinal("F6")).ToString();
                    _prodotto.Codice = reader.GetValue(reader.GetOrdinal("INPUT")).ToString();

                    // INIZIALIZZO GLI IMBALLI
                    var imballi = new Imballi(_nastro, _bordo, _tazza, _prodotto, _cassainferro);
                    _imballi = imballi;

                    // CONTROLLO CHE L'IMBALLO SIA CORRETTO
                    realCassaLength = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("REALE")));
                    realcassaheight = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F16")));
                    deltaLength = realCassaLength - ((int)imballi.Lunghezza.Max());
                    deltaheight = realcassaheight - ((int)imballi.Altezza.Max());


                    if (imballi.Numerofile == Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F19")))
                        && imballi.Numerofile == 1)
                    {
                        Assert.That(imballi.Lunghezza.Max(), Is.EqualTo(realCassaLength).Within(2000));
                        Console.WriteLine("DIMENSIONI CASSA");
                        Console.WriteLine(_prodotto.Codice + "| D_l: " + deltaLength);
                        Console.WriteLine(_prodotto.Codice + "| D_h: " + deltaheight);
                    }
                }
            }
            reader.Close();

            // IMPORTANTE
            // Nella riga 1645 della classe imballi devo decommentare il messagebox quando non sono in ambiente di test

            
        }
    }
}