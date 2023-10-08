using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class Motore
    {
        // Coefficiente di start
        public double startCoeff { get; set; }
        public double motorPower { get; set; }

        public void GetCarattersticheMotore()
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
                if (temp.ToString() == "4")
                {
                    this.startCoeff = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Valore")));
                    break;
                }
            }
        }

        public void GetMotorPower(double kW)
        {
            this.motorPower = 0;
            if (kW <= 0.55)
            { this.motorPower = 0.55; }
            else if (0.55 < kW & kW <= 0.75)
            { this.motorPower = 0.75; }
            else if (0.75 < kW & kW <= 1.1)
            { this.motorPower = 1.1; }
            else if (1.1 < kW & kW <= 1.5)
            { this.motorPower = 1.5; }
            else if (1.5 < kW & kW <= 1.85)
            { this.motorPower = 1.85; }
            else if (1.85 < kW & kW <= 2.2)
            { this.motorPower = 2.2; }
            else if (2.2 < kW & kW <= 3)
            { this.motorPower = 3; }
            else if (3 < kW & kW <= 4)
            { this.motorPower = 4; }
            else if (4 < kW & kW <= 5.5)
            { this.motorPower = 5.5; }
            else if (5.5 < kW & kW <= 7.5)
            { this.motorPower = 7.5; }
            else if (7.5 < kW & kW <= 11)
            { this.motorPower = 11; }
            else if (11 < kW & kW <= 15)
            { this.motorPower = 15; }
            else if (15 < kW & kW <= 18.5)
            { this.motorPower = 18.5; }
            else if (18.5 < kW & kW <= 22)
            { this.motorPower = 22; }
            else if (22 < kW & kW <= 30)
            { this.motorPower = 30; }
            else if (30 < kW & kW <= 37)
            { this.motorPower = 37; }
            else if (37 < kW & kW <= 45)
            { this.motorPower = 45; }
            else if (45 < kW & kW <= 55)
            { this.motorPower = 55; }
            else if (55 < kW & kW <= 75)
            { this.motorPower = 75; }
            else if (75 < kW & kW <= 80)
            { this.motorPower = 80; }
            else if (80 < kW & kW <= 110)
            { this.motorPower = 110; }
            else if (110 < kW & kW <= 132)
            { this.motorPower = 132; }
            else if (132 < kW & kW <= 160)
            { this.motorPower = 160; }
            else if (160 < kW & kW <= 200)
            { this.motorPower = 200; }
            else if (200 < kW & kW <= 220)
            { this.motorPower = 220; }
            else if (220 < kW & kW <= 250)
            { this.motorPower = 250; }
            else if (250 < kW & kW <= 300)
            { this.motorPower = 300; }
            else if (300 < kW & kW <= 370)
            { this.motorPower = 370; }
            else if (370 < kW & kW <= 405)
            { this.motorPower = 405; }

        }
    }
}

