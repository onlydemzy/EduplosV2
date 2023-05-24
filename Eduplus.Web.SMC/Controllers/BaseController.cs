using KS.Web.Security;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System;
using System.ComponentModel;
using OfficeOpenXml;
using System.IO;
using Eduplus.Domain.CoreModule;
using Eduplus.Services.Contracts;
using Eduplus.Services.Implementations;
using Eduplus.DTO.AcademicModule;
using System.Drawing;
using OfficeOpenXml.Style;

namespace Eduplus.Web.SMC.Controllers
{
    public class BaseController : Controller
    {
        // = HttpContext.Cache.Get("uData") as UserData;
         
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }       
         

        protected void Export2Excel<T>(List<T> list, string workSheet)
        {
            ExcelPackage package = new ExcelPackage();
            var wrkSheet = package.Workbook.Worksheets.Add(workSheet);
            var data = ConvertToDataTable(list);
            int tCol = data.Columns.Count-1;
            int tRow = data.Rows.Count;

            for (int i = 0; i <= tCol; i++)
            {

                wrkSheet.Cells[1, i + 1].Value = data.Columns[i].ColumnName.ToString();
            }
            
            for (int r = 0; r < tRow; r++)
            {
                for (int c = 0; c <= tCol; c++)
                {
                    
                        wrkSheet.Cells[r + 2, c + 1].Value = data.Rows[r][c].ToString();
                  
                }

            }
            wrkSheet.View.FreezePanes(1, 1);
            wrkSheet.Cells[1,1,1,tCol+1].Style.Font.Bold = true;

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                package.SaveAs(memoryStream);

                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
       
          
    }
       protected DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;

        }


        public void GenerateBroadsheetExcel(BroadSheetDTO dto, UserData udata, string title)
        {
            var stream = new MemoryStream();
            var excelPackage = new ExcelPackage();
            var workSheet = excelPackage.Workbook.Worksheets.Add("Broadsheet");
            Image logo = null;
            if (udata.Logo != null)
            {
                using (var logoStream = new MemoryStream(udata.Logo))
                {
                    logo = Image.FromStream(logoStream);
                }
            }
            var excelImage = workSheet.Drawings.AddPicture("logo", logo);
            excelImage.SetSize(100, 100);
            excelImage.SetPosition(1, 0, 7, 0);
            
            //Add Header and Title
            

            workSheet.Cells[7, 8].Value = udata.InstitutionName;
            workSheet.Cells[7, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[7, 8].Style.Font.Bold = true;
            workSheet.Cells[7, 8].Style.Font.Size =13;

            workSheet.Cells[8, 1].Value = title;
            workSheet.Cells[8, 1].Style.Font.Bold = true;
            workSheet.Cells[8, 1].Style.Font.Size = 11;
            workSheet.Cells[8, 1].Style.Font.UnderLine = true;
            //Add Descriptive texts
            string fac;

            if (dto.Faculty.Contains("School")) { fac = "School"; }
            else if (dto.Faculty.Contains("College")) { fac = "College"; }
            else { fac = "Faculty"; }
            workSheet.Cells[9, 1].Value = fac + ": " + dto.Faculty;
            workSheet.Cells[10, 1].Value = "Department: " + dto.Department;
            workSheet.Cells[11, 1].Value = "Programme Type: " + dto.ProgrammeType;
            workSheet.Cells[12, 1].Value = "Programme: " + dto.Programme;

            workSheet.Cells[9, 13].Value = "Session: " + dto.Session;
            workSheet.Cells[10, 13].Value = "Semester: " + dto.Semester;
            workSheet.Cells[11, 13].Value = "Level: " + dto.Level;
            workSheet.Cells[12, 13].Value = "Programme: " + dto.Programme;
            
            // Add Result Table
            
            workSheet.Cells[13, 1].Value = "S/N";
            workSheet.Column(1).Width = 4;
            workSheet.Cells[13, 1].Style.Font.Size = 7;
            workSheet.Cells[13, 1].Style.Font.Name = "Arial";
            workSheet.Cells[13, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[13, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[13, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[13, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[13, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            int tColHeadings = dto.Results.Columns.Count;
            int colIndex = 2;
            foreach (DataColumn c in dto.Results.Columns)
            {
                var heading= c.ColumnName.ToString();
                if (heading == "OutStandings" || heading == "Repeat")
                {
                    workSheet.Column(colIndex).Width = 20;
                }
                else if (heading == "CH") { workSheet.Column(colIndex).Width = 4; }
                else { workSheet.Column(colIndex).Width = 6; }
                workSheet.Cells[13, colIndex].Value = heading.ToUpper();
                workSheet.Cells[13, colIndex].Style.WrapText = true;
                workSheet.Cells[13, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[13, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[13, colIndex].Style.Font.Size = 7;
                workSheet.Cells[13, colIndex].Style.Font.Name = "Arial";
                //Apply borders
                workSheet.Cells[13, colIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[13, colIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[13, colIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[13, colIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                colIndex++;

            }
            int count = 0;
            int rowIndex = 14;
            
            bool color = true;
            foreach (DataRow r in dto.Results.Rows)
            {
                workSheet.Cells[rowIndex,1 ].Value = count+1;
                workSheet.Cells[rowIndex, 1].Style.Font.Size = 7;
                workSheet.Cells[rowIndex, 1].Style.Font.Name = "Arial";
                workSheet.Cells[rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                int colIndex2 = 1;
                if (color == true)
                {
                    workSheet.Row(rowIndex).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Row(rowIndex).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F0F0F0"));
                    color = false;
                }
                else { color = true; }
                foreach (DataColumn c in dto.Results.Columns)
                {

                    workSheet.Cells[rowIndex, colIndex2 + 1].Value = r[c.ColumnName].ToString();
                    workSheet.Cells[rowIndex, colIndex2 + 1].Style.Font.Size = 7;
                    workSheet.Cells[rowIndex, colIndex2 + 1].Style.Font.Name = "Arial";
                    workSheet.Cells[rowIndex, colIndex2 + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, colIndex2 + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, colIndex2 + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, colIndex2 + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if ((colIndex2 + 1) == 2) {
                        workSheet.Column(colIndex2 + 1).AutoFit();
                    }
                    else { workSheet.Cells[rowIndex, colIndex2 + 1].Style.WrapText = true; }
                    
                    colIndex2++;
                     
                }
                count++;
                rowIndex++;
            }
            rowIndex = rowIndex + 3;
            workSheet.Cells[rowIndex, 4].Value = "___________________________";
            workSheet.Cells[rowIndex,4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[rowIndex, 4].Style.Font.Size = 7;
            workSheet.Cells[rowIndex, 4].Style.Font.Name = "Arial";
            workSheet.Cells[rowIndex, 10].Value = "___________________________";
            workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[rowIndex, 10].Style.Font.Size = 7;
            workSheet.Cells[rowIndex, 10].Style.Font.Name = "Arial";

            workSheet.Cells[rowIndex, 16].Value = "___________________________";
            workSheet.Cells[rowIndex, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[rowIndex, 16].Style.Font.Size = 7;
            workSheet.Cells[rowIndex, 16].Style.Font.Name = "Arial";

            workSheet.Cells[rowIndex+1, 4].Value = "Exams Officer";
            workSheet.Cells[rowIndex+1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[rowIndex+1, 4].Style.Font.Size = 7;
            workSheet.Cells[rowIndex+1, 4].Style.Font.Name = "Arial";
            workSheet.Cells[rowIndex+1, 10].Value = "HOD";
            workSheet.Cells[rowIndex+1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[rowIndex+1, 10].Style.Font.Size = 7;
            workSheet.Cells[rowIndex+1, 10].Style.Font.Name = "Arial";
            workSheet.Cells[rowIndex+1, 16].Value = "Dean of "+fac;
            workSheet.Cells[rowIndex+1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[rowIndex+1, 16].Style.Font.Size = 7;
            workSheet.Cells[rowIndex+1, 16].Style.Font.Name = "Arial";


            workSheet.PrinterSettings.PaperSize = ePaperSize.Legal;
            //workSheet.PrinterSettings.RepeatRows = new ExcelAddress(String.Format("'{14'}!${0}:${0}",firstRowNumberExcel, "Broadsheet"));
            //ExcelAddress(String.Format("'{1}'!${0}:${0}", firstRowNummerExcel, myWorkSheetName
            workSheet.Column(27).PageBreak = true;
            workSheet.PrinterSettings.TopMargin = 0.05M;
            workSheet.PrinterSettings.LeftMargin = 0.05M;
            workSheet.PrinterSettings.RightMargin = 0.05M;
            workSheet.PrinterSettings.BottomMargin = 0.05M;
            workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
            workSheet.PrinterSettings.FitToHeight = 1;          
            workSheet.View.FreezePanes(14, 3);
            workSheet.Protection.IsProtected = true;
            workSheet.Protection.AllowSelectLockedCells = false;
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                excelPackage.SaveAs(memoryStream);

                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
             
        }

    }
}