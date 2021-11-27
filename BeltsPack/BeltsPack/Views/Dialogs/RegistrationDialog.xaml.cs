using MaterialDesignThemes.Wpf;
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
using System.Xml;
using System.Xml.Linq;

namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for RegistrationDialog.xaml
    /// </summary>
    public partial class RegistrationDialog : UserControl
    {
        public string username { get; set; }
        private static readonly string INFO_CUSTOMERS = "InfoCustomer.xml";
        public string connectionstring { get; set; }
        public RegistrationDialog()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private static string GetFullPath(string localPdfName)
        {
            // return string.Format("{0}Assets\\Pdf\\" + localPdfName, Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));
            return string.Format("{0}Assets\\XML\\" + localPdfName, "");
        }
        private async void BtnConferma_Click(object sender, RoutedEventArgs e)
        {
            // Vado a leggere su info customer la stringa di database di questo cliente
            XmlDocument infocustomer = new XmlDocument();
            //infocustomer.Load(GetFullPath(INFO_CUSTOMERS));
            try
            {
                infocustomer.Load(@"Assets\XML\" + INFO_CUSTOMERS);
                // Ci conferma l'esistenza dell'utente
                bool EsistenzaUtente = false;

                foreach (XmlNode node in infocustomer.DocumentElement)
                {
                    string name = node.Attributes[0].InnerText;
                    string pass = node.Attributes[1].InnerText;
                    if (name == this.username & pass == this.Password.Password.ToString())
                    {
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            new XDocument(
                            new XElement("root",
                                new XElement("username", new XAttribute("name", name)),
                                        new XElement("dbconnectionstring", new XAttribute("string", child.InnerText)))
                                        )
                        .Save(@"C:\ASquared\Info.xml");
                        }

                        EsistenzaUtente = true;
                    }
                }

            if(EsistenzaUtente)
            {
                // Chiudo il form
                DialogHost.CloseDialogCommand.Execute(null, null);

                // Comunico che è tutto avvenuto correttamente
                await DialogsHelper.ShowConfirmDialog("Registrazione effettuata con successo!", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            else
            {
                // Chiudo il form
                DialogHost.CloseDialogCommand.Execute(null, null);

                // Comunico che è tutto avvenuto correttamente
                await DialogsHelper.ShowConfirmDialog("Username o password non sono corretti", ConfirmDialog.ButtonConf.OK_ONLY);
            }
            }
            catch (Exception ex)
            {
                await DialogsHelper.ShowConfirmDialog("Abbiamo riscontrato il seguente errore: " + ex, ConfirmDialog.ButtonConf.OK_ONLY);
            }
        }
    }
}
