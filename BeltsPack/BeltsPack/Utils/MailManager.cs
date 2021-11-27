using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.Utils
{
    class Attachment
    {
        public string path { get; set; }
        public string name { get; set; }

        public Attachment(string path, string name)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    class Mail
    {
        private List<Attachment> _attachments;
        private string _subject;
        private string _to;
        public string Body { get; set; }

        public Mail(string subject, string to)
        {
            this._attachments = new List<Attachment>();
            this._subject = subject;
            this._to = to;
        }

        public Mail AddAttachment(string filepath, string name = "Allegato")
        {
            this._attachments.Add(new Attachment(filepath, name));
            return this;
        }

        public void Show()
        {
            Microsoft.Office.Interop.Outlook.Application objApp = new Microsoft.Office.Interop.Outlook.Application();
            // the CreateItem method returns an object which has to be typecast to MailItem before using it.
            var mail = (Microsoft.Office.Interop.Outlook.MailItem)objApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

            // set the main fields of the email
            mail.To = _to;
            mail.Subject = _subject;
            mail.Body = Body;

            // add each attachment
            for (var i = 0; i < _attachments.Count; i++)
            {
                mail.Attachments.Add(
                    (object)_attachments[i].path,
                    Microsoft.Office.Interop.Outlook.OlAttachmentType.olEmbeddeditem,
                    i + 1,
                    (object)_attachments[i].name);
            }

            // this will show the Outlook window, to review the email before sending it
            mail.Display();

            // the following line would send directly the email, without changing/reviewing it
            //  mail.Send();
        }
    }

    class MailManager
    {
        public static readonly string DEFAULT_TO = "alessandro.allegretti1@gmail.com";

        public Mail PrepareEmail(string subject, string to)
        {
            return new Mail(subject, to);
        }

        public Mail PrepareEmail(string subject)
        {
            return new Mail(subject, DEFAULT_TO);
        }
    }
}
