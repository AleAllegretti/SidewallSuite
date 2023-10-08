using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BeltsPack.Models;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using BeltsPack.Views.Dialogs;
using System.Globalization;

namespace BeltsPack.Utils
{
    public class DatabaseSQL
    {
        private static readonly string TABELLA_IMBALLI_LEGNO = "ImballiLegno";
        private static readonly string TABELLA_IMBALLI_TOTALI = "ImballiTotali";
        private static readonly string TABELLA_LISTINO_PALADINI = "ListinoPaladini";
        private static readonly string TABELLA_COSTI_ACCESSORI = "Accessori";
        private static readonly string TABELLA_COSTI_GESTIONE = "CostiGestione";
        private static readonly string TABELLA_COSTI_FERRO = "CostoFerro";
        private static readonly string TABELLA_LIMITI_TRASPORTO = "LimitiTrasporti";
        private static readonly string TABELLA_IMPOSTAZIONI = "Parametri";
        public readonly string TABELLA_BORDI = "Bordi";
        public readonly string TABELLA_NASTRI = "NastriBase";
        public readonly string TABELLA_TAZZE = "Tazze";
        public readonly string TABELLA_ARTICOLI = "AR";
        public readonly string TABELLA_CLIENTI = "CFCliEx";
        public readonly string TABELLA_CATEGORIE = "Categories";
        public readonly string TABELLA_TEST = "DatabaseTest";
        public readonly string TABELLA_CASSE = "TipologiaCasse";
        public readonly string TABELLA_AGENTI = "Agente";
        public readonly string TABELLA_CALCOLI = "ParametriFoglioDiCalcolo";
        public readonly string TABELLA_CALCOLI_INPUT = "InputCalcoli";
        public readonly string TABELLA_CALCOLI_OUTPUT = "OutputCalcoli";

        private Nastro _nastro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;
        private Imballi _imballi;
        private Materiale _materiale;

        // Properties
        private string ConnectionString { get; }
        private SqlConnection _connection;

        /// <summary>
        /// You can still create a DatabaseSQL using a different connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public DatabaseSQL(string connectionString)
        {
            if (connectionString.Length < 7)
            {
                connectionString = "Server=DELLALE\\SQLEXPRESS;Database=DBSidewall;Trusted_Connection=True;  Min Pool Size=10; Max Pool Size=1000";
            }
            this.ConnectionString = connectionString;

            // connect at creation
            this.OpenConnection();
        }

        public void OpenConnection()
        {
            if (this._connection == null)
            {
                _connection = new SqlConnection(this.ConnectionString);
                this._connection.Open();
            }
        }

        public void CloseConnection()
        {
            // you cannot close the connection in the destructor of the class: 
            // https://social.msdn.microsoft.com/Forums/en-US/b23d8492-1696-4ccb-b4d9-3e7fbacab846/internal-net-framework-data-provider-error-1?forum=adodotnetdataproviders
            this._connection.Close();
            this._connection = null;
        }

        public SqlCommand CreateCommand(string query)
        {
            return new SqlCommand(query, this._connection);
        }

        public SqlCommand CreateImballiLegnoCommand()
        {
            return this.CreateCommand("SELECT Larghezza,Lunghezza,Altezza,Peso,Costo FROM " + TABELLA_IMBALLI_LEGNO);
        }

        public SqlCommand ArticleSearchCommand(string tipo, int spessoreSup, string famiglia, 
            string gruppo, string sottogruppo, int classe, double larghezza, int spessoreInf, int numTele, int numTessuti)
        {
            // Trasformo la larghezza in string perchè SQL non accetta la virgola come separatore
            string larghezzaSt;
            larghezzaSt = larghezza.ToString(CultureInfo.InvariantCulture);

            // Modifico la lunghezza della classe perchè il campo su Arca può contenere al massimo 2 cifre
            string classeSt = "";

            if(classe.ToString().Length == 3)
            {
                classeSt = classe.ToString().Substring(classe.ToString().Length - 3, 2);
            }
            else
            {
                classeSt = classe.ToString().Substring(classe.ToString().Length - 3, 3);
            }
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARGruppo3,Cd_ARMisura,LarghezzaMKS," +
                "Cd_ARClasse1,Cd_ARClasse3,Cd_ARClasse3,Descrizione FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo1 = " + "'" + famiglia +
                "' AND  Cd_ARGruppo2 = " + "'" + gruppo +
                "' AND  Cd_ARGruppo3 = " + "'" + sottogruppo +
                "' AND  LarghezzaMKS >= " + larghezzaSt +
                " AND  Cd_ARClasse1 = " + "'" + classe.ToString().Substring(classe.ToString().Length-3, 2) +
                "' AND  Cd_ARClasse2 = " + "'" + numTessuti + "+" + numTele +
                "' AND  Cd_ARClasse3 = " + "'" + spessoreSup + "+" + spessoreInf + "'");
        }

