using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSummary.xaml
    /// </summary>
    public partial class DialogSummary : UserControl
    {
        public string cliente { get; set; }
        public string Ncommessa { get; set; }
        public int lunghezzaCassa { get; set; }
        public int altezzaCassa { get; set; }
        public int larghezzaCassa { get; set; }
        public int pesoImballo { get; set; }
        public double costoImballo { get; set; }
        public int pesoNastro { get; set; }
        public DialogSummary(string cliente, string commessa, int lunghezza, int altezza, int larghezza, int pesoImballo, double costoImballo, int pesoTotaleNastro)
        {
            this.DataContext = this;
            this.cliente = cliente;
            this.Ncommessa = commessa;
            this.lunghezzaCassa = lunghezza;
            this.altezzaCassa = altezza;
            this.larghezzaCassa = larghezza;
            this.pesoImballo = pesoImballo;
            this.costoImballo = Math.Round(costoImballo,0);
            this.pesoNastro = pesoTotaleNastro + pesoImballo;

            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Ritorno alla schermata di input
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
