using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BeltsPack.Models
{
	enum TipologiaTrasporto
	{
		NaveStandard,
		Camion,
		Aereo
	}
	public class ProdottoSelezionato
	{

        public string Personalizzazione { get; set; }
		public string Commessa { get; set; }
		public string Cliente { get; set; }
		public string Codice { get; set; }
		public string Data { get; set; }
		public int LunghezzaNastro { get; set; }
		public int LarghezzaNastro { get; set; }
		public int AltezzaBordo { get; set; }
		public string Aperto { get; set; }
		public int Peso { get; set; }
		public int LarghezzaCassa { get; set; }
		public int LunghezzaCassa { get; set; }
		public int AltezzaCassa { get; set; }
		public int Configurazione { get; set; }
		public string TipoTrasporto { get; set; }
		public string Criticita { get; set; }
		public int DiametroCorrugato { get; set; }
		public int DiametroSubbio { get; set; }
		public int NumeroSubbi { get; set; }
		public int NumeroCorrugati { get; set; }
		public int LunghezzaSingoloCorrugato { get; set; }
		public string Note { get; set; }
		public string Stato { get; set; }
		public string DataConsegna { get; set; }
		public string NotePaladini { get; set; }
		public string PresenzaIncroci { get; set; }
		public string PresenzaGanci { get; set; }
		public string PresenzaReteLaterale { get; set; }
		public string CassaVerniciata { get; set; }
		public string PresenzaLamieraBase { get; set; }
		public int VersioneCodice { get; set; }
		public int LunghezzaCassaReale { get; set; }
		public int AltezzaCassaReale { get; set; }
		public string Conformita { get; set; }
		public string NotePostProduzione { get; set; }
		public int LarghezzaCassaReale { get; set; }
		public string Utente { get; set; }
		public double PesoTotaleNastro { get; set; }

	}
    public class Prodotto
    {
		// Numero tazze per fila
		public int NumeroTazzexFila { get; set; }
		// Spazio tra file di tazze
		public int SpazioFile { get; set; }
		// Trattamento nastro
		public string TrattamentoNastro { get; set; }
		// Trattamento bordo
		public string TrattamentoBordo { get; set; }
		// Trattamento tazze
		public string TrattamentoTazze { get; set; }
		// Codice Trasporto
		public string CodiceTrasporto { get; set; }
		// Descrizione Trasporto
		public string DescrizioneTrasporto { get; set; }
		// Quantita Trasporto
		public string QuantitaTrasporto { get; set; }
		// UM Trasporto
		public string UMTrasporto { get; set; }
		// Codice Imballo
		public string CodiceImballo { get; set; }
		// Descrizione Imballo
		public string DescrizioneImballo { get; set; }
		// Quantita Imballo
		public string QuantitaImballo { get; set; }
		// UM Imballo
		public string UMImballo { get; set; }
		// Codice Prodotto
		public string CodiceProdotto { get; set; }
		// Descrizione Prodotto
		public string DescrizioneProdotto { get; set; }
		// Quantita Prodotto
		public string QuantitaProdotto { get; set; }
		// UM Prodotto
		public string UMProdotto { get; set; }
		public string FormaTazze { get; set; }
        public int LarghezzaBordo { get; set; }
        public int PassoTazze { get; set; }
        public int ClasseNastro { get; set; }
        public string TipoNastro { get; set; }
        public string PresenzaFix { get; set; }
        public string PresenzaBlinkers { get; set; }
        public int AltezzaTazze { get; set; }
        public int LunghezzaNastro { get; set; }
		public int LarghezzaNastro { get; set; }
		public int AltezzaBordo { get; set; }
		public string Aperto { get; set; }
		public int LarghezzaAggSoloBordi { get; set; }
		public int FattoreAltezza { get; set; }
        public int AltezzaApplicazioni { get; set; }
		public double PesoTotaleNastro { get; set; }
		public int VersioneCodice { get; set; }
		// Cliente
		public string Cliente { get; set; }

		// Codice articolo
		public string Codice { get; set; }

		// Altezza
		public double Altezza{ get; set; }

		// Tipologia
		private string tipologia;

		public string Tipologia
		{
			get { return tipologia; }
			set { tipologia = value; }
		}

		// Pista laterale
		private int pistaLaterale;

		public int PistaLaterale
		{
			get { return pistaLaterale; }
			set { pistaLaterale = value; }
		}

		// Tipologia di trasporto
		private string tipologiatrasporto;

		public string TipologiaTrasporto
		{
			get { return tipologiatrasporto; }
			set { tipologiatrasporto = value; }
		}
		// Nome utente
		public string Utente { get; set; }

		public string DeterminoNomeUtente(string root)
        {
			XmlDocument infocustomer = new XmlDocument();
			string nomeutente = "";

			if (File.Exists(root) == true)
			{
				infocustomer.Load(@"C:\ASquared\Info.xml");
				foreach (XmlNode node in infocustomer.DocumentElement)
				{
					nomeutente = node.Attributes[0].InnerText.ToString();
					break;
				}
			}
			return nomeutente;
		}
		public void SetPesoTotale(double pesoNastro, double pesoTazze, double pesoBordi)
		{
			this.PesoTotaleNastro = Math.Round(pesoNastro + pesoTazze + pesoBordi,1);
		}
	}
}
