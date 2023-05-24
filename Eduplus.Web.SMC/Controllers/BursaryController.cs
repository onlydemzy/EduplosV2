using Eduplus.Domain.BurseryModule;
using Eduplus.DTO.BursaryModule;
using Eduplus.Services.Contracts;
using KS.Domain.AccountsModule;
using KS.Services.Contract;
using KS.UI.ViewModel;
using KS.Web.Security;
using Newtonsoft.Json;
using OfficeOpenXml;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
    [KSWebAuthorisation]
    public class BursaryController : BaseController
    {
        private readonly IBursaryService _bursaryService;
        private readonly IAccountsService _accountsService;
        private readonly IStudentsAccountsService _studentAccounts;
        private readonly IGeneralDutiesService _generalDuties;
        public BursaryController(IAccountsService accountsService,IBursaryService bursaryService,IStudentsAccountsService studentsAccounts,
            IGeneralDutiesService generalDuties)
        {
            _accountsService = accountsService;
            _bursaryService = bursaryService;
            _studentAccounts = studentsAccounts;
            _generalDuties = generalDuties;
        }
        public ActionResult Accounts()
        {
            return View();
        }

        public JsonResult AccountsList()
        {
            var accts = _accountsService.AllAccounts();
            return this.Json(accts, JsonRequestBehavior.AllowGet);
        }

        public Accounts SaveAccount(Accounts account)
        {
            var accounts = _accountsService.SaveAccount(account, User.UserId);
            return accounts;
        }

        public ActionResult AccountGroups()
        {
            return View();
        }

        
        #region FEESCHEDULE OPERATIONS
        public ActionResult FeeSchedules()
        {
            return View();
        }
        public JsonResult AllFeeSchedules()
        {
            var schedules = _bursaryService.FetchFeeSchedules();
            return this.Json(schedules, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddFeeSchedule()
        {
            return View();
        }
        
        public string AddFeeScheduleLine(FeeScheduleDetailsDTO scheduleLine)
        {
            string msg;
            var fee=_bursaryService.SaveFeeScheduleLineItem(scheduleLine,out msg,User.UserId);
            if (msg == "Ok")
                return JsonConvert.SerializeObject(fee);
            else return null;

        }
        public JsonResult ScheduleDetail(int scheduleId)
        {
            var fee = _bursaryService.FetchFeeScheduleById(scheduleId);

            return Json(fee,JsonRequestBehavior.AllowGet);
        }
        public string ApplySessionFeeSchedule(int scheduleId)
        {
            string msg = _bursaryService.ApplyFeeSchedule(scheduleId, User.UserId);
            return msg;
        }
        public string DeleteFeeScheduleLineItem(FeeScheduleDetailsDTO scheduleLine)
        {
            return _bursaryService.DeletefeeScheduleLineItem(scheduleLine, User.UserId);
        }

        #endregion

        #region STUDENT'S FEE OPERATIONS-BURSARY

        public JsonResult FetchStudentInvoice(string invoiceNo)
        {
            return Json(_studentAccounts.GetStudentPaymentInvoice(invoiceNo), JsonRequestBehavior.AllowGet);
        }
       

        [HttpGet]
        public ActionResult StudentAccount()
        {
            return View();
        }

        public JsonResult FetchStudentAccount(string matricNumber)
        {
            
            var student =_bursaryService.StudentAccountSummary(matricNumber.ToUpper());
           
            return Json(student, JsonRequestBehavior.AllowGet);
        }

        public ViewResult DebitStudentAccount(string studentId)
        {
            return View();
        }
        public string SaveStudentDebit(TransactionDTO viewModel)
        {
            
            return _bursaryService.DebitStudentAccount(viewModel, User.UserId);
        }

        public ActionResult CreditStudent(string studentId )
        {
            return View();
        }
        public string SaveStudentCredit(TransactionDTO viewModel)
        {
            outPutMsg result = new outPutMsg();

            string msg="";
            var tc=_studentAccounts.GenerateStudentInvoiceToCreditStudentAccount(viewModel, User.UserId,out msg);
            result.message = msg;
            if (msg != "Ok")
            {
                result.message = msg;
                result.status = 0;
            }
            else
            {
                result.value = tc;
                result.status = 1;
            }
            var chk = JsonConvert.SerializeObject(result);
            return chk;
        }

        public ActionResult StudentAccountsDetail(string studentId=null)
        {
            string _regNo;
            if (!string.IsNullOrEmpty(studentId))
            {
                _regNo = studentId;
            }
            else
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                _regNo = user.UserId;
            }
            
            var statement = _bursaryService.StudentAccountStatement(_regNo);
            return new ViewAsPdf(statement);
        }

        public ActionResult ViewStudentPayments()
        {
            return View();
        }

        public ActionResult ViewStudentPaymentsAsPDF(DateTime fromDate, DateTime toDate, string accountCode, string deptCode, string progType, string rpt)
        {
            FeesCollectionDTO dto = null;
            switch (rpt)
            {
                case "Other Fees":
                    dto = _bursaryService.StudentPayments(fromDate, toDate, accountCode, deptCode, progType);
                    break;
                case "School Fee":
                    dto = _bursaryService.SchoolFeePayments(fromDate, toDate, deptCode, progType);
                    break;
            }

            return new ViewAsPdf(dto)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }


        public ActionResult ViewStudentPaymentsAsExcel(DateTime fromDate, DateTime toDate, string accountCode, string deptCode, string progType, string rpt)
        {
            FeesCollectionDTO dto = null;
            switch (rpt)
            {
                case "Other Fees":
                    dto = _bursaryService.StudentPayments(fromDate, toDate, accountCode, deptCode, progType);
                    break;
                case "School Fee":
                    dto = _bursaryService.SchoolFeePayments(fromDate, toDate, deptCode,progType);
                    break;
                     
            }
            if (dto != null)
            {
                Export2Excel<FeesCollectionDetailsDTO>(dto.Details, dto.Title);
                return View("ViewStudentPayments");
            }
            else
            return View("ViewStudentPayments");

        }
        public ActionResult SchoolFeePayments()
        {
            return View();
        }
        public ActionResult ViewSchoolFeePaymentsAsPDF(int sessionId,string deptCode)
        {

            var payments = _bursaryService.SchoolFeePayments(sessionId,deptCode);

            return new ViewAsPdf(payments)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }


        public ActionResult ViewSchoolFeePaymentsAsExcel(int sessionId, string deptCode)
        {
            var payments = _bursaryService.SchoolFeePayments(sessionId, deptCode);
            if (payments != null)
            {
                Export2Excel<FeesCollectionDetailsDTO>(payments.Details, payments.Session + "-" + payments.Department);
            }

            return View();

        }

        public ActionResult CollectionSummary()
        {
            return View();
        }
        public JsonResult FetchCollectionSummary(DateTime from, DateTime to, string progType)
        {
            return Json(_bursaryService.FeesCollectionSummary(from, to, progType), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region POSTING OLD FEES FROM OLD PORTAL
        [KSWebAuthorisation]
        public ActionResult PostConfirmedOldSchoolFeePayment()
        {
            return View();
        }

        [KSWebAuthorisation]
        public string FetchConfirmedSchoolFeePaymentAmount(object[] data1)
        {
            int sessionId = (int)data1[0];
            int installment = (int)data1[1];
            int level = (int)data1[2];
            string matric = (string)data1[3];
            string flag;
            //Check if student is owing


            var schedule = _studentAccounts.GenerateExpectedSchoolFeesPayment(matric, sessionId, installment, level, out flag);
            outPutMsg result = new outPutMsg();
            result.message = flag;
            result.value = schedule;
            var chk = JsonConvert.SerializeObject(result);

            return chk;
        }
        [KSWebAuthorisation]
        public string SubmitConfirmedSchoolFeePayment(object[] data1)
        {
            int sessionId = (int)data1[0];
            int installmentId = (int)data1[1];
            double amount = (int)data1[2];
            string matric = (string)data1[3];
            string flag;
            //Check if student is owing


            var schedule = _studentAccounts.SubmitGenerateExpectedSchoolFeesPayment(matric, sessionId, amount, installmentId, User.UserId, User.Username, out flag);
            outPutMsg result = new outPutMsg();
            result.message = flag;
            result.value = schedule;
            var chk = JsonConvert.SerializeObject(result);

            return chk;
        }
        #endregion

        #region FEES EXCEPTION OPERATIONS
        public ActionResult StudentFeeExemptions()
        {
            return View();
        }

        public JsonResult FetchStudentsForExemption(string programCode)
        {
            var students = _studentAccounts.StudentsForExemption(programCode);
            return Json(students, JsonRequestBehavior.AllowGet);
        }

        public void AddExemptions(List<FeeExempListDTO> students)
        {
            var chosen = students.Where(a => a.Exempt == true).ToList();
            _studentAccounts.AddBulkExemptions(chosen,User.FullName);
        }
        #endregion

        #region ACCOUNTING OPERATIONS
        public ActionResult ExpenseEntry()
        {
            return View();
        }
        #endregion

        #region OTHER CHARGES OPERATION
        public ActionResult OtherCharges()
        {
            return View();
        }
        public string SaveOtherCharge(OtherCharges ocharge)
        {
            return  _bursaryService.SaveOtherCharge(ocharge, User.UserId);
            
        }
        #endregion

        #region REPORTING
        [HttpGet]
        public ActionResult DailyCollections()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DailyCollections(DateTime? fromDate, DateTime? toDate,string rptType)//Split or Block
        {
            if(fromDate==null || toDate==null)
            {
                ModelState.AddModelError("", "A required field is missing");
                return View();
            }
            DailyCollectionDTO data;
            if(rptType=="Bulk")
            {
                data = _bursaryService.DailyRevenueCollectionsBulk(fromDate.Value, toDate.Value);
            }
            else if (rptType == "Full")
            {
                data = _bursaryService.DailyRevenueCollectionsFull(fromDate.Value, toDate.Value);
            }
            else
            {
                ModelState.AddModelError("", "Invalid Report Type Selected");
                return View();
            }
            if (data==null)
            {
                ModelState.AddModelError("", "No records found for selected period");
                return View();
            }

            ExcelPackage package = new ExcelPackage();
            var wrkSheet = package.Workbook.Worksheets.Add(fromDate.Value.ToString("dd-MMM-yyyy")+"to "+toDate.Value.ToString("dd-MMM-yyyy"));
            //wrkSheet.Cells[1,1].Value = "Pay Date";
            wrkSheet.Cells[1, 1].Value = "Date";
            wrkSheet.Cells[1, 2].Value = "S/N";
            wrkSheet.Cells[1, 3].Value = "Name";
            //wrkSheet.Cells[1, 4].Value = "MatricNo";
            wrkSheet.Cells[1, 4].Value = "Particulars";
            wrkSheet.Cells[1, 5].Value = "Amount";
            wrkSheet.Cells[1, 6].Value = "Pay Type";
            wrkSheet.Cells[1, 7].Value = "PayDate";
            wrkSheet.Cells[1, 8].Value = "Ref";
            int startIndex = 2;
             
            List<DailyCollectionDetailsDTO> dto = new List<DailyCollectionDetailsDTO>();
            foreach(var h in data.Headers)
            {
                wrkSheet.Cells[startIndex, 1].Value = h.Date;
                wrkSheet.Cells[startIndex, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                wrkSheet.Cells[startIndex, 1].Style.Font.Bold = true;
                int serialCount = 1;
                int lastCount = h.Details.Count;
                foreach (var d in h.Details)
                {
                    wrkSheet.Cells[startIndex, 2].Value = serialCount;
                    wrkSheet.Cells[startIndex, 3].Value = d.Name;
                    //wrkSheet.Cells[startIndex, 4].Value = d.Particulars;
                    wrkSheet.Cells[startIndex, 4].Value = d.Particulars;
                    wrkSheet.Cells[startIndex, 5].Value = d.Amount;
                    wrkSheet.Cells[startIndex, 5].Style.Numberformat.Format = "#,##0.00";
                    wrkSheet.Cells[startIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    wrkSheet.Cells[startIndex, 6].Value = d.PayType;
                    wrkSheet.Cells[startIndex, 7].Value = d.PayDate;
                    wrkSheet.Cells[startIndex, 7].Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                    wrkSheet.Cells[startIndex, 8].Value = d.TransRef;
                    
                    serialCount++;
                    startIndex++;
                }
                //Insert Summary
                wrkSheet.Cells[startIndex, 3].Value = "Sub Total";
                wrkSheet.Cells[startIndex, 3].Style.Font.Bold = true;
                wrkSheet.Cells[startIndex, 5].Value = h.Total;
                wrkSheet.Cells[startIndex, 5].Style.Numberformat.Format = "#,##0.00";
                wrkSheet.Cells[startIndex, 5].Style.Font.Bold = true;
                wrkSheet.Cells[startIndex, 5].Style.HorizontalAlignment=OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                startIndex++;
            }

            //Final Total
            
            wrkSheet.Cells[startIndex + 1, 3].Value = "Grand Total";
            wrkSheet.Cells[startIndex + 1, 3].Style.Font.Bold = true;
            wrkSheet.Cells[startIndex + 1, 5].Value = data.Total;
            wrkSheet.Cells[startIndex + 1, 5].Style.Numberformat.Format = "#,##0.00";
            wrkSheet.Cells[startIndex + 1, 5].Style.Font.Bold = true;
            wrkSheet.Cells[startIndex + 1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            wrkSheet.View.FreezePanes(2, 1);

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                 
                package.SaveAs(memoryStream);

                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            return View(); 
        }
        public ActionResult SessionBalancesReport()
        {
            return View();
        }
        public ActionResult StudentBalancesByProgTypeAsPDF(int sessionId,string progType)
        {
            var st = _bursaryService.StudentBalancesByProgType(sessionId, progType);
            var prog = _generalDuties.GetProgrammeTypes().Where(a => a.Type == progType).SingleOrDefault();
            ViewBag.progr = progType;
             
            ViewBag.session = _generalDuties.FetchSessions().Where(s => s.SessionId == sessionId).FirstOrDefault().Title;
             
            return new ViewAsPdf(st)
            {

                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }

        public ActionResult StudentBalancesByProgTypeAsExcel(int sessionId, string progType)
        {
            var dto = _bursaryService.StudentBalancesByProgType(sessionId, progType);
            var prog = _generalDuties.GetProgrammeTypes().Where(a => a.Type == progType).SingleOrDefault();
            ViewBag.progr = progType;

            ViewBag.session = _generalDuties.FetchSessions().Where(s => s.SessionId == sessionId).FirstOrDefault().Title;

            if (dto.Count == 0)
            {
                ModelState.AddModelError("", "No records found for selected period");
                return View();
            }

            ExcelPackage package = new ExcelPackage();
            var wrkSheet = package.Workbook.Worksheets.Add("StudentBalances");
            //wrkSheet.Cells[1,1].Value = "Pay Date";
            wrkSheet.Cells[1, 1].Value = "S/N";
            wrkSheet.Cells[1, 2].Value = "MatricNo";
            wrkSheet.Cells[1, 3].Value = "Name";
            wrkSheet.Cells[1, 4].Value = "Amount Owed";
            wrkSheet.Cells[1, 5].Value = "Amount Paid";
            wrkSheet.Cells[1, 6].Value = "Balance";
            
             
            int total = dto.Count;
            int count = 1;
            for (int i = 0; i < total; i++)
            {


                wrkSheet.Cells[i + 2, 1].Value = count++;
                wrkSheet.Cells[i + 2, 2].Value = dto[i].MatricNumber;
                wrkSheet.Cells[i + 2, 3].Value = dto[i].Name;

                wrkSheet.Cells[i + 2, 4].Value = dto[i].AmountOwed;
                wrkSheet.Cells[i + 2, 4].Style.Numberformat.Format = "{0:#,##0.00;(#,##0.00)}";
                wrkSheet.Cells[i + 2, 5].Value = dto[i].AmountPaid;
                wrkSheet.Cells[i + 2, 5].Style.Numberformat.Format = "{0:#,##0.00;(#,##0.00)}";
                wrkSheet.Cells[i + 2, 6].Value = dto[i].Balance;
                wrkSheet.Cells[i + 2, 6].Style.Numberformat.Format = "{0:#,##0.00;(#,##0.00)}";
                 
            }

            wrkSheet.Cells[total + 2, 2].Value = "Total";
            wrkSheet.Cells[total + 2, 2].Style.Font.Bold = true;
            wrkSheet.Cells[total + 2, 4].Value = dto.Sum(a => a.AmountOwed);
            wrkSheet.Cells[total + 2, 4].Style.Numberformat.Format = "{0:#,##0.00;(#,##0.00)}";
            wrkSheet.Cells[total + 2, 4].Style.Font.Bold = true;

            wrkSheet.Cells[total + 2, 5].Value = dto.Sum(a => a.AmountPaid);
            wrkSheet.Cells[total + 2, 5].Style.Numberformat.Format = "{0:#,##0.00;(#,##0.00)}";
            wrkSheet.Cells[total + 2, 5].Style.Font.Bold = true;

            wrkSheet.Cells[total + 2, 6].Value = dto.Sum(a => a.Balance);
            wrkSheet.Cells[total + 2, 6].Style.Numberformat.Format = "{0:#,##0.00;(#,##0.00)}";
            wrkSheet.Cells[total + 2, 6].Style.Font.Bold = true;
            wrkSheet.View.FreezePanes(1, 1);

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                package.SaveAs(memoryStream);

                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            return View();
        }

        public ActionResult StudentBalancesByDeptAsPDF(int sessionId, string deptCode)
        {
            var st = _bursaryService.StudentBalancesByProgType(sessionId, deptCode);
            var prog = _generalDuties.AllAcademicDepartments().Where(a => a.DepartmentCode == deptCode).SingleOrDefault();
            ViewBag.progr = prog;

            ViewBag.session = _generalDuties.FetchSessions().Where(s => s.SessionId == sessionId).FirstOrDefault().Title;

            return new ViewAsPdf(st)
            {

                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }

        public ActionResult FeesCollections()
        {
            return View();
        }

        public ActionResult SessionCollectionsSummary(string sessionID)
        {
            var col = _bursaryService.RevenueAccountsSummaryCollectionsBySession(sessionID);
            ViewBag.sess = sessionID;
            return new ViewAsPdf(col)
            {

                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }
        public ActionResult SessionCollectionsByAccount(string accountCode, string session)
        {
            var col = _bursaryService.RevenueAccountCollectionsDetails(session,accountCode);
             
            return new ViewAsPdf(col)
            {

                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }
        public ActionResult SessionCollectionsByAccountPayDate(string accountCode, DateTime fromDate,DateTime toDate)
        {
            var col = _bursaryService.RevenueAccountCollectionsDetails(fromDate,toDate,accountCode);

            return new ViewAsPdf(col)
            {

                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 1, Bottom = 2 }
            };
        }
        #endregion
        #region FEEOPTIONS
        public ActionResult FeeOptions()
        {
            return View();
        }
        public JsonResult GetFeeOptions()
        {
            return Json(_bursaryService.GetFeeOptions(),JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}