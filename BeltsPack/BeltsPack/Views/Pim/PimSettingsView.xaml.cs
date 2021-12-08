using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using static BeltsPack.Models.Pim;
using System;

namespace BeltsPack.Views
{
    /// <summary>
    /// Interaction logic for PimSettingsView.xaml
    /// </summary>
    public partial class PimSettingsView : Page
    {
        List<Categoria> _Categoria = new List<Categoria>();
        public PimSettingsView()
        {
            InitializeComponent();

            // Carico tutte le categorie disponibili a db
            this.ReloadCategories();
        }


        private async void AddCategoria_Click(object sender, RoutedEventArgs e)
        {          
            // Apro il dialog per settare il nome della categoria
            var catName= await DialogsHelper.SelectCatNameDialog(this._Categoria);

            // Creo la nuova chip
            this.CreateNewChip(catName.Name);           

            // Vado ad inserire la categoria nella tabella
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();

            // Creo il comando per ottenere l'imballo selezionato
            SqlCommand createCommand = dbSQL.UpdateCategoriesCommand(catName.Name);
            createCommand.ExecuteNonQuery();
        }

        private void RemoveCategoria(object sender, RoutedEventArgs e)
        {
            // Ricreo il bottone
            Button btn = (Button)sender;
            string btnName = btn.Name.Substring(3);

            // Vado ad inserire la categoria nella tabella
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();

            // Creo il comando per ottenere l'imballo selezionato
            SqlCommand createCommand = dbSQL.RemoveCategoriesCommand(btnName);
            createCommand.ExecuteNonQuery();

            this.NavigationService.Navigate(new PimSettingsView());
        }
        private void CreateNewChip(string name)
        {
            try
            {
                // Creo il bordo della chip
                Border border = new Border();
                border.BorderBrush = Brushes.LightGray;
                border.BorderThickness = new Thickness(1, 1, 1, 1);
                border.Background = Brushes.WhiteSmoke;
                border.CornerRadius = new CornerRadius(5);
                border.Height = 40;
                border.Width = Double.NaN;

                // Creo lo stack panel dove ci metto i componenti
                StackPanel stackPanelMain = new StackPanel();
                stackPanelMain.Orientation = Orientation.Horizontal;
                stackPanelMain.Name = "StackMain" + name;
                stackPanelMain.Width = Double.NaN;
                stackPanelMain.Margin = new Thickness(10);

                // Creo lo stack panel dove ci metto i componenti
                StackPanel stackPanelChld = new StackPanel();
                stackPanelChld.Orientation = Orientation.Horizontal;
                stackPanelChld.Name = "StackChild" + name;
                stackPanelChld.Width = Double.NaN;

                // Creo il checkbox
                CheckBox checkBox = new CheckBox();
                checkBox.Name = "CB" + name;

                // CReo il label
                Label label = new Label();
                label.Name = "LB" + name;
                label.Content = name;
                label.FontWeight = FontWeights.DemiBold;
                label.Margin = new Thickness(5);
                label.Width = Double.NaN;

                // Creo l'icona
                Image image = new Image();
                BitmapImage imagebm = new BitmapImage();
                imagebm.BeginInit();
                imagebm.UriSource = new Uri(@"\Assets\Images\Close_Window.png", UriKind.Relative);
                imagebm.EndInit();
                image.Height = 15;
                image.Width = 15;
                image.Source = imagebm;

                // Creo il bottone
                Button button = new Button();
                button.Name = "Btn" + name;
                button.Background = Brushes.Transparent;
                button.Content = image;
                button.BorderThickness = new Thickness();
                button.BorderBrush = Brushes.Transparent;
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Margin = new Thickness(1);
                button.Width = Double.NaN;
                button.Click += new RoutedEventHandler(RemoveCategoria);

                // Inserisco il bottone a schermo
                stackPanelChld.Children.Add(checkBox);
                stackPanelChld.Children.Add(label);
                stackPanelChld.Children.Add(button);
                border.Child = stackPanelChld;
                stackPanelMain.Children.Add(border);
                this.PanelCat.Children.Add(stackPanelMain);
                this.PanelCat.UpdateLayout();
            }
            catch
            {
                System.Windows.MessageBox.Show("Assicurati che nel nome non ci siano spazi vuoti.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void ReloadCategories()
        {
            try
            {
                // Inizializzo le categorie
                this._Categoria.Clear();

                // Carico le categorie inserite
                DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();

                // Creo il comando per ottenere le categorie presenti
                SqlCommand createCommand = dbSQL.CreateCategorieCommand();
                createCommand.ExecuteNonQuery();
                SqlDataReader rdr = createCommand.ExecuteReader();
                rdr.Read();
                do
                {
                    this._Categoria.Add(new Categoria()
                    {
                        Name = rdr["CategoryName"].ToString(),
                    });
                }
                while (rdr.Read());

                // Per ogni categoria vado a creare un bottone
                foreach (var name in this._Categoria)
                {
                    // Creo la nuova chip
                    this.CreateNewChip(name.Name);
                }
            }
            catch
            {

            }
            
        }
    }
}
