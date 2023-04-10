using System;
using Syncfusion.Pdf.Parsing;
using System.Windows.Forms;
using System.IO;
using BeltsPack.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using System.Data.SqlClient;
using BeltsPack.ViewModels;
using System.Windows;
using static BeltsPack.Models.Prodotto;

namespace BeltsPack.Utils
{
    public class PdfUtils
    {
        private static readonly string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string RESOURCE_NAME_SCHEDA_PRODUZIONE = "Scheda_Tecnica_Produzione.pdf";
        private static readonly string RESOURCE_NAME_SCHEDA_PRODUZIONE_LEGNO = "Scheda_Tecnica_Produzione_Legno_Mod.pdf";
        private static readonly string RESOURCE_NAME_SCHEDA_PALADINI = "Scheda_Tecnica_Paladini3.pdf";
        private static readonly string RESOURCE_NAME_SCHEDA_POSTPRODUZIONE = "Scheda_Tecnica_PostProduzione_Mod.pdf";
        private static readonly string RESOURCE_NAME_TDS_BORDI_E_TAZZE = "Sidewalls_Cleats_";
        private static readonly string RESOURCE_NAME_TDS_BORDI = "Sidewalls";
        private static readonly string RESOURCE_NAME_TDS_TAZZE = "Cleats_";

        public string SAVING_PATH;
        private static string GetFullPath(string localPdfName)
        {
            // return string.Format("{0}Assets\\Pdf\\" + localPdfName, Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));
            return string.Format("{0}Assets\\Pdf\\" + localPdfName, RunningPath);
        }

        public string FillSchedaProduzione(ProdottoSelezionato prodottoSelezionato)
        {
            // Carica il template
            //string pdfTemplate = GetFullPath(RESOURCE_NAME_SCHEDA_PRODUZIONE);
            string pdfTemplate =@"Assets\Pdf\" + RESOURCE_NAME_SCHEDA_PRODUZIONE;
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfTemplate);
            PdfLoadedForm loadedForm = loadedDocument.Form;
            PdfLoadedTextBoxField ClienteField = loadedForm.Fields[0] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField CommessaField = loadedForm.Fields[1] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DataField = loadedForm.Fields[2] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField TipologiaField = loadedForm.Fields[3] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghezzaNastroField = loadedForm.Fields[4] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghezzaNastroField = loadedForm.Fields[5] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltezzaBordoField = loadedForm.Fields[6] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghezzaCassaField = loadedForm.Fields[7] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghezzaCassaField = loadedForm.Fields[8] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltezzaCassaField = loadedForm.Fields[9] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField PesoField = loadedForm.Fields[10] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField CriticitàField = loadedForm.Fields[11] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField TipologiaTrasportoField = loadedForm.Fields[12] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField PesoTotaleField = loadedForm.Fields[13] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DiamPolistiroloField = loadedForm.Fields[14] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField NumeroDischiField = loadedForm.Fields[15] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DiamCorrugatoField = loadedForm.Fields[16] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghCorrugatoField = loadedForm.Fields[17] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField NumCorrugatoField = loadedForm.Fields[18] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField PersonalizzazioneField = loadedForm.Fields[19] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField NoteField = loadedForm.Fields[20] as PdfLoadedTextBoxField;

            PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
            PdfGraphics graphics = page.Graphics;
            PdfBitmap image = new PdfBitmap(@"Assets\Images\Configurazione" + prodottoSelezionato.Configurazione + ".png");
            graphics.DrawImage(image, new PointF(160, 500), new SizeF(280, 150));

