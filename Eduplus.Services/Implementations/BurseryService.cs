using Eduplos.Services.Contracts;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Eduplos.Domain.BurseryModule;
using Eduplos.DTO.BursaryModule;
using Eduplos.ObjectMappings;
using Eduplos.Domain.CoreModule;
using Eduplos.Services.UtilityServices;
using KS.Domain.AccountsModule;
using System.Data.Entity;

namespace Eduplos.Services.Implementations
{
    public class BursaryService : IBursaryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BursaryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region Other Charges Operations
        public OtherCharges FetchOtherChargesAmount(string otherCharge, string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            return _unitOfWork.OtherChargesRepository.GetSingle(a => a.Description == otherCharge && a.ProgrammeType == student.ProgrammeType);
        }
        public OtherCharges FetchApplicationFeeAmount(string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            return _unitOfWork.OtherChargesRepository.GetSingle(a => (a.Description == "Admission Fee" || a.Description == "Screening Fee"
            || a.Description == "Application Fee" || a.Description == "Form Fee") && a.ProgrammeType == student.ProgrammeType);
        }
        public Accounts FetchSingleAccount(string title)
        {
            return _unitOfWork.AccountsRepository.GetSingle(a => a.Title == title);
        }
        public List<OtherCharges> AllOtherCharges()
        {
            return _unitOfWork.OtherChargesRepository.GetAll()
                .OrderBy(a => a.AccountCode).ToList();
        }
        public List<OtherCharges> FetchStudentOtherChargesByProgramType(string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            return _unitOfWork.OtherChargesRepository.GetFiltered(a => a.ProgrammeType == student.ProgrammeType)
                .OrderBy(a => a.Description).ToList();
        }
        public string SaveOtherCharge(OtherCharges otherCharge, string userId)
        {
            if (otherCharge.ChargeId > 0)
            {
                var dbCharge = _unitOfWork.OtherChargesRepository.Get(otherCharge.ChargeId);
                dbCharge.Amount = otherCharge.Amount;

                dbCharge.ProgrammeType = otherCharge.ProgrammeType;
                _unitOfWork.Commit(userId);
                return "Operation was successfull";
            }
            else
            {
                _unitOfWork.OtherChargesRepository.Add(new OtherCharges
                {
                    Amount = otherCharge.Amount,
                    AccountCode = otherCharge.AccountCode,
                    Description = otherCharge.Description,
                    ProgrammeType = otherCharge.ProgrammeType
                });
                _unitOfWork.Commit(userId);
                return "operation was successful";
            }
        }
        /// <Fetch othercharges for transcript payment>
        /// 
        /// </summary>
        /// <returns></returns>
        public OtherCharges FetchTranscriptcharges(string deliveryArea, string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            if (deliveryArea == "Nigeria")
                return _unitOfWork.OtherChargesRepository.GetFiltered(a => a.ProgrammeType == student.ProgrammeType
                  && a.Description == "Transcript-Local").FirstOrDefault();
            else return _unitOfWork.OtherChargesRepository.GetFiltered(a => a.ProgrammeType == student.ProgrammeType
              && a.Description == "Transcript-Abroad").FirstOrDefault(); ;
        }

        #endregion

        #region FEE OPTIONS
        public List<FeeOptions> GetActiveFeeOptions()
        {
            return _unitOfWork.FeeOptionsRepository.GetFiltered(f => f.Enabled == true).ToList();
        }
        public List<FeeOptions> StudentFeeOptionsByProgType(string studentId)
        {
            var studentProg = _unitOfWork.StudentRepository.Get(studentId);
            return _unitOfWork.FeeOptionsRepository.GetFiltered(a => a.ProgrammeType == studentProg.ProgrammeType)
                .OrderBy(a => a.Installment).ToList();
        }
        public FeeOptions SaveFeeOptions(FeeOptions option, string userId)
        {
            var dbOption = _unitOfWork.FeeOptionsRepository.Get(option.Installment);
            dbOption.Enabled = option.Enabled;
            dbOption.PercentageTuition = option.PercentageTuition;
            dbOption.Register1 = option.Register1;
            dbOption.Register2 = option.Register2;
            dbOption.Write1 = option.Write1;
            dbOption.Write2 = option.Write2;
            dbOption.Write3 = option.Write3;
            dbOption.Register3 = option.Register3;
            dbOption.PercentageSundry = option.PercentageSundry;
            _unitOfWork.Commit(userId);
            return option;
        }
        #endregion
        #region FEESCHEDULE OPERATIONS

        public List<FeeOptions> GetFeeOptions()
        {
            return _unitOfWork.FeeOptionsRepository.GetAll().ToList();
        }


        public FeeScheduleDTO FetchFeeScheduleById(int scheduleId)
        {
            var schedule = _unitOfWork.FeeScheduleRepository.Get(scheduleId);
            if (schedule == null)
                return new FeeScheduleDTO();
            var dto = BursaryModuleMappings.FeeScheduleToScheduleDTO(schedule);
            return dto;
        }
        public FeeScheduleDTO FetchCurrentFeeSchedule(string facultyCode, string programmeType)
        {
            int currentSession = _unitOfWork.SessionRepository.GetFiltered(s => s.IsCurrent == true).FirstOrDefault().SessionId;
            var schedule = _unitOfWork.FeeScheduleRepository.GetFiltered(f => f.SessionId == currentSession && f.FacultyCode == facultyCode
                && f.ProgrammeType == programmeType).FirstOrDefault();

            if (schedule == null)
                return null;
            var dto = BursaryModuleMappings.FeeScheduleToScheduleDTO(schedule);
            return dto;
        }
        public List<FeeScheduleDTO> FetchFeeSchedules()
        {
            var schedule = _unitOfWork.FeeScheduleRepository.GetAll().ToList();

            List<FeeScheduleDTO> dto = new List<FeeScheduleDTO>();
            if (schedule.Count == 0)
                return new List<FeeScheduleDTO>();
            foreach (var s in schedule)
            {
                dto.Add(BursaryModuleMappings.FeeScheduleToFeeScheduleDTOSummary(s));
            }
            return dto;
        }

