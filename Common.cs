using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BoxUpload
{
    class Common
    {
        public string json;
        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //最大数値
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        //Columns
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        //Rows
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }
                        dataTable.Rows.Add(dataRow); //Datatable
                    }
                }

            }
            catch
            {
            }
            result = dataTable;
            return result;

        }
    }
    public class SetRectangle
    {
        public PdfDocument pdfDoc;
        public ITextExtractionStrategy strategy;
        public string sn;
        PdfReader pdfReader;
        int x1, x2, y1, y2;
        public SetRectangle(int x1, int y1, int x2, int y2, string sourceFileName)
        {
            Rectangle rect = new Rectangle(x1, y1, x2, y2);
            TextRegionEventFilter regionFilter = new TextRegionEventFilter(rect);

            pdfReader = new PdfReader(sourceFileName);
            pdfDoc = new PdfDocument(pdfReader);
            strategy = new FilteredTextEventListener(new LocationTextExtractionStrategy(), regionFilter);
            sn = sourceFileName;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        public string GetPreName(int page)
        {
            SetRectangle invoiceRec = new SetRectangle(x1, y1, x2, y2, sn);
            return PdfTextExtractor.GetTextFromPage(invoiceRec.pdfDoc.GetPage(page), invoiceRec.strategy);
        }
    }
}
