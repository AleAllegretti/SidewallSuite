using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace BeltsPack.Utils
{
    public class EsportatorePDF
    {
        BaseColor SidewallColor = new BaseColor(102, 102, 102);
        
        public void CreaPDF()
        {
            
            // Crea la tipologia di documento
            Document SchedaTecnica = new Document(PageSize.A4, 0, 0, 20, 20);
            // Imposta i margini
            SchedaTecnica.SetMargins(-40, -40, 20, 20);
            // Path di salvataggio
            FileStream fileStream = new FileStream(SavingHelper.PDF_OUTPUT_PATH + "Prova.pdf", FileMode.Create, FileAccess.Write);
            // Scrittura
            PdfWriter pdfWriter = PdfWriter.GetInstance(SchedaTecnica, fileStream);
            // Apre il documento
            SchedaTecnica.Open();
            
            // Crea intestazione
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("C:/Ale/Lavoro/Morinat/BeltsPack/BeltsPack/BeltsPack/Assets/Images/LOGO_SIDEWALL.png");
            PdfPCell CellLogo = new PdfPCell(logo);
            PdfPTable TableLogo = new PdfPTable(2);
            float[] TableLogoWidths = new float[] { 5.0f, 8.0f };
            TableLogo.SetWidths(TableLogoWidths);


            // Crea data          
            PdfPCell CellData = CreaCella("Data");
            PdfPCell CellData2 = new PdfPCell(new Phrase("Ciao"));
            TableLogo.AddCell(CellData2);
            SchedaTecnica.Add(TableLogo);
            // Chiude il documento
            SchedaTecnica.Close();
        }

        public void SetFont()
        {
            FontFactory.Register("C:\\Windows\\Fonts\\georgia.ttf", "Helvetica");
            var SidewallFont = new iTextSharp.text.Font(FontFactory.GetFont("HELVETICA", 11, iTextSharp.text.Font.NORMAL, SidewallColor));

        }
        private PdfPCell CreaCella(string text)
        { 
           // Crea cella
            PdfPCell nestedcell = new PdfPCell(new Phrase("ALe"));
            nestedcell.Border = 0;
            // Crea tabella
            PdfPTable nestedTable = new PdfPTable(1);
            nestedTable.AddCell(nestedcell);

            // Formatta la cella
            PdfPCell cell = new PdfPCell(nestedTable);
            cell.PaddingLeft = 2;
            cell.PaddingBottom = -2;
            cell.Border = 0;

            // Output
            return cell;
        }

    }
}
