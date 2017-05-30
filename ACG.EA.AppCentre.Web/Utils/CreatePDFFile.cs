using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using FontLines = System.Drawing;

namespace ExportToExcel
{
    public class CreatePDFFile
    {
        public CreatePDFFile()
        {

        }
        //private const string logo = @"D:\Users\iali\Pictures\" + "\\Allstate_logo.jpg";
        private static string logo = System.Configuration.ConfigurationManager.AppSettings["pdfFilePath"].ToString() + @"\Allstate_logo.png";
        
        public static bool CreatePDFDocument(DataTable dt, string filename, System.Web.HttpResponse Response, string user)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                CreatePDFDocumentAsStream(dt, filename, Response, user);
                ds.Tables.Remove(dt);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        private static bool CreatePDFDocumentAsStream(DataTable ds, string filename, HttpResponse Response, string user)
        {
            Document doc = new Document();
            if (File.Exists(filename))
                File.Delete(filename);

            FileStream fs = new FileStream(filename,
               System.IO.FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                doc.AddTitle("Users Permission Report");
                doc.AddSubject("This is a report for permissions on a specified application");
                doc.AddKeywords("Metadata, iTextSharp, application, Appcentre");
                doc.AddCreator(user);
                doc.AddAuthor(user);
                doc.AddHeader("ITextSharp Pdf Print", "Permissions Report");

                AddPageWithImage(doc, logo);
                AddPageWithBasicFormatting(doc, filename, user);

                // Add page labels to the document
                PdfPageLabels pdfPageLabels = new PdfPageLabels();
                pdfPageLabels.AddPageLabel(1, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "AppCentre Reporting");
                //pdfPageLabels.AddPageLabel(5, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Image");
                writer.PageLabels = pdfPageLabels;
                WritePDFFile(ds, ref doc, filename);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                Response.Write(ex.InnerException);
                return false;
            }
            finally
            {
                doc.Close();
                fs.Close();
                requestReport(Response, filename);
            }
        }

        private static void requestReport(HttpResponse Response, string filename)
        {
            if (File.Exists(filename))
            {
                PdfReader reader = new PdfReader(filename);

                System.IO.MemoryStream m = new System.IO.MemoryStream();

                PdfStamper stamper = new PdfStamper(reader, m);
                stamper.ViewerPreferences = PdfWriter.PageLayoutSinglePage | PdfWriter.PageModeUseThumbs;
                stamper.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
                Response.OutputStream.Close();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            else
                Response.Write(filename + " does not exist");
        }

        private static void WritePDFFile(DataTable dt, ref Document document, string filename)
        {
            //throw new NotImplementedException();
            //string hdrTxt = filename.Substring(0, filename.IndexOf("_") - 1) + " permissions report";
            //document.Add(new Paragraph(hdrTxt));
            //document.Add(new Paragraph(DateTime.Now.ToShortDateString()));
            AddParagraph(document, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, new Chunk("\n\n\n"));
            int c = dt.Columns.Count;
            PdfPTable table = new PdfPTable(c);
            int[] width = new int[] { };
            ArrayList<float> tbWidth = new ArrayList<float>();
            //table.SetWidthPercentage(widths, new Rectangle(6, 10));
            table.WidthPercentage = 100.0f;
            table.DefaultCell.Border = (PdfPCell.BOTTOM_BORDER);
            table.DefaultCell.BorderColor = new BaseColor(255, 255, 255);
            table.DefaultCell.BorderColorBottom = new BaseColor(255, 255, 255);
            table.DefaultCell.Padding = 10;
            PdfPCell cell;
            Phrase text;
            foreach (DataColumn header in dt.Columns)
            {
                text = new Phrase(header.ColumnName, new Font(Font.FontFamily.HELVETICA, 16, Font.BOLDITALIC, BaseColor.BLACK));
                cell = new PdfPCell(text);
                cell.PaddingLeft = 2f; cell.HorizontalAlignment = 1;
                tbWidth.Add(2);
                table.AddCell(cell);
            }
            //if (tbWidth.Count > 0)
            //{
            //    tbWidth.CopyTo(width);
            //    table.SetWidths(width);
            //}
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //PdfPRow pRow = new PdfPRow();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = dt.Rows[i][j].ToString();
                    text = new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.BLACK));
                    //text.Font = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL, BaseColor.BLACK);
                    cell = new PdfPCell(text);
                    cell.PaddingLeft = 2f; cell.HorizontalAlignment = 1;
                    //cell.BackgroundColor = new Color(ColorTranslator.FromHtml("#C2D69B"));
                    table.AddCell(cell);
                    //table.col
                    //table.Rows = new List<PdfPRow>();
                }
            }
            //document.Open();
            document.Add(table);
        }

        private static Font _largeFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.BLACK);
        private static Font _standardFont = new Font(Font.FontFamily.HELVETICA, 14, Font.NORMAL, BaseColor.BLACK);
        private static Font _smallFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);

        private static void AddPageWithBasicFormatting(iTextSharp.text.Document doc, string filename, string user)
        {
            // Write page content.  Note the use of fonts and alignment attributes.
            string fName = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - filename.LastIndexOf("\\") - 1);
            //string appName = fName.Substring(0, filename.IndexOf("_") - 1);
            string hdrTxt = fName.Substring(0, fName.IndexOf("_")) + " " + fName.Substring(fName.IndexOf("_") + 1, fName.IndexOf("Permissions") - fName.IndexOf("_") - 1) + " Permissions Report";
            AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n" + hdrTxt + "\n\n"));
            string creator = "GENERATED BY " + user + "\n\n";
            AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, new Chunk(creator));
            
            // Write additional page content            
            AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _smallFont, new Chunk("ON " +
                DateTime.Now.Day.ToString() + " " +
                System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
                DateTime.Now.Year.ToString().ToUpper() + " AT " +
                DateTime.Now.ToShortTimeString()));
        }

        private static void AddParagraph(Document doc, int alignment, iTextSharp.text.Font font, iTextSharp.text.IElement content)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.SetLeading(0f, 1.2f);
            paragraph.Alignment = alignment;
            paragraph.Font = font;
            paragraph.Add(content);
            doc.Add(paragraph);
        }

        private static void AddPageWithImage(iTextSharp.text.Document doc, String imagePath)
        {
            // Read the image file
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new Uri(imagePath));

            // Set the page size to the dimensions of the image BEFORE adding a new page to the document. 
            float imageWidth = image.Width;
            float imageHeight = image.Height;
            image.ScaleToFit(140f, 120f);
            doc.SetMargins(10, 10, 5, 5);
            //doc.SetPageSize(new iTextSharp.text.Rectangle(imageWidth, imageHeight));
            image.Alignment = iTextSharp.text.Element.ALIGN_TOP;
            image.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
            image.SpacingAfter = 1f;
            // Add a new page
            //doc.Open();
            doc.NewPage();

            // Add the image to the page 
            doc.Add(image);
            image = null;
        }
        
    }
}