﻿using System;
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
		public double[] LimiteLunghezza { get; set; }

		// Limite in larghezza
		public double[] LimiteLarghezza { get; set; }

		// Limite in altezza
		public double[] LimiteAltezza { get; set; }

        // Tipologia cassa
        public string[] TipologiaCassa { get; set; }
        // Tipo Trasporto
        public string[] TipoTrasporto { get; set; }
		// Determino il trasporto di default
		public string[] TrasportoDefault { get; set; }
		// Fattibilità nave
		public bool[] FattibilitaNave { get; set; }
		// Fattibilità camion
		public bool[] FattibilitaCamion { get; set; }
		// Fattibilità imballo
		public bool[] FattibilitaTrasporto { get; set; }
        // Presenza ganci
        public string[] PresenzaGanci { get; set; }
        // Presenza incroci spalle
        public string[] IncrociSpalle { get; set; }
		// Solo ritti
		public string[] PresenzaSoloRitti { get; set; }
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
        // Pannelli sandwich
        public bool PannelliSandwich { get; set; }
        // Diagonali ad incrocio
        public bool DiagonaliIncrocio { get; set; }
		// Prezzo gestione cassa
		public double[] PrezzoGestioneCassa { get; set; }
        // Prezzo pannelli sandwich
        public double[] PrezzoPannelliSandwich { get; set; }
        // Peso pannelli sandwich
        public double[] PesoPannelliSandwich { get; set; }
        // Prezzo manodopera pannelli sandwich
        public double PrezzoManodoperaPannelliSandwich { get; set; }
        // Prezzo manodopera diagonali
        public double[] PrezzoManodoperaDiagonali { get; set; }
		// Prezzo manodopera incrocio
		public double[] PrezzoManodoperaIncroci { get; set; }
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
		// Peso subbi in polistirolo
		public double[] PesoSubbiPolistirolo { get; set; }

		// Prezzo subbi in polistirolo
		public double[] PrezzoSubbiPolistirolo { get; set; }
		// Peso corrugati
		public double[] PesoCorrugati { get; set; }

		// Prezzo corrugati
		public double[] PrezzoCorrugati { get; set; }

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
