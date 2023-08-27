using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class Rullo
    {
        // Peso rullo (portate = carico)
        public double peso { get; set; }
        // Passo rullo (portante = carico)
        public double passo { get; set; }
        // Coefficiente di attrito
        public double coeffAttrito { get; set; }
        // Efficienza
        public double efficienza { get; set; }

    }
}
