using NUnit.Framework;
using BeltsPack;
using BeltsPack.Models;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using BeltsPack.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace TestCasseinFerro
{
    public class CalcoliTest
    {
        [Test]
        public void CapacityTest()
        {
            // Imposto gli input e i parametri del nastro
            var _nastro = new Nastro();
            var _tazza = new Tazza();
            var _prodotto = new Prodotto();
            var _bordo = new Bordo();
            var _material = new Materiale();
            var _rullo = new Rullo();
            var _tamburo = new Tamburo();
            var _motore = new Motore();
            var _calcoloCapacity = new CalcoliImpianto(_nastro, _tazza, _prodotto, _bordo, _material, _rullo, _tamburo, _motore);

            // CARATTERISTICHE MOTORE
            _motore.startCoeff = 1.3;

            // CARATTERISTICHE TAMBURO
            _tamburo.coeffAttrito = 0.35;

            // CARATTERISTICHE RULLO
            _rullo.peso = 33.4;
            _rullo.passo = 1;
            _rullo.coeffAttrito = 0.022;
            _rullo.efficienza = 0.85;

            // CARATTERISTICHE TAZZA
            _tazza.Spessore = 127;

            // CARATTERISTICHE NASTRO
            _nastro.LarghezzaUtile = 850;
            _nastro.speed = 1.653;
            _nastro.inclinazione = 45;
            _nastro.Peso = 92.8571;
            _nastro.centerDistance = 120.57;
            _nastro.elevazione = 102.5;
            _nastro.lunghTrattoCarico = 4;
            _nastro.caricoExtra = 1.2;
            _nastro.Larghezza = 1400;

            // CARATTERISTICHE MATERIALE
            _material.density = 1.60;
            _material.fillFactor = 0.75;
            _material.surchAngle = 20;

            // CARATTERISTICHE IMPIANTO
            _calcoloCapacity.Qreq = 636;
            _calcoloCapacity.mu = 0.35;
            _calcoloCapacity.alpha = Math.PI;
            _calcoloCapacity.s1 = 0.02;
            _calcoloCapacity.S2 = 0.03;

            // Calcolo la sezione sottesa al listello
            _calcoloCapacity.GetSezioneListello();

            // Calcolo la sezione che mi rappresenta lì'area di riempimento
            _calcoloCapacity.GetFillingSection();

            // Calcolo la sezione di frizione del materiale
            _calcoloCapacity.SurAngle = 20;
            _calcoloCapacity.GetFrictionSection();

            // Calcolo la sezione totale
            _calcoloCapacity.TotalSection();

            // Calcolo la sezione d'interferenza
            _tazza.Passo = 300;
            _calcoloCapacity.GetPitchSection();

            // Calcolo la portata
            _calcoloCapacity.GetCapacity();

            // Calcolo il coefficiente di sicurezza
            _nastro.SetLengthCoeff();
            _nastro.SetLunghOriz();
            _nastro.SetAngoloMedio();
            _calcoloCapacity.SetPeriphForce();
            _calcoloCapacity.TensionsCalculation();
            _calcoloCapacity.TakeUpCalculations();

            // Stampo i risultati
            Console.WriteLine("CALCOLO SEZIONI");
            Console.WriteLine("Sezione listello: " + _calcoloCapacity.Sezlis);
            Console.WriteLine("Sezione di riempimento: " + _calcoloCapacity.Sb);
            Console.WriteLine("Sezione frizione: " + _calcoloCapacity.FricSection);
            Console.WriteLine("Sezione totale: " + _calcoloCapacity.CleatCapacity);
            Console.WriteLine("Sezione d'interferenza: " + _calcoloCapacity.PitchSection);
            Console.WriteLine("---");
            Console.WriteLine("CALCOLO PORTATA");
            Console.WriteLine("Portata listello: " + _calcoloCapacity.CleatVolume);
            Console.WriteLine("Portata lorda: " + _calcoloCapacity.Q);
            Console.WriteLine("Portata effettiva [m3/h]: " + _calcoloCapacity.Qeff);
            Console.WriteLine("Portata massica [ton/h]: " + _calcoloCapacity.Qteff);
            Console.WriteLine("---");
            Console.WriteLine("CALCOLO COEFF. SICUREZZA");
            Console.WriteLine("Length coeff: " + _nastro.lengthCoeff);
            Console.WriteLine("Length coeff: " + _nastro.lengthCoeff);

            // Verico che i valori calcolati siano corretti
            Assert.That(_calcoloCapacity.Qeff, Is.EqualTo(738).Within(2));
            Assert.That(_calcoloCapacity.Qteff, Is.EqualTo(1181).Within(2));
        }
    }
 
}
