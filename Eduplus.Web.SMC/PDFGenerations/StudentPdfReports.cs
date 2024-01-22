using Eduplos.Domain.CoreModule;
using Eduplos.DTO.CoreModule;
using Eduplos.Web.SMC.PDFGenerations.EventHandlers;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Data;
using System.IO;
using System.Linq;

namespace Eduplos.Web.SMC.PDFGenerations
{
    public class StudentPDFReports
    {
        public static byte[] CreateStudentProfilePdf(StudentDTO stu, UserData data)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
             
            // pdf.SetDefaultPageSize(PageSize.A4);
            //var fonts=PdfFontFactory.CreateFont(StandardFonts.)
            document.SetTopMargin(1);
            document.SetLeftMargin(2);
            document.SetRightMargin(1);
            document.SetBottomMargin(1);

    
            Image logo = new Image(ImageDataFactory.Create(data.Regbanner))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetWidth(570).SetHeight(100);

            //Book mark image
            Image bk = new Image(ImageDataFactory.Create(data.Logo))
               .ScaleToFit(data.WataWidth, data.WataHeight)
                .SetFixedPosition(data.VWata, data.HWata);

            PdfBackgroundEventHandlers bgd = new PdfBackgroundEventHandlers(bk, data.WataOpacity);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, bgd);

            //Set backgroundcolors
            var grayBack = new DeviceRgb(r: 184, g: 184, b: 184);
            var linecolor = new DeviceRgb(r: 000, g: 000, b: 000);
            var textfield = new DeviceRgb(230, 255, 179);//rgb(0,15,94)rgb(250,42,0)
            var whitefont = new DeviceRgb(r: 0, g: 0, b: 0);
            Border whiteborder = new SolidBorder(whitefont, 1,35);
            //Table Segment
            //Header table

            var table = new Table(1);

