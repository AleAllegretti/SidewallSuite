using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class CassaInFerro
    {
		// Numero configurazione
		public int Configurazione { get; set; }

		// Limite in lunghezza
		private double limitelunghezza;

		public double LimiteLunghezza
		{
			get { return limitelunghezza; }
			set { limitelunghezza = value; }
		}

		// Limite in larghezza
		private double limitelarghezza;

		public double LimiteLarghezza
		{
			get { return limitelarghezza; }
			set { limitelarghezza = value; }
		}

		// Limite in altezza
		private double limitealtezza;

		public double LimiteAltezza
		{
			get { return limitealtezza; }
			set { limitealtezza = value; }
		}

		// Lunghezza
		private double lunghezza;

		public double Lunghezza
		{
			get { return lunghezza; }
			set { lunghezza = value; }
		}

		// Larghezza
		private double larghezza;

		public double Larghezza
		{
			get { return larghezza; }
			set { larghezza = value; }
		}

		// Altezza
		private double altezza;

		public double Altezza
		{
			get { return altezza; }
			set { altezza = value; }
		}

		// Lunghezza iniziale
		private double lunghezzainiziale;

		public double LunghezzaIniziale
		{
			get { return lunghezzainiziale; }
			set { lunghezzainiziale = value; }
		}

		// Diagonali ad incrocio
		public bool DiagonaliIncrocio { get; set; }

		// Tamponatura con rete
		public bool TamponaturaConRete { get; set; }

		// Tamponatura con rete base
		public bool TamponaturaConReteBase { get; set; }

		// Verniciatura
		public bool Verniciatura { get; set; }

		// Fondo in lamiera
		public bool FondoLamiera { get; set; }

		// Solo ritti
		public bool SoloRitti { get; set; }

		// Personalizzazione
		public string Personalizzazione { get; set; }

		// Peso traversini base
		public double[] PesoTraversiniBase { get; set; }

		// Prezzo traversini base
		public double[] PrezzoTraversiniBase { get; set; }

		// Peso traversini superiori
		public double[] PesoTraversiniSuperiori { get; set; }

		// Prezzo traversini superiori
		public double[] PrezzoTraversiniSuperiori { get; set; }

		// Peso longheroni
		public double[] PesoLongheroni { get; set; }

		// Prezzo longheroni
		public double[] PrezzoLongheroni { get; set; }

		// Peso ritti
		public double[] PesoRitti { get; set; }

		// Prezzo ritti
		public double[] PrezzoRitti { get; set; }
		// Numero ritti
		public int[] NumeroRitti { get; set; }

		// Peso diagonali
		public double[] PesoDiagonali { get; set; }
		// Peso diagonali ultima campata
		public double[] PesoDiagonaliUltimaCampata { get; set; }

		// Prezzo diagonali
		public double[] PrezzoDiagonali { get; set; }
		// Prezzo diagonali ultima campata
		public double[] PrezzoDiagonaliUltimaCampata { get; set; }

		// Peso rete di tamponatura fianchi
		public double[] PesoReteTamponatura { get; set; }

		// Prezzo rete di tamponatura fianchi
		public double[] PrezzoReteTamponatura { get; set; }

		// Prezzo rete di tamponatura base
		public double[] PrezzoReteTamponaturaBase { get; set; }

		// Peso rete di tamponatura base
		public double[] PesoReteTamponaturaBase { get; set; }

		// Prezzo cassa senza accessori
		public double[] PrezzoCassaSenzaAcc { get; set; }

		// Prezzo finale cassa con accessori
		public double[] PrezzoCassaFinale { get; set; }

		// Peso finale cassa
		public double[] PesoFinale { get; set; }

		// Stato dell'imballo
		public string Stato { get; set; }
		// Lunghezza diagonali
		public double[] LunghezzaDiagonali { get; set; }
		// Numero diagonali
		public int[] NumeroDiagonali { get; set; }
		// Lunghezza diagonali
		public double[] LunghezzaDiagonaleUltimaCampata { get; set; }
		// Lunghezza ultima campata
		public double[] LunghezzaUltimaCampata { get; set; }
		// Peso incroci
		public double[] PesoIncroci { get; set; }
		// Peso incroci ultima campata
		public double[] PesoIncrocioUltimaCampata { get; set; }
		// Prezzo incroci
		public double[] PrezzoIncroci { get; set; }
		// Prezzo incroci ultima campata
		public double[] PrezzoIncrocioUltimaCampata { get; set; }
		// Prezzo verniciatura
		public double[] PrezzoVerniciatura { get; set; }
		// Prezzo ganci
		public double PrezzoGanci { get; set; }
		// Peso ganci
		public double PesoGanci { get; set; }
		// Prezzo etichette ganci
		public double PrezzoEtichetteGanci { get; set; }
		// Peso etichette ganci
		public double PesoEtichetteGanci { get; set; }
		// Prezzo longheroni rinforzo
		public double[] PrezzoLongheroniRinforzo { get; set; }
		// Peso longheroni rinforzo
		public double[] PesoLongheroniRinforzo { get; set; }
		// Prezzo pluriball in alluminio
		public double[] PrezzoPluriballAlluminio { get; set; }
		// Peso pluriball in alluminio
		public double[] PesoPluriballAlluminio { get; set; }
		// Numero traversini base
		public int[] NumeroTraversiniBase { get; set; }
		// Indice della configurazione più conveniente
        public int IndiceConfConveniente { get; set; }
        // Altezza longherone
        public int AltezzaLongherone { get; set; }
        // Cassa doppia
        public bool DoppiaFila { get; set; }
    }
}