        public FeeScheduleDetailsDTO SaveFeeScheduleLineItem(FeeScheduleDetailsDTO schedule, out string msg, string userId)
        {
            //Check if its firstLine
            var feeSchedule = _unitOfWork.FeeScheduleRepository.Get(schedule.ScheduleId);
            /*if (feeSchedule != null && feeSchedule.Status == "Applied")//Schedule Exist, add Line
            {
                msg = "Fees already applied. Items cannot be added";
                return null;
            }*/
            if (feeSchedule != null)//Existing schedule, addline item
            {
                var chk = feeSchedule.Details.Where(a => a.AppliesTo == schedule.AppliesTo && a.Type == schedule.Type
                && a.AccountCode == schedule.AccountCode).FirstOrDefault();
                if(chk!=null)
                {
                    msg= "Item already added";
                    return null;
                }
                FeeScheduleDetail dt = new FeeScheduleDetail();
                dt.AccountCode = schedule.AccountCode;
                dt.Amount = schedule.Amount;
                dt.AppliesTo = schedule.AppliesTo;
                dt.Type = schedule.Type;
                dt.ScheduleId = feeSchedule.ScheduleId;

                feeSchedule.Details.Add(dt);
                _unitOfWork.Commit(userId);
                msg = "Ok";
                schedule.ScheduleId = feeSchedule.ScheduleId;
                schedule.FacultyCode = feeSchedule.FacultyCode;
                schedule.ProgrammeType = feeSchedule.ProgrammeType;
                schedule.ScheduleDetailId = dt.ScheduleDetailId;
                return schedule;
            }
            else //null add
            {
                FeeSchedule fs = new FeeSchedule();
                fs.FacultyCode = schedule.FacultyCode;
                fs.ProgrammeType = schedule.ProgrammeType;
                fs.SessionId = schedule.SessionId;
                fs.SetDate = DateTime.UtcNow;
                fs.Status = "Not Applied";
                fs.Total = 0;
                List<FeeScheduleDetail> fsd = new List<FeeScheduleDetail>();
                fsd.Add(new FeeScheduleDetail
                {
                    Amount = schedule.Amount,
                    AccountCode = schedule.AccountCode,
                    AppliesTo = schedule.AppliesTo,
                    Type = schedule.Type
                });
                fs.Details = fsd;
                _unitOfWork.FeeScheduleRepository.Add(fs);
                _unitOfWork.Commit(userId);
                msg = "Ok";
                schedule.ScheduleId = fs.ScheduleId;
                schedule.FacultyCode = fs.FacultyCode;
                schedule.SessionId = fs.SessionId;
                schedule.ProgrammeType = fs.ProgrammeType;
                schedule.ScheduleDetailId = fs.Details.First().ScheduleDetailId;
                return schedule;
            }

        }
        public string DeletefeeScheduleLineItem(FeeScheduleDetailsDTO schedule, string userID)
        {
            var dbschedule = _unitOfWork.FeeScheduleDetailRepository.Get(schedule.ScheduleDetailId);
            if (dbschedule == null)
                return "01";
            //if (dbschedule.FeeSchedule.Status == "Applied")
               // return "02";

            else
            {
                _unitOfWork.FeeScheduleDetailRepository.Remove(dbschedule);
                _unitOfWork.Commit(userID);
                return "00";
            }
        }