        public SqlCommand RaspaturaBordoSearchCommand(string gruppo, int altezza, string trattamento)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGruppo3,Altezza FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo2 = " + "'" + gruppo +
                "' AND  Altezza LIKE " + "'" + altezza +
                "%' AND  Cd_ARGruppo3 LIKE " + "'%" + trattamento + "%'");
        }
        public SqlCommand RaspaturaTazzeSearchCommand(string gruppo, int altezza, string trattamento, string forma)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGruppo3,Cd_ARClasse1,Altezza FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo2 = " + "'" + gruppo +
                "' AND  Cd_ARClasse1 = " + "'" + forma +
                 "' AND  Altezza >= " + "'" + altezza +
                "' AND  Cd_ARGruppo3 LIKE " + "'%" + trattamento + "%' ORDER BY Altezza ASC");
        }
        public SqlCommand AttrezzaggioSearchCommand(int altezza, string famiglia, string sottogruppo, string altezzastr)
        {
            if (altezzastr == "")
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGruppo3 FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo2 = " + "'" + sottogruppo +
                "' AND  Cd_ARGruppo3 <= " + "'" + altezza + "' ORDER BY Cd_ARGruppo3 DESC");
            }
            else
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGruppo3 FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo2 = " + "'" + sottogruppo +
                "' AND  Cd_ARGruppo3 = " + "'" + altezzastr + "'");
            }
            
        }
        public SqlCommand ApplicazioneBordoSearchCommand(int altezza, string famiglia, string sottogruppo)
        {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGruppo3 FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo2 = " + "'" + sottogruppo +
                 "' AND  Cd_ARGruppo3 = " + "'" + famiglia +
                "' AND  Altezza = " + "" + altezza);

        }
        public SqlCommand PreparazioneSearchCommand(int altezza, string famiglia, string sottogruppo, string descrizione)
        {
            if (descrizione !="" && altezza != 0)
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARGruppo3,Cd_ARMisura FROM " +
               TABELLA_ARTICOLI +
               " Where Cd_ARGruppo3 = " + "'" + altezza +
               "' AND  Descrizione LIKE " + "'%" + "Preparazione nastro"  +
               "%' AND  Cd_ARGruppo1 LIKE " + "'" + famiglia + "'");
            }
            else if (descrizione != "" && altezza == 0)
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARGruppo3,Cd_ARMisura FROM " +
                               TABELLA_ARTICOLI +
                               " Where Descrizione LIKE " + "'%" + "Preparazione nastro Cleated Belt" +
                               "%' AND  Cd_ARGruppo1 LIKE " + "'" + famiglia + "'");
            }
            else
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo3 LIKE " + "'" + altezza +
                "' AND  Cd_ARGruppo2 = " + "'" + sottogruppo +
                "' AND  Cd_ARGruppo1 LIKE " + "'" + famiglia + "'");
            }
            
        }

        public SqlCommand FixSearchCommand(int altezza, string famiglia, string sottogruppo)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo2 = " + "'" + sottogruppo +
                "' AND  Cd_ARGruppo3 >= " + "'" + altezza +
                "' AND  Cd_ARMisura NOT LIKE '%mt%' " +
                "AND  Cd_ARGruppo1 LIKE " + "'" + famiglia + "'" +
                "ORDER BY Cd_ARGruppo3 ASC");
        }
        public SqlCommand ProdottoSearchCommand(string descrizione)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARGruppo1,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where  Descrizione LIKE '%" + descrizione + "%' AND Cd_ARGruppo1 is NULL");
        }
        public SqlCommand ImballoSearchCommand(string descrizione)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where  Descrizione LIKE '%" + descrizione + "%'");
        }
        public SqlCommand BlkSearchCommand(int altezza, string sottogruppo, string trattamento)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo1 = " + "'" + sottogruppo +
                "' AND  Cd_ARGruppo3 >= " + "'" + altezza +
                "' AND Cd_ARGruppo2 LIKE " + "'%" + trattamento +
                "%' AND Cd_Ar LIKE " + "'%" + "BLK" + "%'" +
                 "ORDER BY Cd_ARGruppo3 ASC");
        }

        public SqlCommand ApplicazioneTazzaSearchCommand(int altezza, string famiglia, string sottogruppo, string forma, int larghezza)
        {
            // Eccezione per tazze TC 230 - 240 e 250
            if (altezza == 230 || altezza == 240 || altezza == 250)
            {
                altezza = 280;
            }

            // Eccezione per tazze TC 90
            if (altezza == 90 && forma == "TC")
            {
                altezza = 110;
            }

            if (forma == "TC" || forma == "TB")
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGRuppo3,Lunghezza FROM " +
                TABELLA_ARTICOLI +
                " Where Descrizione LIKE " + "'%" + forma + "%'" +
                " AND  Lunghezza >= " + larghezza +
                " AND  Descrizione LIKE " + "'%" + altezza +
                "%' AND  Cd_ARGRuppo3 LIKE " + "'%" + sottogruppo +
                "%' AND  Cd_ARGruppo2 LIKE " + "'" + famiglia + "' ORDER BY Lunghezza ASC");
            }
            else
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura,Cd_ARGRuppo3,Lunghezza FROM " +
                TABELLA_ARTICOLI +
                " Where Descrizione LIKE " + "'%" + forma + "%'" +
                " AND Descrizione NOT LIKE " + "'%TC%'" +
                " AND Descrizione NOT LIKE " + "'%TB%'" +
                " AND  Lunghezza >= " + larghezza +
                " AND  Descrizione LIKE " + "'%" + altezza +
                "%' AND  Cd_ARGRuppo3 LIKE " + "'%" + sottogruppo +
                "%' AND  Cd_ARGruppo2 LIKE " + "'" + famiglia + "' ORDER BY Lunghezza ASC");
            }
        }
        public SqlCommand GiunzioneSearchCommand(string famiglia, string gruppo, int altezzabordo, int larghezzanas)
        {
            if (altezzabordo > 160 && larghezzanas > 800)
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARMisura,Cd_ARClasse1,Cd_ARClasse2 FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo1 LIKE " + "'" + famiglia + "'" +
                " AND  Cd_ARGruppo2 LIKE " + "'" + gruppo +
                "' AND  Cd_ARClasse1 = " + "'" + 63 +
                "' AND  Cd_ARClasse2 = " + "'" + 160 + "'");
            }
            else if (altezzabordo > 160 && larghezzanas <= 800)
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARMisura,Cd_ARClasse1,Cd_ARClasse2 FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo1 LIKE " + "'" + famiglia + "'" +
                " AND  Cd_ARGruppo2 LIKE " + "'" + gruppo +
                "' AND  Cd_ARClasse1 = " + "'" + 63 +
                "' AND  Cd_ARClasse2 = " + "'" + 80 + "'");
            }
            else if (altezzabordo <= 160 && larghezzanas <= 800)
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARMisura,Cd_ARClasse1,Cd_ARClasse2 FROM " +
               TABELLA_ARTICOLI +
               " Where Cd_ARGruppo1 LIKE " + "'" + famiglia + "'" +
               " AND  Cd_ARGruppo2 LIKE " + "'" + gruppo +
               "' AND  Cd_ARClasse1 = " + "'" + 16 +
               "' AND  Cd_ARClasse2 = " + "'" + 80 + "'");
            }
            else
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARMisura,Cd_ARClasse1,Cd_ARClasse2 FROM " +
               TABELLA_ARTICOLI +
               " Where Cd_ARGruppo1 LIKE " + "'" + famiglia + "'" +
               " AND  Cd_ARGruppo2 LIKE " + "'" + gruppo +
               "' AND  Cd_ARClasse1 = " + "'" + 16 +
               "' AND  Cd_ARClasse2 = " + "'" + 160 + "'");
            }
        }

        public SqlCommand GiunzioneBordiSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand CommissioniSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand MovimentazioneSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand ApplicazioneBlkSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo2,Cd_ARMisura FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand BordoSearchCommand(double altezza, double larghezza,
            string famiglia, string gruppo, string sottogruppo)
        {
            string larghezzaString;
            larghezzaString = larghezza.ToString().Replace(",", ".");

            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARGruppo3,Cd_ARMisura,LarghezzaMKS FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo1 = " + "'" + famiglia +
                "' AND  Cd_ARGruppo2 = " + "'" + gruppo +
                "' AND  Cd_ARGruppo3 = " + "'" + sottogruppo +
                "' AND  Altezza LIKE " + "'%" + altezza +
                "%' AND  LarghezzaMks = " + "'" + larghezzaString + "'");
        }
        public SqlCommand TazzeSearchCommand(double altezza, double larghezza, string trattamento,
            string famiglia, string tele, string forma)
        {
            string larghezzaString;
            larghezzaString = larghezza.ToString().Replace(",", ".");

            return this.CreateCommand("SELECT Cd_AR,Descrizione,Cd_ARMisura,Cd_ARGruppo1,Cd_ARGruppo2,Cd_ARGruppo3,Obsoleto,Altezza FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_ARGruppo1 = " + "'" + famiglia +
                "' AND  Cd_ARGruppo2 = " + "'" + tele +
                "' AND  Cd_ARGruppo3 = " + "'" + trattamento +
                "' AND  Cd_ARClasse1 = " + "'" + forma +
                "' AND  Obsoleto = " + "'" + 0 +
                "' AND  Altezza =" + altezza);
        }
        public static DatabaseSQL CreateDefault()
        {
            var args = Environment.GetCommandLineArgs();    
            if (args.Length > 1)
            {
                // use the connection string provided as CLI argument
                return new DatabaseSQL(args[1]);
            }
            else
            {
                return new DatabaseSQL(GetDbConnectionString());
            }
        }

        public static DatabaseSQL CreateARCF()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // use the connection string provided as CLI argument
                return new DatabaseSQL(args[1]);
            }
            else
            {
                return new DatabaseSQL("Data Source = Server1; Initial Catalog = ADB_SIDEW; User ID = ProgrammaImballi; Password = morinat");
            }
        }

        public static string GetDbConnectionString()
        {
            // Vado a leggere su info customer la stringa di database di questo cliente
            XmlDocument infocustomer = new XmlDocument();
            infocustomer.Load(@"C:\ASquared\Info.xml");
            string connectionstring = "";

            foreach (XmlNode node in infocustomer.DocumentElement)
            {
                connectionstring = node.Attributes[0].InnerText;
                foreach (XmlNode child in node.ChildNodes)
                {
                connectionstring = child.InnerText;
                }
                
            }

        return connectionstring;

        }
        public SqlCommand UpdateNoteDbTotaleCommand(string codice, string note, int versione)
        {
            return this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET Note = '" + note +"' Where Codice = " + "'" + codice + "' AND  Versione = " + versione );
        }

        public SqlCommand UpdateNotePaladiniDbTotaleCommand(string codice, string notePaladini, int versione)
        {
            return this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET NotePaladini = '" + notePaladini + "' Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
        }

        public SqlCommand UpdateDataConsegnaCassaCommand(string codice, string dataConsegnaCassa, int versione)
        {
            return this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET DataConsegnaCassa = '" + dataConsegnaCassa + "' Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
        }

        public SqlCommand UpdateDbTotaleCommand(string codice, int versione)
        {
            return this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET Stato = 'Inviato' Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
        }
        public SqlCommand UpdateDatiPostProduzione(string codice, int versione, ProdottoSelezionato prodotto)
        {
            return this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET LunghezzaCassaReale = '" + prodotto.LunghezzaCassaReale + "'," +
                "AltezzaCassaReale = " + prodotto.AltezzaCassaReale + ", LarghezzaCassaReale = " + prodotto.LarghezzaCassaReale + "," +
                "NotePostProduzione = '" + prodotto.NotePostProduzione + "', ConformitaCassa = '" + prodotto.Conformita + "', Stato = 'Completato'" + 
                " Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
        }
        public SqlCommand UpdateDataAggiornamento(string id, string tabella)
        {
            return this.CreateCommand("UPDATE " + tabella + " SET DataUltimoAggiornamento = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'" +
                " Where ID = " + "'" + id + "'");
        }
        public SqlCommand WriteBinaryCommandPDF(string codice, int versione, byte[] binarypdf)
        {
            var cmd = this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET BinarySchedaProduzione = @binarypdf Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
            cmd.Parameters.Add("@binarypdf", SqlDbType.VarBinary).Value = binarypdf;
            return cmd;
        }
        public SqlCommand WriteBinaryCommandSchedaPostProduzione(string codice, int versione, byte[] binarypdf)
        {
            var cmd = this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET BinarySchedaPostProduzione = @binarypdf Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
            cmd.Parameters.Add("@binarypdf", SqlDbType.VarBinary).Value = binarypdf;
            return cmd;
        }
        public SqlCommand WriteBinaryCommandPDFPaladini(string codice, int versione, byte[] binarypdf)
        {
            var cmd = this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET BinarySchedaPaladini = @binarypdf Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
            cmd.Parameters.Add("@binarypdf", SqlDbType.VarBinary).Value = binarypdf;
            return cmd;
        }

        public SqlCommand WriteBinaryCommandImage(string codice, int versione, byte[] binaryimage)
        {
            var cmd = this.CreateCommand("UPDATE " + TABELLA_IMBALLI_TOTALI + " SET BinayDisposizioneNastro = @binaryimage Where Codice = " + "'" + codice + "' AND  Versione = " + versione);
            cmd.Parameters.Add("@binaryimage", SqlDbType.VarBinary).Value = binaryimage;
            return cmd;
        }

        public SqlCommand CreateDbTotaleCommand()
        {
            return this.CreateCommand("SELECT Codice,Versione,Cliente,DataConsegnaCassa,LunghezzaNastro,LunghezzaImballo,AltezzaImballo,LarghezzaImballo,CostoImballo," +
                "Stato,Data,NomeUtente,Note,NotePaladini,Criticita,LarghezzaNastro,AltezzaBordo,AltezzaTazze,PesoImballo," +
                "ApertoChiuso,TipologiaImballo," +
                "Configurazione,TipologiaTrasporto,Personalizzazione,NumeroCorrugati, NumeroSubbi, DiametroCorrugati," +
                "DiametroSubbi, LunghezzaCorrugati,PresenzaIncroci,PresenzaGanci,PresenzaReteLaterale,CassaVerniciata," +
                "PresenzaLamieraBase,PesoTotaleNastro, PresenzaFix, PresenzaBlinkers, TipoNastro, ClasseNastro, PassoTazze," +
                " PistaLaterale, BaseBordo, FormaTazze, NumeroTazzexFila, SpazioFile, TrattamentoNastro, TrattamentoBordo, TrattamentoTazze, TazzeTelate, Qty, TipologiaTrasportoDett FROM " + TABELLA_IMBALLI_TOTALI + " ORDER BY Cliente ASC");
        }

        public SqlCommand CreateDbInputCalcoliCommand()
        {
            return this.CreateCommand("SELECT Codice,Versione FROM " + TABELLA_CALCOLI_INPUT );
        }
        public SqlCommand CreateDbProduzioneCommand(string stato)
        {
            var cmd = this.CreateCommand("SELECT Codice,Versione,DataConsegnaCassa,Cliente,LunghezzaNastro,LunghezzaImballo,AltezzaImballo,LarghezzaImballo FROM " + TABELLA_IMBALLI_TOTALI + " Where Stato = @stato");
            cmd.Parameters.AddWithValue("@Stato", stato);
            return cmd;
        }

        public SqlCommand CheckCodeExistance(string codice)
        {
            return this.CreateCommand("SELECT Codice FROM " + TABELLA_IMBALLI_TOTALI + " WHERE EXISTS (SELECT Codice FROM " 
                + TABELLA_IMBALLI_TOTALI + " WHERE Codice = '" + codice + "')");
        }
        public SqlCommand CreateSettingPedaneCommand()
        {
            return this.CreateCommand("SELECT ID,Larghezza,Lunghezza,Altezza,Peso,Costo,Note,DataUltimoAggiornamento FROM " + TABELLA_IMBALLI_LEGNO);
        }

        public SqlCommand CreateSettingBordiCommand()
        {
            return this.CreateCommand("SELECT ID,Altezza,LarghezzaBase,PassoOnda,Peso,DiametroCorrugato,DiametroPolistirolo,MinPulleyDiam,MinWheelDiam,DataUltimoAggiornamento FROM " + TABELLA_BORDI + " ORDER BY Altezza ASC");
        }
        public SqlCommand CreateSettingsCalcoliCommand()
        {
            return this.CreateCommand("SELECT ID, Descrizione, Valore, DataUltimoAggiornamento, Note FROM " + TABELLA_CALCOLI + " ORDER BY ID ASC");
        }
        public SqlCommand CreateClientiCommand()
        {
            return this.CreateCommand("SELECT Id_CF,Descrizione,Provvigione FROM " + TABELLA_CLIENTI + " ORDER BY Descrizione ASC");
        }
        public SqlCommand CreateAgentiCommand(string codiceAgente)
        {
            return this.CreateCommand("SELECT Cd_Agente,Descrizione,Provvigione, Email FROM " + TABELLA_AGENTI + " where Cd_Agente like '%" +
                codiceAgente + "%'");
        }
        public SqlCommand ClienteSearchCommand(string nomeCliente)
        {
            return this.CreateCommand("SELECT Descrizione,Provvigione,Cd_DOPorto,Localita,Email,Cd_Nazione,Cd_Agente_1 FROM " +
                TABELLA_CLIENTI +
                " Where Descrizione LIKE " + "'%" + nomeCliente + "%'");
        }

        public SqlCommand CreateSettingNastriCommand()
        {
            return this.CreateCommand("SELECT ID,NomeNastro,SiglaNastro,Classe,PesoMQ,NumeroTessuti,NumeroTele,SpessoreSup,SpessoreInf,MinimoDiametroPulley,LunghezzaGradinoGiunta,DataUltimoAggiornamento FROM " + TABELLA_NASTRI);
        }

        public SqlCommand CreateSettingCasseCommand()
        {
            return this.CreateCommand("SELECT ID, Nome_Cassa, L_Min, L_Max, P_Min, P_Max, W_Longherone, H_Longherone, S_Longherone, " +
                "Presenza_Ganci, Rinforzo_Longherone, L_Rinforzo_Longherone, Solo_Ritti, Incroci_Spalle, DataUltimoAggiornamento FROM " + TABELLA_CASSE);
        }
        public SqlCommand CreateSettingDistinctNastriCommand()
        {
            return this.CreateCommand("SELECT DISTINCT NomeNastro FROM " + TABELLA_NASTRI);
        }
        public SqlCommand CreateSettingDistinctClasseCommand(string NomeNastro)
        {
            return this.CreateCommand("SELECT DISTINCT Classe, NomeNastro FROM " + TABELLA_NASTRI + " where NomeNastro like '" + NomeNastro + "'");
        }
        public SqlCommand CreateSettingTazzeCommand()
        {
            return this.CreateCommand("SELECT ID,Altezza,FormaTC,FormaTCW,FormaT,FormaTW,FormaTB,FormaC,PesoTC,PesoTCW,PesoT,PesoTW,PesoTB,PesoC,LarghezzaTazzeTC,LarghezzaTazzeTCW," +
                "LarghezzaTazzeT,LarghezzaTazzeTW,SezioneTC, SezioneC, SezioneT, SezioneTCW, SezioneTW, SezioneTB " +
                "LarghezzaTazzeTB,LarghezzaTazzeC,DataUltimoAggiornamento," +
                "SpessoreT, SpessoreTC, SpessoreC, SpessoreTCW, SpessoreTW, SpessoreTB FROM " + TABELLA_TAZZE + " ORDER BY Altezza ASC");
        }
        public SqlCommand CreateSettingPaladiniCommand()
        {
            return this.CreateCommand("SELECT ID,Codice,Descrizione,Prezzo,DataUltimoAggiornamento FROM " + TABELLA_LISTINO_PALADINI);
        }

        public SqlCommand CreateSettingParametriCommand()
        {
            return this.CreateCommand("SELECT ID,Elemento,Valore,Note,DataUltimoAggiornamento FROM " + TABELLA_IMPOSTAZIONI);
        }

        public SqlCommand CreateDbTestCommand()
        {
            //return this.CreateCommand("SELECT INPUT, F2, F3, F4, F5, F6, F7, CALCOLATA, F9, F10, F11, F12, F13, REALE, F15, F16, F17, F18, F19, RISULTATO, NOTE FROM " + TABELLA_TEST);
            return this.CreateCommand("SELECT LunghezzaNastro, LarghezzaNastro, AltezzaBordo, AltezzaTazze, ApertoChiuso, Codice FROM " + TABELLA_IMBALLI_TOTALI);
        }

        public SqlCommand ConsultaListinoPaladini()
        {
            return this.CreateCommand("SELECT ID,Codice,Descrizione,Prezzo,Peso FROM " + TABELLA_LISTINO_PALADINI);
        }

        public SqlCommand CreateSettingAccessoriCommand()
        {
            return this.CreateCommand("SELECT ID,Codice,Descrizione,Prezzo,UnitaMisura,Peso,DataUltimoAggiornamento FROM " + TABELLA_COSTI_ACCESSORI);
        }

        public SqlCommand CreateSettingCostiGestioneCommand()
        {
            return this.CreateCommand("SELECT ID,Codice,Descrizione,Prezzo,DataUltimoAggiornamento FROM " + TABELLA_COSTI_GESTIONE);
        }

        public SqlCommand CreateSettingCostiFerroCommand()
        {
            return this.CreateCommand("SELECT ID,Codice,Descrizione,Prezzo,UnitaMisura,PesoMetro,UnitaMisuraPeso,Spessore,Larghezza,Altezza,DataUltimoAggiornamento FROM " + TABELLA_COSTI_FERRO);
        }

        public SqlCommand CostoCassaFerroCommand()
        {
            return this.CreateCommand("SELECT Codice,Prezzo,PesoMetro,Altezza FROM " + TABELLA_COSTI_FERRO);
        }

        public SqlCommand CreateSettingLimitiTrasportoCommand()
        {
            return this.CreateCommand("SELECT ID,Trasporto,Lunghezza,Larghezza, Altezza, Camion, Nave, DataUltimoAggiornamento, Priorita" +
                " FROM " + TABELLA_LIMITI_TRASPORTO + " ORDER BY Priorita" );
        }

        public SqlCommand UpdateDbCommand(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto, Imballi imballi, int i)
        {
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._imballi = imballi;
            string Cassa = this._imballi.GetTipologiaImballo();

            return this.CreateCommand("INSERT INTO ImballiTotali(Codice, Cliente, Data, LunghezzaNastro, LarghezzaNastro, AltezzaBordo, AltezzaTazze, ApertoChiuso, TipologiaImballo, " +
                "CostoImballo, PesoImballo, LarghezzaImballo, LunghezzaImballo, AltezzaImballo, Stato, Versione, Criticita, NomeUtente, TipologiaTrasporto, PesoTotaleNastro, Note," +
                "PresenzaFix, PresenzaBlinkers, TipoNastro, ClasseNastro, PassoTazze, PistaLaterale, BaseBordo, FormaTazze, NumeroTazzexFila, SpazioFile," +
                "TrattamentoNastro, TrattamentoBordo, TrattamentoTazze, TazzeTelate, Qty) VALUES" +
                "('" + prodotto.Codice + "', '" + prodotto.Cliente.ToString() + "' ,'" + DateTime.Now.Date.ToString("d/M/yyyy") + "'," + nastro.Lunghezza + "," + nastro.Larghezza +
                "," + bordo.Altezza + "," + tazza.Altezza + ", '" + nastro.TipologiaNastro() + "', '" + this._imballi.Tipologia + "','" + imballi.Costo[i] + "€" + "'," +
                imballi.Peso[i] + "," + _imballi.Larghezza[i] + "," + _imballi.Lunghezza[i] + "," + _imballi.Altezza[i] + ",'Offerta','" + prodotto.VersioneCodice + "','" +
                "Bassa'" + ",'" + this._prodotto.Utente + "','" + this._prodotto.TipologiaTrasporto + "'," + Math.Round(this._prodotto.PesoTotaleNastro, 0) + ",'" + this._imballi.Note +
                "','" + this._prodotto.PresenzaFix + "','" + this._prodotto.PresenzaBlinkers + "','" + this._nastro.Tipo + "'," + this._nastro.Classe + "," + this._tazza.Passo + "," + this._prodotto.PistaLaterale +
                "," + this._bordo.Larghezza + ",'" + this._tazza.Forma + "'," + this._tazza.NumeroFile + "," + this._tazza.SpazioFileMultiple + ",'" + this._nastro.Trattamento + "','" +
                this._bordo.Trattamento + "','" + this._tazza.Trattamento + "','" + this._tazza.Telata + "'," + this._prodotto.Qty + ")");
            
        }

        public SqlCommand UpdateDbCommandFerro(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto, Imballi imballi, int i, CassaInFerro cassaInFerro)
        {
            // Oggetti
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._imballi = imballi;

            // Variabili
            string Cassa = this._imballi.GetTipologiaImballo();

            // Determino la presenza degli accessori
            string presenzaincroci = cassaInFerro.DiagonaliIncrocio ? "Si" : "No";
            string presenzaganci = _imballi.Lunghezza[i] >= 8000 ? "Si" : "No";
            string presenzaretelaterale = cassaInFerro.TamponaturaConRete ? "Si" : "No";
            string cassaverniciata = cassaInFerro.Verniciatura ? "Si" : "No";
            string presenzalamierabase = cassaInFerro.FondoLamiera ? "Si" : "No";

            return this.CreateCommand("INSERT INTO ImballiTotali(Codice, Cliente, Stato, Data, LunghezzaNastro, LarghezzaNastro, AltezzaBordo, AltezzaTazze, ApertoChiuso, TipologiaImballo, CostoImballo, PesoImballo, LarghezzaImballo," +
                "LunghezzaImballo, AltezzaImballo, Configurazione, Personalizzazione, Criticita, TipologiaTrasporto, Versione, NumeroCorrugati, NumeroSubbi, DiametroCorrugati," +
                "DiametroSubbi, LunghezzaCorrugati, Note, NotePaladini, PresenzaIncroci, PresenzaGanci, PresenzaReteLaterale, CassaVerniciata, PresenzaLamieraBase, NomeUtente," +
                "PesoTotaleNastro, PresenzaFix, PresenzaBlinkers, TipoNastro, ClasseNastro, PassoTazze, PistaLaterale, BaseBordo, FormaTazze, NumeroTazzexFila, SpazioFile," +
                "TrattamentoNastro, TrattamentoBordo, TrattamentoTazze, TazzeTelate, Qty, TipologiaTrasportoDett) VALUES" +
                "('" + prodotto.Codice + "', '" + prodotto.Cliente.ToString() + "' ,'" + cassaInFerro.Stato.ToString() + "','" + DateTime.Now.Date.ToString("d/M/yyyy") + "'," + nastro.Lunghezza + "," + nastro.Larghezza +
                "," + bordo.Altezza + "," + tazza.Altezza + ", '" + nastro.TipologiaNastro() + "', '" + this._imballi.Tipologia + " ','" + cassaInFerro.PrezzoCassaFinale[i] + "€" + "'," +
                Math.Round(cassaInFerro.PesoFinale[i],0) + "," + Math.Round(_imballi.Larghezza[i],0) + "," + Math.Round(_imballi.Lunghezza[i],0) + "," + Math.Round(_imballi.Altezza[i],0) + "," + cassaInFerro.Configurazione + ",'" + cassaInFerro.Personalizzazione.ToString() + "','" + 
                imballi.Criticita[i] + "','" + this._prodotto.TipologiaTrasporto.ToString() + "'," + this._prodotto.VersioneCodice + "," + this._imballi.NumeroCurveCorrugati[i]
                + "," + this._imballi.NumeroCurvePolistirolo[i] + "," + this._imballi.DiametroCorrugato + "," + this._imballi.DiametroPolistirolo + "," + this._nastro.Larghezza + ",'" +
                this._imballi.Note.ToString() + "','" + this._imballi.NotePaladini.ToString() + "','" + presenzaincroci + "','" + presenzaganci +
                "','" + presenzaretelaterale + "','" + cassaverniciata + "','" + presenzalamierabase + "','" + this._prodotto.Utente + "'," + Math.Round(this._prodotto.PesoTotaleNastro,0) +
                ",'" + this._prodotto.PresenzaFix + "','" + this._prodotto.PresenzaBlinkers + "','" + this._nastro.Tipo + "'," + this._nastro.Classe + "," + this._tazza.Passo + "," + this._prodotto.PistaLaterale + 
                "," + this._bordo.Larghezza + ",'" + this._tazza.Forma + "'," + this._tazza.NumeroFile + "," + this._tazza.SpazioFileMultiple + ",'" + this._nastro.Trattamento + "','" +
                this._bordo.Trattamento + "','" + this._tazza.Trattamento + "','" + this._tazza.Telata + "'," + this._prodotto.Qty + ",'" + this._prodotto.tipologiaTrasportoDett + "')");

        }

        public SqlCommand UpdateDbCommandInputCalcoli(Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto, Materiale materiale)
        {
            // Oggetti
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;
            this._materiale = materiale;

            return this.CreateCommand("INSERT INTO InputCalcoli(Codice, Capacity, FillingFactor, VelocitaNastro, PendenzaMax, Elevazione, DistDalCentro, NomeMateriale, " +
                "DensitaMateriale, AngoloCarico, DimensioneSingolo, Versione) VALUES" +
                "('" + prodotto.Codice + "'," + nastro.capacityRequired + "," + materiale.fillFactor.ToString().Replace(",",".") + "," + nastro.speed.ToString().Replace(",", ".") + "," + nastro.inclinazione + "," + nastro.elevazione.ToString().Replace(",", ".") +
                "," + nastro.centerDistance.ToString().Replace(",", ".") + ",'" + materiale.Nome + "'," + materiale.density.ToString().Replace(",", ".") + "," +
                 materiale.surchAngle.ToString().Replace(",", ".") + "," + materiale.DimSingolo.ToString().Replace(",", ".") + "," + this._prodotto.VersioneCodice + ")");

        }
        public SqlCommand DeleteRowImpostazioni(int ID, string nomeTabella)
        {
            var cmd = this.CreateCommand("DELETE from " + nomeTabella + " Where ID = @ID");
            // Creates the command to retrieve all the checks 
            // you must use the row "Id" (because you can have multiple checks for the same articleCode)
            cmd.Parameters.AddWithValue("@ID", ID);

            return cmd;
        }

        public SqlCommand DeleteRowDBTotaleCommand(string numOfferta, int verImballo)
        {
            var cmd = this.CreateCommand("DELETE from " + TABELLA_IMBALLI_TOTALI + " Where Codice = @Codice AND Versione = @Versione");
            // Creates the command to retrieve all the checks 
            // you must use the row "Id" (because you can have multiple checks for the same articleCode)
            cmd.Parameters.AddWithValue("@Codice", numOfferta);
            cmd.Parameters.AddWithValue("@Versione", verImballo);

            return cmd;
        }

        public SqlCommand ReadDBTotaleCommand(string codice, int versione)
        {
            var cmd = this.CreateCommand("SELECT * from " + TABELLA_IMBALLI_TOTALI + " Where Codice = @Codice AND Versione = @Versione");
            // Creates the command to retrieve all the checks 
            // you must use the row "Id" (because you can have multiple checks for the same articleCode)
            cmd.Parameters.AddWithValue("@Codice", codice);
            cmd.Parameters.AddWithValue("@Versione", versione);

            return cmd;
        }
        public async void EliminaRiga(object selecteditem, string nometabella)
        {
            int ID = 0;
            try
            {
                var dataRow = (selecteditem as DataRowView).Row;
                ID = Convert.ToInt32(dataRow["ID"].ToString());
            }
            catch
            {
                await DialogsHelper.ShowMessageDialog("Devi prima selezionare l'articolo da eliminare");
            }
            // Create the Database wrapper
            DatabaseSQL databaseSQL = DatabaseSQL.CreateDefault();

            // Creates the command to retrieve this check
            SqlCommand createCommand = databaseSQL.DeleteRowImpostazioni(ID, nometabella);
            createCommand.ExecuteNonQuery();

            //  The article's been eliminated correctly
            await DialogsHelper.ShowMessageDialog("Articolo eliminato correttamente");
        }
        public SqlCommand UpdateCategoriesCommand(string catName)
        {
            return this.CreateCommand("INSERT INTO " + TABELLA_CATEGORIE + "(CategoryName) VALUES" + "('" + catName + "')");
        }
        public SqlCommand CreateCategorieCommand()
        {
            return this.CreateCommand("SELECT CategoryName FROM " + TABELLA_CATEGORIE);
        }
        public SqlCommand EmptyCategorieCommand()
        {
            return this.CreateCommand("SELECT CategoryName FROM " + TABELLA_CATEGORIE + " Where CategoryName like '%zzzzzz123345643%'");
        }
        public SqlCommand RemoveCategoriesCommand(string catName)
        {
            return this.CreateCommand("DELETE FROM " + TABELLA_CATEGORIE + " Where CategoryName like '%" + catName + "%'");
        }
    }
}
