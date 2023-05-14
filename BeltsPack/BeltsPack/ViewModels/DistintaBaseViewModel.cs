using BeltsPack.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.ViewModels
{
    public class DistintaBaseViewModel
    {
        private Imballi _imballi;
        private CassaInFerro _cassaInFerro;
        private Nastro _nastro;
        private Bordo _bordo;
        private Tazza _tazza;
        private Prodotto _prodotto;
        private int NumeroConfigurazione;

        private ObservableCollection<DistintaBase> _distintaBase;
        public ObservableCollection<DistintaBase> DistintaBase
        {
            get { return _distintaBase; }
            set { _distintaBase = value; }
        }
        public DistintaBaseViewModel(Imballi imballi, CassaInFerro cassaInFerro, int numeroconfigurazione, 
            Nastro nastro, Bordo bordo, Tazza tazza, Prodotto prodotto)
        {
            this._cassaInFerro = cassaInFerro;
            this._imballi = imballi;
            this.NumeroConfigurazione = numeroconfigurazione;
            this._nastro = nastro;
            this._bordo = bordo;
            this._tazza = tazza;
            this._prodotto = prodotto;

            _distintaBase = new ObservableCollection<DistintaBase>();
            this.GenerateOrders();
        }

        private void GenerateOrders()
        {
            // Nastro
            _distintaBase.Add(new DistintaBase("Nastro base " + this._nastro.Tipo + " " + this._nastro.Classe,
                (this._nastro.Lunghezza + this._nastro.lunghezzaGiunta) * 0.001,
                1,
                0,
                this._nastro.PesoTotale));

            // Bordo
            if(this._prodotto.Tipologia == "Bordi e tazze" | this._prodotto.Tipologia == "Solo bordi")
            {
                _distintaBase.Add(new DistintaBase("Bordo H" + this._bordo.Altezza,
               this._bordo.LunghezzaTotale * 0.001,
               1,
               0,
               this._bordo.PesoTotale));
            }

            // Tazze
            if (this._prodotto.Tipologia == "Bordi e tazze" | this._prodotto.Tipologia == "Solo tazze")
            {
                _distintaBase.Add(new DistintaBase("Tazze " + this._tazza.Forma + this._tazza.Altezza,
               this._tazza.LunghezzaTotale * 0.001,
               1,
               0,
               this._tazza.PesoTotale));
            }

            // Prezzo gestione cassa
            _distintaBase.Add(new DistintaBase("Gestione cassa",
                0,
                1,
                this._cassaInFerro.PrezzoGestioneCassa[NumeroConfigurazione],
                0));

            // Longheroni superiori e inferiori
            _distintaBase.Add(new DistintaBase("Longherone sup. / inf.", 
                _imballi.Lunghezza[NumeroConfigurazione] * 0.001,
                (this._imballi.Numerofile + 1) * 2, 
                this._cassaInFerro.PrezzoLongheroni[NumeroConfigurazione],
                this._cassaInFerro.PesoLongheroni[NumeroConfigurazione] * 0.001 ));

            // Longheroni rinforzo
            if (this._imballi.Lunghezza[NumeroConfigurazione] >= 8000)
            {
                _distintaBase.Add(new DistintaBase("Longheroni rinforzo", 
                2,
                4,
                this._cassaInFerro.PrezzoLongheroniRinforzo[NumeroConfigurazione],
                this._cassaInFerro.PesoLongheroniRinforzo[NumeroConfigurazione] * 0.001));
            }
                
            // Ritti
            _distintaBase.Add(new DistintaBase("Ritti", 
                (this._imballi.Altezza[NumeroConfigurazione] - this._cassaInFerro.AltezzaLongherone * 3) * 0.001,
                this._cassaInFerro.NumeroRitti[NumeroConfigurazione],
                this._cassaInFerro.PrezzoRitti[NumeroConfigurazione],
                this._cassaInFerro.PesoRitti[NumeroConfigurazione] * 0.001));

            // Se la lunghezza della cassa è maggiore o uguale di 8 metri
            if(this._imballi.Lunghezza[NumeroConfigurazione] >= 8000)
            {
                // Manodopera incroci
                _distintaBase.Add(new DistintaBase("Manodopera diagonali",
                    0,
                    1,
                    this._cassaInFerro.PrezzoManodoperaDiagonali[NumeroConfigurazione],
                    0));

                // Diagonali campate da 2000
                _distintaBase.Add(new DistintaBase("Diagonali campate 2000 [mm]", 
                    this._cassaInFerro.LunghezzaDiagonali[NumeroConfigurazione] * 0.001,
                    this._cassaInFerro.NumeroDiagonali[NumeroConfigurazione],
                    this._cassaInFerro.PrezzoDiagonali[NumeroConfigurazione],
                    this._cassaInFerro.PesoDiagonali[NumeroConfigurazione] * 0.001));

                // Diagonali ultima campata
                _distintaBase.Add(new DistintaBase("Diagonali ultima campata", 
                    this._cassaInFerro.LunghezzaDiagonaleUltimaCampata[NumeroConfigurazione]*0.001,
                    2,
                    this._cassaInFerro.PrezzoDiagonaliUltimaCampata[NumeroConfigurazione],
                    this._cassaInFerro.PesoDiagonaliUltimaCampata[NumeroConfigurazione] * 0.001));
            }
            
            // Se la cassa è solo ritti
            if (this._cassaInFerro.SoloRitti == false)
            {
                _distintaBase.Add(new DistintaBase("Traversini sup.", 
                    this._imballi.Larghezza[NumeroConfigurazione] * 0.001, 
                    this._cassaInFerro.NumeroRitti[NumeroConfigurazione] / 2,
                    this._cassaInFerro.PrezzoTraversiniSuperiori[NumeroConfigurazione],
                    this._cassaInFerro.PesoTraversiniSuperiori[NumeroConfigurazione] * 0.001));
            }

            // Traversini inferiori
            _distintaBase.Add(new DistintaBase("Traversini inf.", 
                this._imballi.Larghezza[NumeroConfigurazione] * 0.001,
                this._cassaInFerro.NumeroTraversiniBase[NumeroConfigurazione], 
                this._cassaInFerro.PrezzoTraversiniBase[NumeroConfigurazione],
                this._cassaInFerro.PesoTraversiniBase[NumeroConfigurazione] * 0.001));

            // Se ci sono gli incroci
            if(this._cassaInFerro.DiagonaliIncrocio)
            {
                // Manodopera incroci
                _distintaBase.Add(new DistintaBase("Manodopera incroci",
                    0,
                    1,
                    this._cassaInFerro.PrezzoManodoperaIncroci[NumeroConfigurazione],
                    0));

                // incroci campate da 2000
                _distintaBase.Add(new DistintaBase("Incroci campate 2000 [mm]",
                    this._cassaInFerro.LunghezzaDiagonali[NumeroConfigurazione] * 0.001,
                    this._cassaInFerro.NumeroDiagonali[NumeroConfigurazione],
                    this._cassaInFerro.PrezzoIncroci[NumeroConfigurazione],
                    this._cassaInFerro.PesoIncroci[NumeroConfigurazione] * 0.001));

                // incroci ultima campata
                _distintaBase.Add(new DistintaBase("Incroci ultima campata",
                    this._cassaInFerro.LunghezzaDiagonaleUltimaCampata[NumeroConfigurazione] * 0.001,
                    2,
                    this._cassaInFerro.PrezzoIncrocioUltimaCampata[NumeroConfigurazione],
                    this._cassaInFerro.PesoIncrocioUltimaCampata[NumeroConfigurazione] * 0.001));
            }


            _distintaBase.Add(new DistintaBase("Rete tamponatura base", 
                this._imballi.Lunghezza[NumeroConfigurazione] * 0.001, 
                1, 
                this._cassaInFerro.PrezzoReteTamponaturaBase[NumeroConfigurazione],
                this._cassaInFerro.PesoReteTamponaturaBase[NumeroConfigurazione] * 0.001));

            // Se c'è la rete di tamponatura laterale
            if(this._cassaInFerro.TamponaturaConRete)
            {
                _distintaBase.Add(new DistintaBase("Rete tamponatura laterale", 
                this._imballi.Lunghezza[NumeroConfigurazione] * 0.001,
                2,
                this._cassaInFerro.PrezzoReteTamponatura[NumeroConfigurazione],
                this._cassaInFerro.PesoReteTamponatura[NumeroConfigurazione] * 0.001));
            }
            
            // Pannelli sandwich
            if (this._cassaInFerro.PannelliSandwich)
            {
                // Costo e peso pannelli
                _distintaBase.Add(new DistintaBase("Pannelli sandwich",
                this._imballi.Lunghezza[NumeroConfigurazione] * 0.001,
                4,
                this._cassaInFerro.PrezzoPannelliSandwich[NumeroConfigurazione],
                this._cassaInFerro.PesoPannelliSandwich[NumeroConfigurazione]));

                // Costo manodopera aggiunta pannelli
                _distintaBase.Add(new DistintaBase("Gestione pannelli sandwich",
                this._imballi.Lunghezza[NumeroConfigurazione] * 0.001,
                4,
                this._cassaInFerro.PrezzoManodoperaPannelliSandwich,
                0));
            }

            // Se c'è la vernicitura
            if(this._cassaInFerro.Verniciatura)
            {
                _distintaBase.Add(new DistintaBase("Verniciatura",
                    0, 
                    1, 
                    this._cassaInFerro.PrezzoVerniciatura[NumeroConfigurazione], 
                    0));
            }

            // Peso e prezzo dei ganci
            _distintaBase.Add(new DistintaBase("Ganci",
                0,
                4,
                this._cassaInFerro.PrezzoGanci,
                this._cassaInFerro.PesoGanci));

            // Prezzo cassa
            _distintaBase.Add(new DistintaBase("Prezzo cassa",
                0,
                1,
                this._cassaInFerro.PrezzoCassaSenzaAcc[NumeroConfigurazione],
                0));

            // Peso e prezzo etichette
            _distintaBase.Add(new DistintaBase("Etichette ganci",
                0,
                4,
                this._cassaInFerro.PrezzoEtichetteGanci,
                this._cassaInFerro.PesoEtichetteGanci));

            // Peso e prezzo dei subbi
            _distintaBase.Add(new DistintaBase("Subbi",
                0,
                Convert.ToInt32(this._imballi.NumeroCurvePolistirolo[NumeroConfigurazione]),
                this._cassaInFerro.PrezzoSubbiPolistirolo[NumeroConfigurazione],
                0));

            // Peso e prezzo dei corrugati
            _distintaBase.Add(new DistintaBase("Corrugati",
                this._imballi.Larghezza[NumeroConfigurazione] *0.001,
                Convert.ToInt32(this._imballi.NumeroCurveCorrugati[NumeroConfigurazione]),
                this._cassaInFerro.PrezzoCorrugati[NumeroConfigurazione],
                this._cassaInFerro.PesoCorrugati[NumeroConfigurazione]));

            // Peso e prezzo pluriball in alluminio
            _distintaBase.Add(new DistintaBase("Pluriball in alluminio",
                0,
                1,
                this._cassaInFerro.PrezzoPluriballAlluminio[NumeroConfigurazione],
                this._cassaInFerro.PesoPluriballAlluminio[NumeroConfigurazione]));

            // Totale
            _distintaBase.Add(new DistintaBase("TOTALE",
                0,
                0,
                this._cassaInFerro.PrezzoCassaFinale[NumeroConfigurazione],
                this._cassaInFerro.PesoFinale[NumeroConfigurazione] + this._prodotto.PesoTotaleNastro));
        }
    }
}