        public string ApplyFeeSchedule(int scheduleId, string userId)
        {
            //Check if students have already been debited
            /*applies to options All,Freshmen,Level100 - 900,Optional
             */
            var schedule = _unitOfWork.FeeScheduleRepository.Get(scheduleId);
            var session = _unitOfWork.SessionRepository.Get(schedule.SessionId);


            if (schedule.Status == "Applied")//already applied
                return "Schedule already applied to students";

            string facultyCode = schedule.FacultyCode;
            string programmeType = schedule.ProgrammeType;
            int sessionId = schedule.SessionId;
            //Sundry Charges
            double samountAll = schedule.Details.Where(s => s.AppliesTo == "All" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double slvl100Amount = schedule.Details.Where(s => s.AppliesTo == "Level100" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double slvl200Amount = schedule.Details.Where(s => s.AppliesTo == "Level200" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double slvl300Amount = schedule.Details.Where(s => s.AppliesTo == "Level300" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double slvl400Amount = schedule.Details.Where(s => s.AppliesTo == "Level400" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double slvl500Amount = schedule.Details.Where(s => s.AppliesTo == "Level500" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double slvl600Amount = schedule.Details.Where(s => s.AppliesTo == "Level600" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double sfreshmenAmount = schedule.Details.Where(s => s.AppliesTo == "Freshmen" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            double sreturningStudentAmount = schedule.Details.Where(s => s.AppliesTo == "Returning Students" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
            //double totalSundry = samountAll + slvl100Amount + slvl200Amount + lvl300Amount + slvl400Amount + slvl500Amount + slvl600Amount + sfreshmenAmount + sreturningStudentAmount;
            //Tuition charges
            double AllTuition = schedule.Details.Where(s => s.AppliesTo == "All" && s.Type == "Tuition").ToList().Sum(s => s.Amount);
            double freshTuition = schedule.Details.Where(s => s.AppliesTo == "Freshmen" && s.Type == "Tuition").ToList().Sum(s => s.Amount);
            double returnTuition = schedule.Details.Where(s => s.AppliesTo == "Returning Students" && s.Type == "Tuition").ToList().Sum(s => s.Amount);

            var students = _unitOfWork.StudentRepository.GetFiltered(s => s.Department.Faculty.FacultyCode == facultyCode &&
              s.ProgrammeType == programmeType && s.Status == "Active").ToList();


            foreach (var s in students)
            {
                StudentPayments sp = new StudentPayments();
                double freshmen = 0;

                if (s.YearAddmitted == session.Title)
                {
                    freshmen = sfreshmenAmount;
                }
                double nonIn = 0;
                if (s.State != "Akwa Ibom")
                {
                    var no = schedule.Details.Where(a => a.AppliesTo == "Non-Indigenes" && a.Type == "Tuition").FirstOrDefault();
                    if (no != null) { nonIn = no.Amount; }
                }
                switch (s.CurrentLevel)
                {
                    case 0:
                        if (s.EntryMode == "Direct Entry" && s.ProgrammeType == "Degree")
                        {
                            sp.Amount = samountAll + slvl200Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        }
                        else
                        {
                            sp.Amount = samountAll + slvl100Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        }
                        break;
                    case 100:
                        sp.Amount = samountAll + slvl200Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 200:
                        sp.Amount = samountAll + slvl300Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 300:
                        sp.Amount = samountAll + slvl400Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 400:
                        sp.Amount = samountAll + slvl500Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 500:
                        sp.Amount = samountAll + slvl500Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 600:
                        sp.Amount = samountAll + slvl600Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                }

                sp.PayDate = DateTime.UtcNow;
                sp.Particulars = session.Title + " Session Debit to students";
                sp.SessionId = sessionId;
                sp.RegNo = s.PersonId;
                sp.PaymentType = "School Fee";
                sp.TransType = "Debit";
                sp.PaymentId = GeneratePaymentId(s.PersonId, sessionId);

                _unitOfWork.StudentPaymentsRepository.Add(sp);
            }
            //UPdate schedule

            schedule.Status = "Applied";
            _unitOfWork.Commit(userId);
            return "Operation completed successfully";
        }

        public string ApplyFeeNewSchedule(string studentId, string userId)
        {
            //Check if students have already been debited
            /*applies to options All,Freshmen,Level100 - 900,Optional
             */
            try
            {
                var student = _unitOfWork.StudentRepository.Get(studentId);
                var session = _unitOfWork.SessionRepository.GetFiltered(a => a.Title == student.YearAddmitted).SingleOrDefault();
                var facultyId = student.Department.FacultyCode;
                var schedule = _unitOfWork.FeeScheduleRepository.GetFiltered(a => a.FacultyCode == facultyId && a.SessionId == session.SessionId).SingleOrDefault();


                string facultyCode = schedule.FacultyCode;
                string programmeType = schedule.ProgrammeType;
                int sessionId = schedule.SessionId;
                //Sundry Charges
                double samountAll = schedule.Details.Where(s => s.AppliesTo == "All" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double slvl100Amount = schedule.Details.Where(s => s.AppliesTo == "Level100" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double slvl200Amount = schedule.Details.Where(s => s.AppliesTo == "Level200" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double lvl300Amount = schedule.Details.Where(s => s.AppliesTo == "Level300" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double slvl400Amount = schedule.Details.Where(s => s.AppliesTo == "Level400" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double slvl500Amount = schedule.Details.Where(s => s.AppliesTo == "Level500" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double slvl600Amount = schedule.Details.Where(s => s.AppliesTo == "Level600" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double sfreshmenAmount = schedule.Details.Where(s => s.AppliesTo == "Freshmen" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                double sreturningStudentAmount = schedule.Details.Where(s => s.AppliesTo == "Returning Students" && s.Type == "Sundry").ToList().Sum(s => s.Amount);
                //double totalSundry = samountAll + slvl100Amount + slvl200Amount + lvl300Amount + slvl400Amount + slvl500Amount + slvl600Amount + sfreshmenAmount + sreturningStudentAmount;
                //Tuition charges
                double AllTuition = schedule.Details.Where(s => s.AppliesTo == "All" && s.Type == "Tuition").ToList().Sum(s => s.Amount);
                double freshTuition = schedule.Details.Where(s => s.AppliesTo == "Freshmen" && s.Type == "Tuition").ToList().Sum(s => s.Amount);
                double returnTuition = schedule.Details.Where(s => s.AppliesTo == "Returning Students" && s.Type == "Tuition").ToList().Sum(s => s.Amount);

                StudentPayments sp = new StudentPayments();
                double freshmen = 0;

                if (student.YearAddmitted == session.Title)
                {
                    freshmen = sfreshmenAmount;
                }
                double nonIn = 0;
                if (student.State != "Akwa Ibom")
                {
                    nonIn = schedule.Details.Where(a => a.AppliesTo == "Non-Indigenes" && a.Type == "Tuition").FirstOrDefault().Amount;
                }
                switch (student.CurrentLevel)
                {
                    case 100:
                        sp.Amount = samountAll + slvl100Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 200:
                        sp.Amount = samountAll + slvl200Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 300:
                        sp.Amount = samountAll + lvl300Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 400:
                        sp.Amount = samountAll + slvl400Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 500:
                        sp.Amount = samountAll + slvl500Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                    case 600:
                        sp.Amount = samountAll + slvl600Amount + freshmen + sreturningStudentAmount + nonIn + AllTuition + returnTuition;
                        break;
                }

                sp.PayDate = DateTime.UtcNow;
                sp.Particulars = session.Title + " Session debit to students";
                sp.SessionId = sessionId;
                sp.RegNo = student.PersonId;

                sp.TransType = "Debit";
                sp.PaymentId = GeneratePaymentId(student.PersonId, sessionId);

                _unitOfWork.StudentPaymentsRepository.Add(sp);

                //UPdate schedule

                _unitOfWork.Commit(userId);
                return "Operation completed successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }


        #endregion

        #region STUDENT FEES OPERATIONS - STUDENT

        public StudentAccountSummaryDTO StudentAccountSummary(string matricNumber)
        {
            string no = matricNumber.ToUpper();
            var student = _unitOfWork.StudentRepository.GetFiltered(s => s.MatricNumber == no || s.PersonId == matricNumber).SingleOrDefault();
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

        public StudentFeeScheduleDTO PaymentReceipt(string paymentReference)
        {
            var stpay = _unitOfWork.StudentPaymentsRepository.Get(paymentReference);
            if (stpay == null)
                return null;
            //fetch student Details
            var student = _unitOfWork.StudentRepository.Get(stpay.RegNo);

            StudentFeeScheduleDTO fs = new StudentFeeScheduleDTO();
            fs.Name = student.Name;
            fs.Programme = student.Programme.Title;
            fs.ProgrammeType = student.ProgrammeType;
            fs.Department = student.Department.Title;
            fs.MatricNumber = student.MatricNumber;
            fs.Installment = stpay.Particulars;
            fs.Session = stpay.Session.Title;
            fs.PayDate = stpay.PayDate;
            //Fetch paymentReference
            var payments = _unitOfWork.TransMasterRepository.GetFiltered(a => a.TransRef == stpay.PaymentId).ToList();
            List<StudentFeeScheduleDetailDTO> details = new List<StudentFeeScheduleDetailDTO>();
            foreach (var p in payments)
            {
                var detail = new StudentFeeScheduleDetailDTO
                {
                    Account = p.Accounts.Title,
                    Amount = p.Amount
                };
                details.Add(detail);
            }
            fs.Details = details;
            fs.Total = fs.Details.Sum(a => a.Amount);

            return fs;

        }
        public List<StudentTransactionDTO> StudentPayments(string studentId, out string balance)
        {
            List<StudentTransactionDTO> dto = new List<StudentTransactionDTO>();
            balance = "";
            var stpays = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.RegNo == studentId).ToList();
            if (stpays.Count == 0 || stpays == null)
                return new List<StudentTransactionDTO>();

            foreach (var a in stpays.Where(a => a.TransType == "Credit"))
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
            return dto.OrderBy(a => a.TransDate).ToList();
        }
        string FetchStudentAccountBalance(string studentId)
        {
            var trans = _unitOfWork.StudentPaymentsRepository.GetFiltered(s => s.RegNo == studentId).ToList();
            if (trans.Count == 0)
                return "0.00";
            double credits = trans.Where(s => s.TransType == "Credit").Sum(a => a.Amount);
            double debit = trans.Where(s => s.TransType == "Debit").Sum(a => a.Amount);
            return StandardGeneralOps.ToMoney(credits - debit);
        }
        double FetchStudentPrevSessionBalance(string studentId, int currentSessionId)
        {
            var trans = _unitOfWork.StudentPaymentsRepository.GetFiltered(s => s.RegNo == studentId && s.SessionId < currentSessionId).ToList();
            double credits = trans.Where(s => s.TransType == "Credit").Sum(a => a.Amount);
            double debit = trans.Where(s => s.TransType == "Debit").Sum(a => a.Amount);
            return credits - debit;
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
            if (payments.Count > 0)
            {
                foreach (var p in payments)
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


        public string CreditStudentAccount(TransactionDTO transaction, string studentNo, string userId)
        {
            //Credit Income Account
            StudentPayments tm = new StudentPayments();
            tm.PaymentId = GeneratePaymentId(studentNo, transaction.SessionId);
            tm.RegNo = studentNo;
            tm.Amount = transaction.Amount;
            tm.Particulars = transaction.Particulars;
            tm.PayDate = transaction.PayDate;

            tm.PayMethod = transaction.PayMethod;
            tm.TransType = "Credit";
            tm.VoucherNo = transaction.VoucherNumber;
            tm.SessionId = transaction.SessionId;
            _unitOfWork.StudentPaymentsRepository.Add(tm);

            //Credit Bank Account

            TransMaster tm1 = new TransMaster();
            tm1.AccountCode = transaction.BankAccountCode;
            tm1.Amount = transaction.Amount;

            tm1.Particulars = transaction.Particulars;
            tm1.TellerNo = transaction.VoucherNumber;
            tm1.TransDate = transaction.PayDate;

            tm1.PayMethod = transaction.PayMethod;
            tm1.TransRef = tm.PaymentId;
            tm1.TransType = "Credit";
            _unitOfWork.TransMasterRepository.Add(tm1);

            _unitOfWork.Commit(userId);

            return "Transaction added successfully";
        }

        #endregion

        #region Reports
        public List<StudentsBalancesDTO> StudentPayments(int sessionId, string progCode, string paymentType)
        {
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.SessionId == sessionId && a.TransType == "Credit" && a.Student.ProgrammeCode == progCode).ToList();
            if (payments.Count == 0)
                return new List<StudentsBalancesDTO>();
            List<StudentsBalancesDTO> dto = new List<StudentsBalancesDTO>();
            foreach (StudentPayments s in payments)
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
        public List<StudentsBalancesDTO> StudentBalancesByProgType(int sessionId, string progType)
        {
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.SessionId == sessionId && a.Student.ProgrammeType == progType

            && a.PaymentType == "School Fee").ToList();
            if (payments.Count == 0)
                return new List<StudentsBalancesDTO>();
            List<StudentsBalancesDTO> dto = new List<StudentsBalancesDTO>();
            foreach (StudentPayments s in payments)
            {
                StudentsBalancesDTO bal = new StudentsBalancesDTO();

                bal.Balance = s.Amount;
                bal.MatricNumber = s.Student.MatricNumber;
                bal.StudentId = s.RegNo;
                bal.Name = s.Student.Name;
                bal.Type = s.TransType;
                dto.Add(bal);
            }
            //balance
            var balances = dto.GroupBy(a => new { a.StudentId, a.MatricNumber, a.Name })
                .Select(b => new StudentsBalancesDTO
                {
                    StudentId = b.Key.StudentId,
                    MatricNumber = b.Key.MatricNumber,
                    Name = b.Key.Name,
                    AmountPaid = b.Where(am => am.Type == "Credit").Sum(at => at.Balance),
                    AmountOwed = b.Where(am => am.Type == "Debit").Sum(at => at.Balance),
                    Balance = b.Where(a => a.Type == "Credit").Sum(a => a.Balance) - b.Where(a => a.Type == "Debit").Sum(a => a.Balance)
                }).AsQueryable();

            return balances.OrderBy(a => a.MatricNumber).ToList();

        }
        public List<StudentsBalancesDTO> StudentBalancesByDept(int sessionId, string deptCode)
        {
            var payments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.SessionId == sessionId && a.Student.DepartmentCode == deptCode
            && a.PaymentType == "School Fee").ToList();
            if (payments.Count == 0)
                return new List<StudentsBalancesDTO>();
            List<StudentsBalancesDTO> dto = new List<StudentsBalancesDTO>();
            foreach (StudentPayments s in payments)
            {
                StudentsBalancesDTO bal = new StudentsBalancesDTO();

                bal.Balance = s.Amount;
                bal.MatricNumber = s.Student.MatricNumber;
                bal.StudentId = s.RegNo;
                bal.Name = s.Student.Name;
                bal.Type = s.TransType;
                dto.Add(bal);
            }
            //balance
            var balances = dto.GroupBy(a => new { a.StudentId, a.MatricNumber, a.Name, a.Type })
                .Select(b => new StudentsBalancesDTO
                {
                    StudentId = b.Key.StudentId,
                    MatricNumber = b.Key.MatricNumber,
                    Name = b.Key.Name,
                    AmountPaid = b.Where(am => am.Type == "Credit").Sum(at => at.Balance),
                    AmountOwed = b.Where(am => am.Type == "Debit").Sum(at => at.Balance),
                    Balance = b.Where(a => a.Type == "Credit").Sum(a => a.Balance) - b.Where(a => a.Type == "Debit").Sum(a => a.Balance)
                }).AsQueryable();

            return balances.OrderBy(a => a.MatricNumber).ToList();

        }

        public FeesCollectionDTO StudentPayments(DateTime fromDate, DateTime toDate, string accountCode, string deptCode)
        {
            var payments = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID"
            && a.ItemCode == accountCode && a.PaymentInvoice.Student.DepartmentCode == deptCode
            && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) <= toDate).ToList();
            if (payments.Count == 0)
                return null;
            FeesCollectionDTO dto = new FeesCollectionDTO();
            var dept = _unitOfWork.DepartmentRepository.Get(deptCode);
            var heading = payments.First();
            dto.FromDate = fromDate;
            dto.ToDate = toDate;
            dto.Title = heading.PaymentInvoice.Department + " " + heading.Item + " Payments";

            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            foreach (var s in payments)
            {
                details.Add(new FeesCollectionDetailsDTO
                {
                    Amount = s.Amount,
                    Name = s.PaymentInvoice.Name,
                    RegNo = s.PaymentInvoice.Regno,
                    PayDate = (DateTime)s.PaymentInvoice.CompletedDate,
                    TransRef = s.PaymentInvoice.TransactionId + "/" + s.PaymentInvoice.TransRef,
                    Particulars = s.PaymentInvoice.Particulars,
                    Programme = s.PaymentInvoice.Programme + " (" + s.PaymentInvoice.ProgrammeType + ")",
                    Level = s.PaymentInvoice.Student.CurrentLevel

                });
            }
            dto.Details = details.OrderBy(a => a.PayDate).ToList();
            dto.Total = details.Sum(a => a.Amount);
            return dto;
        }

        public FeesCollectionDTO StudentPayments(DateTime fromDate, DateTime toDate, string accountCode, string deptCode, string progType)
        {
            List<InvoiceDetails> payments;
            string dept = "All Departments";
            if (string.IsNullOrEmpty(deptCode) || deptCode == "undefined")
            {
                payments = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID"
                && a.ItemCode == accountCode && a.PaymentInvoice.ProgrammeType == progType
                 && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) <= toDate).ToList();
            }
            else
            {
                payments = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID"
                && a.ItemCode == accountCode && a.PaymentInvoice.Student.DepartmentCode == deptCode && a.PaymentInvoice.ProgrammeType == progType
                && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) <= toDate).ToList();
                dept = _unitOfWork.DepartmentRepository.Get(deptCode).Title;

            }

            if (payments.Count == 0)
                return null;
            FeesCollectionDTO dto = new FeesCollectionDTO();

            var heading = payments.First();
            dto.FromDate = fromDate;
            dto.ToDate = toDate;
            dto.Title = dept + " " + heading.Item + " Payments";

            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            foreach (var s in payments)
            {
                details.Add(new FeesCollectionDetailsDTO
                {
                    Amount = s.Amount,
                    Name = s.PaymentInvoice.Name,
                    RegNo = s.PaymentInvoice.Regno,
                    PayDate = (DateTime)s.PaymentInvoice.CompletedDate,
                    TransRef = s.PaymentInvoice.TransactionId + "/" + s.PaymentInvoice.TransRef,
                    Particulars = s.PaymentInvoice.Particulars,
                    Programme = s.PaymentInvoice.Programme + " (" + s.PaymentInvoice.ProgrammeType + ")",
                    Level = s.PaymentInvoice.Student.CurrentLevel
                });
            }
            dto.Details = details.OrderBy(a => a.PayDate).ToList();
            dto.Total = details.Sum(a => a.Amount);
            return dto;
        }

        public List<AccountsDTO> RevenueAccountsSummaryCollectionsBySession(string sessionPaid)
        {
            return _unitOfWork.InvoiceDetailsRepository.RevenueAccountsSummaryCollectionsBySession(sessionPaid);
            /*if (record.Count == 0)
                return null;
            List<AccountsDTO> dto = new List<AccountsDTO>();
            foreach(var r in record)
            {
                dto.Add(new AccountsDTO
                {
                    AccountCode = r.ItemCode,
                    Title = r.Item,
                    Amount = r.Amount
                });
            }
            return dto;*/

        }
        public FeesCollectionDTO RevenueAccountCollectionsDetails(string sessionPaid, string accountCode)
        {
            var invoice = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID" &&
            a.PaymentInvoice.Session == sessionPaid && a.ItemCode == accountCode).ToList();
            if (invoice.Count == 0)
                return null;
            FeesCollectionDTO collect = new FeesCollectionDTO();
            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            var first = invoice.First();

            collect.Title = first.Item;
            collect.Session = first.PaymentInvoice.Session;

            foreach (var a in invoice)
            {
                FeesCollectionDetailsDTO ad = new FeesCollectionDetailsDTO();
                ad.Amount = a.Amount;
                ad.Name = a.PaymentInvoice.Name;
                ad.Programme = a.PaymentInvoice.Programme + "(" + a.PaymentInvoice.ProgrammeType + ")";
                ad.RegNo = a.PaymentInvoice.Regno;
                ad.PayDate = (DateTime)a.PaymentInvoice.CompletedDate;
                details.Add(ad);
            }

            collect.Details = details.OrderBy(a => a.PayDate).ToList();
            collect.Total = collect.Details.Sum(a => a.Amount);
            return collect;

        }
        public FeesCollectionDTO RevenueAccountCollectionsDetails(DateTime fromDate, DateTime toDate, string accountCode)
        {
            var invoice = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID" &&
            (fromDate >= (DateTime)a.PaymentInvoice.CompletedDate && toDate <= (DateTime)a.PaymentInvoice.CompletedDate)
            && a.ItemCode == accountCode).ToList();
            if (invoice.Count == 0)
                return null;
            FeesCollectionDTO collect = new FeesCollectionDTO();
            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            var first = invoice.First();
            collect.FromDate = fromDate;
            collect.ToDate = toDate;
            collect.Title = first.Item;
            collect.Session = first.PaymentInvoice.Session;

            foreach (var a in invoice)
            {
                FeesCollectionDetailsDTO ad = new FeesCollectionDetailsDTO();
                ad.Amount = a.Amount;
                ad.Name = a.PaymentInvoice.Name;
                ad.Programme = a.PaymentInvoice.Programme + "(" + a.PaymentInvoice.ProgrammeType + ")";
                ad.RegNo = a.PaymentInvoice.Regno;
                ad.PayDate = (DateTime)a.PaymentInvoice.CompletedDate;
                details.Add(ad);
            }

            collect.Details = details.OrderBy(a => a.PayDate).ToList();
            collect.Total = collect.Details.Sum(a => a.Amount);
            return collect;

        }

        public FeesCollectionDTO RevenueAccountCollectionsDetailsAllFees(DateTime fromDate, DateTime toDate)
        {
            var invoice = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID" &&
            (fromDate >= (DateTime)a.PaymentInvoice.CompletedDate && toDate <= (DateTime)a.PaymentInvoice.CompletedDate)).ToList();
            if (invoice.Count == 0)
                return null;
            FeesCollectionDTO collect = new FeesCollectionDTO();
            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            var first = invoice.First();
            collect.FromDate = fromDate;
            collect.ToDate = toDate;
            collect.Title = first.Item;
            collect.Session = first.PaymentInvoice.Session;

            foreach (var a in invoice)
            {
                FeesCollectionDetailsDTO ad = new FeesCollectionDetailsDTO();
                ad.Amount = a.Amount;
                ad.Name = a.PaymentInvoice.Name;
                ad.Programme = a.PaymentInvoice.Programme + "(" + a.PaymentInvoice.ProgrammeType + ")";
                ad.RegNo = a.PaymentInvoice.Regno;
                ad.PayDate = StandardGeneralOps.ConvertUtcToLocalTimeZone((DateTime)a.PaymentInvoice.CompletedDate);
                details.Add(ad);
            }

            collect.Details = details.OrderBy(a => a.PayDate).ToList();
            collect.Total = collect.Details.Sum(a => a.Amount);
            return collect;

        }

        public DailyCollectionDTO DailyRevenueCollectionsBulk(DateTime fromDate, DateTime toDate)
        {
            var invoices = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Status == "PAID" &&
            DbFunctions.TruncateTime(a.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.CompletedDate) <= toDate).ToList();
            if (invoices.Count == 0)
                return null;
            DailyCollectionDTO collect = new DailyCollectionDTO();
            List<DailyPayHeadersDTO> headers = new List<DailyPayHeadersDTO>();

            List<PaymentInvoiceDTO> dto = new List<PaymentInvoiceDTO>();

            foreach (var v in invoices)
            {
                dto.Add(new PaymentInvoiceDTO
                {
                    Amount = v.Amount,
                    PaymentType = v.PaymentType,
                    Particulars = v.Particulars,
                    CompletedDate = StandardGeneralOps.ConvertUtcToLocalTimeZone(v.CompletedDate.Value),
                    Name = v.Name,
                    Regno = v.Regno,
                    TransRef = v.TransactionId + "/" + v.TransRef
                });
            }


            var first = invoices.First();
            collect.FromDate = fromDate;
            collect.ToDate = toDate;
            collect.Title = "Daily fees collection from " + fromDate.ToString("dd-MMM-yyyy") + "to " + toDate.ToString("dd-MMM-yyyy");

            var hrds = (from i in dto
                        group i by i.CompletedDate.Value.Date into nd
                        select new DailyPayHeadersDTO
                        {
                            Date = nd.Key,
                            Total = nd.Sum(s => s.Amount)
                        }).OrderBy(s => s.Date).ToList();

            foreach (var h in hrds)
            {
                List<DailyCollectionDetailsDTO> details = new List<DailyCollectionDetailsDTO>();

                foreach (var i in dto.Where(c => c.CompletedDate.Value.Date == h.Date))
                {
                    if (i != null) {
                        details.Add(new DailyCollectionDetailsDTO
                        {
                            Amount = i.Amount,
                            Name = i.Name,
                            Particulars = i.Particulars,
                            PayDate = i.CompletedDate.Value,
                            PayType = i.PaymentType,
                            RegNo = i.Regno,
                            TransRef = i.TransRef
                        });
                    }
                }
                h.Details = details.OrderBy(a => a.PayDate).ToList();
            }

            collect.Headers = hrds;
            collect.Total = dto.Sum(a => a.Amount);
            return collect;

        }
        public DailyCollectionDTO DailyRevenueCollectionsFull(DateTime fromDate, DateTime toDate)
        {
            var invoices = _unitOfWork.InvoiceDetailsRepository.GetFiltered(a => a.PaymentInvoice.Status == "PAID" &&
            DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) <= toDate).ToList();
            if (invoices.Count == 0)
                return null;
            DailyCollectionDTO collect = new DailyCollectionDTO();
            List<DailyPayHeadersDTO> headers = new List<DailyPayHeadersDTO>();

            List<PaymentInvoiceDTO> dto = new List<PaymentInvoiceDTO>();

            foreach (var v in invoices)
            {
                dto.Add(new PaymentInvoiceDTO
                {
                    Amount = v.Amount,
                    PaymentType = v.Item,
                    Particulars = v.PaymentInvoice.Particulars,
                    CompletedDate = v.PaymentInvoice.CompletedDate.Value,
                    Name = v.PaymentInvoice.Name,
                    Regno = v.PaymentInvoice.Regno,
                    TransRef = v.PaymentInvoice.TransactionId + "/" + v.PaymentInvoice.TransRef
                });
            }


            var first = invoices.First();
            collect.FromDate = fromDate;
            collect.ToDate = toDate;
            collect.Title = "Daily fees collection from " + fromDate.ToString("dd-MMM-yyyy") + "to " + toDate.ToString("dd-MMM-yyyy");

            var hrds = (from i in dto
                        group i by i.CompletedDate.Value.Date into nd
                        select new DailyPayHeadersDTO
                        {
                            Date = nd.Key,
                            Total = nd.Sum(s => s.Amount)
                        }).OrderBy(s => s.Date).ToList();

            foreach (var h in hrds)
            {
                List<DailyCollectionDetailsDTO> details = new List<DailyCollectionDetailsDTO>();

                foreach (var i in dto.Where(c => c.CompletedDate.Value.Date == h.Date))
                {
                    if (i != null)
                    {
                        details.Add(new DailyCollectionDetailsDTO
                        {
                            Amount = i.Amount,
                            Name = i.Name,
                            Particulars = i.Particulars,
                            PayDate = i.CompletedDate.Value,
                            PayType = i.PaymentType,
                            RegNo = i.Regno,
                            TransRef = i.TransRef
                        });
                    }
                }
                h.Details = details.OrderBy(a => a.PayDate).ToList();
            }

            collect.Headers = hrds;
            collect.Total = dto.Sum(a => a.Amount);
            return collect;

        }
        public double SessionTotalCollections(string session)
        {
            var invoices = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Session == session && a.Status == "PAID").ToList();

            return invoices.Sum(a => a.Amount);
        }
        public List<FeesCollectionDTO> SessionTotalCollectionsProgrammeType(string session)
        {
            var invoices = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Session == session && a.Status == "PAID").ToList();
            if (invoices.Count > 0)
            {
                List<FeesCollectionDTO> dto = new List<FeesCollectionDTO>();
                var inv = from v in invoices
                          group v by v.ProgrammeType into nvp
                          select new FeesCollectionDTO
                          {
                              Title = nvp.Key,
                              Total = nvp.Sum(a => a.Amount)
                          };
                return inv.OrderBy(a => a.Title).ToList();
            }

            return new List<FeesCollectionDTO>();
        }

        /*public FeesCollectionDTO FeesCollectionSummary(DateTime from, DateTime to, string progType)
        {
            var fees = _unitOfWork.InvoiceDetailsRepository
                .GetFiltered(a=>DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) >= from && DbFunctions.TruncateTime(a.PaymentInvoice.CompletedDate) <= to)
                .Select(a=> new FeesCollectionDetailsDTO
                {
                    Name = a.ItemCode,
                    Particulars = a.Item,
                    Amount = a.Amount
                }).ToList();
            if (fees.Count() == 0)
            { return null; }
            var result = (from f in fees
                         group f by new { f.Name, f.Particulars } into nf
                         select new FeesCollectionDetailsDTO
                         {
                             Name = nf.Key.Name,
                             Particulars = nf.Key.Particulars,
                             Amount = nf.Sum(a => a.Amount)
                         }).OrderBy(a=>a.Particulars);
            FeesCollectionDTO dto = new FeesCollectionDTO();
            dto.FromDate=from;
            dto.ToDate = to;
            dto.ProgrammeType = progType;
            List<FeesCollectionDetailsDTO> dtail = new List<FeesCollectionDetailsDTO>();
            foreach( var r in result)
            {
                dtail.Add(new FeesCollectionDetailsDTO
                {
                    Amount = r.Amount,
                    Name = r.Name,
                    Particulars = r.Particulars
                });
            }

            dto.Total = dtail.Sum(a => a.Amount);
            return dto;

        }*/
        public FeesCollectionDTO FeesCollectionSummary(DateTime from, DateTime to, string progType)
        {
            var fees = _unitOfWork.InvoiceDetailsRepository.FeesCollectionSummary(from, to, progType);
            if (fees.Count == 0)
                return null;
               
            FeesCollectionDTO dto = new FeesCollectionDTO();
            dto.FromDate = from;
            dto.ToDate = to;
            dto.ProgrammeType = progType;
            List<FeesCollectionDetailsDTO> dtail = new List<FeesCollectionDetailsDTO>();
            foreach (var r in fees)
            {
                dtail.Add(new FeesCollectionDetailsDTO
                {
                    Amount = r.Amount,
                    Name = r.Name,
                    Particulars = r.Particulars
                });
            }

            dto.Total = dtail.Sum(a => a.Amount);
            dto.Details = dtail.OrderBy(a=>a.Particulars).ToList();
            return dto;

        }

        public FeesCollectionDTO SchoolFeePayments(int sessionId, string deptCode)
        {
            var sesson = _unitOfWork.SessionRepository.Get(sessionId);
            var studentPayments = _unitOfWork.StudentPaymentsRepository.GetFiltered(a => a.SessionId == sessionId &&
              a.TransType == "Credit" && a.PaymentType == "School Fee" && a.Student.DepartmentCode == deptCode).ToList();
            if (studentPayments.Count == 0)
                return null;
            var dept = _unitOfWork.DepartmentRepository.Get(deptCode);
            FeesCollectionDTO dto = new FeesCollectionDTO();
            dto.Session = sesson.Title;
            dto.Department = dept.Title;
            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            foreach (var s in studentPayments)
            {

                details.Add(new FeesCollectionDetailsDTO
                {
                    Amount = s.Amount,
                    Name = s.Student.Name,
                    RegNo = s.Student.MatricNumber,
                    Programme = s.Student.Programme.Title,
                    TransRef = s.ReferenceCode,

                    Level = s.Student.CurrentLevel
                });

            }
            dto.Details = details;
            dto.Total = details.Sum(a => a.Amount);
            return dto;
        }

        public FeesCollectionDTO SchoolFeePayments(int sessionId, string deptCode, string progType)
        {
            var sesson = _unitOfWork.SessionRepository.Get(sessionId);
            var studentPayments = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Session == sesson.Title &&
              a.Status == "PAID" && a.PaymentType == "School Fee" && a.ProgrammeType == progType
              && a.Student.DepartmentCode == deptCode).ToList();
            if (studentPayments.Count == 0)
                return null;
            var dept = _unitOfWork.DepartmentRepository.Get(deptCode);
            FeesCollectionDTO dto = new FeesCollectionDTO();
            dto.Session = sesson.Title;
            dto.Department = dept.Title;
            dto.ProgrammeType = progType;
            dto.Title = sesson.Title + " school Fee payments";
            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            foreach (var s in studentPayments)
            {

                details.Add(new FeesCollectionDetailsDTO
                {
                    Amount = s.Amount,
                    Name = s.Name,
                    RegNo = s.Student.MatricNumber,
                    Programme = s.Programme,
                    TransRef = s.TransactionId + "/" + s.TransRef,
                    Particulars = s.Particulars,
                    Level = s.Student.CurrentLevel
                });

            }
            dto.Details = details;
            dto.Total = details.Sum(a => a.Amount);
            return dto;
        }
        public FeesCollectionDTO SchoolFeePayments(DateTime fromDate, DateTime toDate, string deptCode, string progType)
        {
            List<PaymentInvoice> studentPayments;
            string dept = "All";
            if (string.IsNullOrEmpty(deptCode) || deptCode == "undefined")
            {
                studentPayments = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Status == "PAID" && a.PaymentType == "School Fee"
                && a.ProgrammeType == progType
                && DbFunctions.TruncateTime(a.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.CompletedDate) <= toDate).ToList();

            }
            else
            {
                studentPayments = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Status == "PAID" && a.PaymentType == "School Fee"
                && a.Student.DepartmentCode == deptCode && a.ProgrammeType == progType
                && DbFunctions.TruncateTime(a.CompletedDate) >= fromDate && DbFunctions.TruncateTime(a.CompletedDate) <= toDate).ToList();
                dept = _unitOfWork.DepartmentRepository.Get(deptCode).Title;

            }


            if (studentPayments.Count == 0)
                return null;

            FeesCollectionDTO dto = new FeesCollectionDTO();
            dto.FromDate = fromDate;
            dto.ToDate = toDate;
            dto.ProgrammeType = progType;

            dto.Title = dept + " department school fee payments";
            List<FeesCollectionDetailsDTO> details = new List<FeesCollectionDetailsDTO>();
            foreach (var s in studentPayments)
            {

                details.Add(new FeesCollectionDetailsDTO
                {
                    Amount = s.Amount,
                    Name = s.Name,
                    RegNo = s.Student.MatricNumber,
                    Programme = s.Programme,
                    TransRef = s.TransactionId + "/" + s.TransRef,
                    Particulars = s.Particulars,
                    Level = s.Student.CurrentLevel,
                    PayDate = (DateTime)s.CompletedDate
                });

            }
            dto.Details = details.OrderBy(a=>a.PayDate).ToList();
            dto.Total = details.Sum(a => a.Amount);
            return dto;
        }
        public List<PaymentInvoiceDTO> AllPendingTransactions()
        {
            var invoices = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => a.Status == "Pending").ToList();
            List<PaymentInvoiceDTO> dtc = new List<PaymentInvoiceDTO>();
            if (invoices.Count > 0)
            {
                foreach (var i in invoices)
                {
                    dtc.Add(new PaymentInvoiceDTO
                    {
                        TransactionId = i.TransactionId,
                        Amount = i.Amount,
                        Particulars = i.Particulars,
                        GeneratedDate = i.GeneratedDate,
                        TransRef = i.TransRef,
                        Name = i.Name,
                        Status = i.Status
                    });
                }
                return dtc;
            }
            return new List<PaymentInvoiceDTO>();
        }
        //Daily Collections
        public List<PaymentTypesCollectionSummaryDTO> DailyPaymentsByPayType(DateTime date)
        {
            var collections = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => DbFunctions.TruncateTime(a.CompletedDate) == date.Date)
                .ToList();
            if (collections.Count == 0)
                return new List<PaymentTypesCollectionSummaryDTO>();
            var nCollections = (from c in collections
                                group c by c.PaymentType into nc
                                select new PaymentTypesCollectionSummaryDTO
                                {
                                    PaymentType = nc.Key,
                                    Amount = nc.Sum(a => a.Amount)
                                }).ToList();
            List<PaymentTypesCollectionSummaryDTO> dto = new List<PaymentTypesCollectionSummaryDTO>();
            foreach(var f in nCollections)
            {
                dto.Add(new PaymentTypesCollectionSummaryDTO
                {
                    PaymentType = f.PaymentType,
                    Amount = f.Amount
                });
            }

            return dto.OrderBy(a => a.PaymentType).ToList();
        }
        public List<PaymentTypesCollectionSummaryDTO> DailyPaymentsByProgType(DateTime date)
        {
            var collections = _unitOfWork.PaymentInvoiceRepository.GetFiltered(a => DbFunctions.TruncateTime(a.CompletedDate) == date.Date)
                .ToList();
            if (collections.Count == 0)
                return new List<PaymentTypesCollectionSummaryDTO>();
            var nCollections = (from c in collections
                                group c by c.ProgrammeType into nc
                                select new PaymentTypesCollectionSummaryDTO
                                {
                                    ProgrammeType = nc.Key,
                                    Amount = nc.Sum(a => a.Amount)
                                }).ToList();
            List<PaymentTypesCollectionSummaryDTO> dto = new List<PaymentTypesCollectionSummaryDTO>();
            foreach (var f in nCollections)
            {
                dto.Add(new PaymentTypesCollectionSummaryDTO
                {
                    ProgrammeType = f.ProgrammeType,
                    Amount = f.Amount
                });
            }

            return dto.OrderBy(a => a.ProgrammeType).ToList();
        }
        #endregion
        
        #region GENERAL HELPERS


        bool CheckIfScheduleExist(int sessionId, string facultyCode, string programmeType)
        {
            var schedule = _unitOfWork.FeeScheduleRepository.GetFiltered(f => f.SessionId == sessionId && f.FacultyCode == facultyCode
              && f.ProgrammeType == programmeType).FirstOrDefault();
            if (schedule == null)
                return false;
            else
                return true;
        }
        
        string GeneratePaymentId(string regno, int session)
        {
            DateTime dt = Convert.ToDateTime("01/01/1900");
            string no = DateTime.Now.Subtract(dt).ToString();
            int len = no.Length-4;
            string uno = no.Substring(len);
            return session + regno + uno;
        }
        Session FetchCurrentSession()
        {
            var session = _unitOfWork.SessionRepository.GetSingle(a => a.IsCurrent == true);
            return session;
        }

        

        
        void SaveMainTransaction(TransactionDTO trans,string transref)
        {
            TransMaster tm = new TransMaster();
            tm.AccountCode = trans.AccountCode;
            tm.Amount = trans.Amount;
            tm.TransType = trans.TransType;
            tm.TransDate = trans.PayDate;
             
            tm.Particulars = trans.Particulars;
            
            tm.TransRef = transref;

            _unitOfWork.TransMasterRepository.Add(tm);
        }
        #endregion

        #region FEE PAYMENT HELPERS 
         
        public bool CheckForDoublePosting(TransactionDTO trans)
        {
            var studentpays = _unitOfWork.StudentPaymentsRepository.GetFiltered(s => s.Amount == trans.Amount && s.RegNo == trans.StudentId
              && s.SessionId == trans.SessionId && s.VoucherNo == trans.VoucherNumber ).SingleOrDefault();
            if (studentpays == null)
                return false;
            else
                return true;
        }
        #endregion

    }
}
