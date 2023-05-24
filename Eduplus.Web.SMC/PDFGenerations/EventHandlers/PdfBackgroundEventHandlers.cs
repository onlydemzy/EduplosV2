using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplus.Web.SMC.PDFGenerations.EventHandlers
{
    public class PdfBackgroundEventHandlers : IEventHandler
    {
        protected Image img;
        protected PdfExtGState gState;

        public void HandleEvent(Event @event)
        {
            /*PdfDocumentEvent docEvent = (PdfDocumentEvent) @event;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(),
                    page.GetResources(), pdfDoc);
            Rectangle area = page.GetPageSize();
            new Canvas(canvas,area)
                .Add(img);
                */

            PdfDocumentEvent docEvent = (PdfDocumentEvent) @event;
            PdfDocument pdf = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new PdfCanvas(
                page.GetLastContentStream(), page.GetResources(), pdf);
            pdfCanvas.SaveState().SetExtGState(gState);
            Canvas canvas = new Canvas(pdfCanvas, page.GetPageSize());


            /*canvas.Add(img
                .ScaleAbsolute(pageSize.GetWidth(), pageSize.GetHeight()));*/

            canvas.Add(img
                /*.ScaleToFit(300, 300)
                .SetFixedPosition(100, 150)*/);

            pdfCanvas.RestoreState();
            pdfCanvas.Release();
        }
        
        public PdfBackgroundEventHandlers(Image image,float opaccity)
        {
            img = image;
            gState = new PdfExtGState().SetFillOpacity(opaccity);
        }
    }
}