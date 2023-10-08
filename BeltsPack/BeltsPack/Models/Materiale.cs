using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class Materiale
    {
        // Dimensione singolo
        public double DimSingolo { get; set; }
        // Nome
        public string Nome { get; set; }
        // Angolo di carico
        public double surchAngle { get; set; }
        // Densità materiale
        public double density { get; set; }
        // Fattore di riempimento
        public double fillFactor { get; set; }
    }
}
