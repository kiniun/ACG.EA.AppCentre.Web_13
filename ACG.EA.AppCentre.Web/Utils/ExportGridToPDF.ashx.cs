using ExportToExcel;
using System;
using System.Data;
using System.IO;
using System.Web;

namespace ExportToExcel
{
    public class ExportGridToPDF : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //write your handler implementation here.
            string tabData = context.Request["pdfData"];
            string user = context.Session["User"].ToString();
            DataTable dt = ConvertCsvData(tabData);
            if (dt == null)
            {
                //  Add some error-catching here...
                return;
            }

            string pdfFilename = context.Request["filename"];

            if (File.Exists(pdfFilename))
                File.Delete(pdfFilename);

            string filePath = System.Configuration.ConfigurationManager.AppSettings["pdfFilePath"].ToString();
            //ACG.EA.AppCentre.Web.Utils.iTextSharpTest txtSharp = new ACG.EA.AppCentre.Web.Utils.iTextSharpTest(@"D:\Users\iali\Documents\Business User Projects\AppCentre\" + pdfFilename);
            //txtSharp.requestCreatedPdfStream(context.Response);
            CreatePDFFile.CreatePDFDocument(dt, filePath + pdfFilename, context.Response,user.ToUpper());
        }

        private DataTable ConvertCsvData(string CSVdata)
        {
            //  Convert a tab-separated set of data into a DataTable, ready for our C# CreatePdfFile libraries
            //  to turn into an Excel file.
            //
            DataTable dt = new DataTable();
            try
            {
                System.Diagnostics.Trace.WriteLine(CSVdata);

                string[] Lines = CSVdata.Split(new char[] { '\r', '\n' });
                if (Lines == null)
                    return dt;
                if (Lines.GetLength(0) == 0)
                    return dt;

                string[] HeaderText = Lines[0].Split('\t');

                int numOfColumns = HeaderText.Length;

                foreach (string header in HeaderText)
                    dt.Columns.Add(header, typeof(string));

                DataRow Row;
                for (int i = 1; i < Lines.GetLength(0); i++)
                {
                    string[] Fields = Lines[i].Split('\t');
                    if (Fields.GetLength(0) == numOfColumns)
                    {
                        Row = dt.NewRow();
                        for (int f = 0; f < numOfColumns; f++)
                            Row[f] = Fields[f].Replace("&apos;", "'");
                        dt.Rows.Add(Row);
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("An exception occurred: " + ex.Message);
                return null;
            }
        }

        #endregion
    }
}