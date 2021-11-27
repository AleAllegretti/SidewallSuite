using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BeltsPack.Models;
using MaterialDesignThemes.Wpf;

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for NastroNonCodDialog.xaml
    /// </summary>
    public partial class NastroNonCodDialog : UserControl
    {
        public int classe { get; set; }
        public double peso { get; set; }
        public string nome { get; set; }
        Nastro _nastro = new Nastro();
        public NastroNonCodDialog(Nastro nastro)
        {
            this.DataContext = this;
            this._nastro = nastro;
            InitializeComponent();
        }

        private void BtnConferma_Click(object sender, RoutedEventArgs e)
        {
            
            if(peso != 0)
            {
                // Assegno i valori
                this._nastro.Peso = peso;
                this._nastro.Tipo = nome;
                this._nastro.Classe = classe;

                // Ritorno alla schermata di input
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            else
            {
                // Inizializza il risultato del dialog
                System.Windows.MessageBox.Show("Assicurati che i dati siano nel formato corretto", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }
    }
}