            table.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER));
            

            document.Add(table);
            var hdrline = new SolidLine(1f);
            hdrline.SetColor(linecolor);
            LineSeparator ls = new LineSeparator(hdrline);
            //document.Add(new LineSeparator(hdrline));
            document.Add(new Paragraph("Student Data Form")
                .SetFontSize(12)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            //Name summarytable

            var bioTable = new Table(new float[] {80f, 260f, 280f });
            bioTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            var progDTable = new Table(new float[] { 100f, 180f });
            if(stu.Foto!=null)
            {
                Image foto = new Image(ImageDataFactory.Create(stu.Foto));
                foto.ScaleToFit(150, 150);     
                bioTable.AddCell(new Cell().Add(foto));
            }
            else
            {
                bioTable.AddCell(new Cell().Add(new Paragraph("Upload your passort")));
            }
            
            var perd = new Table(new float[] { 70,180 });
            perd.AddCell(new Cell().Add(new Paragraph("Surname ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Surname).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Firstname ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Firstname).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Middlename ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Middlename).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Sex ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Sex).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Marital Status ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.MaritalStatus).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Date of Birth ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.BirthDate??"").SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Entry Mode ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.EntryMode ?? "").SetFontSize(9)));

            var perProg = new Table(new float[] { 80, 200 });
            perProg.AddCell(new Cell().Add(new Paragraph("Student ID ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.StudentId).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Matric No ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.MatricNumber??"").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Department ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.Department ?? "").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Programme ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.ProgrammeType??"").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Course ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.Programme).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Year admitted ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.YearAdmitted).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Duration ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.Duration.ToString()+" years").SetFontSize(9)));

            bioTable.AddCell(new Cell().Add(perd).SetBorder(Border.NO_BORDER));
            bioTable.AddCell(new Cell().Add(perProg).SetBorder(Border.NO_BORDER));

            
            document.Add(bioTable);
            
            var pd = new Table(new float[] { 115, 200 });
            pd.AddCell(new Cell(1, 2)
             .Add(new Paragraph("Personal Data").SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));

            pd.AddCell(new Cell().Add(new Paragraph("Phone ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Phone??"").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Email ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Email??"").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Residential Address ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.ResidentialAddress??"").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Permanent Home Addr ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.PermanentHomeAdd??"").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Home Town ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.HomeTown ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("LGA ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Lg ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("State ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.State ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Country ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Country ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Name of Spouse ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.SpouseName ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Spouse Address ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.SpouseAddress ?? "").SetFontSize(9)));
             
            var sponsor = new Table(new float[] { 115, 200 });
            sponsor.AddCell(new Cell(1, 2)
             .Add(new Paragraph("Sponsor / Next of Kin Info").SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));
            sponsor.AddCell(new Cell().Add(new Paragraph("Name of Sponsor ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.Referee ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Address of sponsor ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.RefereeAddress ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Email of sponsor").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.RefereeMail ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Phone of sponsor ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.RefereePhone ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Next of Kin ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.NextKin ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Kin Address ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.KinAddress ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Kin Phone ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.Phone ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Kin Email ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.KinMail ?? "").SetFontSize(9)));
            
            var combot= new Table(new float[] { 315, 315 });
            combot.AddCell(new Cell().Add(pd).SetBorder(Border.NO_BORDER));
            combot.AddCell(new Cell().Add(sponsor).SetBorder(Border.NO_BORDER));
            document.Add(combot);
             
            var olvl = new Table(new float[] { 500, 500 }).SetKeepTogether(true);
            olvl.AddCell(new Cell(1, 2)
             .Add(new Paragraph("O/Level Result").SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));
            var olvl1r = new Table(new float[] { 200, 20 });
            var olvl2r = new Table(new float[] { 200, 20 });
            if (stu.Olevels.Count>0)
            {
                 
                var les = new Table(1);
                var les1 = new Table(1);

                var lv1 = stu.Olevels.Where(a => a.SitAttempt == 1).FirstOrDefault();
                var olv1h = new Table(new float[] { 80, 300 });
                var lv2 = stu.Olevels.Where(a => a.SitAttempt == 2).FirstOrDefault();
                var olv2h = new Table(new float[] { 80, 300 });
                
                
                if (lv1!=null)
                {
                    olv1h.AddCell(new Cell().Add(new Paragraph("Exam Type ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.ExamType ?? "").SetFontSize(9)));
                    olv1h.AddCell(new Cell().Add(new Paragraph("Year ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.ExamYear.ToString() ?? "").SetFontSize(9)));
                    olv1h.AddCell(new Cell().Add(new Paragraph("Center ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.Venue ?? "").SetFontSize(9)));
                    olv1h.AddCell(new Cell().Add(new Paragraph("Exam No ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.ExamNumber ?? "").SetFontSize(9)));

                    les.AddCell(new Cell().Add(olv1h));
                    olvl1r.AddCell(new Cell().Add(new Paragraph("Subject").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olvl1r.AddCell(new Cell().Add(new Paragraph("Grade").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    foreach (var r1 in lv1.Details)
                    {
                        olvl1r.AddCell(new Cell().Add(new Paragraph(r1.Subject).SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                        olvl1r.AddCell(new Cell().Add(new Paragraph(r1.Grade).SetFontSize(9)));
                    }
                    les.AddCell(new Cell().Add(olvl1r));
                }

                if (lv2 != null)
                {
                    olv2h.AddCell(new Cell().Add(new Paragraph("Exam Type ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.ExamType ?? "").SetFontSize(9)));
                    olv2h.AddCell(new Cell().Add(new Paragraph("Year ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.ExamYear.ToString() ?? "").SetFontSize(9)));
                    olv2h.AddCell(new Cell().Add(new Paragraph("Center ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.Venue ?? "").SetFontSize(9)));
                    olv2h.AddCell(new Cell().Add(new Paragraph("Exam No ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.ExamNumber ?? "").SetFontSize(9)));

                    les1.AddCell(new Cell().Add(olv2h));
                    olvl2r.AddCell(new Cell().Add(new Paragraph("Subject").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olvl2r.AddCell(new Cell().Add(new Paragraph("Grade").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    foreach (var r2 in lv2.Details)
                    {
                        olvl2r.AddCell(new Cell().Add(new Paragraph(r2.Subject).SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                        olvl2r.AddCell(new Cell().Add(new Paragraph(r2.Grade).SetFontSize(9)));
                    }
                    les1.AddCell(new Cell().Add(olvl2r));
                }

                olvl.AddCell(new Cell().Add(les).SetBorder(Border.NO_BORDER));
                olvl.AddCell(new Cell().Add(les1).SetBorder(Border.NO_BORDER));
                
            }
 
            document.Add(olvl);
            //Jamb Record
            
            if (stu.Jamb!=null)
            {


                var jambTab = new Table(1).SetKeepTogether(true);
                jambTab.AddCell(new Cell().Add(new Paragraph("Jamb Scores")));
                var jscores = new Table(new float[] { 100, 30 });

                var jh = new Table(new float[] { 100, 30 });
                jh.AddCell(new Cell().Add(new Paragraph("Year: ").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                jh.AddCell(new Cell().Add(new Paragraph(stu.Jamb.JambYear.ToString() ?? "").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                jh.AddCell(new Cell().Add(new Paragraph("Registration No: ").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                jh.AddCell(new Cell().Add(new Paragraph(stu.Jamb.JambRegNumber ?? "").SetFontSize(9)).SetBorder(Border.NO_BORDER));


                jambTab.AddCell(new Cell().Add(jh));
                jscores.AddCell(new Cell().Add(new Paragraph("Subject").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                jscores.AddCell(new Cell().Add(new Paragraph("Score").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                foreach (var r2 in stu.Jamb.Scores)
                {
                    jscores.AddCell(new Cell().Add(new Paragraph(r2.Subject).SetFontSize(9)).SetBorderTop(whiteborder));
                    jscores.AddCell(new Cell().Add(new Paragraph(r2.Score.ToString()).SetFontSize(9)));
                }
                jscores.AddCell(new Cell().Add(new Paragraph("Total Score").SetFontSize(9)).SetBorderTop(whiteborder));
                jscores.AddCell(new Cell().Add(new Paragraph(stu.Jamb.Total.ToString()).SetFontSize(9)).SetBorderTop(whiteborder));
                jambTab.AddCell(new Cell().Add(jscores));
                document.Add(jambTab);
            }

            if (stu.Alevels.Count()>0)
            {
                document.Add(new Paragraph("A/Level Certificate(s)"));
                
                var acerts = new Table(new float[] { 70, 70,150,100 });
                  
                acerts.AddCell(new Cell().Add(new Paragraph("Start Month").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                acerts.AddCell(new Cell().Add(new Paragraph("End Month").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                acerts.AddCell(new Cell().Add(new Paragraph("Institution").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                acerts.AddCell(new Cell().Add(new Paragraph("Certificate Obtained").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                foreach (var r2 in stu.Alevels)
                {
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.StartMonth).SetFontSize(9)));
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.EndMonth).SetFontSize(9)));
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.Institution).SetFontSize(9)));
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.Qualification).SetFontSize(9)));
                }
                
                document.Add(acerts);
            }

            document.Add(new Paragraph("Declaration").SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(14)
                .SetUnderline()
                .SetBold());
            document.Add(new Paragraph("I, "+stu.FullName+ "  hereby declare that the information supplied above "+
                "is true to the best of my knowledge and if any of the information given  is discovered to be false, "+
                "will lead to summary dismisal from the College. I also agree to be bound by the College's regulations "+
                "and to be of good behaviour at all times throughout my stay in the College." ).SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetFontSize(9));

            document.Add(new Paragraph("Date:____/___/____               Sign:_____________________").SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(9));
                 
            document.Add(new LineSeparator(hdrline));
            document.Add(new Paragraph("FOR OFFICE USE ONLY").SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(9)
                .SetBold());
            document.Add(new Paragraph("DATE OF ADDMISSION: ________________________").SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(9));

            document.Add(new Paragraph("CHECKED BY: ____________________________________________      DATE:___/__/___").SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(9));
                
            //test link
            
            //document.Add(new Paragraph(link));
            document.Close();

            return stream.ToArray();
        }

        public static byte[] CreateStudentApplicationSlipPdf(StudentDTO stu, UserData data)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);

            // pdf.SetDefaultPageSize(PageSize.A4);
            //var fonts=PdfFontFactory.CreateFont(StandardFonts.)
            document.SetTopMargin(1);
            document.SetLeftMargin(2);
            document.SetRightMargin(1);
            document.SetBottomMargin(1);


            Image logo = new Image(ImageDataFactory.Create(data.Regbanner))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetWidth(570).SetHeight(100);

            //Book mark image
            Image bk = new Image(ImageDataFactory.Create(data.Logo))
               .ScaleToFit(data.WataWidth, data.WataHeight)
                .SetFixedPosition(data.VWata, data.HWata);

            PdfBackgroundEventHandlers bgd = new PdfBackgroundEventHandlers(bk, data.WataOpacity);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, bgd);

            //Set backgroundcolors
            var grayBack = new DeviceRgb(r: 184, g: 184, b: 184);
            var linecolor = new DeviceRgb(r: 000, g: 000, b: 000);
            var textfield = new DeviceRgb(230, 255, 179);//rgb(0,15,94)rgb(250,42,0)
            var whitefont = new DeviceRgb(r: 0, g: 0, b: 0);
            Border whiteborder = new SolidBorder(whitefont, 1, 35);
            //Table Segment
            //Header table

            var table = new Table(1);

            table.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER));


            document.Add(table);
            var hdrline = new SolidLine(1f);
            hdrline.SetColor(linecolor);
            LineSeparator ls = new LineSeparator(hdrline);
            //document.Add(new LineSeparator(hdrline));
            document.Add(new Paragraph(stu.YearAdmitted + " Admission Slip")
                .SetFontSize(12)
                .SetBold()
                .SetTextAlignment(TextAlignment.LEFT));
            //Name summarytable

            var bioTable = new Table(new float[] { 80f, 260f, 280f });
            bioTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            var progDTable = new Table(new float[] { 100f, 180f });
            if (stu.Foto != null)
            {
                Image foto = new Image(ImageDataFactory.Create(stu.Foto));
                foto.ScaleToFit(150, 150);
                bioTable.AddCell(new Cell().Add(foto));
            }
            else
            {
                bioTable.AddCell(new Cell().Add(new Paragraph("Upload your passort")));
            }

            var perd = new Table(new float[] { 70, 180 });
            perd.AddCell(new Cell().Add(new Paragraph("Surname ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Surname).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Firstname ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Firstname).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Middlename ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Middlename).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Sex ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.Sex).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Marital Status ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.MaritalStatus).SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Date of Birth ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.BirthDate ?? "").SetFontSize(9)));
            perd.AddCell(new Cell().Add(new Paragraph("Handicapped? ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perd.AddCell(new Cell().Add(new Paragraph(stu.IsHandicapped ?? "").SetFontSize(9)));
            var perProg = new Table(new float[] { 80, 200 });
            perProg.AddCell(new Cell().Add(new Paragraph("Student ID ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.StudentId).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Department ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.Department ?? "").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Programme ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.ProgrammeType ?? "").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Course ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.Programme).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Session ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.YearAdmitted).SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Duration ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.Duration.ToString() + " years").SetFontSize(9)));
            perProg.AddCell(new Cell().Add(new Paragraph("Entry Mode ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            perProg.AddCell(new Cell().Add(new Paragraph(stu.EntryMode ?? "").SetFontSize(9)));
            bioTable.AddCell(new Cell().Add(perd).SetBorder(Border.NO_BORDER));
            bioTable.AddCell(new Cell().Add(perProg).SetBorder(Border.NO_BORDER));


            document.Add(bioTable);

            var pd = new Table(new float[] { 115, 200 });
            pd.AddCell(new Cell(1, 2)
             .Add(new Paragraph("Personal Data").SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));

            pd.AddCell(new Cell().Add(new Paragraph("Phone ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Phone ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Email ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Email ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Residential Address ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.ResidentialAddress ?? "").SetFontSize(9)));
            
            pd.AddCell(new Cell().Add(new Paragraph("LGA ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Lg ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("State ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.State ?? "").SetFontSize(9)));
            pd.AddCell(new Cell().Add(new Paragraph("Country ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            pd.AddCell(new Cell().Add(new Paragraph(stu.Country ?? "").SetFontSize(9)));


            var sponsor = new Table(new float[] { 115, 200 });
            sponsor.AddCell(new Cell(1, 2)
             .Add(new Paragraph("Sponsor / Next of Kin Info").SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));
            sponsor.AddCell(new Cell().Add(new Paragraph("Name of Sponsor ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.Referee ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Address of sponsor ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.RefereeAddress ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Email of sponsor").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.RefereeMail ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Phone of sponsor ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.RefereePhone ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Next of Kin ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.NextKin ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Kin Address ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.KinAddress ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Kin Phone ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.Phone ?? "").SetFontSize(9)));
            sponsor.AddCell(new Cell().Add(new Paragraph("Kin Email ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
            sponsor.AddCell(new Cell().Add(new Paragraph(stu.KinMail ?? "").SetFontSize(9)));

            var combot = new Table(new float[] { 315, 315 });
            combot.AddCell(new Cell().Add(pd).SetBorder(Border.NO_BORDER));
            combot.AddCell(new Cell().Add(sponsor).SetBorder(Border.NO_BORDER));
            document.Add(combot);

            var olvl = new Table(new float[] { 500, 500 }).SetKeepTogether(true);
            olvl.AddCell(new Cell(1, 2)
             .Add(new Paragraph("O/Level Result").SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));
            var olvl1r = new Table(new float[] { 200, 20 });
            var olvl2r = new Table(new float[] { 200, 20 });
            if (stu.Olevels.Count > 0)
            {

                var les = new Table(1);
                var les1 = new Table(1);

                var lv1 = stu.Olevels.Where(a => a.SitAttempt == 1).FirstOrDefault();
                var olv1h = new Table(new float[] { 80, 300 });
                var lv2 = stu.Olevels.Where(a => a.SitAttempt == 2).FirstOrDefault();
                var olv2h = new Table(new float[] { 80, 300 });


                if (lv1 != null)
                {
                    olv1h.AddCell(new Cell().Add(new Paragraph("Exam Type ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.ExamType ?? "").SetFontSize(9)));
                    olv1h.AddCell(new Cell().Add(new Paragraph("Year ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.ExamYear.ToString() ?? "").SetFontSize(9)));
                    olv1h.AddCell(new Cell().Add(new Paragraph("Center ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.Venue ?? "").SetFontSize(9)));
                    olv1h.AddCell(new Cell().Add(new Paragraph("Exam No ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv1h.AddCell(new Cell().Add(new Paragraph(lv1.ExamNumber ?? "").SetFontSize(9)));

                    les.AddCell(new Cell().Add(olv1h));
                    olvl1r.AddCell(new Cell().Add(new Paragraph("Subject").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olvl1r.AddCell(new Cell().Add(new Paragraph("Grade").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    foreach (var r1 in lv1.Details)
                    {
                        olvl1r.AddCell(new Cell().Add(new Paragraph(r1.Subject).SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                        olvl1r.AddCell(new Cell().Add(new Paragraph(r1.Grade).SetFontSize(9)));
                    }
                    les.AddCell(new Cell().Add(olvl1r));
                }

                if (lv2 != null)
                {
                    olv2h.AddCell(new Cell().Add(new Paragraph("Exam Type ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.ExamType ?? "").SetFontSize(9)));
                    olv2h.AddCell(new Cell().Add(new Paragraph("Year ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.ExamYear.ToString() ?? "").SetFontSize(9)));
                    olv2h.AddCell(new Cell().Add(new Paragraph("Center ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.Venue ?? "").SetFontSize(9)));
                    olv2h.AddCell(new Cell().Add(new Paragraph("Exam No ").SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olv2h.AddCell(new Cell().Add(new Paragraph(lv2.ExamNumber ?? "").SetFontSize(9)));

                    les1.AddCell(new Cell().Add(olv2h));
                    olvl2r.AddCell(new Cell().Add(new Paragraph("Subject").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    olvl2r.AddCell(new Cell().Add(new Paragraph("Grade").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                    foreach (var r2 in lv2.Details)
                    {
                        olvl2r.AddCell(new Cell().Add(new Paragraph(r2.Subject).SetFontSize(9)).SetBackgroundColor(textfield).SetFontColor(whitefont).SetBorderTop(whiteborder));
                        olvl2r.AddCell(new Cell().Add(new Paragraph(r2.Grade).SetFontSize(9)));
                    }
                    les1.AddCell(new Cell().Add(olvl2r));
                }

                olvl.AddCell(new Cell().Add(les).SetBorder(Border.NO_BORDER));
                olvl.AddCell(new Cell().Add(les1).SetBorder(Border.NO_BORDER));

            }

            document.Add(olvl);
            //Jamb Record

            if (stu.Jamb != null)
            {

                var jambTab = new Table(1).SetKeepTogether(true);
                jambTab.AddCell(new Cell().Add(new Paragraph("Jamb Record")));
                var jscores = new Table(new float[] { 100, 30 });

                var jh = new Table(new float[] { 100, 30 });
                jh.AddCell(new Cell().Add(new Paragraph("Year: ").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                jh.AddCell(new Cell().Add(new Paragraph(stu.Jamb.JambYear.ToString() ?? "").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                jh.AddCell(new Cell().Add(new Paragraph("Registration No: ").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                jh.AddCell(new Cell().Add(new Paragraph(stu.Jamb.JambRegNumber ?? "").SetFontSize(9)).SetBorder(Border.NO_BORDER));


                jambTab.AddCell(new Cell().Add(jh));
                jscores.AddCell(new Cell().Add(new Paragraph("Subject").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                jscores.AddCell(new Cell().Add(new Paragraph("Score").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                if(stu.Jamb.Scores!=null)
                {
                    foreach (var r2 in stu.Jamb.Scores)
                    {
                        jscores.AddCell(new Cell().Add(new Paragraph(r2.Subject).SetFontSize(9)).SetBorderTop(whiteborder));
                        jscores.AddCell(new Cell().Add(new Paragraph(r2.Score.ToString()).SetFontSize(9)));
                    }
                    jscores.AddCell(new Cell().Add(new Paragraph("Total Score").SetFontSize(9)).SetBorderTop(whiteborder));
                    jscores.AddCell(new Cell().Add(new Paragraph(stu.Jamb.Total.ToString()).SetFontSize(9)).SetBorderTop(whiteborder));

                    jambTab.AddCell(new Cell().Add(jscores));
                }
                
                document.Add(jambTab);
            }

            if (stu.Alevels.Count > 0)
            {
                var acerts = new Table(new float[] { 70, 70, 150, 100 }).SetKeepTogether(true);
                
                acerts.AddCell(new Cell(1, 4).Add(new Paragraph("A/Level Certificate(s)").SetTextAlignment(TextAlignment.CENTER)));

                acerts.AddCell(new Cell().Add(new Paragraph("Start Month").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                acerts.AddCell(new Cell().Add(new Paragraph("End Month").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                acerts.AddCell(new Cell().Add(new Paragraph("Institution").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                acerts.AddCell(new Cell().Add(new Paragraph("Certificate Obtained").SetFontSize(9)).SetBackgroundColor(grayBack).SetFontColor(whitefont).SetBorderTop(whiteborder));
                foreach (var r2 in stu.Alevels)
                {
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.StartMonth).SetFontSize(9)));
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.EndMonth).SetFontSize(9)));
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.Institution).SetFontSize(9)));
                    acerts.AddCell(new Cell().Add(new Paragraph(r2.Qualification).SetFontSize(9)));
                }

                document.Add(acerts);
            }

            document.Add(new Paragraph("Declaration").SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(14)
                .SetUnderline()
                .SetBold());
            document.Add(new Paragraph("I, " + stu.FullName + "  hereby declare that the information supplied above " +
                "is true to the best of my knowledge and if any of the information given  is discovered to be false, " +
                "should be disqualified from gaining admission.").SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetFontSize(9));

            document.Add(new Paragraph("Date:____/___/____               Sign:_____________________").SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(9));

            document.Add(new LineSeparator(hdrline));
            document.Add(new Paragraph("Please submit  a copy of your completed screening  form at the admission office along with  your  credentials.").SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(9)
                .SetBold());
            

            //test link

            //document.Add(new Paragraph(link));
            document.Close();

            return stream.ToArray();
        }

        public static byte[] GenerateMatricRegister(MatricRegisterDTO dto, UserData udata)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);

            var document = new Document(pdf, PageSize.A4.Rotate());
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
            logotab.AddCell(new Cell().Add(new Paragraph("Matriculation Register").SetTextAlignment(TextAlignment.CENTER).SetUnderline())
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



            mainHeader.AddCell(new Cell().Add(hdr1).SetBorder(Border.NO_BORDER));

            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Programme Type: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.ProgrammeType).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Session: ").SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));
            hdr2.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(dto.Session).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9)));

            mainHeader.AddCell(new Cell().Add(hdr2).SetBorder(Border.NO_BORDER));
            document.Add(mainHeader);


            // Add Record
            foreach(var h in dto.Headings)
            {
                var prog = new Table(new float[] { 8,40,40,150, 30,150, 50,50, 50, 65, 150, 65, 65,65})
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                prog.AddCell(new Cell(1, 14)
                    .Add(new Paragraph(h.Heading).SetBold())
                    .SetTextAlignment(TextAlignment.CENTER));

                prog.AddCell(new Cell().Add(new Paragraph("S/N ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("StudentId ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Matric No ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Student ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Sex").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Birth Date ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Jamb Number").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Jamb Score").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Phone ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Email ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Address ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("LGA ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("State ").SetFontSize(9)));
                prog.AddCell(new Cell().Add(new Paragraph("Signature ").SetFontSize(9)));


                int count = 0;
                foreach(var s in h.Details)
                {
                    count++;
                    prog.AddCell(new Cell().Add(new Paragraph(count.ToString()))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.StudentId ?? ""))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.MatricNo??""))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.Name))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.Sex))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.BirthDate??""))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.JambRegNo))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.JambScore.ToString()))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.Phone))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.Email))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.Address))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.Lg))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(s.State))).SetFontSize(9);
                    prog.AddCell(new Cell().Add(new Paragraph(""))).SetFontSize(9);

                }
                document.Add(prog);
            }
            FooterHandler footerHandler = new FooterHandler();
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerHandler);
            footerHandler.WriteTotal(pdf);
            document.Close();
            return stream.ToArray();
        }



    }
}