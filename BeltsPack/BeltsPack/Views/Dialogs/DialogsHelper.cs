using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeltsPack.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using static BeltsPack.Models.Pim;
using static BeltsPack.Models.Prodotto;
using static BeltsPack.Views.Dialogs.AttachmentSelectionDialog;

namespace BeltsPack.Views.Dialogs
{
    public enum ConfirmDialogResult
    {
        Yes,
        No,
        Cancel
    }
    class DialogsHelper
    {
        private static readonly string DIALOG_HOST_ID = "MainDialogHost";

        public delegate void ProgressDialogEventHandler(DialogSession eventArgs);

        public static async Task<ConfirmDialogResult> ShowConfirmDialog(string message, ConfirmDialog.ButtonConf conf = ConfirmDialog.ButtonConf.YES_NO)
        {
            var view = new ConfirmDialog(conf);
            // set the message
            view.Message = message;

            return (ConfirmDialogResult)await DialogHost.Show(view, DIALOG_HOST_ID);
        }

        public static async Task ShowMessageDialog(string message)
        {
            var view = new ConfirmDialog(ConfirmDialog.ButtonConf.OK_ONLY);
            // set the message
            view.Message = message;

            await DialogHost.Show(view, DIALOG_HOST_ID);
            
        }

        public static void ShowProgress(ProgressDialogEventHandler handler, string message = null)
        {
            var view = new ProgressDialog();
            // set the optional message
            if (message != null)
            {
                view.Message = message;
            }

            DialogHost.Show(view, DIALOG_HOST_ID, (_, e2) => handler(e2.Session), (_, _e2) => { });
        }

        public static void ShowNote(ProgressDialogEventHandler handler, string message = null)
        {
            var view = new ProgressDialog();
            // set the optional message
            if (message != null)
            {
                view.Message = message;
            }

            DialogHost.Show(view, DIALOG_HOST_ID, (_, e2) => handler(e2.Session), (_, _e2) => { });
        }

        public static async Task<Attachements> ShowAttachmentSelectionDialog(List<Attachements> attachements)
        {
            var view = new AttachmentSelectionDialog(attachements);

            return (Attachements)await DialogHost.Show(view, DIALOG_HOST_ID);
        }
        public static async Task<Fornitore> ShowLoghiSelectionDialog(List<Fornitore> fornitori)
        {
            var view = new LogoSelectionDialog();

            return (Fornitore)await DialogHost.Show(view, DIALOG_HOST_ID);
        }
        public static async Task<Categoria> SelectCatNameDialog(List<Categoria> categoria)
        {
            var view = new CategoryNameDialog();

            return (Categoria)await DialogHost.Show(view, DIALOG_HOST_ID);
        }
    }
}
