using BeltsPack.Models;
using BeltsPack.Utils;
using BeltsPack.ViewModels;
using MaterialDesignThemes.Wpf;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CostWeightSummary.xaml
    /// </summary>
    public partial class CostWeightSummary : UserControl
    {

        public CostWeightSummary(Imballi imballi, CassaInFerro cassaInFerro, int numeroConfigurazione, Nastro nastro,
            Bordo bordo, Tazza tazza, Prodotto prodotto)
        {

            InitializeComponent();

            // Popola la griglia
            DistintaBaseViewModel distintaBaseViewModel = new DistintaBaseViewModel(imballi, cassaInFerro, numeroConfigurazione, nastro,
                bordo, tazza, prodotto);
            gridelencomateriale.ItemsSource = distintaBaseViewModel.DistintaBase;

        }

        private void EsportaExcel_Click(object sender, RoutedEventArgs e)
        {
            
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = gridelencomateriale.ExportToExcel(gridelencomateriale.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            // Abilita i filtri nel foglio Excel
            workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

            // Abilita i bordi
            workBook.Worksheets[0].UsedRange.BorderInside(ExcelLineStyle.Medium, ExcelKnownColors.Black);
            workBook.Worksheets[0].UsedRange.BorderAround(ExcelLineStyle.Medium, ExcelKnownColors.Black);

            // Setta la larghezza della prima colonna
            workBook.Worksheets[0].SetColumnWidth(1, 33);

            // Stile dell'intestazione
            workBook.Worksheets[0].Range["A1:E1"].CellStyle.Font.Bold = true;
            workBook.Worksheets[0].Range["A1:E1"].CellStyle.Color = System.Drawing.Color.LightGray;
            workBook.Worksheets[0].Range["A1:E1"].CellStyle.Font.Size = 10;

            // Salvataggio file
            string FileName = "EstrazioneCostoPesi_CassaInFerro_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
 
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filepath = path + "\\" + FileName;
            try
            {
                workBook.Version = ExcelVersion.Excel2013;
                workBook.SaveAs(filepath);

                // Inizializza il risultato del dialog
                System.Windows.MessageBox.Show("Il file è stato salvato nel desktop.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Apro il file
                System.Diagnostics.Process.Start(filepath);
            }
            catch (Exception ex)
            {
                // Inizializza il risultato del dialog
                System.Windows.MessageBox.Show("C'è già un file aperto con lo stesso nome", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
               
        }

        private void Chiudi_Click(object sender, RoutedEventArgs e)
        {
            // Chiude il dialog
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
