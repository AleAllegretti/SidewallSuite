using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class Tamburo
    {
        // Coefficiente di attrito
        public double coeffAttrito { get; set; }

        public void GetCarattersticheTamburo()
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
                if (temp.ToString() == "1")
                {
                    this.coeffAttrito = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                    break;
                }
            }
        }
    }
}
