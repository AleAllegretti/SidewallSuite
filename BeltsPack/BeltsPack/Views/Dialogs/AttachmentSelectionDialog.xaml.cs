using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Controls;


namespace BeltsPack.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AttachmentSelectionDialog.xaml
    /// </summary>
    public partial class AttachmentSelectionDialog : UserControl
    {
        

        public AttachmentSelectionDialog(List<Attachements> attachements)
        {
            InitializeComponent();

            if (attachements.Count < 1)
            {
                throw new Exception("You must provide at least an attachment to select");
            }

            // populate the list
            datagridAttachements.ItemsSource = attachements;
            // and select the first item in the list (so we should not care about empty selection)
            this.datagridAttachements.SelectedIndex = 0;
        }
        public class Attachements
        {
            /// <summary>
            /// Create a new attachment, optionally providing its bytes (useful for the image preview, if it's an image)
            /// </summary>
            /// <param name="dbColumn"></param>
            /// <param name="label"></param>
            /// <param name="isPdf">True if this attachment is a PDF file, false otherwise</param>
            /// <param name="rawBytes">The bytes representing the binary data of this attachment (optional)</param>
            public Attachements(string dbColumn, string label, bool isPdf, byte[] rawBytes = null)
            {
                DbColumn = dbColumn ?? throw new ArgumentNullException(nameof(dbColumn));
                Label = label ?? throw new ArgumentNullException(nameof(label));
                IsPdf = isPdf;
                RawBytes = rawBytes;
            }

            public byte[] RawBytes { get; }
            public bool IsPdf { get; set; }
            public string DbColumn { get; set; }
            public string Label { get; set; }
            public string Filename { get; set; }
        }

        private void MostraAllegati_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Attachements selectedItem = (Attachements)this.datagridAttachements.SelectedItem;
            // close the dialog with the selected item as result
            DialogHost.CloseDialogCommand.Execute(selectedItem, null);
        }

        private void AnnullaSelezione_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
