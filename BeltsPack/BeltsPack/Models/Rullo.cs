using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public void GetCarattersticheRullo()
        {
            // Crea il wrapper del database
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();

            // Crea il comando SQL
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingsCalcoliCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("ID"));
                if (temp.ToString() == "2")
                {
                    this.coeffAttrito = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                if (temp.ToString() == "3")
                {
                    this.efficienza = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
                if (temp.ToString() == "5")
                {
                    this.passo = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                }
            }

       
        }

        public void GetRulloWeight(double beltWidth)
        {
            this.peso = 0;
            if (beltWidth <= 400)
            { this.peso = 2.7; }
            else if (400 < beltWidth & beltWidth <= 500)
            { this.peso = 5.5; }
            else if (500 < beltWidth & beltWidth <= 650)
            { this.peso = 10; }
            else if (650 < beltWidth & beltWidth <= 800)
            { this.peso = 11.3; }
            else if (800 < beltWidth & beltWidth <= 1000)
            { this.peso = 18.8; }
            else if (1000 < beltWidth & beltWidth <= 1200)
            { this.peso = 30.2; }
            else if (1200 < beltWidth & beltWidth <= 1400)
            { this.peso = 33.4; }
            else if (1400 < beltWidth & beltWidth <= 1600)
            { this.peso = 37.4; }
            else if (1600 < beltWidth & beltWidth <= 1800)
            { this.peso = 58; }
            else if (1800 < beltWidth & beltWidth <= 2000)
            { this.peso = 63; }
            else if (2000 < beltWidth & beltWidth <= 2200)
            { this.peso = 68.5; }
        }
    }
}
