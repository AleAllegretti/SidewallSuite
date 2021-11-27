using BeltsPack.Views.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using BeltsPack.Models;
using System.IO;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        public string username { get; set; }
        private static readonly string INFO_CUSTOMERS = "InfoCustomer.xml";
        public LoginView()
        {
            this.DataContext = this;
            InitializeComponent();

            // Verifico che la cartella con le credenziali esiste, altrimenti faccio comparire il form per la registrazione
            string root = @"C:\ASquared\Info.xml";
            if (File.Exists(root) == true)
            {
                this.StackMember.IsEnabled = false;
            }
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

                if (EsistenzaUtente)
                {

                    // Comunico che è tutto avvenuto correttamente
                    await DialogsHelper.ShowConfirmDialog("Registrazione effettuata con successo!", ConfirmDialog.ButtonConf.OK_ONLY);
                    // Navigo sulla schermata di input
                    Prodotto prodotto = new Prodotto();
                    this.NavigationService.Navigate(new InputView(prodotto));
                }
                else
                {

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
