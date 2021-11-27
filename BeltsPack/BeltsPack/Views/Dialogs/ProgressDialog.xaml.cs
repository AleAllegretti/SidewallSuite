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

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : UserControl
    {
        public bool ShowMessage
        {
            get
            {
                return !String.IsNullOrEmpty(Message);
            }
        }

        public string Message { get; set; }
        public ProgressDialog()
        {
            InitializeComponent();
            // this enable the Data Binding
            this.DataContext = this;
        }
    }
}