            // Riempie la scheda pdf
            PesoTotaleField.Text = (prodottoSelezionato.PesoTotaleNastro + prodottoSelezionato.Peso).ToString();
            ClienteField.Text = prodottoSelezionato.Cliente;
            TipologiaField.Text = prodottoSelezionato.Configurazione.ToString();
            LunghezzaNastroField.Text = prodottoSelezionato.LunghezzaNastro.ToString();
            LunghezzaCassaField.Text = prodottoSelezionato.LunghezzaCassa.ToString();
            LarghezzaCassaField.Text = prodottoSelezionato.LarghezzaCassa.ToString();
            CriticitàField.Text = prodottoSelezionato.Criticita.ToString();
            TipologiaTrasportoField.Text = prodottoSelezionato.TipoTrasporto.ToString();
            DiamPolistiroloField.Text = prodottoSelezionato.DiametroSubbio.ToString();
            NumeroDischiField.Text = prodottoSelezionato.NumeroSubbi.ToString();
            PersonalizzazioneField.Text = prodottoSelezionato.Personalizzazione.ToString();
            CommessaField.Text = prodottoSelezionato.Codice;
            DataField.Text = prodottoSelezionato.Data;
            LarghezzaNastroField.Text = prodottoSelezionato.LarghezzaNastro.ToString();
            AltezzaBordoField.Text = prodottoSelezionato.AltezzaBordo.ToString();
            AltezzaCassaField.Text = prodottoSelezionato.AltezzaCassa.ToString();
            PesoField.Text = prodottoSelezionato.Peso.ToString();
            DiamCorrugatoField.Text = prodottoSelezionato.DiametroCorrugato.ToString();
            LunghCorrugatoField.Text = prodottoSelezionato.LunghezzaSingoloCorrugato.ToString();
            NumCorrugatoField.Text = prodottoSelezionato.NumeroCorrugati.ToString();
            NoteField.Text = prodottoSelezionato.Note;

