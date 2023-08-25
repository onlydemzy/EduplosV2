using Eduplus.Services.Contracts;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Eduplus.Domain.BurseryModule;
using Eduplus.DTO.BursaryModule;
using Eduplus.Domain.CoreModule;
using Eduplus.Services.UtilityServices;
using KS.Domain.AccountsModule;
using Eduplus.Web.SMC.ViewModels;
using OfficeOpenXml;
using System.Data.Entity;
using Eduplus.ObjectMappings;
using Eduplus.DTO.CoreModule;
using Eduplus.Domain.AcademicModule;

namespace Eduplus.Services.Implementations
{
    public class StudentsAccountsService:IStudentsAccountsService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public StudentsAccountsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public StudentsAccountsService()
        {

        }

        
        public List<PaymentInvoiceDTO> FetchInvoiceforConfirmationProcess()
        {
            List<PaymentInvoiceDTO> dto = new List<PaymentInvoiceDTO>();
            string date = "04/01/2021";
            var dat = Convert.ToDateTime(date);
            var invs = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Status == "PAID" && !string.IsNullOrEmpty(a.TransRef)
            && a.ApprovalChannel=="Manual" && DbFunctions.TruncateTime(a.CompletedDate)>=dat.Date
            ).ToList();
            foreach (var i in invs)
            {
                dto.Add(BursaryModuleMappings.PaymentInvoiceToInvoiceDTO(i,null,null,null));
                
            }
            return dto.OrderBy(a => a.GeneratedDate).ToList();
        }

        public void UPdateManualInvoiceMethd(List<TransIdsProcessMthdDTO> proc)
        {
            List<string> trans = new List<string>();
            foreach(var a in proc)
            {
                trans.Add(a.transactionID);
            }
             
            var inv = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => trans.Contains(a.TransactionId)).ToList();
            foreach(var i in proc)
            {
                var nInv = inv.Where(a => a.TransactionId == i.transactionID).Single();
                nInv.ApprovalChannel = i.TransMthd;
            }
            _unitOfWork.Commit("Sys Admin");
        }
        public List<PaymentInvoiceDTO> FetchCompletePay4Confirmation(DateTime startDate)
        {
            List<PaymentInvoiceDTO> dto = new List<PaymentInvoiceDTO>();
            var now = DateTime.UtcNow;
            
            var invs = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Status == "Pending" && !string.IsNullOrEmpty(a.TransRef)
            && a.GeneratedDate>=startDate).ToList();
            foreach (var i in invs)
            {
                PaymentInvoiceDTO dt = new PaymentInvoiceDTO();
                dt.TransactionId = i.TransactionId;
                dt.Amount = i.Amount;
                dt.GeneratedBy = i.GeneratedBy;
                dt.GeneratedDate = i.GeneratedDate;
                dt.Session = i.Session;
                dt.TransRef = i.TransRef;
                dt.PaymentType = i.PaymentType;
                dt.PayOptionId = i.PayOptionId;
                dt.StudentId = i.StudentId;
                dt.ProgrammeType = i.ProgrammeType;
                dto.Add(dt);
            }
            return dto.OrderBy(a => a.GeneratedDate).ToList();
        }
        
        public bool CheckPreviousSessionDebt(string studentId, int sessionId, out string amt)
        {
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.RegNo == studentId && a.SessionId < sessionId).ToList();
            var bal = payments.Where(c => c.TransType == "Credit").Sum(a => a.Amount) - payments.Where(c => c.TransType == "Debit").Sum(a => a.Amount);
            
            if (bal < 0)
            {
                amt = StandardGeneralOps.ToMoney(bal);
                return true;
            }

            else
            {
                amt = "";
                return false;
            }

                
        }
        #region FRESH STUDENTS OPERATIONS
        public int CheckAdmissionFeePayment(string studentId)
        {
            var payInvoice = _unitOfWork.PaymentInvoiceRepository.GetSingle(s => s.StudentId == studentId
              && s.PaymentType == "Screening Fee");
            if (payInvoice==null)
                return 0;
            if (payInvoice.Status == "Pending")
                return 1;
            if (payInvoice.Status == "PAID")
                return 0;
            else
                return 2;
        }
        public int CheckAcceptanceFeePayment(string studentId)
        {
            var payInvoice = _unitOfWork.PaymentInvoiceRepository.GetSingle(s => s.StudentId == studentId
              && s.PaymentType == "Acceptance Form");
            if (payInvoice == null)
                return 0;
            if (payInvoice.Status == "Pending")
                return 1;
            else
                return 2;
        }
        public int CheckIfOtherFeePaymentExist(string studentId,string paymentType)
        {
            var payInvoice = _unitOfWork.PaymentInvoiceRepository.GetSingle(s => s.StudentId == studentId
              && s.PaymentType == paymentType);
            if (payInvoice == null)
                return 0;
            if (payInvoice.Status == "Pending")
                return 1;
            else
                return 2;
        }
       
        #endregion
        public string DebitNewStudent(string studentId, string userId)
        {
            //Fetch student
            
            var student = _unitOfWork.StudentRepository.Get(studentId);
            var currentSession = _unitOfWork.SessionRepository.GetFiltered(a => a.IsCurrent == true).SingleOrDefault();
            var admitSession = _unitOfWork.SessionRepository.GetFiltered(a => a.Title == student.YearAddmitted).SingleOrDefault();

            //Check if Student has already been debited Include debit as transaction type
            var stpay = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.RegNo == studentId && a.SessionId == currentSession.SessionId
            && a.PaymentType == "School Fee").FirstOrDefault();
            if (stpay != null)
                return "Student already Debit for choosen session";
            //fetch schedule
            var schedule = _unitOfWork.FeeScheduleRepository.GetFiltered(f => f.SessionId == currentSession.SessionId &&
            f.ProgrammeType == student.ProgrammeType && f.FacultyCode == student.Department.FacultyCode &&f.Status=="Applied").FirstOrDefault();
            if(schedule==null)
            {
                return "Schedule yet to be applied";
            }
           
            var details = schedule.Details.ToList();
            double tuitionAll = details.Where(a => a.AppliesTo == "All" && a.Type == "Tuition").ToList().Sum(s => s.Amount);
            double freshtuition = 0;
            double rturningTuition = 0;
            double allFreshsundries=0;
            if (student.YearAddmitted == currentSession.Title)
            {
               freshtuition= details.Where(a => a.AppliesTo == "Freshmen" && a.Type == "Tuition").ToList().Sum(s=>s.Amount);
                allFreshsundries = details.Where(a => a.AppliesTo == "Freshmen" && a.Type == "Sundry").Sum(a => a.Amount);
            }
            if(student.YearAddmitted != currentSession.Title)
            {
                rturningTuition = details.Where(a => a.AppliesTo == "Returning Students" && a.Type == "Tuition").ToList().Sum(s => s.Amount);
            }
            double nonIn = 0;
            double nonIndiTution = details.Where(a => a.AppliesTo == "Non-Indigenes"&&a.Type=="Tuition").Sum(a => a.Amount);
            
            if(student.State!="Akwa Ibom")
            {
                nonIn = nonIndiTution;
            }
            double finalTuition = nonIn + tuitionAll + freshtuition + rturningTuition;
            //All sundry charges
            double allsundries= details.Where(a => a.AppliesTo == "All" && a.Type == "Sundry").Sum(a=>a.Amount);
            
            //Level wide charges
            double lvlWideAmt = 0;
            switch(student.CurrentLevel)
            {
                case 0:
                    if (student.EntryMode == "Direct Entry" && student.ProgrammeType == "Degree")
                    {
                        lvlWideAmt = details.Where(a => a.AppliesTo == "Level200").ToList().Sum(a => a.Amount);
                    }
                    else
                    {
                        lvlWideAmt = details.Where(a => a.AppliesTo == "Level100").ToList().Sum(a => a.Amount);
                    }
                    break;
                case 100:
                    lvlWideAmt = details.Where(a => a.AppliesTo == "Level200").ToList().Sum(a => a.Amount);
                    break;
                case 200:
                    lvlWideAmt = details.Where(a => a.AppliesTo == "Level300").ToList().Sum(a => a.Amount);
                    break;
                case 300:
                    lvlWideAmt = details.Where(a => a.AppliesTo == "Level400").ToList().Sum(a => a.Amount);
                    break;
                case 400:
                    lvlWideAmt = details.Where(a => a.AppliesTo == "Level500").ToList().Sum(a => a.Amount);
                    break;
                case 500:
                    lvlWideAmt = details.Where(a => a.AppliesTo == "Level600").ToList().Sum(a => a.Amount);
                    break;
                case 600:
                    lvlWideAmt = details.Where(a => a.AppliesTo == "Level600").ToList().Sum(a => a.Amount);
                    break;
                default:
                    lvlWideAmt = 0;
                    break;
            }
            
            StudentPayments sp = new StudentPayments
            {
                Amount = finalTuition+allsundries+lvlWideAmt+allFreshsundries,
                
                PayDate = DateTime.UtcNow,
                Particulars = admitSession.Title+" Session debit to freshmen",
                SessionId = admitSession.SessionId,
                RegNo = studentId,
                TransType = "Debit",
                PaymentId = GeneratePaymentId(studentId, admitSession.SessionId),
                PaymentType="School Fee"

            };
            _unitOfWork.StudentPaymentsRepository.Add(sp);

            _unitOfWork.Commit("System "+userId);
            return "Success";
        }



        #region FEES OPERATIONS

        public string GenerateTranscriptInvoice(string transcriptNo, int otherChargeId, int sessionId, string userId, out string flag)
        {
            var paySession = _unitOfWork.SessionRepository.Get(sessionId);
            var currentSemester = _unitOfWork.SemesterRepository.GetFiltered(a => a.IsCurrent == true).SingleOrDefault();
            
            flag = "Ok";
            var charge = _unitOfWork.OtherChargesRepository.Get(otherChargeId);
            if (charge == null)
            {
                flag = "Invalid charge selected";
                return null;
            }

            var student = _unitOfWork.StudentRepository.Get(userId);
            if (student == null)
            {
                flag = "Invalid student data";
                return null;
            }
            var progType = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);

            PaymentInvoice invoice = new PaymentInvoice();
            invoice.TransactionId = TransactionId(student.ProgrammeType);
            invoice.GeneratedDate = DateTime.UtcNow;

            invoice.GeneratedBy = student.Name;
            invoice.Particulars = "Payment for Transcript_"+transcriptNo;
            invoice.PaymentType = charge.Description;
            invoice.Regno = student.MatricNumber;
            invoice.StudentId =userId;
            invoice.Status = "Pending";
            invoice.ProgrammeType = student.ProgrammeType;
            invoice.Name = student.Name;
            invoice.Email = student.Email;
            invoice.Department = student.Department.Title;
            invoice.Programme = student.Programme.Title;
            invoice.Session = currentSemester.Session.Title;
            invoice.Phone = student.Phone;
            invoice.Installment = "Full";

            invoice.Semester = currentSemester.SemesterTitle;
 
            invoice.Amount = charge.Amount;
            //Generate TransRef
            if (progType.ApplyGatewayCharge == true)
            {
                var payGateway = progType.PaymentGateWays.Where(a => a.IsDefault == true).SingleOrDefault();
                invoice.ServiceCharge = CalculateServiceCharge(null, invoice.PaymentType, invoice.Installment, progType);
                remitaRRRGenResponse rrr = null;
                switch (payGateway.Name)
                {
                    case "Remita":
                        rrr = PaymentGatewayService.GenerateRemitaRRR(invoice.TransactionId, invoice.Department,
                        invoice.Session, invoice.Programme, progType, invoice.Particulars, invoice.Name, student.Email, student.Phone,
                        invoice.PaymentType, invoice.Regno, invoice.Amount, invoice.ServiceCharge);
                        if (rrr.statuscode == "025")
                        {
                            invoice.TransRef = rrr.RRR;

                        }
                        else
                        {
                            flag = rrr.statusMessage;
                            return null;

                        }
                        break;
                }

            }

            List<InvoiceDetails> details = new List<InvoiceDetails>
            {
                new InvoiceDetails
            {
                Amount = charge.Amount,
                Item = charge.Description,
                ItemCode = charge.AccountCode,
                InvoiceNo=invoice.TransactionId
            } };

            invoice.Details = details;
            _unitOfWork.PaymentInvoiceRepository.Add(invoice);
            _unitOfWork.Commit(userId);

            return invoice.TransactionId;
        }
        public PaymentInvoice GenerateStudentPaymentInvoice(string regNo,int otherChargeId,int sessionId, string userId, out string flag)
        {
            var paySession = _unitOfWork.SessionRepository.Get(sessionId);
            var currentSemester = _unitOfWork.SemesterRepository.GetFiltered(a=>a.IsCurrent==true).SingleOrDefault();
            double amount = 0;
            flag = "Ok";
            var charge = _unitOfWork.OtherChargesRepository.Get(otherChargeId);
            if (charge == null)
            {
                flag = "Invalid charge selected";
                return null;
            }
            
            var student = _unitOfWork.StudentRepository.Get(regNo);
            if (student == null)
            {
                flag = "Invalid student data";
                return null;
            }
            var progType = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);       
                           
            PaymentInvoice invoice = new PaymentInvoice();
            invoice.TransactionId = TransactionId(student.ProgrammeType);
            invoice.GeneratedDate = DateTime.UtcNow;
            
            invoice.GeneratedBy = student.Name;
            invoice.Particulars = paySession.Title + " For /" + charge.Description+"-Payment";
            invoice.PaymentType = charge.Description;
            if (paySession.HideFreshmenMatricNo == true && student.YearAddmitted == paySession.Title)
            {
                invoice.Regno = null;
            }
            else { invoice.Regno = student.MatricNumber; }

            invoice.StudentId = regNo;
            invoice.Status = "Pending";
            invoice.ProgrammeType = student.ProgrammeType;
            invoice.Name = student.Name;
            invoice.Email = student.Email;
            invoice.Department = student.Department.Title;
            invoice.Programme = student.Programme.Title;
            invoice.Session = currentSemester.Session.Title;
            invoice.Phone = student.Phone;
            invoice.Installment = "Full";
             
            invoice.Semester = currentSemester.SemesterTitle;
            
            if (charge.Description != "Convocation Fee")
            {
                amount = charge.Amount;
            }
            else
            {
                amount = GetConvocPayData(student);
                invoice.Particulars = student.GradYear + " For /" + charge.Description + "-Payment";
            }
            if (amount == 0)
            {
                flag = "Error generating invoice, Amount could not be set. Try again";
                return null;
            }
                
            invoice.Amount = amount;
            //Generate TransRef
            if (progType.ApplyGatewayCharge == true)
            {
                var payGateway = progType.PaymentGateWays.Where(a => a.IsDefault == true).SingleOrDefault();
                invoice.ServiceCharge = CalculateServiceCharge(null, invoice.PaymentType, invoice.Installment, progType);
                remitaRRRGenResponse rrr = null;
                switch (payGateway.Name)
                {
                    case "Remita":
                        rrr = PaymentGatewayService.GenerateRemitaRRR(invoice.TransactionId, invoice.Department,
                        invoice.Session, invoice.Programme, progType, invoice.Particulars, invoice.Name, student.Email, student.Phone,
                        invoice.PaymentType, invoice.Regno, invoice.Amount, invoice.ServiceCharge);
                        if (rrr.statuscode == "025")
                        {
                            invoice.TransRef = rrr.RRR;
                            
                        }
                        else
                        {
                            flag = rrr.statusMessage;
                            return null;
                            
                        }
                        break;
                }

            }
           
            List<InvoiceDetails> details=new List<InvoiceDetails>
            {
                new InvoiceDetails
            {
                Amount = amount,
                Item = charge.Description,
                ItemCode = charge.AccountCode,
                InvoiceNo=invoice.TransactionId
            } };

            invoice.Details=details;
            _unitOfWork.PaymentInvoiceRepository.Add(invoice);
            _unitOfWork.Commit(userId);

            return invoice;
        }
       
        public string GenerateStudentInvoiceToCreditStudentAccount(TransactionDTO debit, string userId,out string flag)
        {

            flag = "";
            var student = _unitOfWork.StudentRepository.Get(debit.StudentId);
            var userData = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);
            if (student == null || string.IsNullOrEmpty(student.Email) || string.IsNullOrEmpty(student.Phone))
            {
                flag = "Update student record with valid email and phone numbers";
                return null;
            }
            var sessions = _unitOfWork.SessionRepository.GetFiltered(a => a.IsCurrent == true || a.SessionId == debit.SessionId).ToList();
            var acct = _unitOfWork.AccountsRepository.Get(debit.AccountCode);
            var currentSes = sessions.Where(a => a.IsCurrent == true).FirstOrDefault();
            var user = _unitOfWork.UserRepository.Get(userId);
            PaymentInvoice sp = new PaymentInvoice();
            sp.Amount = debit.Amount;
            sp.StudentId = debit.StudentId;
            sp.PaymentType = acct.Title;
            sp.Particulars = sessions.Where(s=>s.SessionId==debit.SessionId).FirstOrDefault().Title+"-"+debit.Particulars;
            sp.Regno = student.MatricNumber;
            sp.Session = sessions.Where(s=>s.IsCurrent==true).FirstOrDefault().Title;
            sp.Semester = currentSes.Semesters.Where(s => s.IsCurrent == true).FirstOrDefault().SemesterTitle;
            sp.TransactionId = TransactionId(student.ProgrammeType);
            sp.GeneratedDate = DateTime.UtcNow;
            sp.GeneratedBy = user.UserName;
            sp.Name = student.Name;
             
            sp.Status = "Pending";
            sp.Department = student.Department.Title;
            sp.Programme = student.Programme.Title;
            sp.ProgrammeType = student.ProgrammeType;
            sp.Phone = student.Phone;
            sp.Email = student.Email;
             

            InvoiceDetails detail = new InvoiceDetails();
            detail.Amount = debit.Amount;
            detail.InvoiceNo = sp.TransactionId;
            detail.Item = acct.Title;
            detail.ItemCode = acct.AccountCode;
            
            if (userData.ApplyGatewayCharge == true)
            {
                if (sp.PaymentType.Contains("School"))
                {
                    sp.Installment = "Part";
                    sp.ServiceCharge = CalculateServiceCharge(null, "School Fee", sp.Installment, userData);
                }
                else
                {
                    sp.Installment = "Full";
                    sp.ServiceCharge = CalculateServiceCharge(null, sp.PaymentType, sp.Installment, userData);
                }

                var payGateway = userData.PaymentGateWays.Where(a => a.IsDefault == true).SingleOrDefault();
                remitaRRRGenResponse rrr = null;
                switch (payGateway.Name)
                {
                    case "Remita":
                        rrr = PaymentGatewayService.GenerateRemitaRRR(sp.TransactionId, sp.Department,
                        sp.Session, sp.Programme, userData, sp.Particulars, sp.Name, student.Email, student.Phone,
                        sp.PaymentType, sp.Regno, sp.Amount, sp.ServiceCharge);
                        if (rrr.statuscode == "025")
                        {
                            sp.TransRef = rrr.RRR; 
                             
                        }
                        else
                        {
                            flag = rrr.statusMessage;
                            return null;
                        }
                        break;
                }

            }
            sp.Details.Add(detail);
            _unitOfWork.PaymentInvoiceRepository.Add(sp);
            _unitOfWork.Commit(userId);
            flag = "Ok";
            return sp.TransactionId;
        }
        private double CalculateServiceCharge(FeeOptions schoolFeeOptions, string payType, string installment,ProgrammeTypes userData)
        {
            
            var gatewayData = userData.PaymentGateWays.Where(a=>a.IsDefault==true).FirstOrDefault();
            
            double serviceCharge = 0;
            
            double gateWayCharge = gatewayData.GatewayCharge;


            if (payType == "School Fee")
            {
                if (schoolFeeOptions == null)//Old school fee
                {
                    serviceCharge = gateWayCharge + (Convert.ToDouble(gatewayData.ProviderMajorFee) / 2);
                    return serviceCharge;
                }
                if (installment != "Full")
                {
                    if (userData.ApplyMajorCharge == true)
                    {
                        serviceCharge = gateWayCharge + (gatewayData.ProviderMajorFee / schoolFeeOptions.Cycle);

                    }
                    else
                    {
                        serviceCharge = Convert.ToDouble(gatewayData.GatewayCharge);
                    }

                }
                else
                {
                    if (userData.ApplyMajorCharge == true)
                    {
                        serviceCharge = gatewayData.GatewayCharge + gatewayData.ProviderMajorFee;

                    }
                    else
                    {
                        serviceCharge = gatewayData.GatewayCharge;
                    }
                }
            }
            else
            {
                if (userData.ApplyMinorCharge == true)
                {
                    serviceCharge = gatewayData.GatewayCharge + gatewayData.ProviderMinorFee;

                }
                else
                {
                    serviceCharge = gatewayData.GatewayCharge;
                }

            }

            return serviceCharge;
        }

        public double GetConvocPayData(Student student)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            List<ConvocYrPayData> data = new List<ConvocYrPayData>();
            var filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "SpecialChargesWithRange.xlsx");
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                
                var worksheet = package.Workbook.Worksheets["ConvocFees"];
                //Read contents
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;
                if (colCount == 0 || rowCount == 0)
                    return 0;
                for (int row = 2; row <= rowCount; row++)
                {
                    ConvocYrPayData dt = new ConvocYrPayData();
                    
                    dt.StartYear = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString());
                    dt.EndYear = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString());
                    dt.Amount = Convert.ToDouble(worksheet.Cells[row, 3].Value.ToString());
                    data.Add(dt);

                }

            }
            int gradeYr = Convert.ToInt32(student.GradYear.Split('/')[0]);
            var amt = data.Where(a => gradeYr >= a.StartYear && gradeYr <= a.EndYear).SingleOrDefault();
            if (amt == null)// Use highest amount
            {
                return data.OrderByDescending(a => a.Amount).First().Amount;
            }
                
            else return amt.Amount;
             
        }
      
        
        public List<PaymentInvoiceDTO> PaymentInvoices(string studentId)
        {
            var payments = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.StudentId == studentId).ToList();
            if (payments.Count == 0)
                return null;
            List<PaymentInvoiceDTO> dto = new List<PaymentInvoiceDTO>();
            foreach(var p in payments)
            {
                var pa = new PaymentInvoiceDTO
                {
                    TransactionId = p.TransactionId,
                    GeneratedDate = p.GeneratedDate,
                    Name = p.Name,
                    Amount = p.Amount,
                    PaymentType = p.PaymentType,
                    ProgrammeType = p.ProgrammeType,
                    Status = p.Status,
                    StudentId = p.StudentId,
                    Particulars = p.Particulars,
                    PayOptionId=p.PayOptionId
                };
                dto.Add(pa);
            }
            return dto;
        }

        
        public string ProcessSuccessfulEPayments(string transId,DateTime completionDate,string confirmChannel,string userId=null)
        {
           
            var payment = _unitOfWork.PaymentInvoiceRepository.Get(transId);
            var session = _unitOfWork.SessionRepository.GetFiltered(a => a.IsCurrent == true).FirstOrDefault();
            var semester = session.Semesters.Where(a => a.IsCurrent == true).FirstOrDefault();
            if (payment.Status == "PAID")
            { return "PAID"; }
            payment.Session = session.Title;

            payment.Status = "PAID";
            payment.CompletedDate = completionDate;
            payment.Semester = semester.SemesterTitle;
            payment.ApprovalChannel = confirmChannel;
            
            //Generate Payment Reference
            string payRef = GeneratePaymentId(payment.StudentId, session.SessionId);
            //Add Amount to Student Payments
            StudentPayments addPay = new StudentPayments();

            addPay.Amount = payment.Amount;
            addPay.PaidBy = payment.Name;
            addPay.Particulars = payment.Particulars;
            addPay.PayMethod = payment.ApprovalChannel;
            addPay.TransType = "Credit";
            addPay.VoucherNo = payment.TransactionId;
            addPay.PaymentType = payment.PaymentType;
            addPay.PayDate = completionDate;
            
            addPay.ReferenceCode = payment.TransactionId + "/" + payment.TransRef;
            addPay.PaymentId = payRef;
            addPay.SessionId = session.SessionId;
            addPay.RegNo = payment.StudentId;
            addPay.VoucherNo = payment.TransactionId;
            _unitOfWork.StudentPaymentsRepository.Add(addPay);

            //Add Amount to Bank Account in GL
            TransMaster bPay = new TransMaster();
            var acct = _unitOfWork.AccountsRepository.GetFiltered(a => a.IsCollectionAccount == true).FirstOrDefault();
            bPay.Amount = payment.Amount;
            bPay.Particulars = payment.Particulars + "by " + payment.Name + "-" + payment.StudentId;
            bPay.PayMethod = confirmChannel;

            bPay.AccountCode = acct.AccountCode;
            bPay.TellerNo = payment.TransactionId;
            bPay.Bank = acct.AccountCode;
            bPay.TransDate = completionDate;

            bPay.TransType = "Credit";

            bPay.TransRef = payRef;

            _unitOfWork.TransMasterRepository.Add(bPay);

            //Add Individual Transaction to accounts

            foreach (var d in payment.Details)
            {
                var trans = new TransMaster
                {
                    AccountCode = d.ItemCode,
                    Amount = d.Amount,
                    TransType = "Credit",
                    TransDate = completionDate,
                    TransRef = payRef,
                    Particulars = d.Item + " payment"

                };
                _unitOfWork.TransMasterRepository.Add(trans);
            }
             
            string user = "";
            if(!string.IsNullOrEmpty(userId))
            {
                user=transId+"-"+userId;             
            }
            else
            {
                user="System on epayment " + payment.TransactionId;                
            }
            _unitOfWork.Commit(user);//Check if it has lateRegFee

            var lr = payment.Details.Where(a => a.Item == "Late Registration").FirstOrDefault();
            if(lr!=null)
            {
                return "Late";
            }
            else 
            return "Transaction completed successfully";
        }
        public PaymentInvoiceDTO GetStudentPaymentInvoice(string transactionId)
        {
            var inv= _unitOfWork.PaymentInvoiceRepository.GetFiltered(a=>a.TransactionId==transactionId
            ||a.TransRef==transactionId).FirstOrDefault();
            if(inv==null)
            {
                return null;
            }
            //{
                var student = _unitOfWork.StudentRepository.Get(inv.StudentId);
            byte[] foto = null;
            if (!string.IsNullOrEmpty(student.PhotoId))
            {
                foto= student.Photo.Foto;
            }
            var dto = BursaryModuleMappings.PaymentInvoiceToInvoiceDTO(inv, student.Email, student.Phone, foto);
            dto.GeneratedDate = StandardGeneralOps.ConvertUtcToLocalTimeZone(dto.GeneratedDate);
            return dto;
            //}           
        }
        public PaymentInvoiceDTO GetStudentPaymentInvoiceForManualConfirmation(string transactionId)
        {
            var invs = _unitOfWork.PaymentInvoiceRepository.Get(transactionId);
            if (invs == null)
                return null;
            PaymentInvoiceDTO inv = new PaymentInvoiceDTO();
            inv.TransactionId = invs.TransactionId;
            inv.Amount = invs.Amount;
            inv.Department = invs.Department;
            inv.GeneratedBy = invs.GeneratedBy;
            inv.GeneratedDate =StandardGeneralOps.ConvertUtcToLocalTimeZone(invs.GeneratedDate);
            inv.Name = invs.Name;
            inv.Particulars = invs.Particulars;
            inv.PaymentType = invs.PaymentType;
            inv.Programme = invs.Programme;
            inv.ProgrammeType = invs.ProgrammeType;
            inv.Regno = invs.Regno;
            inv.Session = invs.Session;
            inv.Status = invs.Status;
            inv.StudentId = invs.StudentId;
            inv.TransRef = invs.TransRef;
            inv.PayOptionId = invs.PayOptionId;
            return inv;
            //}           
        }
        public PaymentInvoiceDTO GetStudentPaymentInvoice(string transactionId,string payref)
        {
            var inv = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a=>a.TransactionId==transactionId&&a.TransRef==payref).SingleOrDefault();
            if (inv != null)
            {
                var student = _unitOfWork.StudentRepository.Get(inv.StudentId);
                byte[] foto = null;
                if(!string.IsNullOrEmpty(student.PhotoId))
                {
                    foto = student.Photo.Foto;
                }
                var dto = BursaryModuleMappings.PaymentInvoiceToInvoiceDTO(inv, student.Email, student.Phone, foto);
                dto.GeneratedDate = StandardGeneralOps.ConvertUtcToLocalTimeZone(dto.GeneratedDate);
                return dto;
            }
            else return null;           
        }

       
    public StudentAccountSummaryDTO StudentAccountSummary(string matricNumber)
        {
            var student = _unitOfWork.StudentRepository.GetFiltered(s => s.MatricNumber == matricNumber.ToUpper()).SingleOrDefault();
            if (student == null)
                return new StudentAccountSummaryDTO();
            StudentAccountSummaryDTO sumary = new StudentAccountSummaryDTO();
            sumary.Name = student.Name;
            sumary.CurrentLevel = student.CurrentLevel;
            sumary.Department = student.Department.Title;
            sumary.Email = student.Email;
            sumary.Phone = student.Phone;
            sumary.RegNo = student.MatricNumber;
            sumary.StudentId = student.PersonId;
            sumary.YearAddmitted = student.YearAddmitted;
            sumary.Balance = FetchStudentAccountBalance(student.PersonId);

            return sumary;
        }
         
        public List<StudentTransactionDTO> StudentPayments(string studentId, out string balance)
        {
            List<StudentTransactionDTO> dto = new List<StudentTransactionDTO>();
            balance = "";
            var stpays = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.RegNo == studentId).ToList();
            if (stpays.Count == 0 || stpays == null)
                return new List<StudentTransactionDTO>();

            foreach(var a in stpays.Where(a=>a.TransType=="Credit"))
            {
                var pay = new StudentTransactionDTO
                {
                    Amount = a.Amount,
                    Particulars = a.Particulars,
                    TransDate = a.PayDate,
                    StudentId = a.RegNo,
                    PaymentId = a.PaymentId
                };
                dto.Add(pay);
            }
            double bal = stpays.Where(a => a.TransType == "Credit").Sum(a => a.Amount) - stpays.Where(a => a.TransType == "Debit").Sum(a => a.Amount);
            balance = StandardGeneralOps.ToMoney(bal);
            return dto.OrderBy(a=>a.TransDate).ToList();
        }
        string FetchStudentAccountBalance(string studentId)
        {
            var trans = _unitOfWork.StudentPaymentsRepository.GetFiltered(s => s.RegNo == studentId).ToList();
            double credits = trans.Where(s => s.TransType == "Credit").Sum(a => a.Amount);
            double debit= trans.Where(s => s.TransType == "Debit").Sum(a => a.Amount);
            return StandardGeneralOps.ToMoney(credits - debit);
        }
        

        public string DebitStudentAccount(TransactionDTO debit, string userId)
        {
            
            var student = _unitOfWork.StudentRepository.Get(debit.StudentId);
            if (student == null)
                return "Invalid student number";
            StudentPayments sp = new StudentPayments();
            sp.Amount = debit.Amount;
            sp.RegNo = debit.StudentId;
            
            sp.Particulars = debit.Particulars;
            sp.PayDate = debit.PayDate;
            sp.SessionId = debit.SessionId;
            sp.VoucherNo = debit.VoucherNumber;
            sp.TransType = "Debit";
            sp.PaymentId = GeneratePaymentId(debit.StudentId, debit.SessionId);

            _unitOfWork.StudentPaymentsRepository.Add(sp);
            _unitOfWork.Commit(userId);
            return "Operation completed successfully";
        }

        public StudentAccountStatementDTO StudentAccountStatement(string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            if (student == null)
                return new StudentAccountStatementDTO();

            StudentAccountStatementDTO dto = new StudentAccountStatementDTO();
            dto.Name = student.Name;
            dto.Department = student.Department.Title;
            dto.MatricNo = student.MatricNumber;
            dto.Programme = student.Programme.Title;
            List<StudentTransactionDTO> details = new List<StudentTransactionDTO>();
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.RegNo == studentId).ToList();
            if(payments.Count>0)
            {
                foreach(var p in payments)
                {
                    StudentTransactionDTO detail = new StudentTransactionDTO();

                    detail.Amount = p.Amount;
                    
                    detail.Type = p.TransType;
                    detail.Particulars = p.Particulars;
                    detail.TransDate = p.PayDate;
                  
                    details.Add(detail);
                }
                dto.Details = details.OrderBy(a => a.TransDate).ToList();
            }
            dto.Balance = dto.Details.Where(a => a.Type == "Credit").Sum(a => a.Amount) - dto.Details.Where(a => a.Type == "Debit").Sum(a => a.Amount);
            return dto;
        }


        #endregion

        #region SCHOOL FEES INVOICE GENERATION
        public PaymentInvoice GenerateSchoolFeesPaymentInvoice(string regNo, int sessionId, int installmentId, int payLevel,string userId, out string flag)
        {
            //Fetch Student
            var student = _unitOfWork.StudentRepository.GetFiltered(s=>s.PersonId==regNo).SingleOrDefault();
            var options = _unitOfWork.FeeOptionsRepository.Get(installmentId);
            var sessionToPay = _unitOfWork.SessionRepository.Get(sessionId);
            var currentSemester = _unitOfWork.SemesterRepository.GetFiltered(a => a.IsCurrent == true).FirstOrDefault();
            var currentSession = currentSemester.Session;
            if(string.IsNullOrEmpty(student.Email)||string.IsNullOrEmpty(student.Phone))
            {
                flag="Missing Input field. Please ensure that your profile is upto date with valid email and phone number";
                return null;
            }
            if (student.ProgrammeType != options.ProgrammeType)
            {
                flag = "Programme type mismatch";
                return null;
            }
            //Fetch schedule for student
            var schedule = _unitOfWork.FeeScheduleRepository.GetFiltered(f => f.SessionId == sessionId &&
                                  f.FacultyCode == student.Department.FacultyCode && 
                                  f.ProgrammeType == student.ProgrammeType && f.Status=="Applied").SingleOrDefault();

            if (schedule == null)
            {
                flag = "Schedule not set for chosen faculty and programme";
                return null;
            }

            string yearAdmitted = student.YearAddmitted;
            var userData = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);
            PaymentInvoice dto = new PaymentInvoice();
            List<InvoiceDetails> details = new List<InvoiceDetails>();

            dto.Department = student.Department.Title;
            dto.Name = student.Name;
            dto.Programme = student.Programme.Title;
            dto.ProgrammeType = student.ProgrammeType;

            if (sessionToPay.HideFreshmenMatricNo == true && student.YearAddmitted == sessionToPay.Title)
            {
                dto.Regno = null;
            }
            else {dto.Regno = student.MatricNumber; }

            var chk = schedule.Details.ToList();
            
            dto.PaymentType = "School Fee";
            dto.TransactionId = TransactionId(student.ProgrammeType);
            dto.StudentId = student.PersonId;
            dto.Session = currentSemester.Session.Title;
            dto.Particulars = sessionToPay.Title + " payment for~" + options.Installment;
            dto.Semester = currentSemester.SemesterTitle;
            dto.Email = student.Email;
            dto.LevelToPay = payLevel;
            dto.PayOptionId = installmentId;
            //Add Tuition fee
            var tui = GetTuitionCharge(schedule.Details.ToList(), student.YearAddmitted, sessionToPay);
            if (tui == null)
            {
                flag = "Error generating invoice. Please try again later";
                return null;
            }
            tui.InvoiceNo = dto.TransactionId;
            tui.Amount = tui.Amount * (options.PercentageTuition / 100);
            details.Add(tui);

            //Add Non- Indigene Charge
            if(student.State!="Akwa Ibom")
            {
                var nonIn = GetTuitionChargeNonIndigene(schedule.Details.ToList());
                if(nonIn!=null)
                {
                    nonIn.Amount = nonIn.Amount * (options.PercentageTuition / 100);
                    nonIn.InvoiceNo = dto.TransactionId;
                    details.Add(nonIn);
                }
                
            }
            
            //Add details to invoice
            
            var sundryCharges = GetStudentSundryCharges(schedule.Details.ToList(), student.YearAddmitted, sessionToPay.Title, payLevel);

            foreach (var s in sundryCharges)
            {
                details.Add(new InvoiceDetails
                {
                    Amount = s.Amount*(options.PercentageSundry)/100,
                    Item = s.Item,
                    ItemCode = s.ItemCode,
                    InvoiceNo = dto.TransactionId
                });
            }
            //Finalise based on options choosen
            if (options.PercentageSundry==100.0 && options.PercentageTuition==100.0)
            {
                
                dto.Installment = "Full";
            }
            else
            {
                dto.Installment = "Part";
            }
            //Now add late Registration if any
            
            var lateRegCharge = _unitOfWork.OtherChargesRepository.GetFiltered(s => s.ProgrammeType == student.ProgrammeType && s.Description == "Late Registration").FirstOrDefault();
                     
            var lateFees = AddLateRegistrationToSchedule(sessionToPay, options,currentSession);
            if (lateFees!=null)
            {
                 
                details.Add(new InvoiceDetails
                {
                    Amount = lateFees.Amount,
                    InvoiceNo = dto.TransactionId,
                    Item =lateFees.Item,
                    ItemCode = lateFees.ItemCode
                });
            }
            dto.Status = "Pending";
            var user = _unitOfWork.UserRepository.Get(userId);
            dto.GeneratedBy = user.UserName;
            dto.GeneratedDate = DateTime.UtcNow;
            dto.Details = details;
            dto.Amount = dto.Details.Sum(a => a.Amount);
            dto.ServiceCharge = CalculateServiceCharge(options, dto.PaymentType, dto.Installment,userData);


            //Generate TransReff
             
            if(userData.ApplyGatewayCharge==true)
            {
                var payGateway = userData.PaymentGateWays.Where(a => a.IsDefault == true).SingleOrDefault();
                remitaRRRGenResponse rrr=null;
                switch (payGateway.Name)
                {
                    case "Remita":
                        rrr = PaymentGatewayService.GenerateRemitaRRR(dto.TransactionId, dto.Department,
                        dto.Session, dto.Programme, userData, dto.Particulars, dto.Name, student.Email, student.Phone,
                        dto.PaymentType, dto.Regno, dto.Amount, dto.ServiceCharge);
                        if (rrr.statuscode == "025")
                        {
                            dto.TransRef = rrr.RRR;
                            
                        }
                        else
                        {
                            flag = rrr.statusMessage;
                            return null;
                        }
                        break;
                }
                
            }
            

            //Save
            _unitOfWork.PaymentInvoiceRepository.Add(dto);

            //Add LateRegistrationLog
            
            _unitOfWork.Commit(userId);
            flag = "Ok";
            dto.Details.OrderBy(a => a.Item).ToList();
            return dto;


        }

        public double GenerateExpectedSchoolFeesPayment(string regNo, int sessionId, int installmentId, int payLevel,  out string flag)
        {
            //Fetch Student
            var student = _unitOfWork.StudentRepository.Get(regNo);
            var options = _unitOfWork.FeeOptionsRepository.Get(installmentId);
            var sessionToPay = _unitOfWork.SessionRepository.Get(sessionId);
            var semesterPaid = _unitOfWork.SemesterRepository.GetFiltered(a => a.IsCurrent == true).FirstOrDefault();

             
            if (student.ProgrammeType != options.ProgrammeType)
            {
                flag = "Programme type mismatch";
                return 0;
            }
            //Fetch schedule for student
            var schedule = _unitOfWork.FeeScheduleRepository.GetFiltered(f => f.SessionId == sessionId &&
                                  f.FacultyCode == student.Department.FacultyCode &&
                                  f.ProgrammeType == student.ProgrammeType && f.Status == "Applied").SingleOrDefault();

            if (schedule == null)
            {
                flag = "Schedule not set for chosen faculty and programme";
                return 0;
            }

            string yearAdmitted = student.YearAddmitted;
            var userData = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);
            StudentPayments dto = new StudentPayments();
            List<InvoiceDetails> details = new List<InvoiceDetails>();
            
            //Add Tuition fee
            var tui = GetTuitionCharge(schedule.Details.ToList(), student.YearAddmitted, sessionToPay);
            if (tui == null)
            {
                flag = "Error generating invoice. Please try again later";
                return 0;
            }
             
            tui.Amount = tui.Amount * (options.PercentageTuition / 100);
            details.Add(tui);

            //Add Non- Indigene Charge
            if (student.State != "Akwa Ibom")
            {
                var nonIn = GetTuitionChargeNonIndigene(schedule.Details.ToList());
                if (nonIn != null)
                {
                    nonIn.Amount = nonIn.Amount * (options.PercentageTuition / 100);
                    details.Add(nonIn);
                }

            }

            //Add details to invoice

            var sundryCharges = GetStudentSundryCharges(schedule.Details.ToList(), student.YearAddmitted, sessionToPay.Title, payLevel);

            foreach (var s in sundryCharges)
            {
                details.Add(new InvoiceDetails
                {
                    Amount = s.Amount * (options.PercentageSundry) / 100,
                    Item = s.Item,
                    ItemCode = s.ItemCode,
                     
                });
            }
            dto.Amount = details.Sum(a => a.Amount);
            
            
            flag = "Ok";
            return dto.Amount;


        }
        public string SubmitGenerateExpectedSchoolFeesPayment(string regNo, int sessionId, double amount, int installmentId, string userId, string username, out string flag)
        {

            var student = _unitOfWork.StudentRepository.Get(regNo);
            StudentPayments dto = new StudentPayments();
            var session = _unitOfWork.SessionRepository.Get(sessionId);
            var options = _unitOfWork.FeeOptionsRepository.Get(installmentId);
            dto.Amount = amount;

            dto.PaidBy = student.Name;
            dto.PayDate = DateTime.UtcNow;
            dto.TransType = "Credit";
            dto.PaymentType = "School Fee";

            dto.RegNo = student.PersonId;
            dto.SessionId = sessionId;
            dto.Particulars = "Posted " + session.Title + " payment for~" + options.Installment;
            dto.PaymentId = GeneratePaymentId(dto.RegNo, sessionId);

            //Save
            _unitOfWork.StudentPaymentsRepository.Add(dto);

            //Add LateRegistrationLog

            _unitOfWork.Commit(userId);
            flag = "Ok";
            return "Process completed successfully";


        }
        List<InvoiceDetails> GetStudentSundryCharges(List<FeeScheduleDetail> details, string yrAdmitted, string session2Pay, int currentlvl)
        {

            List<InvoiceDetails> dto = new List<InvoiceDetails>();

            //Add AllSundry charges
            var sundry = GetAllSundryCharges(details, yrAdmitted, session2Pay, currentlvl);
            if (sundry.Count > 0)
            {
                foreach (var s in sundry)
                {
                    InvoiceDetails sfd = new InvoiceDetails
                    {
                        Item = s.Item,
                        Amount = s.Amount,
                        ItemCode = s.ItemCode
                    };
                    dto.Add(sfd);
                }
            }

            //Add LevelWide charges
             
            var lvlWide = AddLevelWideSchedule(details, currentlvl);
            if (lvlWide.Count > 0)
            {
                foreach (var l in lvlWide)
                {
                    InvoiceDetails sfd = new InvoiceDetails
                    {
                        Item = l.Item,
                        Amount = l.Amount,
                        ItemCode = l.ItemCode
                    };
                    dto.Add(sfd);
                }
            }

            return dto;
        }

        InvoiceDetails GetTuitionCharge(List<FeeScheduleDetail> details, string yrAdmitted, Session session2Pay)
        {

            var freshmentution = details.Where(a => a.AppliesTo == "Freshmen" && a.Type == "Tuition").FirstOrDefault();
            var alltution = details.Where(a => a.AppliesTo == "All" && a.Type == "Tuition").FirstOrDefault();
            var returningStudenttution = details.Where(a => a.AppliesTo == "Returning Students" && a.Type == "Tuition").FirstOrDefault();

            if (alltution != null)
            {
                return new InvoiceDetails
                {
                    Item = alltution.Accounts.Title,
                    ItemCode = alltution.AccountCode,
                    Amount = alltution.Amount
                };

            }
            else if (returningStudenttution != null && session2Pay.Title != yrAdmitted)
            {
                return new InvoiceDetails
                {
                    Item = returningStudenttution.Accounts.Title,
                    ItemCode = returningStudenttution.AccountCode,
                    Amount = returningStudenttution.Amount
                };

            }
            else if (freshmentution != null && session2Pay.Title == yrAdmitted)
            {
                return new InvoiceDetails
                {
                    Item = alltution.Accounts.Title,
                    ItemCode = alltution.AccountCode,
                    Amount = alltution.Amount
                };

            }
            else return null;
        }
        InvoiceDetails GetTuitionChargeNonIndigene(List<FeeScheduleDetail> details)
        {
            
            var non = details.Where(a => a.AppliesTo == "Non-Indigenes" && a.Type=="Tuition").FirstOrDefault();
            
            if (non != null)
            {
                return new InvoiceDetails
                {
                    Item = non.Accounts.Title,
                    ItemCode = non.AccountCode,
                    Amount = non.Amount
                };

            }
            
            else return null;
        }
        List<InvoiceDetails> GetAllSundryCharges(List<FeeScheduleDetail> details, string yrAdmitted, string session2Pay, int currentlvl)
        {
            List<InvoiceDetails> dto = new List<InvoiceDetails>();
            // Add Sundry Charges
            

                foreach (var f in details.Where(a => ((a.AppliesTo == "All") && a.Type == "Sundry")))
                {
                    InvoiceDetails sfd = new InvoiceDetails
                    {
                        Item = f.Accounts.Title,
                        Amount = f.Amount,
                        ItemCode = f.AccountCode
                    };
                    dto.Add(sfd);
                }

            if (yrAdmitted == session2Pay)//Add Freshmen
            //freshman
            {
                foreach (var f in details.Where(a =>  a.AppliesTo == "Freshmen" && a.Type == "Sundry"))
                {
                    InvoiceDetails sfd = new InvoiceDetails
                    {
                        Item = f.Accounts.Title,
                        Amount = f.Amount,
                        ItemCode = f.AccountCode
                    };
                    dto.Add(sfd);
                }
            }
            return dto;
        }

        InvoiceDetails AddLateRegistrationToSchedule(Session session2Pay, FeeOptions payOption, Session currentSession)
        {
            var lateRegCharge = _unitOfWork.OtherChargesRepository.GetFiltered(a => a.ProgrammeType == payOption.ProgrammeType
              && a.Description == "Late Registration").SingleOrDefault();
            //DateTime.UtcNow.Date >= currentSemester.LateRegistration1StartDate.Value.Date && currentSemester.IsCurrent == true
            if (payOption.Installment == "Entire Session" && (session2Pay.SessionId < currentSession.SessionId))
            {

                return new InvoiceDetails
                {
                    Amount = lateRegCharge.Amount * currentSession.Semesters.Count,
                    ItemCode = lateRegCharge.AccountCode,
                    Item = lateRegCharge.Description
                };
                

            }
            else if (payOption.Installment == "Entire Session" && (session2Pay.SessionId == currentSession.SessionId))
            {
                int count = 0;
                foreach (var s in currentSession.Semesters)
                {
                    if (DateTime.UtcNow.Date >= s.LateRegistrationStartDate)
                    { count = count + 1; }
                }
                if (count > 0)
                {
                    return new InvoiceDetails
                    {
                        ItemCode = lateRegCharge.AccountCode,
                        Item = lateRegCharge.Description,
                        Amount = lateRegCharge.Amount * count
                    };
                }
                else return null;
            }
            else if (payOption.Installment.Contains("1st Semester"))
            {
                var sem = session2Pay.Semesters.Where(a => a.SemesterTitle.Contains("1st Semester")).FirstOrDefault();
                if (session2Pay.SessionId < currentSession.SessionId)
                {
                    return new InvoiceDetails
                    {
                        Amount = lateRegCharge.Amount,
                        ItemCode = lateRegCharge.AccountCode,
                        Item = lateRegCharge.Description
                    };
                }
                else if (session2Pay.SessionId == currentSession.SessionId)
                {

                    var semester = currentSession.Semesters.Where(a => a.SemesterTitle.Contains("1st Semester")).FirstOrDefault();
                    if (DateTime.UtcNow.Date >= semester.LateRegistrationStartDate)
                    {
                        return new InvoiceDetails
                        {
                            Amount = lateRegCharge.Amount,
                            ItemCode = lateRegCharge.AccountCode,
                            Item = lateRegCharge.Description
                        };
                    }
                    else
                    {
                        return null;
                    }

                }
                else return null;

            }

            else if (payOption.Installment.Contains("2nd Semester"))
            {
                var sem = session2Pay.Semesters.Where(a => a.SemesterTitle.Contains("2nd Semester")).FirstOrDefault();
                if (session2Pay.SessionId < currentSession.SessionId)
                {
                    return new InvoiceDetails
                    {
                        Amount = lateRegCharge.Amount,
                        ItemCode = lateRegCharge.AccountCode,
                        Item = lateRegCharge.Description
                    };
                }
                else if (session2Pay.SessionId == currentSession.SessionId)
                {

                    var semester = currentSession.Semesters.Where(a => a.SemesterTitle.Contains("2nd Semester")).FirstOrDefault();
                    if (DateTime.UtcNow.Date >= semester.LateRegistrationStartDate)
                    {
                        return new InvoiceDetails
                        {
                            Amount = lateRegCharge.Amount,
                            ItemCode = lateRegCharge.AccountCode,
                            Item = lateRegCharge.Description
                        };
                    }
                    else
                    {
                        return null;
                    }

                }
                else return null;

            }

            else if (payOption.Installment.Contains("3rd Semester"))
            {
                var sem = session2Pay.Semesters.Where(a => a.SemesterTitle.Contains("3rd Semester")).FirstOrDefault();
                if (session2Pay.SessionId < currentSession.SessionId)
                {
                    return new InvoiceDetails
                    {
                        Amount = lateRegCharge.Amount,
                        ItemCode = lateRegCharge.AccountCode,
                        Item = lateRegCharge.Description
                    };
                }
                else if (session2Pay.SessionId == currentSession.SessionId)
                {

                    var semester = currentSession.Semesters.Where(a => a.SemesterTitle.Contains("3rd Semester")).FirstOrDefault();
                    if (DateTime.UtcNow.Date >= semester.LateRegistrationStartDate)
                    {
                        return new InvoiceDetails
                        {
                            Amount = lateRegCharge.Amount,
                            ItemCode = lateRegCharge.AccountCode,
                            Item = lateRegCharge.Description
                        };
                    }
                    else
                    {
                        return null;
                    }

                }
                else return null;

            }
            else return null;

        }
        List<InvoiceDetails> AddLevelWideSchedule(List<FeeScheduleDetail> details, int payLvl)
        {
            var lvl100 = details.Where(a => a.AppliesTo == "Level100").ToList();
            var lvl200 = details.Where(a => a.AppliesTo == "Level200").ToList();
            var lvl300 = details.Where(a => a.AppliesTo == "Level300").ToList();
            var lvl400 = details.Where(a => a.AppliesTo == "Level400").ToList();
            var lvl500 = details.Where(a => a.AppliesTo == "Level500").ToList();
            var lvl600 = details.Where(a => a.AppliesTo == "Level600").ToList();

            List<InvoiceDetails> dto = new List<InvoiceDetails>();

            switch (payLvl)
            {
                case 100:
                    if (lvl100.Count > 0)
                    {
                        foreach (var f in details.Where(a => (a.AppliesTo == "Level100")))
                        {
                            InvoiceDetails sfd = new InvoiceDetails
                            {
                                Item = f.Accounts.Title,
                                Amount = f.Amount,
                                ItemCode = f.AccountCode
                            };
                            dto.Add(sfd);
                        }
                    }
                    break;
                case 200:
                    if (lvl200.Count > 0)
                    {
                        foreach (var f in details.Where(a => (a.AppliesTo == "Level200")))
                        {
                            InvoiceDetails sfd = new InvoiceDetails
                            {
                                Item = f.Accounts.Title,
                                Amount = f.Amount,
                                ItemCode = f.AccountCode
                            };
                            dto.Add(sfd);
                        }
                    }
                    break;
                case 300:
                    if (lvl300.Count > 0)
                    {
                        foreach (var f in details.Where(a => (a.AppliesTo == "Level300")))
                        {
                            InvoiceDetails sfd = new InvoiceDetails
                            {
                                Item = f.Accounts.Title,
                                Amount = f.Amount,
                                ItemCode = f.AccountCode
                            };
                            dto.Add(sfd);
                        }
                    }
                    break;
                case 400:
                    if (lvl400.Count > 0)
                    {
                        foreach (var f in details.Where(a => (a.AppliesTo == "Level400")))
                        {
                            InvoiceDetails sfd = new InvoiceDetails
                            {
                                Item = f.Accounts.Title,
                                Amount = f.Amount,
                                ItemCode = f.AccountCode
                            };
                            dto.Add(sfd);
                        }
                    }
                    break;
                case 500:
                    if (lvl500.Count > 0)
                    {
                        foreach (var f in details.Where(a => (a.AppliesTo == "Level500")))
                        {
                            InvoiceDetails sfd = new InvoiceDetails
                            {
                                Item = f.Accounts.Title,
                                Amount = f.Amount,
                                ItemCode = f.AccountCode
                            };
                            dto.Add(sfd);
                        }
                    }
                    break;
                case 600:
                    if (lvl600.Count > 0)
                    {
                        foreach (var f in details.Where(a => (a.AppliesTo == "Level600")))
                        {
                            InvoiceDetails sfd = new InvoiceDetails
                            {
                                Item = f.Accounts.Title,
                                Amount = f.Amount,
                                ItemCode = f.AccountCode
                            };
                            dto.Add(sfd);
                        }
                    }
                    break;

            }


            return dto;
        }
        #endregion

        #region Reports
        
        public List<StudentsBalancesDTO> StudentPayments(int sessionId,string progCode)
        {
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.SessionId == sessionId && a.TransType=="Credit" && a.Student.ProgrammeCode == progCode).ToList();
            if (payments.Count == 0)
                return new List<StudentsBalancesDTO>();
            List<StudentsBalancesDTO> dto = new List<StudentsBalancesDTO>();
            foreach(StudentPayments s in payments)
            {
                StudentsBalancesDTO bal = new StudentsBalancesDTO();

                bal.Balance = s.Amount;
                bal.MatricNumber = s.Student.MatricNumber;
                bal.StudentId = s.RegNo;
                bal.Name = s.Student.Name;
                dto.Add(bal);
            }

            var final = dto.GroupBy(a => new { a.StudentId, a.MatricNumber, a.Name })
                .Select(b => new StudentsBalancesDTO
                {
                    StudentId = b.Key.StudentId,
                    MatricNumber = b.Key.MatricNumber,
                    Name = b.Key.Name,
                    Balance = b.Sum(a => a.Balance)
                });

            return final.ToList();

        }
        

        public List<StudentsBalancesDTO> StudentPayments(DateTime fromDate, DateTime toDate)
        {
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.TransType == "Credit" && (a.PayDate>=fromDate && a.PayDate<=toDate)).ToList();
            if (payments.Count == 0)
                return null;
            List<StudentsBalancesDTO> dto = new List<StudentsBalancesDTO>();
            foreach (StudentPayments s in payments)
            {
                StudentsBalancesDTO bal = new StudentsBalancesDTO();

                bal.Balance = s.Amount;
                bal.MatricNumber = s.Student.MatricNumber;
                bal.StudentId = s.RegNo;
                bal.Name = s.Student.Name;
                bal.PayDate = s.PayDate;
                dto.Add(bal);
            }        
            return dto.OrderBy(a=> a.PayDate).OrderBy(a=>a.Name).ToList();
        }

        
        #endregion
        #region GENERAL HELPERS
   
        string GeneratePaymentId(string regno, int session)
        {
            DateTime dt = Convert.ToDateTime("01/01/1900");
            string no = DateTime.Now.Subtract(dt).ToString();
            int len = no.Length-4;
            string uno = no.Substring(len);
            return session + regno + uno;
        }

        string TransactionId(string progType)
        {
            DateTime dt = Convert.ToDateTime("01/01/1900");
            string no = DateTime.Now.Subtract(dt).ToString();
            int len = no.Length - 5;
            string uno = no.Substring(len);
            var no2 = DateTime.Now.ToOADate().ToString();
            int len2 = no2.Length-6;
            string uno1 = no2.Substring(len2);
            string final = uno+uno1;
            string finalFinal="";
            switch (progType)
            {
                case "PreDegree":
                    finalFinal = "01" + final;
                    break;
                case "JUPEB":
                    finalFinal = "01" + final;
                    break;
                case "Degree":
                    finalFinal = "02" + final;
                    break;
                case "PGD":
                    finalFinal = "03" + final;
                    break;
                case "MSc":
                    finalFinal = "04" + final;
                    break;
                case "PhD":
                    finalFinal = "05" + final;
                    break;
                default:
                    finalFinal = "06" + final;
                    break;

            }
            //check if already exist in db
            var trans = _unitOfWork.PaymentInvoiceRepository.Get(finalFinal);
            if (trans == null)
            {
                return finalFinal;
            }
               
            else
                return TransactionId(progType);
        }
        
        #endregion

        #region FEE EXEMPTIONS
        public void AddFeeException(FeesExceptions exception)
        {
            
            
            _unitOfWork.FeesExceptionsRepository.Add(exception);
            _unitOfWork.Commit(exception.InputtedBy);
        }

        public List<FeeExempListDTO> StudentsForExemption(string progCode)
        {
            
            var curentSemester = _unitOfWork.SemesterRepository.GetSingle(a => a.IsCurrent == true);
            //Get Existing guys
            

            string prog = _unitOfWork.ProgrammeRepository.Get(progCode).Title;
            

            var st = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.Student.ProgrammeCode == progCode && a.Student.Status == "Active");
            List<FeeExempListDTO> fe = new List<FeeExempListDTO>();
            foreach(var s in st)
            {
                FeeExempListDTO dto = new FeeExempListDTO();
                dto.Amount = s.Amount;
                dto.Department = s.Student.Department.Title;
                dto.Programme = s.Student.Programme.Title;
                dto.Matricnumber = s.Student.MatricNumber;
                dto.Name = s.Student.Name;
                dto.StudentId = s.RegNo;
                dto.TransType = s.TransType;
                fe.Add(dto);

            }

            var final = from e in fe
                        group e by new { e.StudentId, e.Matricnumber, e.Name, e.Department, e.Programme }
                      into ne
                        select new FeeExempListDTO
                        {
                            StudentId = ne.Key.StudentId,
                            Matricnumber = ne.Key.Matricnumber,
                            Name = ne.Key.Name,
                            Department = ne.Key.Department,
                            Programme = ne.Key.Programme,
                            Amount = ne.Where(a => a.TransType == "Credit").Sum(a => a.Amount) - ne.Where(a => a.TransType == "Debit").Sum(a => a.Amount)
                        };

            var feeExemptions = _unitOfWork.FeesExceptionsRepository.GetFiltered(e => e.SemesterId == curentSemester.SemesterId && e.Programme == prog).ToList();
            if (feeExemptions.Count > 0)// Filter st
            {
                List<string> ids = new List<string>();
                foreach (var f in feeExemptions)
                {
                    ids.Add(f.StudentId);
                }
                return final.Where(a => !ids.Contains(a.StudentId)).OrderBy(a => a.Name).ToList();
            }
            else
            {
                return final.OrderBy(a => a.Name).ToList(); ;
            }
            
        }

        public void AddBulkExemptions(List<FeeExempListDTO> exemptions,string user)
        {
            List<FeesExceptions> fe = new List<FeesExceptions>();
            var currentSemester = _unitOfWork.SemesterRepository.GetSingle(a => a.IsCurrent == true);
            List<string> ids = new List<string>();
            //Extract ids
            foreach (var f in exemptions)
            {
                ids.Add(f.StudentId);
            }
            //Fetch Students Based on Ids
            var students = _unitOfWork.StudentRepository.GetFiltered(a => ids.Contains(a.PersonId));
            foreach (var e in exemptions)
            {
                var dto = new FeesExceptions
                {
                    AmountOwed = e.Amount,
                    AuthorizedBy = user,
                    IsApproved = true,
                    Department = e.Department,
                                        
                    Programme=e.Programme,
                    RegNo = e.Matricnumber,
                    StudentName = e.Name,
                    StudentId = e.StudentId,
                    SemesterId=currentSemester.SemesterId


                };
                _unitOfWork.FeesExceptionsRepository.Add(dto);
            }
            
            _unitOfWork.Commit(user);
        }
        public IEnumerable<FeesExceptions> FetchFeeExceptions(int semesterId)
        {
            
            var exceptions = _unitOfWork.FeesExceptionsRepository.GetFiltered(m => m.SemesterId == semesterId && m.IsApproved == false)
                .OrderBy(m => m.Department).OrderBy(a => a.StudentName);
            return exceptions;
        }

        public FeesExceptions StudentInException(string studentId, int semesterId)
        {
            
            var student = _unitOfWork.FeesExceptionsRepository.GetFiltered(f => f.StudentId == studentId && f.SemesterId == semesterId && f.IsApproved == true).SingleOrDefault();
            return student;
        }

        public List<FeesExceptions> ApprovedExcemptionList(List<int> exceptionIds, string approvedBy, string userId)
        {
            foreach (var s in exceptionIds)
            {
                
                var dbexception = _unitOfWork.FeesExceptionsRepository.GetFiltered(x => x.ExceptionId == s).FirstOrDefault();
                dbexception.IsApproved = true;
                dbexception.AuthorizedBy = approvedBy;
               
            }

            _unitOfWork.Commit(userId);
            return null;
        }

        public List<FeesExceptions> AllSemesterExcemptions(int semesterId)
        {
            
            var exceptions = _unitOfWork.FeesExceptionsRepository.GetFiltered(m => m.SemesterId == semesterId)
                .OrderBy(m => m.Department).OrderBy(a => a.StudentName);
            return exceptions.ToList();
        }
        #endregion

         

    }
}
