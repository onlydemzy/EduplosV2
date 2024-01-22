using Eduplos.Domain.AcademicModule;
using Eduplos.Domain.CoreModule;
using Eduplos.DTO.AcademicModule;
using Eduplos.Web.SMC.PDFGenerations.EventHandlers;
using iText.Barcodes;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using KS.AES256Encryption;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;

namespace Eduplos.Web.SMC.PDFGenerations
{
    public class StudentResultPdfGenerator
    {
        public static byte[] CreateTransacriptPdf(TranscriptDTO transcript,string transcriptNo,UserData uData)
        {

            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);

            // pdf.SetDefaultPageSize(PageSize.A4);
            //var fonts=PdfFontFactory.CreateFont(StandardFonts.)
            document.SetTopMargin(7);
            document.SetLeftMargin(2);
            document.SetRightMargin(1);
            document.SetBottomMargin(1);
            //var uData=HttpContext.Current.Cache.Get("userData") as UserData;
            //Book mark image
            Image bk = new Image(ImageDataFactory.Create(uData.Logo))
               .ScaleToFit(uData.WataWidth, uData.WataHeight)
                .SetFixedPosition(uData.VWata, uData.HWata);

            PdfBackgroundEventHandlers bgd = new PdfBackgroundEventHandlers(bk,uData.WataOpacity);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, bgd);

            //Add header
            Image header = new Image(ImageDataFactory.Create(uData.Regbanner))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetWidth(570).SetHeight(130);
            var hdr = new Table(1);
            hdr.AddCell(new Cell().Add(header).SetBorder(Border.NO_BORDER));
            document.Add(hdr);

            document.Add(new Paragraph("Student Transcript")
                .SetFontSize(16)
                .SetBold()
                .SetUnderline()
                .SetTextAlignment(TextAlignment.CENTER));

            //Add student Biodata
            var bioTable = new Table(new float[] { 80f, 260f, 280f });
            bioTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            var progDTable = new Table(new float[] { 100f, 180f });
            if (transcript.Photo != null)
            {
                Image foto = new Image(ImageDataFactory.Create(transcript.Photo));
                foto.ScaleToFit(80, 90);
                bioTable.AddCell(new Cell().Add(foto));
            }
            else
            {
                bioTable.AddCell(new Cell().Add(new Paragraph("")));
            }
            var grayBack = new DeviceRgb(r: 184, g: 184, b: 184);
            var perd = new Table(new float[] { 70, 180 })
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetVerticalBorderSpacing(2);