            // Impostazioni del saving dialog
            string FileName;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    // Nome del file
                    FileName = "SchedaProduzione" + "_" + prodottoSelezionato.Cliente + "_" + prodottoSelezionato.Codice + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf";
                    // Rende il documento read-only
                    loadedDocument.Form.Flatten = true;
                    // Salva il file
                    loadedDocument.Save(fbd.SelectedPath + "\\" + FileName);
                    // Apre il file pdf
                    //System.Diagnostics.Process.Start(fbd.SelectedPath + "\\" + FileName);
                    // Memorizzo il path di salvataggio
                    SAVING_PATH = fbd.SelectedPath;
                    // Output
                    return fbd.SelectedPath + "\\" + FileName;
                }
                else
                {
                    return null;
                }
                
            }
        }

        public string FillSchedaTDSSidewallsCleats(Prodotto prodotto, string path, Nastro nastro, Bordo bordo, Fornitore selectedLogo, Tazza tazza)
        {

            // Carica il template
            string pdfTemplate = "";
            if (prodotto.Tipologia == "Bordi e tazze")
            {
                if (tazza.Forma == "TB")
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_BORDI_E_TAZZE + "T" + ".pdf";
                }
                else
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_BORDI_E_TAZZE + tazza.Forma + ".pdf";
                }
            }
            else if (prodotto.Tipologia == "Solo tazze" && tazza.NumeroFile == 1)
            {
                if (tazza.Forma == "TB")
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_TAZZE + "T" + ".pdf";
                }
                else
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_TAZZE + tazza.Forma + ".pdf";
                }
            }
            else if (prodotto.Tipologia == "Solo tazze" && tazza.NumeroFile != 1 && tazza.NumeroFile <= 5)
            {
                if (tazza.Forma == "TB")
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_TAZZE + "T" + "_x_" + tazza.NumeroFile + ".pdf";
                }
                else
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_TAZZE + tazza.Forma + "_x_" + tazza.NumeroFile + ".pdf";
                }
            }
            else if (prodotto.Tipologia == "Solo tazze" && tazza.NumeroFile != 1 && tazza.NumeroFile > 5)
            {
                if (tazza.Forma == "TB")
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_TAZZE + "T" + "_x_5" + ".pdf";
                }
                else
                {
                    pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_TAZZE + tazza.Forma + "_x_5" + ".pdf";
                }
            }
            else
            {
                pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_TDS_BORDI + ".pdf";
            }

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfTemplate);
            PdfLoadedForm loadedForm = loadedDocument.Form;
            PdfLoadedFormFieldCollection fieldCollection = loadedForm.Fields as PdfLoadedFormFieldCollection;
            PdfLoadedField loadedField = null;

            // Cliente
            if (fieldCollection.TryGetField("1", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = prodotto.Cliente;
            }
            // Commessa
            if (fieldCollection.TryGetField("2", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = prodotto.Codice;
            }
            // Mail
            if (fieldCollection.TryGetField("3", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = prodotto.EmailCliente;
            }
            // Larghezza
            if (fieldCollection.TryGetField("4", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = nastro.Larghezza.ToString();
            }
            // Lunghezza
            if (fieldCollection.TryGetField("5", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = nastro.Lunghezza.ToString();
            }
            // Piste laterali
            if (fieldCollection.TryGetField("6", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = prodotto.PistaLaterale.ToString();
            }
            // Larghezza bordo
            if (fieldCollection.TryGetField("7", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = bordo.Larghezza.ToString();
            }
            // Larghezza utile
            if (fieldCollection.TryGetField("8", out loadedField))
            {
                if (tazza.NumeroFile == 1 || tazza.NumeroFile == 0)
                {
                    (loadedField as PdfLoadedTextBoxField).Text = nastro.LarghezzaUtile.ToString();
                }
                else
                {
                    (loadedField as PdfLoadedTextBoxField).Text = (tazza.Lunghezza / tazza.NumeroFile).ToString();
                }
                
            }
            // Altezza bordo
            if (fieldCollection.TryGetField("9", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = bordo.SiglaTele + "-" + bordo.Altezza.ToString();
            }
            // Altezza tazze
            if (fieldCollection.TryGetField("10", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = tazza.SiglaTele + "-" + tazza.Forma + tazza.Altezza.ToString();
            }
            // Nastro base
            if (fieldCollection.TryGetField("11", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = nastro.Tipo + " " + nastro.Classe + "/" + nastro.NumTessuti + "+" +
                        nastro.NumTele + " " + nastro.SpessoreSup + "+" + nastro.SpessoreInf;
            }
            // Passo
            if (fieldCollection.TryGetField("12", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = tazza.Passo.ToString();
            }
            // Qualità gomma
            if (fieldCollection.TryGetField("13", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = nastro.Trattamento;
            }
            // Min pulley diam
            if (fieldCollection.TryGetField("14", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = bordo.MinPulleyDiam.ToString();
            }
            // Min wheel diam
            if (fieldCollection.TryGetField("15", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = bordo.MinWheelDiam.ToString();
            }
            // Op. temperature
            if (fieldCollection.TryGetField("16", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = nastro.RangeTemperatura;
            }
            // Quantity
            if (fieldCollection.TryGetField("17", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = prodotto.Qty.ToString();
            }
            // Notes
            if (fieldCollection.TryGetField("18", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = "SIDEWALL AND CLEATS ARE HOT VULCANIZED.";
            }
            // Presenza olio
            if (fieldCollection.TryGetField("19", out loadedField))
            {
                if (nastro.SiglaTrattamento == "OR" || nastro.SiglaTrattamento == "ORK")
                {
                    (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta2";
                }
                else
                {
                    (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta1";
                }
            }
            // Aperto chiuso
            if (fieldCollection.TryGetField("20", out loadedField))
            {
                if (nastro.Aperto)
                {
                    (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta2";
                }
                else
                {
                    (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta1";
                }
            }
            // Fix
            if (tazza.Forma == "TC" || tazza.Forma == "T")
            {
                if (fieldCollection.TryGetField("21", out loadedField))
                {
                    if (prodotto.PresenzaFix == "Si")
                    {
                        (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta3";
                    }
                    else
                    {
                        (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta1";
                    }
                }
                // Blinkers
                if (fieldCollection.TryGetField("22", out loadedField))
                {

                    if (prodotto.PresenzaBlinkers == "Si")
                    {
                        (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta1";
                    }
                    else
                    {
                        (loadedField as PdfLoadedRadioButtonListField).SelectedValue = "Scelta4";
                    }
                }
            }
            // Spazio tra le file
            if (fieldCollection.TryGetField("23", out loadedField))
            {
                (loadedField as PdfLoadedTextBoxField).Text = tazza.SpazioFileMultiple.ToString();
            }

            // Logo distributore
            if (selectedLogo.ImageLocalPath != "")
            {
                PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
                PdfGraphics graphics = page.Graphics;
                PdfBitmap image = new PdfBitmap(selectedLogo.ImageLocalPath.ToString());
                graphics.DrawImage(image, new PointF(360, 60), new SizeF(selectedLogo.Height, selectedLogo.Width));
            }          

            try
            {
                // Impostazioni del saving dialog
                string FileName;
                // Nome del file
                FileName = "TDS" + "_" + prodotto.Cliente + "_" + prodotto.Codice + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf";
                // Rende il documento read-only
                loadedDocument.Form.Flatten = false;
                // Salva il file
                loadedDocument.Save(path + "\\" + FileName);
                // Output
                return path + "\\" + FileName;
            }
            catch
            {
                System.Windows.MessageBox.Show("C'è stato un problema nella creazione della TDS.\nSe il problema persiste contattare l'assistenza.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return "";
            }
        }
        public string FillSchedaPaladini(ProdottoSelezionato prodottoSelezionato, string selectedPath)
        {
            // Carica il template
            string pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_SCHEDA_PALADINI;
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfTemplate);
            PdfLoadedForm loadedForm = loadedDocument.Form;
            PdfLoadedTextBoxField ClienteField = loadedForm.Fields[0] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField CommessaField = loadedForm.Fields[1] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ConsegnaField = loadedForm.Fields[2] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghLonghField = loadedForm.Fields[3] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltLonghField = loadedForm.Fields[4] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField SpessoreLongField = loadedForm.Fields[5] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltezzaRittoField = loadedForm.Fields[6] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField SpessoreRittoField = loadedForm.Fields[7] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DiametroDiagField = loadedForm.Fields[8] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField SpessoreDiagField = loadedForm.Fields[9] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghLongField = loadedForm.Fields[10] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField QuantLongField = loadedForm.Fields[11] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ComponenteLongField = loadedForm.Fields[12] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghLongRinfField = loadedForm.Fields[13] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField QuantLongRinfField = loadedForm.Fields[14] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ComponenteLongRinfField = loadedForm.Fields[15] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghRittiField = loadedForm.Fields[16] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField QuantRittiField = loadedForm.Fields[17] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ComponenteRittiField = loadedForm.Fields[18] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField QuantDiagField = loadedForm.Fields[19] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ComponenteDiagField = loadedForm.Fields[20] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghTraveInfField = loadedForm.Fields[21] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField QuantTraveInfField = loadedForm.Fields[22] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ComponenteTraveInfField = loadedForm.Fields[23] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ReteTampBaseSiField = loadedForm.Fields[24] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ReteTampBaseNoField = loadedForm.Fields[25] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ReteTampLatSiField = loadedForm.Fields[26] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ReteTampLatNoField = loadedForm.Fields[27] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField GanciSiField = loadedForm.Fields[28] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField GanciNoField = loadedForm.Fields[29] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField VerniciaturaSiField = loadedForm.Fields[30] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField VerniciaturaNoField = loadedForm.Fields[31] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField NoteField = loadedForm.Fields[32] as PdfLoadedTextBoxField;

            // Immagine cassa
            PdfLoadedPage page = loadedDocument.Pages[1] as PdfLoadedPage;
            PdfGraphics graphics = page.Graphics;
            PdfBitmap image = new PdfBitmap(@"Assets\Images\Cassa8MetriBase.jpg");

            // Calcolo delta tra la larghezza della cassa e quella del nastro per capire
            // se la cassa è in doppia fila
            int deltalarghezza;
            deltalarghezza = prodottoSelezionato.LarghezzaCassa - prodottoSelezionato.LarghezzaNastro * 2;

            if (prodottoSelezionato.LunghezzaCassa < 8000 & deltalarghezza < 0)
            {
                image = new PdfBitmap(@"Assets\Images\NoRinforziNoreteLaterale.png");
            }
            else if (prodottoSelezionato.LunghezzaCassa > 8000 & prodottoSelezionato.PresenzaIncroci == "Si" & deltalarghezza < 0)
            {
                image = new PdfBitmap(@"Assets\Images\IncrociRinforzi8metri.png");
            }
            else if(prodottoSelezionato.LunghezzaCassa > 8000 & prodottoSelezionato.PresenzaIncroci == "No" & deltalarghezza < 0)
            {
                image = new PdfBitmap(@"Assets\Images\Cassa8MetriBase.jpg");
            }
            else if (prodottoSelezionato.LunghezzaCassa < 8000 & deltalarghezza > 0)
            {
                image = new PdfBitmap(@"Assets\Images\Cassa_Doppia.png");
            }
            else if (prodottoSelezionato.LunghezzaCassa > 8000 & deltalarghezza > 0)
            {
                image = new PdfBitmap(@"Assets\Images\Cassa_Doppia_8_Metri.png");
            }

            graphics.DrawImage(image, new PointF(160, 350), new SizeF(280, 150));

            // Riempie la scheda pdf
            ClienteField.Text = prodottoSelezionato.Cliente;
            CommessaField.Text = prodottoSelezionato.Codice;
            ConsegnaField.Text = prodottoSelezionato.DataConsegna.ToString();

            // Componenti principali
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateSettingCostiFerroCommand();
            reader = creaComando.ExecuteReader();
            while (reader.Read())
            {
                var temp = reader.GetValue(reader.GetOrdinal("Codice"));
                if (Convert.ToString(temp) == "TUBOLARE80504")
                {
                    LarghLonghField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Larghezza"))).ToString();
                    AltLonghField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Altezza"))).ToString();
                    SpessoreLongField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Spessore"))).ToString();
                }
                if (Convert.ToString(temp) == "TUBOLARE40403")
                {
                    AltezzaRittoField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Larghezza"))).ToString();
                    SpessoreRittoField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Spessore"))).ToString();
                }
                if (Convert.ToString(temp) == "TUBOLAREDIAM20")
                {
                    DiametroDiagField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Larghezza"))).ToString();
                    SpessoreDiagField.Text = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Spessore"))).ToString();
                }
            }

            // Longheroni inferiori
            int quantlongheroni = 4;
            if(deltalarghezza > 0)
            {
                quantlongheroni = 6;
            }

            LunghLongField.Text = prodottoSelezionato.LunghezzaCassa.ToString();
            QuantLongField.Text = quantlongheroni.ToString();
            ComponenteLongField.Text = "(A)";

            // Longheroni di rinforzo
            if (Convert.ToInt32(prodottoSelezionato.LunghezzaCassa) >= 8000)
            {
                LunghLongRinfField.Text = "2000";
                QuantLongRinfField.Text = quantlongheroni.ToString();
                ComponenteLongRinfField.Text = "(A)";
            }
            else
            {
                LunghLongRinfField.Text = "-";
                QuantLongRinfField.Text = "0";
                ComponenteLongRinfField.Text = "-";
            }

            // Ritti
            int numerofile = 1;
            if (deltalarghezza > 0)
            {
                numerofile = 2;
            }
            LunghRittiField.Text = Convert.ToString(prodottoSelezionato.AltezzaCassa - Convert.ToInt32(AltLonghField.Text) * 3);
            QuantRittiField.Text = Convert.ToString((Math.Floor(prodottoSelezionato.LunghezzaCassa / 2 * 0.001) + 1) * (numerofile + 1));
            ComponenteRittiField.Text = "(B)";

            // Diagonali
            QuantDiagField.Text = Convert.ToString((Math.Floor(prodottoSelezionato.LunghezzaCassa / 2 * 0.001) + 1) * 2);
            ComponenteDiagField.Text = "(B)";

            // Traverini inferiori
            LunghTraveInfField.Text = prodottoSelezionato.LarghezzaCassa.ToString();
            QuantTraveInfField.Text = Convert.ToString(prodottoSelezionato.LunghezzaCassa / 1000 + 1);
            ComponenteTraveInfField.Text = "(A)";

            // Ganci
            if (prodottoSelezionato.PresenzaGanci == "Si")
            {
                GanciSiField.Text = "X";
            }
            else
            {
                GanciNoField.Text = "X";
            }

            // Rete laterale
            if (prodottoSelezionato.PresenzaReteLaterale == "Si")
            {
                ReteTampLatSiField.Text = "X";
            }
            else
            {
                ReteTampLatNoField.Text = "X";
            }

            // Verniciatura
            if (prodottoSelezionato.CassaVerniciata == "Si")
            {
                VerniciaturaSiField.Text = "X";
            }
            else
            {
                VerniciaturaNoField.Text = "X";
            }

            // Tamponatura base
            ReteTampBaseSiField.Text = "X";
            ReteTampBaseNoField.Text = "";
   
            // Campo note
            NoteField.Text = prodottoSelezionato.NotePaladini.ToString();

            // Impostazioni del saving dialog
            string Filename = "SchedaTecnicaPaladini" + "_" + prodottoSelezionato.Cliente + "_" + prodottoSelezionato.Codice + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf"; 
            // Rende il documento read-only
            loadedDocument.Form.Flatten = true;
            // Salva il file
            loadedDocument.Save(selectedPath + "\\" + Filename);
            // Output
            return selectedPath + "\\" + Filename;
        }

        public string FillSchedaPostProduzione(ProdottoSelezionato prodottoSelezionato, ReportProduzioneModel reportProduzioneModel)
        {
            // Carica il template
            //string pdfTemplate = GetFullPath(RESOURCE_NAME_SCHEDA_POSTPRODUZIONE);
            string pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_SCHEDA_POSTPRODUZIONE;
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfTemplate);
            PdfLoadedForm loadedForm = loadedDocument.Form;
            PdfLoadedTextBoxField ClienteField = loadedForm.Fields[0] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField CommessaField = loadedForm.Fields[1] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DataField = loadedForm.Fields[2] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField ConformitaField = loadedForm.Fields[3] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghRealeField = loadedForm.Fields[4] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghTeoricaField = loadedForm.Fields[5] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DeltaLunghField = loadedForm.Fields[6] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltRealeField = loadedForm.Fields[7] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltTeoricaField = loadedForm.Fields[8] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DeltaAltField = loadedForm.Fields[9] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghrealeField = loadedForm.Fields[10] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghTeoricaField = loadedForm.Fields[11] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DeltaLarghField = loadedForm.Fields[12] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField NoteField = loadedForm.Fields[13] as PdfLoadedTextBoxField;

            // Riempie il modulo
            ClienteField.Text = prodottoSelezionato.Cliente;
            CommessaField.Text = prodottoSelezionato.Codice;
            DataField.Text = prodottoSelezionato.Data;
            ConformitaField.Text = prodottoSelezionato.Conformita;
            LunghRealeField.Text = prodottoSelezionato.LunghezzaCassaReale.ToString();
            LunghTeoricaField.Text = prodottoSelezionato.LunghezzaCassa.ToString();
            DeltaLunghField.Text = Convert.ToString(Math.Abs(prodottoSelezionato.LunghezzaCassaReale - prodottoSelezionato.LunghezzaCassa));
            AltRealeField.Text = prodottoSelezionato.AltezzaCassaReale.ToString();
            AltTeoricaField.Text = prodottoSelezionato.AltezzaCassa.ToString();
            DeltaAltField.Text = Convert.ToString(Math.Abs(prodottoSelezionato.AltezzaCassaReale - prodottoSelezionato.AltezzaCassa));
            LarghrealeField.Text = prodottoSelezionato.LarghezzaCassaReale.ToString();
            LarghTeoricaField.Text = prodottoSelezionato.LarghezzaCassa.ToString();
            DeltaLarghField.Text = Convert.ToString(Math.Abs(prodottoSelezionato.LarghezzaCassaReale - prodottoSelezionato.LarghezzaCassa));
            NoteField.Text = prodottoSelezionato.NotePostProduzione.ToString();

            PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
            PdfGraphics graphics = page.Graphics;
            int i = 1;
            foreach (var imageProd in reportProduzioneModel.AttachedImages)
            {             
                if (i == 1)
                {
                    PdfBitmap image = new PdfBitmap(imageProd.Fullpath);
                    graphics.DrawImage(image, new PointF(168, 330), new SizeF(250, 155));
                }
                else
                {

                    PdfBitmap image = new PdfBitmap(imageProd.Fullpath);
                    graphics.DrawImage(image, new PointF(168, 530), new SizeF(250, 155));
                }
                
                i += 1;
            }
            
            // Rende il documento read - only
            loadedDocument.Form.Flatten = true;
            // Salva il file
            string root = @"C:\Temp";
            string Filename = "SchedaPostProduzione" + "_" + prodottoSelezionato.Cliente + "_" + prodottoSelezionato.Codice + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf";
            // If directory does not exist, don't even try   
            if (Directory.Exists(root))
            {
                loadedDocument.Save(root + "\\" + Filename);
            }
            else
            {
                Directory.CreateDirectory(root);
                loadedDocument.Save(root + "\\" + Filename);
            }
            // Output
            return root + "\\" + Filename;
        }

        public string FillSchedaProduzioneLegno(ProdottoSelezionato prodottoSelezionato)
        {
            // Carica il template
            //string pdfTemplate = GetFullPath(RESOURCE_NAME_SCHEDA_PRODUZIONE);
            string pdfTemplate = @"Assets\Pdf\" + RESOURCE_NAME_SCHEDA_PRODUZIONE_LEGNO;
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfTemplate);
            PdfLoadedForm loadedForm = loadedDocument.Form;
            PdfLoadedTextBoxField ClienteField = loadedForm.Fields[0] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField CommessaField = loadedForm.Fields[1] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField DataField = loadedForm.Fields[2] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField TipologiaField = loadedForm.Fields[3] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghezzaNastroField = loadedForm.Fields[4] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghezzaNastroField = loadedForm.Fields[5] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltezzaBordoField = loadedForm.Fields[6] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LunghezzaCassaField = loadedForm.Fields[7] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField LarghezzaCassaField = loadedForm.Fields[8] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField AltezzaCassaField = loadedForm.Fields[9] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField PesoField = loadedForm.Fields[10] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField CriticitàField = loadedForm.Fields[11] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField TipologiaTrasportoField = loadedForm.Fields[12] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField PesoTotaleField = loadedForm.Fields[13] as PdfLoadedTextBoxField;
            PdfLoadedTextBoxField NoteField = loadedForm.Fields[14] as PdfLoadedTextBoxField;

            PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
            PdfGraphics graphics = page.Graphics;
            PdfBitmap image = new PdfBitmap(@"Assets\Images\Imballo_bobina.png");
            graphics.DrawImage(image, new PointF(227, 448), new SizeF(160, 160));

            // Riempie la scheda pdf
            PesoTotaleField.Text = (prodottoSelezionato.PesoTotaleNastro + prodottoSelezionato.Peso).ToString();
            ClienteField.Text = prodottoSelezionato.Cliente;
            TipologiaField.Text = prodottoSelezionato.Configurazione.ToString();
            LunghezzaNastroField.Text = prodottoSelezionato.LunghezzaNastro.ToString();
            LunghezzaCassaField.Text = prodottoSelezionato.LunghezzaCassa.ToString();
            LarghezzaCassaField.Text = prodottoSelezionato.LarghezzaCassa.ToString();
            CriticitàField.Text = prodottoSelezionato.Criticita.ToString();
            TipologiaTrasportoField.Text = prodottoSelezionato.TipoTrasporto.ToString();
            CommessaField.Text = prodottoSelezionato.Codice;
            DataField.Text = prodottoSelezionato.Data;
            LarghezzaNastroField.Text = prodottoSelezionato.LarghezzaNastro.ToString();
            AltezzaBordoField.Text = prodottoSelezionato.AltezzaBordo.ToString();
            AltezzaCassaField.Text = prodottoSelezionato.AltezzaCassa.ToString();
            PesoField.Text = prodottoSelezionato.Peso.ToString();
            NoteField.Text = prodottoSelezionato.Note;

            // Impostazioni del saving dialog
            string FileName;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    // Nome del file
                    FileName = "SchedaProduzione" + "_" + prodottoSelezionato.Cliente + "_" + prodottoSelezionato.Codice + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf";
                    // Rende il documento read-only
                    loadedDocument.Form.Flatten = true;
                    // Salva il file
                    loadedDocument.Save(fbd.SelectedPath + "\\" + FileName);
                    // Apre il file pdf
                    //System.Diagnostics.Process.Start(fbd.SelectedPath + "\\" + FileName);
                    // Memorizzo il path di salvataggio
                    SAVING_PATH = fbd.SelectedPath;
                    // Output
                    return fbd.SelectedPath + "\\" + FileName;
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
