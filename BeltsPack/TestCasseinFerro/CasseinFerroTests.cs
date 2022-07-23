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
            int realCassaLength;
            int realcassaheight;
            int deltaLength;
            int k = 0;

            _nastro.Classe = 500;
            _nastro.Tipo = "TEXBEL";
            _nastro.Trattamento = "AY";
            _nastro.Aperto = true;
            _bordo.Larghezza = 50;
            _tazza.Altezza = 40;
            _tazza.Forma = "T";
            _tazza.Larghezza = 100;
            _prodotto.Tipologia = "Bordi e tazze";

            // Inizializzo la funzione imballi
            var _cassainferro = new CassaInFerro();
           

            // Per il display dei risultati
            List<string> TestResults = new List<string>();

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

                    // CONTROLLO CHE L'IMBALLO SIA CORRETTO
                    realCassaLength = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("REALE")));
                    realcassaheight = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F16")));
                    deltaLength = realCassaLength - ((int)imballi.Lunghezza.Max());
                    


                    if (imballi.Numerofile == Convert.ToInt32(reader.GetValue(reader.GetOrdinal("F19")))
                        && imballi.Numerofile == 1)
                    {
                        Assert.That(imballi.Lunghezza.Max(), Is.EqualTo(realCassaLength).Within(2000));
                        Console.WriteLine(_prodotto.Codice + "| C_L: " + imballi.Lunghezza.Max() + "| R_L: " + realCassaLength + "| D: " + deltaLength);
                        Console.WriteLine(_prodotto.Codice + "| C_h: " + imballi.Altezza.Max() + "| R_h: " + realcassaheight);
                    }
                }
            }
            reader.Close();

            // IMPORTANTE
            // Nella riga 1645 della classe imballi devo decommentare il messagebox quando non sono in ambiente di test
        }
    }
}