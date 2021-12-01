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
        public readonly string TABELLA_CLIENTI = "CF";

        private Nastro _nastro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;
        private Imballi _imballi;

        // Properties
        private string ConnectionString { get; }
        private SqlConnection _connection;

        /// <summary>
        /// You can still create a DatabaseSQL using a different connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public DatabaseSQL(string connectionString)
        {
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

        public SqlCommand ArticleSearchCommand(string tipo, string trattamento, string famiglia, string gruppo, string sottogruppo, int classe = 0, double larghezza = 0)
        {
            // Trasformo la larghezza in string perchè SQL non accetta la virgola come separatore
            string larghezzaSt;
            larghezzaSt = larghezza.ToString(CultureInfo.InvariantCulture);

            return this.CreateCommand("SELECT Cd_AR,Descrizione,Famiglia,Gruppo,SottoGruppo,Unita_Misura_Pr,Prezzo,LarghezzaMKS FROM " +
                TABELLA_ARTICOLI +
                " Where Famiglia = " + "'" + famiglia +
                "' AND  Gruppo = " + "'" + gruppo +
                "' AND  SottoGruppo = " + "'" + sottogruppo +
                "' AND  LarghezzaMKS >= " + larghezzaSt +
                " AND  Descrizione LIKE " + "'%" + trattamento +
                "%' AND  Cd_AR LIKE " + "'%" + classe +
                "%' AND  Cd_AR LIKE " + "'%" + tipo + "%'");
        }

        public SqlCommand RaspaturaBordoSearchCommand(string gruppo, int altezza, string trattamento)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr,SottoGruppo FROM " +
                TABELLA_ARTICOLI +
                " Where Gruppo = " + "'" + gruppo +
                "' AND  Descrizione LIKE " + "'%" + altezza +
                "' AND  SottoGruppo LIKE " + "'%" + trattamento + "%'");
        }
        public SqlCommand RaspaturaTazzeSearchCommand(string gruppo, int altezza, string trattamento, string forma)
        {
            // Eccezione per tazze TC 230 - 240 e 250
            if (altezza == 230 || altezza == 240 || altezza == 250)
            {
                altezza = 220;
            }

            // Eccezione per tazze T 30 - 40 - 55
            if (forma == "T")
            {
                if (altezza == 30 || altezza == 40 || altezza == 55 || altezza == 75)
                {
                    altezza = 20;
                }
            }

            // Eccezione per tazze T 90 - 100 - 110
            if (forma == "T")
            {
                if (altezza == 100 || altezza == 110)
                {
                    altezza = 90;
                }
            }

            // Eccezione per tazze TC 90
            if (altezza == 90 && forma == "TC")
            {
                altezza = 110;
            }

            // Eccezione per tazze TB 30 - 40 - 50
            if (forma == "TB")
            {
                if (altezza == 30 || altezza == 40 || altezza == 50)
                {
                    altezza = 50;
                }
            }

            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr,SottoGruppo FROM " +
                TABELLA_ARTICOLI +
                " Where Gruppo = " + "'" + gruppo +
                "' AND  Cd_AR LIKE " + "'%" + "RAS" + forma + altezza +
                "' AND  SottoGruppo LIKE " + "'%" + trattamento + "%'");
        }
        public SqlCommand AttrezzaggioSearchCommand(int altezza, string famiglia, string sottogruppo)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Sottogruppo = " + "'" + sottogruppo +
                "' AND  Descrizione LIKE " + "'%" + altezza +
                "%' AND  Famiglia LIKE " + "'" + famiglia + "'");
        }
        public SqlCommand PreparazioneSearchCommand(int altezza, string famiglia, string sottogruppo, string descrizione = "")
        {
            if (descrizione !="")
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
               TABELLA_ARTICOLI +
               " Where Sottogruppo = " + "'" + sottogruppo +
               "' AND  Descrizione LIKE " + "'%" + "Preparazione nastro Cleated Belt"  +
               "%' AND  Famiglia LIKE " + "'" + famiglia + "'");
            }
            else
            {
                return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Sottogruppo = " + "'" + sottogruppo +
                "' AND  Descrizione LIKE " + "'%" + "Preparazione nastro B " + altezza +
                "%' AND  Famiglia LIKE " + "'" + famiglia + "'");
            }
            
        }

        public SqlCommand FixSearchCommand(string altezza, string famiglia, string sottogruppo)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Sottogruppo = " + "'" + sottogruppo +
                "' AND  Descrizione LIKE " + "'%" + altezza +
                "%' AND  Famiglia LIKE " + "'" + famiglia + "'");
        }
        public SqlCommand ProdottoSearchCommand(string descrizione)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where  Descrizione LIKE '%" + descrizione + "%'");
        }
        public SqlCommand ImballoSearchCommand(string descrizione)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where  Descrizione LIKE '%" + descrizione + "%'");
        }
        public SqlCommand BlkSearchCommand(string altezza, string famiglia, string sottogruppo, string trattamento)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Sottogruppo = " + "'" + sottogruppo +
                "' AND  Descrizione LIKE " + "'%" + altezza +
                "' AND Cd_Ar LIKE " + "'%" + trattamento +
                "%' AND Cd_Ar LIKE " + "'%" + "BLK" +
                "%' AND  Famiglia LIKE " + "'" + famiglia + "'");
        }

        public SqlCommand ApplicazioneTazzaSearchCommand(int altezza, string famiglia, string sottogruppo, string forma)
        {
            // Eccezione per tazze TC 230 - 240 e 250
            if (altezza == 230 || altezza == 240 || altezza == 250)
            {
                altezza = 220;
            }

            // Eccezione per tazze TC 90
            if (altezza == 90 && forma == "TC")
            {
                altezza = 110;
            }

            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where SottoGruppo LIKE " + "'" + forma + "'" +
                " AND  Descrizione LIKE " + "'%" + altezza +
                "%' AND  Famiglia LIKE " + "'" + famiglia + "'");
        }
        public SqlCommand GiunzioneSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand CommissioniSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand ApplicazioneBlkSearchCommand(string codice)
        {
            return this.CreateCommand("SELECT Cd_AR,Descrizione,Gruppo,Unita_Misura_Pr FROM " +
                TABELLA_ARTICOLI +
                " Where Cd_Ar LIKE " + "'%" + codice + "%'");
        }
        public SqlCommand BordoSearchCommand(string tipo, double altezza, double larghezza, string trattamento, string famiglia, string gruppo, string sottogruppo)
        {
            string larghezzaString;
            larghezzaString = larghezza.ToString().Replace(",", ".");

            return this.CreateCommand("SELECT Cd_AR,Descrizione,Famiglia,Gruppo,SottoGruppo,Unita_Misura_Pr,Prezzo,LarghezzaMKS FROM " +
                TABELLA_ARTICOLI +
                " Where Famiglia = " + "'" + famiglia +
                "' AND  Gruppo = " + "'" + gruppo +
                "' AND  SottoGruppo = " + "'" + sottogruppo +
                "' AND  Cd_AR LIKE " + "'%" + trattamento +
                "%' AND  Cd_AR LIKE " + "'%" + altezza +
                "%' AND  LarghezzaMKS LIKE " + "'%" + larghezzaString +
                "%' AND  Cd_AR LIKE " + "'%" + tipo + "%'");
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
                " PistaLaterale, BaseBordo, FormaTazze, NumeroTazzexFila, SpazioFile, TrattamentoNastro, TrattamentoBordo, TrattamentoTazze, TazzeTelate FROM " + TABELLA_IMBALLI_TOTALI + " ORDER BY Cliente ASC");
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
        public SqlCommand CreateClientiCommand()
        {
            return this.CreateCommand("SELECT Id_CF,Descrizione,Agente_Descrizione,Provvigione FROM " + TABELLA_CLIENTI + " ORDER BY Descrizione ASC");
        }
        public SqlCommand ClienteSearchCommand(string nomeCliente)
        {
            return this.CreateCommand("SELECT Descrizione,Provvigione,Agente_Descrizione,Cd_DOPorto,Localita,Email FROM " +
                TABELLA_CLIENTI +
                " Where Descrizione LIKE " + "'%" + nomeCliente + "%'");
        }
        public SqlCommand CreateSettingNastriCommand()
        {
            return this.CreateCommand("SELECT ID,NomeNastro,Classe,PesoMQ,SpessoreSup,SpessoreInf,NumeroTele,NumeroTessuti,MinimoDiametroPulley,DataUltimoAggiornamento FROM " + TABELLA_NASTRI);
        }
        public SqlCommand CreateSettingTazzeCommand()
        {
            return this.CreateCommand("SELECT ID,Altezza,FormaTC,FormaT,FormaTB,FormaC,PesoTC,PesoT,PesoTB,PesoC,LarghezzaTazzeTC,LarghezzaTazzeT,LarghezzaTazzeTB,LarghezzaTazzeC,DataUltimoAggiornamento FROM " + TABELLA_TAZZE);
        }
        public SqlCommand CreateSettingPaladiniCommand()
        {
            return this.CreateCommand("SELECT ID,Codice,Descrizione,Prezzo,DataUltimoAggiornamento FROM " + TABELLA_LISTINO_PALADINI);
        }

        public SqlCommand CreateSettingParametriCommand()
        {
            return this.CreateCommand("SELECT ID,Elemento,Valore,Note,DataUltimoAggiornamento FROM " + TABELLA_IMPOSTAZIONI);
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
            return this.CreateCommand("SELECT ID,Descrizione,Prezzo,DataUltimoAggiornamento FROM " + TABELLA_COSTI_GESTIONE);
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
            return this.CreateCommand("SELECT ID,Trasporto,Lunghezza,Larghezza, Altezza,DataUltimoAggiornamento FROM " + TABELLA_LIMITI_TRASPORTO);
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
                "TrattamentoNastro, TrattamentoBordo, TrattamentoTazze, TazzeTelate) VALUES" +
                "('" + prodotto.Codice + "', '" + prodotto.Cliente.ToString() + "' ,'" + DateTime.Now.Date.ToString("d/M/yyyy") + "'," + nastro.Lunghezza + "," + nastro.Larghezza +
                "," + bordo.Altezza + "," + tazza.Altezza + ", '" + nastro.TipologiaNastro() + "', '" + this._imballi.Tipologia + "','" + imballi.Costo[i] + "€" + "'," +
                imballi.Peso[i] + "," + _imballi.Larghezza[i] + "," + _imballi.Lunghezza[i] + "," + _imballi.Altezza[i] + ",'Offerta','" + prodotto.VersioneCodice + "','" +
                "Bassa'" + ",'" + this._prodotto.Utente + "','" + this._prodotto.TipologiaTrasporto + "'," + Math.Round(this._prodotto.PesoTotaleNastro, 0) + ",'" + this._imballi.Note +
                "','" + this._prodotto.PresenzaFix + "','" + this._prodotto.PresenzaBlinkers + "','" + this._nastro.Tipo + "'," + this._nastro.Classe + "," + this._tazza.Passo + "," + this._prodotto.PistaLaterale +
                "," + this._bordo.Larghezza + ",'" + this._tazza.Forma + "'," + this._tazza.NumeroFile + "," + this._tazza.SpazioFileMultiple + ",'" + this._nastro.Trattamento + "','" +
                this._bordo.Trattamento + "','" + this._tazza.Trattamento + "','" + this._tazza.Telata + "')");
            
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
                "TrattamentoNastro, TrattamentoBordo, TrattamentoTazze, TazzeTelate) VALUES" +
                "('" + prodotto.Codice + "', '" + prodotto.Cliente.ToString() + "' ,'" + cassaInFerro.Stato.ToString() + "','" + DateTime.Now.Date.ToString("d/M/yyyy") + "'," + nastro.Lunghezza + "," + nastro.Larghezza +
                "," + bordo.Altezza + "," + tazza.Altezza + ", '" + nastro.TipologiaNastro() + "', '" + this._imballi.Tipologia + " ','" + cassaInFerro.PrezzoCassaFinale[i] + "€" + "'," +
                Math.Round(cassaInFerro.PesoFinale[i],0) + "," + _imballi.Larghezza[i] + "," + _imballi.Lunghezza[i] + "," + _imballi.Altezza[i] + "," + cassaInFerro.Configurazione + ",'" + cassaInFerro.Personalizzazione.ToString() + "','" + 
                imballi.Criticita[i] + "','" + this._prodotto.TipologiaTrasporto.ToString() + "'," + this._prodotto.VersioneCodice + "," + this._imballi.NumeroCurveCorrugati[i]
                + "," + this._imballi.NumeroCurvePolistirolo[i] + "," + this._imballi.DiametroCorrugato + "," + this._imballi.DiametroPolistirolo + "," + this._nastro.Larghezza + ",'" +
                this._imballi.Note.ToString() + "','" + this._imballi.NotePaladini.ToString() + "','" + presenzaincroci + "','" + presenzaganci +
                "','" + presenzaretelaterale + "','" + cassaverniciata + "','" + presenzalamierabase + "','" + this._prodotto.Utente + "'," + Math.Round(this._prodotto.PesoTotaleNastro,0) +
                ",'" + this._prodotto.PresenzaFix + "','" + this._prodotto.PresenzaBlinkers + "','" + this._nastro.Tipo + "'," + this._nastro.Classe + "," + this._tazza.Passo + "," + this._prodotto.PistaLaterale + 
                "," + this._bordo.Larghezza + ",'" + this._tazza.Forma + "'," + this._tazza.NumeroFile + "," + this._tazza.SpazioFileMultiple + ",'" + this._nastro.Trattamento + "','" +
                this._bordo.Trattamento + "','" + this._tazza.Trattamento + "','" + this._tazza.Telata + "')");

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
    }
}
