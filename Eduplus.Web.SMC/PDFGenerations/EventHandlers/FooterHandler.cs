using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplus.Web.SMC.PDFGenerations.EventHandlers
{
    public class FooterHandler:IEventHandler
    {
        protected PdfFormXObject placeholder;
        protected float side = 20;
        protected float x = 350;
        protected float y = 10;
        protected float space = 4.5f;
        protected float descent = 3;
        public FooterHandler()
        {
            placeholder = new PdfFormXObject(new Rectangle(0, 0, side, side));
        }

        public virtual void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdf = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            int pageNumber = pdf.GetPageNumber(page);
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new PdfCanvas(page);
            Canvas canvas = new Canvas(pdfCanvas, pageSize);
            canvas.SetFontSize(10);
            Paragraph p = new Paragraph().Add("Page ").Add(pageNumber.ToString()).Add(" of");
            canvas.ShowTextAligned(p, x, y, TextAlignment.RIGHT);
            canvas.Close();
            pdfCanvas.AddXObject(placeholder, x + space, y - descent);
            pdfCanvas.Release();
        }
        public void WriteTotal(PdfDocument pdfDoc)
        {
            Canvas canvas = new Canvas(placeholder, pdfDoc);
            canvas.SetFontSize(10);
            canvas.ShowTextAligned(pdfDoc.GetNumberOfPages().ToString(), 0, descent, TextAlignment.LEFT);
            canvas.Close();
        }
    }
}