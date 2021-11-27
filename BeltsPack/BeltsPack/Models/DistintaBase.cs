using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Models
{
    public class DistintaBase
    {
		string nomeMateriale;
		double lunghezza;
		int quantita;
		double costo;
		double peso;

		[Display(Name = "Nome del componente")]
		public string NomeMateriale
		{
			get { return nomeMateriale; }
			set { nomeMateriale = value; }
		}
		[Display(Name = "Lunghezza [m]")]
		public double Lunghezza
		{
			get { return lunghezza; }
			set { lunghezza = value; }
		}
		[Display(Name = "Quantità [nr]")]
		public int Quantita
		{
			get { return quantita; }
			set { quantita = value; }
		}
		[Display(Name = "Costo [€]")]
		public double Costo
		{
			get { return costo; }
			set { costo = value; }
		}
		[Display(Name = "Peso [kg]")]
		public double Peso
		{
			get { return peso; }
			set { peso = value; }
		}

		public DistintaBase(string nomeMateriale, double lunghezza, int quantita, double costo, double peso)
		{
			this.NomeMateriale = nomeMateriale;
			this.Lunghezza = lunghezza;
			this.Quantita = quantita;
			this.Costo = costo;
			this.Peso = peso;

		}

	}
}