            perd.AddCell(new Cell().Add(new Paragraph("MatricNo ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.RegNo).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Name ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.Name).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Sex ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.Gender).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Year Admitted ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.YearAdmitted).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Entry mode ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.EntryMode??"").SetFontSize(9)));
            if (transcript.BaseCGPA > 0)
            {
                perd.AddCell(new Cell().Add(new Paragraph("Base CGPA ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
                perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.BaseCGPA.ToString()).SetFontSize(9)));
            }
             
            var perProg = new Table(new float[] { 80, 180 })
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetVerticalBorderSpacing(2);
            perProg.AddCell(new Cell().Add(new Paragraph("Duration (years) ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.Duration.ToString()??"").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Programme ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.Programme ?? "").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Programme Type ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.ProgrammeType).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("CGPA ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.CGPA).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Class ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(transcript.DegreeClass).SetFontSize(9)));

            bioTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(perd));
            bioTable.AddCell(new Cell().Add(perProg).SetBorder(Border.NO_BORDER));
            
            document.Add(bioTable);
            document.Add(new Paragraph("Academic Record").SetTextAlignment(TextAlignment.CENTER));

            //Adding Academic Records
            var recs = new Table(1);
            
            foreach(var hd in transcript.SemesterSummaries)
            {
                var acada = new Table(new float[] {20,200,20,20,20,20 })
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                acada.AddCell(new Cell(1,6)
                    .Add(new Paragraph(hd.Title).SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));
                acada.AddCell(new Cell().Add(new Paragraph("Course Code ").SetFontSize(9)));
                acada.AddCell(new Cell().Add(new Paragraph("Title ").SetFontSize(9)));
                acada.AddCell(new Cell().Add(new Paragraph("Unit ").SetFontSize(9)));
                acada.AddCell(new Cell().Add(new Paragraph("Grade ").SetFontSize(9)));
                acada.AddCell(new Cell().Add(new Paragraph("Grade Point ").SetFontSize(9)));
                acada.AddCell(new Cell().Add(new Paragraph("Quality Point ").SetFontSize(9)));
                
                foreach (var r in hd.SemesterResults)
                {
                   
                    acada.AddCell(new Cell().Add(new Paragraph(r.CourseCode))).SetFontSize(9);
                    acada.AddCell(new Cell().Add(new Paragraph(r.CourseTitle))).SetFontSize(9);
                    acada.AddCell(new Cell().Add(new Paragraph(r.CHr.ToString()))).SetFontSize(9);
                    acada.AddCell(new Cell().Add(new Paragraph(r.Grade))).SetFontSize(9);
                    acada.AddCell(new Cell().Add(new Paragraph(r.GradePoint.ToString()))).SetFontSize(9);
                    acada.AddCell(new Cell().Add(new Paragraph(r.QualityPoint.ToString()))).SetFontSize(9);
                    
                }
                acada.AddCell(new Cell().Add(new Paragraph("GPA= "+hd.GPA).SetBold().SetFontSize(9)));
                acada.AddCell(new Cell().Add(new Paragraph("CGPA= "+hd.CGPA).SetBold().SetFontSize(9)));
                
                acada.SetKeepTogether(true);

                document.Add(acada);

                
            }

            var finalTable = new Table(2).SetMarginLeft(50)
                .SetMarginTop(50);
            Image sig = new Image(ImageDataFactory.Create(uData.RegSign));
            sig.SetHeight(100).SetWidth(100);

                
            finalTable.AddCell(new Cell().Add(sig)
                .SetBorder(Border.NO_BORDER)
                .SetBold()
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.BOTTOM));

            
            string encript = DataEncryption.AESEncryptData("Transcript_"+transcriptNo);
            var req = HttpContext.Current.Request;
            BarcodeQRCode qrcode = new BarcodeQRCode(req.Url.Scheme+"://"+req.Url.Authority+"/DocuVerify/Verify?q="+encript);
            
            PdfFormXObject barcodeObject = qrcode.CreateFormXObject(ColorConstants.BLACK, pdf);
            Image barcodeImage = new Image(barcodeObject).SetWidth(150f).SetHeight(150f);
            finalTable.AddCell(new Cell().Add(barcodeImage));
            finalTable.AddCell(new Cell().Add(new Paragraph("Registrar")
                .SetBold()).SetBorder(Border.NO_BORDER));
            finalTable.AddCell(new Cell().Add(new Paragraph("Scan me to Verify").SetTextAlignment(TextAlignment.CENTER)));
            document.Add(finalTable);

            var tab = new Table(new float[] { 50, 50 }).SetMarginLeft(50);
            tab.SetCaption(new Div().Add(new Paragraph("Grading System")
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)));
            tab.AddCell(new Cell().Add(new Paragraph("Range").SetTextAlignment(TextAlignment.CENTER)));
            tab.AddCell(new Cell().Add(new Paragraph("Class").SetTextAlignment(TextAlignment.CENTER)));
            foreach(var g in transcript.GradClass)
            {
                tab.AddCell(new Cell().Add(new Paragraph(g.Low.ToString()+"-"+g.High.ToString()).SetTextAlignment(TextAlignment.CENTER)));
                tab.AddCell(new Cell().Add(new Paragraph(g.Remark )));
            }
            document.Add(tab);
            document.Close();
            return stream.ToArray();
        }

        public static byte[] GenerateBroadsheetPdf(BroadSheetDTO dto, UserData udata, string title)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            
            var document = new Document(pdf,PageSize.LEGAL.Rotate());
            document.SetTopMargin(4);
            document.SetLeftMargin(2);
            document.SetRightMargin(2);
            document.SetBottomMargin(20);
            
            //Add Header and Title
            var log = new Image(ImageDataFactory.Create(udata.Logo)).SetWidth(100).SetHeight(100).SetHorizontalAlignment(HorizontalAlignment.CENTER);


            var logotab = new Table(new float[] { 900 });
            logotab.AddCell(new Cell().Add(log).SetBorder(Border.NO_BORDER));
            logotab.AddCell(new Cell().Add(new Paragraph(udata.InstitutionName).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            logotab.AddCell(new Cell().Add(new Paragraph(title).SetTextAlignment(TextAlignment.LEFT).SetUnderline())
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            document.Add(logotab);

            //Add Descriptive texts
            string fac;
            Table mainHeader = new Table(2);
            var hdr1 = new Table(new float[] { 70, 350 });


            var hdr2 = new Table(new float[] { 50, 350 });
            if (dto.Faculty.Contains("School")) { fac = "School"; }
            else if (dto.Faculty.Contains("College")) { fac = "College"; }
            else { fac = "Faculty"; }
            Paragraph para = new Paragraph("");

            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(fac).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Faculty).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Department: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Department).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Programme Type: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.ProgrammeType).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Programme: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Programme).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));

            mainHeader.AddCell(new Cell().Add(hdr1).SetBorder(Border.NO_BORDER));

            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Session: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Session).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Semester: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Semester).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Level: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Level.ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10)));

            mainHeader.AddCell(new Cell().Add(hdr2).SetBorder(Border.NO_BORDER));
            document.Add(mainHeader);


            // Add Result Table
            var resultTable = new Table(dto.Results.Columns.Count + 1).SetWidth(1000);

            resultTable.AddCell(new Cell().Add(new Paragraph("S/N").SetFontSize(9)));
            foreach (DataColumn c in dto.Results.Columns)
            {
                resultTable.AddCell(new Cell().Add(new Paragraph(c.ColumnName.ToString()).SetFontSize(9)).SetKeepTogether(true));

            }
            bool condition = true;
            var grayBack = new DeviceRgb(r: 240, g: 240, b: 240);
            var whiteBack = new DeviceRgb(255, 255, 255);
            int count = 0;
            foreach (DataRow r in dto.Results.Rows)
            {
                resultTable.AddCell(new Cell().Add(new Paragraph((count + 1).ToString()).SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                foreach (DataColumn c in dto.Results.Columns)
                {
                    // table.AddCell(new Cell().Add(new Paragraph("1").SetFontSize(8)));
                    resultTable.AddCell(new Cell().Add(new Paragraph(r[c.ColumnName].ToString()).SetFontSize(9)));

                }
                count++;
            }
            // Table row alternate colors
            int tRows = resultTable.GetNumberOfRows();
            int cCols = resultTable.GetNumberOfColumns();
            for (int r = 0; r < tRows; r++)
            {
                for (int c = 0; c < cCols; c++)
                {
                    resultTable.GetCell(r, c).SetBackgroundColor(condition ? whiteBack : grayBack);
                }
                condition = !condition;
            }

            document.Add(resultTable);
            var sing = new Table(3).SetMarginTop(50).SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetHorizontalBorderSpacing(50);
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------------").SetFontSize(10)));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------------").SetFontSize(10)));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------------").SetFontSize(10)));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Exams Officer").SetFontSize(10)).SetTextAlignment(TextAlignment.CENTER));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Head of Department").SetFontSize(10)).SetTextAlignment(TextAlignment.CENTER));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Dean of " + fac).SetFontSize(10)).SetTextAlignment(TextAlignment.CENTER));
            document.Add(sing);

            FooterHandler footerHandler = new FooterHandler();
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerHandler);
            footerHandler.WriteTotal(pdf);

            document.Close();
            return stream.ToArray();
        }
        public static byte[] GenerateFinalResultsheetPdf(BroadSheetDTO dto, UserData udata, string title,List<GraduatingClass> gradClass)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);

            var document = new Document(pdf, PageSize.LEGAL.Rotate());
            document.SetTopMargin(4);
            document.SetLeftMargin(2);
            document.SetRightMargin(2);
            document.SetBottomMargin(20);

            //Add Header and Title
            var log = new Image(ImageDataFactory.Create(udata.Logo)).SetWidth(100).SetHeight(100).SetHorizontalAlignment(HorizontalAlignment.CENTER);


            var logotab = new Table(new float[] { 900 });
            logotab.AddCell(new Cell().Add(log).SetBorder(Border.NO_BORDER));
            logotab.AddCell(new Cell().Add(new Paragraph(udata.InstitutionName).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            logotab.AddCell(new Cell().Add(new Paragraph(title).SetTextAlignment(TextAlignment.LEFT).SetUnderline())
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            document.Add(logotab);

            //Add Descriptive texts
            string fac;
            Table mainHeader = new Table(2);
            var hdr1 = new Table(new float[] { 70, 350 });


            var hdr2 = new Table(new float[] { 50, 350 });
            if (dto.Faculty.Contains("School")) { fac = "School"; }
            else if (dto.Faculty.Contains("College")) { fac = "College"; }
            else { fac = "Faculty"; }
            Paragraph para = new Paragraph("");

            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(fac).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Faculty).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Department: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Department).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Programme Type: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.ProgrammeType).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Programme: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr1.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Programme).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));

            mainHeader.AddCell(new Cell().Add(hdr1).SetBorder(Border.NO_BORDER));

            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Session: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Session).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Batch: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Name).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));


            mainHeader.AddCell(new Cell().Add(hdr2).SetBorder(Border.NO_BORDER));
            document.Add(mainHeader);


            // Add Result Table
            var resultTable = new Table(dto.Results.Columns.Count + 1).SetWidth(1000);

            resultTable.AddCell(new Cell().Add(new Paragraph("S/N").SetFontSize(8)));
            foreach (DataColumn c in dto.Results.Columns)
            {
                resultTable.AddCell(new Cell().Add(new Paragraph(c.ColumnName.ToString()).SetFontSize(8)).SetKeepTogether(true));

            }
            bool condition = true;
            var grayBack = new DeviceRgb(r: 240, g: 240, b: 240);
            var whiteBack = new DeviceRgb(255, 255, 255);
            int count = 0;
            foreach (DataRow r in dto.Results.Rows)
            {
                resultTable.AddCell(new Cell().Add(new Paragraph((count + 1).ToString()).SetFontSize(8)).SetTextAlignment(TextAlignment.CENTER));
                foreach (DataColumn c in dto.Results.Columns)
                {
                    // table.AddCell(new Cell().Add(new Paragraph("1").SetFontSize(8)));
                    resultTable.AddCell(new Cell().Add(new Paragraph(r[c.ColumnName].ToString()).SetFontSize(8)));

                }
                count++;
            }
            // Table row alternate colors
            int tRows = resultTable.GetNumberOfRows();
            int cCols = resultTable.GetNumberOfColumns();
            for (int r = 0; r < tRows; r++)
            {
                for (int c = 0; c < cCols; c++)
                {
                    resultTable.GetCell(r, c).SetBackgroundColor(condition ? whiteBack : grayBack);
                }
                condition = !condition;
            }
            document.Add(resultTable);
            var bigTab = new Table(2);
            var tab = new Table(new float[] { 50, 50 }).SetMarginLeft(50);
            tab.SetCaption(new Div().Add(new Paragraph("Grading System")
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)));
            tab.AddCell(new Cell().Add(new Paragraph("Range").SetTextAlignment(TextAlignment.CENTER)));
            tab.AddCell(new Cell().Add(new Paragraph("Class").SetTextAlignment(TextAlignment.CENTER)));
            foreach (var g in gradClass)
            {
                tab.AddCell(new Cell().Add(new Paragraph(g.Low.ToString() + "-" + g.High.ToString()).SetTextAlignment(TextAlignment.CENTER)));
                tab.AddCell(new Cell().Add(new Paragraph(g.Remark)));
            }
            bigTab.AddCell(new Cell().Add(tab));
            var tabLabel = new Table(2);
            tabLabel.SetCaption(new Div().Add(new Paragraph("Label")).SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            tabLabel.AddCell(new Cell().Add(new Paragraph("Abr").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("Meaning").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("CH").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("Credit Hour").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("TQP").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("Total Quality Points").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("CGPA").SetTextAlignment(TextAlignment.CENTER)));
            tabLabel.AddCell(new Cell().Add(new Paragraph("Cumulative Grade Point Average").SetTextAlignment(TextAlignment.CENTER)));

            bigTab.AddCell(new Cell().Add(tabLabel));
            document.Add(bigTab);

            
            var sing = new Table(3).SetMarginTop(50).SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetHorizontalBorderSpacing(50);
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------------").SetFontSize(9)));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------------").SetFontSize(9)));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------------").SetFontSize(9)));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Exams Officer").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Head of Department").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
            sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Dean of " + fac).SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));

            document.Add(sing);

            FooterHandler footerHandler = new FooterHandler();
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerHandler);
            footerHandler.WriteTotal(pdf);

            document.Close();
            return stream.ToArray();
        }

        public static byte[] StudentCourseForm(StudentAcademicProfileDTO reg,UserData uData)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);

            // pdf.SetDefaultPageSize(PageSize.A4);
            //var fonts=PdfFontFactory.CreateFont(StandardFonts.)
            document.SetTopMargin(7);
            document.SetLeftMargin(2);
            document.SetRightMargin(1);
            document.SetBottomMargin(1);
            //var uData=HttpContext.Current.Cache.Get("userData") as UserData;
            //Book mark image
            Image bk = new Image(ImageDataFactory.Create(uData.Logo))
               .ScaleToFit(uData.WataWidth, uData.WataHeight)
                .SetFixedPosition(uData.VWata, uData.HWata);

            PdfBackgroundEventHandlers bgd = new PdfBackgroundEventHandlers(bk, uData.WataOpacity);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, bgd);

            //Add header
            var log = new Image(ImageDataFactory.Create(uData.Logo)).SetWidth(100).SetHeight(100).SetHorizontalAlignment(HorizontalAlignment.CENTER);

            //Lines
            var linecolor = new DeviceRgb(r: 000, g: 000, b: 000);
            var hdrline = new SolidLine(1f);
            hdrline.SetColor(linecolor);
            LineSeparator ls = new LineSeparator(hdrline);
            
            //Data
            var logotab = new Table(new float[] { 900 });
            logotab.AddCell(new Cell().Add(log).SetBorder(Border.NO_BORDER));
            logotab.AddCell(new Cell().Add(new Paragraph(uData.InstitutionName).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            logotab.AddCell(new Cell().Add(ls));
            logotab.AddCell(new Cell().Add(new Paragraph("Course Registration Form").SetTextAlignment(TextAlignment.LEFT).SetUnderline())
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            document.Add(logotab);
            
            //Add student Biodata
            var bioTable = new Table(new float[] { 80f, 260f, 280f });
            bioTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            var progDTable = new Table(new float[] { 100f, 180f });
            if (reg.Photo != null)
            {
                Image foto = new Image(ImageDataFactory.Create(reg.Photo));
                foto.ScaleToFit(100, 100);
                bioTable.AddCell(new Cell().Add(foto));
            }
            else
            {
                bioTable.AddCell(new Cell().Add(new Paragraph("Upload Passport")));
            }
            var grayBack = new DeviceRgb(r: 184, g: 184, b: 184);
            var perd = new Table(new float[] { 70, 180 })
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetVerticalBorderSpacing(2);

            perd.AddCell(new Cell().Add(new Paragraph("MatricNo ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.MatricNumber??"").SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Name ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Name).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Level ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Level.ToString()).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Session ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Session).SetFontSize(9)));
            var perProg = new Table(new float[] { 80, 180 })
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetVerticalBorderSpacing(2);
            
            
            perProg.AddCell(new Cell().Add(new Paragraph("Semester ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Semester).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Department ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Department).SetFontSize(9)));

            perProg.AddCell(new Cell().Add(new Paragraph("Programme Type ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.ProgrammeType).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Programme ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Programme).SetFontSize(9)));
            
            bioTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(perd));
            bioTable.AddCell(new Cell().Add(perProg).SetBorder(Border.NO_BORDER));

            document.Add(bioTable);
            ls.SetMarginTop(5);
            document.Add(ls);
            
            //Adding Academic Records
             
            // Add Result Table
            var courseTable = new Table(5).SetHorizontalAlignment(HorizontalAlignment.CENTER).SetMarginTop(5);
            
            courseTable.AddCell(new Cell().Add(new Paragraph("S/N").SetBold().SetFontSize(8)).SetBackgroundColor(grayBack));
            int count = 0;
            
            courseTable.AddCell(new Cell().Add(new Paragraph("Course Code ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Title ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Unit ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Type ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            
            foreach (var r in reg.Results)
            {
                courseTable.AddCell(new Cell().Add(new Paragraph((count + 1).ToString()).SetFontSize(8)).SetTextAlignment(TextAlignment.CENTER));
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CourseCode))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CourseTitle))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CreditHour.ToString()))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.Type))).SetFontSize(9);
                count++;
           }
           courseTable.AddCell(new Cell(1,3).Add(new Paragraph("Total").SetBold().SetFontSize(9)));
            courseTable.AddCell(new Cell().Add(new Paragraph(reg.TotalCreditUnit.ToString()).SetBold().SetFontSize(9)));
            document.Add(courseTable);


            var sing = new Table(3).SetMarginTop(50).SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetHorizontalBorderSpacing(50);
            string fac = reg.Faculty;
            if (reg.Faculty.Contains("Faculty")) { fac = "Faculty"; }
            else if (reg.Faculty.Contains("College")) { fac = "College"; }
            else { fac = "School"; }
            
            
            if (!string.IsNullOrEmpty(uData.AffiliateInfo)&&reg.ProgrammeType=="Degree")
            {
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Dean").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Exams Officer").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Director of Programme").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
            }
            else
            {
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Exams Officer").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Head of Department").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Dean of "+fac).SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
            }
            document.Add(sing);
            
            document.Close();
            return stream.ToArray();

        }

        public static byte[] StudentSemesterResult(StudentAcademicProfileDTO reg, UserData uData)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);

            // pdf.SetDefaultPageSize(PageSize.A4);
            //var fonts=PdfFontFactory.CreateFont(StandardFonts.)
            document.SetTopMargin(7);
            document.SetLeftMargin(2);
            document.SetRightMargin(1);
            document.SetBottomMargin(1);
            //var uData=HttpContext.Current.Cache.Get("userData") as UserData;
            //Book mark image
            Image bk = new Image(ImageDataFactory.Create(uData.Logo))
               .ScaleToFit(uData.WataWidth, uData.WataHeight)
                .SetFixedPosition(uData.VWata, uData.HWata);

            PdfBackgroundEventHandlers bgd = new PdfBackgroundEventHandlers(bk, uData.WataOpacity);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, bgd);

            //Add header
            var log = new Image(ImageDataFactory.Create(uData.Logo)).SetWidth(100).SetHeight(100).SetHorizontalAlignment(HorizontalAlignment.CENTER);

            //Lines
            var linecolor = new DeviceRgb(r: 000, g: 000, b: 000);
            var hdrline = new SolidLine(1f);
            hdrline.SetColor(linecolor);
            LineSeparator ls = new LineSeparator(hdrline);

            //Data
            var logotab = new Table(new float[] { 900 });
            logotab.AddCell(new Cell().Add(log).SetBorder(Border.NO_BORDER));
            logotab.AddCell(new Cell().Add(new Paragraph(uData.InstitutionName).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            logotab.AddCell(new Cell().Add(ls));
            logotab.AddCell(new Cell().Add(new Paragraph("Semester Result").SetTextAlignment(TextAlignment.LEFT).SetUnderline())
                .SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER));
            document.Add(logotab);

            //Add student Biodata
            var bioTable = new Table(new float[] { 80f, 260f, 280f });
            bioTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            var progDTable = new Table(new float[] { 100f, 180f });
            if (reg.Photo != null)
            {
                Image foto = new Image(ImageDataFactory.Create(reg.Photo));
                foto.ScaleToFit(100, 100);
                bioTable.AddCell(new Cell().Add(foto));
            }
            else
            {
                bioTable.AddCell(new Cell().Add(new Paragraph("Upload Passport")));
            }
            var grayBack = new DeviceRgb(r: 184, g: 184, b: 184);
            var perd = new Table(new float[] { 70, 180 })
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetVerticalBorderSpacing(2);

            perd.AddCell(new Cell().Add(new Paragraph("MatricNo ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.MatricNumber ?? "").SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Name ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Name).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Level ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Level.ToString()).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Session ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perd.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Session).SetFontSize(9)));
            var perProg = new Table(new float[] { 80, 180 })
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetVerticalBorderSpacing(2);


            perProg.AddCell(new Cell().Add(new Paragraph("Semester ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Semester).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Department ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Department).SetFontSize(9)));

            perProg.AddCell(new Cell().Add(new Paragraph("Programme Type ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.ProgrammeType).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Programme ").SetFontSize(9)).SetBackgroundColor(grayBack).SetBorder(Border.NO_BORDER));
            perProg.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(reg.Programme).SetFontSize(9)));

            bioTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(perd));
            bioTable.AddCell(new Cell().Add(perProg).SetBorder(Border.NO_BORDER));

            document.Add(bioTable);
            ls.SetMarginTop(5);
            document.Add(ls);

            //Adding Academic Records

            // Add Result Table
            var courseTable = new Table(9).SetHorizontalAlignment(HorizontalAlignment.CENTER).SetMarginTop(5);

            courseTable.AddCell(new Cell().Add(new Paragraph("S/N").SetBold().SetFontSize(8)).SetBackgroundColor(grayBack));
            int count = 0;

            courseTable.AddCell(new Cell().Add(new Paragraph("Course Code ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Title ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Unit ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("CA1 ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("CA2 ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Exam ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Total ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            courseTable.AddCell(new Cell().Add(new Paragraph("Grade ").SetBold().SetFontSize(9)).SetBackgroundColor(grayBack));
            foreach (var r in reg.Results)
            {
                
                courseTable.AddCell(new Cell().Add(new Paragraph((count + 1).ToString()).SetFontSize(8)).SetTextAlignment(TextAlignment.CENTER));
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CourseCode).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CourseTitle))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CreditHour.ToString()).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CA1.ToString()).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.CA2.ToString()).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.Exam.ToString()).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.Score.ToString()).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                courseTable.AddCell(new Cell().Add(new Paragraph(r.Grade==null?string.Empty:r.Grade).SetTextAlignment(TextAlignment.CENTER))).SetFontSize(9);
                count++;
            }
           courseTable.AddCell(new Cell(1, 2).Add(new Paragraph("GPA= "+reg.GPA).SetBold().SetFontSize(9)));
           courseTable.AddCell(new Cell(1, 2).Add(new Paragraph("CGPA= " + reg.CGPA).SetBold().SetFontSize(9)));
             
            document.Add(courseTable);


            var sing = new Table(2).SetMarginTop(50).SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetHorizontalBorderSpacing(50);
            string fac = reg.Faculty;
            if (reg.Faculty.Contains("Faculty")) { fac = "Faculty"; }
            else if (reg.Faculty.Contains("College")) { fac = "College"; }
            else { fac = "School"; }


            if (!string.IsNullOrEmpty(uData.AffiliateInfo) && reg.ProgrammeType == "Degree")
            {
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                 
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Exams Officer").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Director of Programme").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
            }
            else
            {
                
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("--------------------------------------").SetFontSize(9)));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Exams Officer").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                sing.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Head of Department").SetFontSize(9)).SetTextAlignment(TextAlignment.CENTER));
                 
            }
            document.Add(sing);

            document.Close();
            return stream.ToArray();

        }
    }
}