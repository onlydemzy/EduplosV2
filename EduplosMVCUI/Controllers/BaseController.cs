using KS.Web.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EduplosMVCUI.Controllers
{
    
        // GET: Base
        public class BaseController : Controller
        {
            protected virtual new CustomPrincipal User
            {
                get { return HttpContext.User as CustomPrincipal; }
            }
        /*
            protected void Export2Excel<T>(List<T> list, string workSheet)
            {
                ExcelPackage package = new ExcelPackage();
                var wrkSheet = package.Workbook.Worksheets.Add(workSheet);
                var data = ConvertToDataTable(list);
                int tCol = data.Columns.Count - 1;
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
                wrkSheet.Cells[1, 1, 1, tCol + 1].Style.Font.Bold = true;

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    package.SaveAs(memoryStream);

                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }


            }
            */
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

      
    }
}