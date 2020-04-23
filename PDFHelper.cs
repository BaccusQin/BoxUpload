using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxUpload
{
    class PDFHelper
    {
        public string sourcePath;
        public string sourceFileName;
        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public void SplitPDF()
        {
            Rectangle rect = new Rectangle(x1, y1, x2, y2);
            TextRegionEventFilter regionFilter = new TextRegionEventFilter(rect);
            ITextExtractionStrategy strategy;
            StringBuilder sb = new StringBuilder();

            PdfReader pdfReader = new PdfReader(sourceFileName);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                strategy = new FilteredTextEventListener(new LocationTextExtractionStrategy(), regionFilter);
                string[] preFilename = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy).Split(' ');
               
            }
        }
        

        
    }
}
