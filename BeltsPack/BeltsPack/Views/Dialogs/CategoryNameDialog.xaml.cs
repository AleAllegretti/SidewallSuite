using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BeltsPack.Models.Pim;

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CategoryNameDialog.xaml
    /// </summary>
    public partial class CategoryNameDialog : UserControl
    {
        public string catName { get; set; }
        public CategoryNameDialog()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void BtnConferma_Click(object sender, RoutedEventArgs e)
        {
            Categoria categoria = new Categoria();
            categoria.Name = this.catName;

            // close the dialog with the selected item as result
            DialogHost.CloseDialogCommand.Execute(categoria, null);
        }
    }
}
