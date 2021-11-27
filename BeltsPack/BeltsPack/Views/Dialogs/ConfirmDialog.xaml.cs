
using System.Windows.Controls;


namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialog : UserControl
    {
        public enum ButtonConf
        {
            OK_ONLY,
            YES_NO,
            YES_NO_CANCEL
        }

        public ButtonConf Conf { get; }

        public string YesMessage
        {
            get
            {
                // when configuration is OK_ONLY it non-sense to show "Sì"
                return Conf == ButtonConf.OK_ONLY ? "Ok" : "Sì";
            }
        }

        public string Message { get; set; }

        public bool ShowNo
        {
            get
            {
                return Conf != ButtonConf.OK_ONLY;
            }
        }

        public bool ShowCancel
        {
            get
            {
                return Conf == ButtonConf.YES_NO_CANCEL;
            }
        }

        public ConfirmDialog(ButtonConf conf)
        {
            this.Conf = conf;

            InitializeComponent();
            // this enable the Data Binding
            this.DataContext = this;
        }
    }
}
