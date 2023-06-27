using Eduplus.Domain.CoreModule;
using System;
using Eduplus.Services.Contracts;
using KS.Core;
using KS.Core.UserManagement;
using KS.Services.Contract;
using System.Linq;
using Eduplus.DTO.CoreModule;
using Eduplus.ObjectMappings;
using Eduplus.DTO.AcademicModule;
using System.Collections.Generic;
using Eduplus.Domain.AcademicModule;
using Eduplus.Services.UtilityServices;
using Eduplus.Domain.BurseryModule;

namespace Eduplus.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public StudentService(IUnitOfWork unitOfWork, IUserService userService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public int CreateNewAlumus(StudentDTO st, out string studentId)
        {
            //check for existing Jamb No
            var dbStudent = _unitOfWork.StudentRepository.GetSingle(a => a.MatricNumber == st.MatricNumber.Trim());

            Student student = new Student();

            if (dbStudent == null)//does not exists, Add
            {
                var session = _unitOfWork.SessionRepository.GetSingle(s => s.Title == st.YearAdmitted);
                var prog = _unitOfWork.ProgrammeRepository.GetSingle(a => a.ProgrammeCode == st.ProgrammeCode);

                student.Surname = StandardGeneralOps.ToTitleCase(st.Surname);
                student.Firstname = StandardGeneralOps.ToTitleCase(st.Firstname);
                student.MIddlename = StandardGeneralOps.ToTitleCase(st.Middlename);
                student.Email = st.Email.ToLower();
                student.Phone = st.Phone;
                student.PersonId = StandardGeneralOps.GeneratePersonId(session.SessionId);
                student.MatricNumber = st.MatricNumber.Trim().ToUpper();
                student.AddmissionCompleteStage = 0;
                student.YearAddmitted = st.YearAdmitted;
                student.GradYear = st.GradYear;
                student.StudyMode = st.StudyMode;
                student.MaritalStatus = st.MaritalStatus;
                student.BDay = st.BDay;
                student.BMonth = st.BMonth;
                student.BYear = st.BYear;
                student.Duration = st.Duration;
                student.EntryMode = st.EntryMode;
                student.ProgrammeType = st.ProgrammeType;
                student.ProgrammeCode = prog.ProgrammeCode;
                student.DepartmentCode = prog.DepartmentCode;
                student.Lg = st.Lg;
                student.State = st.State;
                student.Country = st.Country;
                student.Title = st.Title;
                student.Sex = st.Sex;
                student.BaseCGPA = 0;
                student.Status = "Graduated";
                student.Referee = st.Referee;
                student.RefereeAddress = st.RefereeAddress;
                student.RefereeMail = st.RefereeMail;
                student.RefereePhone = st.RefereePhone;
                student.kinAddress = st.KinAddress;
                student.KinMail = st.KinMail;
                student.KinPhone = st.KinPhone;
                student.Relationship = st.Relationship;
                student.ResidentialAddress = st.ResidentialAddress;
                student.GradBatch = st.GradBatch;
                _unitOfWork.StudentRepository.Add(student);

                //Add Student to User

                User user = new User();

                user.UserName = student.MatricNumber;
                user.UserCode = student.PersonId;
                user.FullName = student.Name;
                user.DepartmentCode = prog.DepartmentCode;
                user.ProgrammeCode = prog.ProgrammeCode;
                user.Password = PasswordMaker.HashPassword(st.Password);
                user.IsActive = true;
                user.LastActivityDate = DateTime.UtcNow;
                user.CreateDate = DateTime.UtcNow;
                user.UserId = student.PersonId;
                user.UserCode = student.PersonId;
                user.Email = student.Email;
                var role = _unitOfWork.RoleRepository.GetSingle(r => r.RoleName == "Alumnus");

                user.UserRoles.Add(role);

                _unitOfWork.UserRepository.Add(user);
                _unitOfWork.Commit(student.PersonId);
                studentId = student.PersonId;
                return 1;
            }
            else
            {
                studentId = "";
                var user = _unitOfWork.UserRepository.GetFiltered(a => a.UserName == dbStudent.MatricNumber || a.UserId == dbStudent.PersonId).FirstOrDefault();

                dbStudent.Email = st.Email;
                dbStudent.Phone = st.Phone;
                if(user!=null)
                {
                    var roles = _unitOfWork.RoleRepository.GetFiltered(a => a.RoleName == "Student" || a.RoleName == "Alumnus").ToList();

                    user.UserRoles.Remove(roles.Where(a => a.RoleName == "Student").SingleOrDefault());
                    user.UserRoles.Add(roles.Where(a => a.RoleName == "Alumnus").SingleOrDefault());
                    user.UserName = dbStudent.MatricNumber;
                    user.IsActive = true;
                    user.Password = PasswordMaker.HashPassword(st.Password);
                    user.Email = dbStudent.Email;
                    user.LastActivityDate = DateTime.UtcNow;
                    _unitOfWork.Commit(dbStudent.PersonId);
                }
                else
                {
                    User user1 = new User();

                    user1.UserName = dbStudent.MatricNumber;
                    user1.UserCode = dbStudent.PersonId;
                    user1.FullName = dbStudent.Name;
                     
                    user1.Password = PasswordMaker.HashPassword(st.Password);
                    user1.IsActive = true;
                    user1.LastActivityDate = DateTime.UtcNow;
                    user1.CreateDate = DateTime.UtcNow;
                    user1.UserId = dbStudent.PersonId;
                    user1.Email = dbStudent.Email;
                    var role = _unitOfWork.RoleRepository.GetSingle(r => r.RoleName == "Alumnus");

                    user1.UserRoles.Add(role);

                    _unitOfWork.UserRepository.Add(user1);
                    _unitOfWork.Commit(dbStudent.PersonId);
                }
                
                return 1;
            }

        }

        #region Manual student admission
        public string NewStudent(Student student, int sessionId, string userId)
        {
            //try
           // {
                string studentId;
                studentId = StandardGeneralOps.GeneratePersonId(sessionId);
                student.PersonId = studentId;
                student.AddmissionCompleteStage = 7;
                _unitOfWork.StudentRepository.Add(student);
                //ad student to user
                User user = new User();
                user.UserId = studentId;
                user.UserCode = studentId;
                user.FullName = student.Name;
                user.Email = student.Email;
                user.UserName = student.Email;
                user.IsActive = true;
                user.CreateDate = DateTime.UtcNow;
                user.CreatedBy = userId;
                user.LastActivityDate = DateTime.UtcNow;
                user.LoginCounter = 0;
                user.ProgrammeCode = student.ProgrammeCode;
                user.DepartmentCode = student.DepartmentCode;
                user.Password = student.Surname.ToLower();

                var role = _unitOfWork.RoleRepository.GetSingle(a => a.RoleName == "Student");
                user.UserRoles.Add(role);
                _unitOfWork.UserRepository.Add(user);


            //debit student account
            var depart = _unitOfWork.DepartmentRepository.Get(student.DepartmentCode);
                var feeschedule = _unitOfWork.FeeScheduleRepository.GetFiltered(f => f.FacultyCode == depart.FacultyCode &&
                  f.SessionId == sessionId).FirstOrDefault();
                double amount = feeschedule.Details.Where(a => (a.AppliesTo == "Freshmen" && a.Type == "Tuition")
                || (a.Type == "Sundry" && a.AppliesTo == "All") || (a.Type == "Sundry" && a.AppliesTo == "Freshmen")).Sum(a => a.Amount);

                StudentPayments sp = new StudentPayments
                {
                    Amount = amount,

                    PayDate = DateTime.UtcNow,
                    Particulars = "Session debit to students",
                    SessionId = sessionId,
                    RegNo = studentId,
                    TransType = "Debit",
                    PaymentId = GeneratePaymentId2(studentId, sessionId)

                };
                _unitOfWork.StudentPaymentsRepository.Add(sp);
                _unitOfWork.Commit(userId);
                return "Stuent successfully added";
            }
            //catch(Exception ex)
           // {
            //    return ex.Message;
           // }
        
        string GeneratePaymentId2(string regno, int session)
        {
            DateTime dt = Convert.ToDateTime("01/01/1900");
            string no = DateTime.Now.Subtract(dt).ToString();
            int len = no.Length - 4;
            string uno = no.Substring(len);
            return session + regno + uno;
        }
        #endregion
        #region FRESH APPLICANT SUBMISSIONS


        public int CreateNewStudentProfile(ProspectiveStudentDTO st,out string studentId)
        {
            //check for existing Jamb No
            var mail = _unitOfWork.StudentRepository.GetSingle(a => a.Email == st.Email && a.Phone==st.Phone);
            
            Student student = new Student();
            
            if (mail==null)//does not exists, Add
            {
               
                var session = _unitOfWork.SessionRepository.GetFiltered(a=>a.IsAdmissionSession==true).FirstOrDefault();
                var prog = _unitOfWork.ProgrammeRepository.GetSingle(a => a.ProgrammeCode == st.ProgrammeCode);
                var stProg = _unitOfWork.ProgrammeTypeRepository.Get(prog.ProgrammeType);

                student.Surname = StandardGeneralOps.ToTitleCase(st.Surname);
                student.Firstname = StandardGeneralOps.ToTitleCase(st.Firstname);
                student.MIddlename = StandardGeneralOps.ToTitleCase(st.MIddlename);
                student.Email = st.Email.ToLower();
                student.Phone = st.Phone;
                student.PersonId = StandardGeneralOps.GeneratePersonId(session.SessionId);
                if (stProg.AcceptAdmissionFee==true)
                {
                    student.AddmissionCompleteStage = 0;
                }
                else
                {
                    student.AddmissionCompleteStage = 1;
                }
                
                student.YearAddmitted = session.Title;
                student.ProgrammeType = st.ProgramType;
                student.ProgrammeCode = prog.ProgrammeCode;
                student.DepartmentCode = prog.DepartmentCode;
                student.BaseCGPA = 0;
                student.Status = "Prospective";
                student.CurrentLevel = 0;

                _unitOfWork.StudentRepository.Add(student);

                //Add Student to User
                
                User user = new User();

                user.UserName = student.Email;
                user.UserCode = student.PersonId;
                user.FullName = student.Name;
                user.DepartmentCode = prog.DepartmentCode;
                user.ProgrammeCode = prog.ProgrammeCode;
                user.Password = PasswordMaker.HashPassword(st.Password);
                user.IsActive = true;
                user.LastActivityDate = DateTime.UtcNow;
                user.CreateDate = DateTime.UtcNow;
                user.UserId = student.PersonId;
                user.Email = student.Email;
                var role = _unitOfWork.RoleRepository.GetSingle(r => r.RoleName == "Prospective");
                
                user.UserRoles.Add(role);

                _unitOfWork.UserRepository.Add(user);
                _unitOfWork.Commit(student.PersonId);
                studentId = student.PersonId;
                return 1;
            }
            else
            {
                studentId = "";
                return 0;
            }
            
        }
        

        #endregion
        public string Step1Submission(StudentDTO dto, string userId,out string flag)
        {
            var dbStudent = _unitOfWork.StudentRepository.Get(dto.StudentId);
            //check for existing Jamb No
            
            
            Student nstudent = new Student();
            

            if (string.IsNullOrEmpty(dto.StudentId))//New Student add
            {
                //check for email n matric
                var st = _unitOfWork.StudentRepository.GetFiltered(a => a.MatricNumber == dto.MatricNumber||a.Email==dto.Email).FirstOrDefault();
                if(st!=null)
                {
                   return flag = "01";
                     
                }
                //check for jambregno
                var jamb = _unitOfWork.JambResultRepository.Get(dto.JambRegNumber);
                if (jamb != null) {
                    return flag="02";
                }

                var ses = _unitOfWork.SessionRepository.GetFiltered(a => a.Title == dto.YearAdmitted).SingleOrDefault();
                Student student = new Student();
                student.Surname = StandardGeneralOps.ToTitleCase(dto.Surname);
                student.Firstname = StandardGeneralOps.ToTitleCase(dto.Firstname);
                student.MIddlename = StandardGeneralOps.ToTitleCase(dto.Middlename);
                student.ResidentialAddress = StandardGeneralOps.ToTitleCase(dto.ResidentialAddress);
                student.MatricNumber = dto.MatricNumber;
                student.BDay = dto.BDay;
                student.BMonth = dto.BMonth;
                student.BYear = dto.BYear;
                student.Phone = dto.Phone;
                student.ResidentialAddress = dto.ResidentialAddress;
                student.Email = dto.Email.ToLower();
                student.EntryMode = dto.EntryMode;
                student.CurrentLevel = dto.CurrentLevel;
                student.Duration = dto.Duration;
                student.ProgrammeCode = dto.ProgrammeCode;
                student.DepartmentCode = dto.DepartmentCode;
                student.ProgrammeType = dto.ProgrammeType;
                student.StudyMode = dto.StudyMode;
                student.kinAddress = StandardGeneralOps.ToTitleCase(dto.KinAddress);
                if (!string.IsNullOrEmpty(dto.KinMail))
                {
                    student.KinMail = dto.KinMail.ToLower();
                }

                student.KinPhone = dto.KinPhone;
                student.NextKin = dto.NextKin;
                student.MailingAddress = StandardGeneralOps.ToTitleCase(dto.MailingAddress);
                student.MaritalStatus = dto.MaritalStatus;
                student.Title = dto.Title;
                student.Country = dto.Country;
                student.State = dto.State;
                student.Lg = dto.Lg;
                student.Sex = dto.Sex;
                student.YearAddmitted = dto.YearAdmitted;
                student.Status = dto.Status;
                student.AdmissionStatus = "Admitted";
                //sponsor
                student.Referee = dto.Referee;
                student.RefereeAddress = StandardGeneralOps.ToTitleCase(dto.RefereeAddress);
                student.RefereePhone = dto.RefereePhone;
                student.RefereeMail = dto.RefereeMail.ToLower();
                student.BaseCGPA = 0;
                    student.ProgrammeCode = dto.ProgrammeCode;

                    student.DepartmentCode = dto.DepartmentCode;
                
                    student.ProgrammeType = dto.ProgrammeType;
                student.PersonId = StandardGeneralOps.GeneratePersonId(ses.SessionId);
                student.IsHandicapped = dto.IsHandicapped;
                student.AddmissionCompleteStage = 2;
                _unitOfWork.StudentRepository.Add(student);
                Role rl = null;
                switch(dto.Status)
                {
                    case "Active":
                        rl= _unitOfWork.RoleRepository.GetFiltered(a => a.RoleName == "Student").SingleOrDefault();
                        break;
                    case "Alumni":
                        rl= _unitOfWork.RoleRepository.GetFiltered(a => a.RoleName == "alumni").SingleOrDefault();
                        break;
                }

                
                User nuser= new User();
                nuser.FullName = student.Name;
                nuser.Email = student.Email;
                nuser.DepartmentCode = student.DepartmentCode;
                nuser.ProgrammeCode = student.ProgrammeCode;
                nuser.CreateDate = DateTime.UtcNow;
                nuser.CreatedBy = userId;
                nuser.LastActivityDate = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(dto.MatricNumber))
                {
                    nuser.UserName = dto.MatricNumber;
                    nuser.Password = PasswordMaker.HashPassword(nuser.UserName);
                }
                nuser.UserRoles.Add(rl);
                 
                
                nuser.UserCode = student.PersonId;
                nuser.UserId = student.PersonId;
                nuser.LoginCounter = 0;
                nuser.IsActive = true;
                _unitOfWork.UserRepository.Add(nuser);
                _unitOfWork.Commit(userId);
                flag = "00";
                return student.PersonId;
            }
            else
            {
                var dbUser = _unitOfWork.UserRepository.Get(dto.StudentId);

                dbStudent.Surname = StandardGeneralOps.ToTitleCase(dto.Surname);
                dbStudent.Firstname = StandardGeneralOps.ToTitleCase(dto.Firstname);
                dbStudent.MIddlename = StandardGeneralOps.ToTitleCase(dto.Middlename);
                dbStudent.ResidentialAddress = StandardGeneralOps.ToTitleCase(dto.ResidentialAddress);
                
                dbStudent.BDay = dto.BDay;
                dbStudent.BMonth = dto.BMonth;
                dbStudent.BYear = dto.BYear;
                dbStudent.Phone = dto.Phone;
                dbStudent.ResidentialAddress = dto.ResidentialAddress;
                dbStudent.Email = dto.Email.ToLower();
                dbStudent.EntryMode = dto.EntryMode;
                dbStudent.CurrentLevel = dto.CurrentLevel;
                dbStudent.Duration = dto.Duration;

                var jambResults = _unitOfWork.JambResultRepository.Get(dto.JambRegNumber);
                if (jambResults==null && !string.IsNullOrEmpty(dto.JambRegNumber) && dto.JambYear>0)
                {
                    _unitOfWork.JambResultRepository.Add(new JambResult
                    {
                        JambRegNumber = dto.JambRegNumber,
                        JambYear = dto.JambYear,
                        StudentId=dto.StudentId,

                    });
                }
                
                dbStudent.StudyMode = dto.StudyMode;
                dbStudent.kinAddress = StandardGeneralOps.ToTitleCase(dto.KinAddress);
                if (!string.IsNullOrEmpty(dto.KinMail))
                {
                    dbStudent.KinMail = dto.KinMail.ToLower();
                }

                dbStudent.KinPhone = dto.KinPhone;
                dbStudent.NextKin = dto.NextKin;
                dbStudent.MailingAddress = StandardGeneralOps.ToTitleCase(dto.MailingAddress);
                dbStudent.MaritalStatus = dto.MaritalStatus;
                dbStudent.Title = dto.Title;
                dbStudent.Country = dto.Country;
                dbStudent.State = dto.State;
                dbStudent.Lg = dto.Lg;
                dbStudent.Sex = dto.Sex;
                if (!string.IsNullOrEmpty(dto.YearAdmitted))
                {
                    dbStudent.YearAddmitted = dto.YearAdmitted;
                }
                
                //sponsor
                dbStudent.Referee = dto.Referee;
                dbStudent.RefereeAddress = StandardGeneralOps.ToTitleCase(dto.RefereeAddress);
                dbStudent.RefereePhone = dto.RefereePhone;
                dbStudent.RefereeMail = dto.RefereeMail.ToLower();
                dbStudent.BaseCGPA = 0;
                if(dbStudent.ProgrammeCode!=dto.ProgrammeCode && !string.IsNullOrEmpty(dto.ProgrammeCode))
                {
                    dbStudent.ProgrammeCode = dto.ProgrammeCode;
                    dbStudent.DepartmentCode = dto.DepartmentCode;
                    
                    //User
                    dbUser.DepartmentCode = dto.DepartmentCode;
                    dbUser.ProgrammeCode = dto.ProgrammeCode;
                    
                }
                if(dbStudent.MatricNumber!=dto.MatricNumber&&!string.IsNullOrEmpty(dto.MatricNumber))
                {
                    dbStudent.MatricNumber = dto.MatricNumber;
                    dbUser.UserName = dto.MatricNumber;
                }
                 if(dbStudent.Name!=(dto.Surname+", "+dto.Firstname+" "+dto.Middlename))
                {
                    dbUser.FullName = dto.Surname + ", " + dto.Firstname + " " + dto.Middlename;
                }
                 if(dbStudent.ProgrammeType != dto.ProgrammeType && !string.IsNullOrEmpty(dto.ProgrammeType))
                {
                    dbStudent.ProgrammeType = dto.ProgrammeType;
                }
                
                dbStudent.IsHandicapped = dto.IsHandicapped;

                if(dbStudent.AddmissionCompleteStage<2)
                {
                    dbStudent.AddmissionCompleteStage = 2;
                }
                        
                _unitOfWork.Commit(userId);
                flag = "00";
                return dto.StudentId;
            }            
        }
        

        #region PRE-ADMISSION OPERATIONS

        public Applicants FetchApplicant(string jambNo)
        {
            var st = _unitOfWork.ApplicantsRepository.GetSingle(a => a.JambNo == jambNo);
            if (st == null)
                return new Applicants();
            else
                return st;
        }

         
        public string SubmitPassport(AppImages passport,string studentId)
        {
                          
                passport.IncludeInSlide = false;
                passport.InsertDate = DateTime.UtcNow;
            var appImage = _unitOfWork.AppImagesRepository.Get(studentId);
            if(appImage!=null)//UPdate
            {
                appImage.Foto = passport.Foto;
                _unitOfWork.Commit(studentId);
                return "Passport successfully uploaded";
            }
            passport.ImageId = studentId;
                _unitOfWork.AppImagesRepository.Add(passport);
                _unitOfWork.Commit(studentId);

                var student = _unitOfWork.StudentRepository.Get(studentId);
            var progt = _unitOfWork.ProgrammeTypeRepository.GetFiltered(a => a.Type == student.ProgrammeType).FirstOrDefault();
            if(student.AddmissionCompleteStage<3)
            {
                student.AddmissionCompleteStage = 3;
            }
                student.PhotoId = passport.ImageId;
                
                _unitOfWork.Commit(studentId);
                
                return "Passport successfully uploaded";
           
        }

        public OlevelResultDTO FetchOlevelResults(string studentId,byte sit)
        {
            var result = _unitOfWork.OLevelResultRepository.GetFiltered(r => r.StudentId == studentId
            && r.SitAttempt==sit).SingleOrDefault();
            if (result== null)
                return null;
            OlevelResultDTO dto = new OlevelResultDTO();
             
                dto.ExamNumber = result.ExamNumber;
                dto.ExamType = result.ExamType;
                dto.ResultId = result.ResultId;
                dto.SitAttempt = result.SitAttempt;
                dto.StudentId = result.StudentId;
                dto.Venue = result.Venue;
                dto.Year = result.ExamYear;

                List<OlevelResultDetailDTO> details = new List<OlevelResultDetailDTO>();
                if (result.OlevelResultDetail.Count() > 0)
                {
                    foreach (var d in result.OlevelResultDetail.ToList())
                    {
                        details.Add(new OlevelResultDetailDTO
                        {
                            DetailId = d.DetailId,
                            Grade = d.Grade,
                            ResultId = d.ResultId,
                            Subject = d.Subject
                        });
                    }
                    dto.Details= details;
                }
           return dto;
        }
        public string DeleteOlevelResult(OlevelResultDetailDTO item,string userid)
        {
            var result = _unitOfWork.OLevelResultDetailRepository.Get(item.DetailId);
            if(result!=null)
            {
                
                _unitOfWork.OLevelResultDetailRepository.Remove(result);
                
                _unitOfWork.Commit(userid);
                return "Subject successfully removed";
            }
            return "Invalid result Id";
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userId"></param>
        /// <param name="flag"> 0=Olevel Ressult added successfully added Ok; 1=olevel examNo Regno already exist</param>
        /// <returns></returns>
        public OlevelResultDetailDTO AddOlevelResult(OlevelResultDetailDTO dto, string userId, out int flag)
        {
            var res = _unitOfWork.OLevelResultRepository.Get(dto.ResultId);
            flag = 0;
            if (res == null)//new entry
            {
                OLevelResult or = new OLevelResult();
                or.ExamNumber = dto.ExamNumber;
                or.ExamType = dto.ExamType;
                or.StudentId = dto.StudentId;
                or.Venue = StandardGeneralOps.ToTitleCase(dto.Venue);
                or.ExamYear = dto.ExamYear;
                or.SitAttempt = dto.SitAttempt;
                or.ResultId = dto.StudentId +"-"+ dto.SitAttempt;
                
                 
                    or.OlevelResultDetail.Add(new OlevelResultDetail
                    {
                        Grade = dto.Grade,
                        Subject = dto.Subject
                    });

                var student = _unitOfWork.StudentRepository.Get(dto.StudentId);
                if(student.AddmissionCompleteStage<4)
                { student.AddmissionCompleteStage = 4; }
                 _unitOfWork.OLevelResultRepository.Add(or);
                _unitOfWork.Commit(userId);

                dto.ResultId = or.ResultId;
                
                return dto;

            }
           else
            {
                if(res.StudentId!=dto.StudentId)
                {
                    flag = 1;
                    return null;
                }
                var dt= new OlevelResultDetail
                {
                    ResultId = dto.ResultId,
                    Subject = dto.Subject,
                    Grade = dto.Grade
                };
                _unitOfWork.OLevelResultDetailRepository.Add(dt);
                _unitOfWork.Commit(userId);
                dto.DetailId = dt.DetailId;
                
                return dto;
            }
            


        }
        public JambDTO GetStudentJambReg(string studentId)
        {
            var result = _unitOfWork.JambResultRepository.GetFiltered(a => a.StudentId == studentId).SingleOrDefault();
            JambDTO dto = new JambDTO();
            if (result != null)
            {
                dto.StudentId = studentId;
                dto.JambRegNumber = result.JambRegNumber;
                dto.JambYear = result.JambYear;
                
                if (result.Scores.Count() > 0)
                {
                    List<JambScoresDTO> sc = new List<JambScoresDTO>();
                    foreach (var d in result.Scores)
                    {
                        sc.Add(new JambScoresDTO
                        {
                            StudentId = result.StudentId,
                            JambRegNumber = dto.JambRegNumber,
                            JambYear = dto.JambYear,
                            Subject = d.Subject,
                            Score = d.Score,
                            ScoreId = d.ScoreId
                        });
                    }
                    dto.Scores = sc;
                    dto.Total = dto.Scores.Sum(a => a.Score);
                }
                else
                {
                    dto.Scores = new List<JambScoresDTO>();
                }
            };

            return dto;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userId"></param>
        /// <param name="flag"> 0=Jamb score added Ok; 1=Jamb Regno already exist</param>
        /// <returns></returns>
        public JambScoresDTO SaveJambScore(JambScoresDTO dto, string userId,out int flag)
        {
            var scores = _unitOfWork.JambResultRepository.Get(dto.JambRegNumber);
            flag = 0;
            int tscore;
            if (scores == null)
            {
                JambResult result = new JambResult();
                result.JambRegNumber = dto.JambRegNumber;
                result.StudentId = dto.StudentId;
                result.JambYear = dto.JambYear;

                result.Scores.Add(new JambScores { Subject = dto.Subject, Score = dto.Score});
                _unitOfWork.JambResultRepository.Add(result);
                
                _unitOfWork.Commit(userId);
                return dto;
            }
            else
            {
                tscore = scores.Scores.Count();
                if(dto.StudentId!=scores.StudentId || scores.Scores.Count()>4)
                {
                    flag = 1;
                    return null;
                }
                var js= new JambScores
                {
                    JambRegNumber = dto.JambRegNumber,
                    Score = dto.Score,
                    Subject = dto.Subject
                };
                scores.JambYear = dto.JambYear;
                _unitOfWork.JambScoresRepository.Add(js);
                if(tscore+1==4)
                {
                    var student = _unitOfWork.StudentRepository.Get(dto.StudentId);
                    if (student.AddmissionCompleteStage < 5)
                    {
                        student.AddmissionCompleteStage = 5;
                    }
                }
                
                _unitOfWork.Commit(userId);
                dto.ScoreId = js.ScoreId;
                return dto;
            }
            
        }

        public string DeleteJambScore(JambScoresDTO dto, string userId)
        {
            var scores = _unitOfWork.JambScoresRepository.Get(dto.ScoreId);
            if (scores == null)
            {
                return "Error: Invalid resultId";
            }
            else
            {

                _unitOfWork.JambScoresRepository.Remove(scores);
                _unitOfWork.Commit(userId);
                return "Jamb Score successfully removed";
            }
        }

        public List<OtherQualificationDTO> GetStudentAlevel(string studentId)
        {
            var result = _unitOfWork.OtherAcademicQualificationsRepository.GetFiltered(a => a.PersonId == studentId).ToList();
           
            if (result.Count > 0)
            {

                List<OtherQualificationDTO> sc = new List<OtherQualificationDTO>();
                foreach (var d in result)
                {
                    sc.Add(new OtherQualificationDTO
                    {
                        PersonId = d.PersonId,
                        StartMonth = d.StartMonth,
                        EndMonth = d.EndMonth,
                        Institution = d.Institution,
                        Qualification = d.Qualification,
                        QualificationId = d.QualificationId
                    });
                }
                return sc;

            }
            else return null;
            
            
        }
        public string SaveAlevel(OtherQualificationDTO dto, string userId)
        {
            var scores = _unitOfWork.OtherAcademicQualificationsRepository.Get(dto.QualificationId);
            if (scores == null)
            {
                
                _unitOfWork.OtherAcademicQualificationsRepository.Add(new OtherAcademicQualifications
                {
                    StartMonth = dto.StartMonth,
                    EndMonth = dto.EndMonth,
                    PersonId = dto.PersonId,
                    Institution = StandardGeneralOps.ToTitleCase(dto.Institution),
                    Qualification = dto.Qualification
                });
                var student = _unitOfWork.StudentRepository.Get(dto.PersonId);
                if (student.AddmissionCompleteStage < 5)
                {
                    student.AddmissionCompleteStage = 5;
                }
                _unitOfWork.Commit(userId);
                return "Result successfully added";
            }
            else
            {

                return "Qualification already exist";

            }
            
            
        }

        public string DeleteAlevel(OtherQualificationDTO dto, string userId)
        {
            var scores = _unitOfWork.OtherAcademicQualificationsRepository.Get(dto.QualificationId);
            if (scores == null)
            {
                return "Error: Invalid resultId";
            }
            else
            {

                _unitOfWork.OtherAcademicQualificationsRepository.Remove(scores);
                _unitOfWork.Commit(userId);
                return "Qualification successfully removed";
            }
        }

        public string CheckCompletedProfile(string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            var jambresult = student.JambResults.FirstOrDefault();
            string msg = "";
            if(string.IsNullOrEmpty(student.PhotoId))
            { msg = "Please Upload your passport"; return msg; }
            else if (student.OlevelResults.Count()==0)
            {
                msg = "O/Level Results missing";
                return msg;
            }

            else if (jambresult==null)
            {
                msg = "Input your UTME Registration number and year";
                return msg;
            }
            else if(student.EntryMode!="Direct Entry")
            {
                var jr = jambresult.Scores.ToList();
                if(jr.Count()<4)
                {
                    msg = "Complete your UTME result";
                    return msg;
                }
                
            }
            else if(student.EntryMode=="Direct Entry"&& student.OtherQualifications.Count()==0)
            {
                return "A/Level Results missing";
            }
            else if(string.IsNullOrEmpty(student.StudyMode))
            {
                return "Incomplete profile information. Please complete your profile";
            }
            return "Ok";
        }

        public string UpdateStudentProgramme(string studentId,string newProgCode,string reason,string userId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            if (student == null)
                return "Error: Invalid student ID";
            var prog = _unitOfWork.ProgrammeRepository.Get(newProgCode);
            student.ProgrammeCode = prog.ProgrammeCode;
            student.DepartmentCode = prog.DepartmentCode;
            student.ProgrammeType = prog.ProgrammeType;
            var user = _unitOfWork.UserRepository.Get(studentId);
            user.ProgrammeCode = prog.ProgrammeCode;
            user.DepartmentCode = prog.DepartmentCode;
            _unitOfWork.Commit(userId);
            return "Operation was completed successfully";
        }
        public StudentDTO StudentApplicationDetail(string studentId)
        {
            var student = _unitOfWork.StudentRepository.GetFiltered(a => a.PersonId == studentId).FirstOrDefault();
            if (student == null)
                return null;
            var dto = CoreModuleMappings.StudentToStudentDTO(student);
            
            //check if student has written jamb
            
                
            return dto;
        }
        public StudentDTO StudentApplicationSummary(string studentId)
        {
            var student = _unitOfWork.StudentRepository.GetFiltered(a => a.PersonId == studentId).FirstOrDefault();
           

            if(student!=null)
            {
                StudentDTO st = new StudentDTO();
                st.Title = student.Title;
                st.FullName = student.Name;
                
                st.Sex = student.Sex;
                
                st.Email = student.Email;
                st.Phone = student.Phone;
                st.Foto = student.Photo.Foto;
                st.StudentId = student.PersonId;
                st.ProgrammeType = student.ProgrammeType;
                st.EntryMode = student.EntryMode;
                
                st.Department = student.Department.Title;
                st.Faculty = student.Department.Faculty.Title;
                st.YearAdmitted = student.YearAddmitted;
                st.Status = student.Status;
                return st;
            }
            return null;
        }

        public string  AdmitStudent(string studentId,string userId)
        {
             
            var student = _unitOfWork.StudentRepository.Get(studentId);
            var userData = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);
            student.AdmissionDate = DateTime.UtcNow;
            student.AdmissionStatus="Admitted";

            //check if 

            _unitOfWork.Commit(userId);
            
            return "Student successfully admitted";
        }
        public StudentDTO AdmissionStatus(string studentID)
        {
            var student = _unitOfWork.StudentRepository.GetFiltered(s => s.PersonId == studentID)
                .FirstOrDefault();

            var dto = CoreModuleMappings.StudentToStudentDTO(student);
            return dto;
                
        }

        public int  CheckAdmissionCompletionStep(string studentID)
        {
            var student = _unitOfWork.StudentRepository.GetFiltered(s => s.PersonId == studentID)
                .FirstOrDefault();
            
            return student.AddmissionCompleteStage;

        }

        public List<ApplicantDTO> FetchApplicants(string session,string programmeCode,string rpt)
        {
            if(rpt=="All")
            {
                var student = _unitOfWork.StudentRepository.GetFiltered(s => s.YearAddmitted == session
                && s.ProgrammeCode == programmeCode && s.Status == "Prospective").ToList(); ;
                if (student != null)
                {
                    List<ApplicantDTO> dtos = new List<ApplicantDTO>();
                    foreach (var s in student)
                    {
                        dtos.Add(CoreModuleMappings.StudentToApplicantDTO(s));
                    }
                    return dtos.OrderBy(a => a.Name).ToList();
                }
                 
                return new List<ApplicantDTO>();
            }
            else
            {
                var prog = _unitOfWork.ProgrammeRepository.Get(programmeCode);
                var userData= _unitOfWork.ProgrammeTypeRepository.Get(prog.ProgrammeType);
                var student = _unitOfWork.StudentRepository.GetFiltered(s => s.YearAddmitted == session
                && s.ProgrammeCode == programmeCode && s.Status == "Prospective" && s.AddmissionCompleteStage==userData.AdmissionPause);
                if (student != null)
                {
                    List<ApplicantDTO> dtos = new List<ApplicantDTO>();
                    foreach (var s in student)
                    {
                        dtos.Add(CoreModuleMappings.StudentToApplicantDTO(s));
                    }
                    return dtos.OrderBy(a => a.Name).ToList();
                }
                else
                return new List<ApplicantDTO>();
            }
            

        }
        public List<ApplicantDTO> FetchApplicantsByDept(string session, string deptCode,string progType, string rpt)
        {
            if (rpt == "All")
            {
                var student = _unitOfWork.StudentRepository.GetFiltered(s => s.YearAddmitted == session
                && s.DepartmentCode == deptCode && s.ProgrammeType==progType && s.Status == "Prospective");
                if (student != null)
                {
                    List<ApplicantDTO> dtos = new List<ApplicantDTO>();
                    foreach (var s in student)
                    {
                        dtos.Add(CoreModuleMappings.StudentToApplicantDTO(s));
                    }
                    return dtos.OrderBy(a => a.Name).ToList();
                }

                return new List<ApplicantDTO>();
            }
            else
            {
                var student = _unitOfWork.StudentRepository.GetFiltered(s => s.YearAddmitted == session
                && s.DepartmentCode == deptCode && s.ProgrammeType == progType && s.Status == "Prospective" && s.AddmissionCompleteStage == 2);
                if (student != null)
                {
                    List<ApplicantDTO> dtos = new List<ApplicantDTO>();
                    foreach (var s in student)
                    {
                        dtos.Add(CoreModuleMappings.StudentToApplicantDTO(s));
                    }
                    return dtos.OrderBy(a => a.Name).ToList();
                }
                else
                    return new List<ApplicantDTO>();
            }


        }
        
        #endregion

        #region POST ADMISSION OPERATIONS
        public List<StudentSummaryDTO> FetchStudents(string programmeCode,string sessionAddmitted)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(s =>  s.ProgrammeCode == programmeCode
              && s.YearAddmitted == sessionAddmitted)
              .Select(d=>new StudentSummaryDTO
              {
                  StudentId = d.PersonId,
                  FullName = d.Name,
                  MatricNumber = d.MatricNumber,
                  Phone = d.Phone,
                  Programme = d.Programme.Title,
                  Status = d.Status
              }).ToList();
            
            if (students.Count == 0)
                return new List<StudentSummaryDTO>();
           
            return students.OrderBy(s=>s.MatricNumber).ToList();
        }

        public List<StudentDTO> FetchStudents(string programmeCode)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(s => s.ProgrammeCode == programmeCode && s.Status=="Active").ToList();
            List<StudentDTO> dto = new List<StudentDTO>();
            if (students.Count == 0 || students == null)
                return new List<StudentDTO>();
            foreach (var s in students)
            {
                var st = new StudentDTO
                {
                    StudentId = s.PersonId,
                    MatricNumber = s.MatricNumber,
                    FullName=s.Name
                };
                dto.Add(st);
            }
            return dto.OrderBy(s => s.MatricNumber).ToList();
        }
        public List<StudentSummaryDTO> SearchStudents(string searchText)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(s => s.PersonId.Contains(searchText)||s.MatricNumber.Contains(searchText)||s.Surname.Contains(searchText)).ToList();
            List<StudentSummaryDTO> dto = new List<StudentSummaryDTO>();
            if (students.Count == 0 || students == null)
                return new List<StudentSummaryDTO>();
            foreach (var s in students)
            {
                var st = new StudentSummaryDTO
                {
                    StudentId = s.PersonId,
                    MatricNumber = s.MatricNumber,
                    FullName=s.Name,
                    Programme=s.Programme.Title,
                    Phone=s.Phone,
                    Status=s.Status
                };
                dto.Add(st);
            }
            return dto.OrderBy(s => s.MatricNumber).ToList();
        }
        public StudentInProgDTO FetchStudentsInProgram(string programmeCode, string sessionAddmitted)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(s => s.ProgrammeCode == programmeCode
              && s.YearAddmitted == sessionAddmitted&&s.Status=="Active").ToList();
            
            if (students.Count == 0 || students == null)
                return new StudentInProgDTO();

            List<StudentDTO> sts = new List<StudentDTO>();
            StudentInProgDTO dto = new StudentInProgDTO();
            foreach (var s in students)
            {
                sts.Add(CoreModuleMappings.StudentToStudentDTO(s));
            }
            var single = sts.First();
            dto.Department = single.Department;
            dto.Faculty = single.Faculty;
            dto.Programme = single.Programme;
            dto.Set = single.YearAdmitted;
            dto.ProgType = single.ProgrammeType;
            dto.Students=sts.OrderBy(a=>a.CurrentLevel).OrderBy(s => s.MatricNumber).ToList();

            dto.Level = dto.Students.Count();
            return dto;
        }
        /// <summary>
        /// Use studentId or matricnumber to search
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public StudentDTO FetchStudent(string studentId)
        {
            if (!string.IsNullOrEmpty(studentId))
            {
                var student = _unitOfWork.StudentRepository.GetFiltered(s => s.PersonId == studentId || s.MatricNumber == studentId).SingleOrDefault();
                if (student == null)
                    return null;
               else return CoreModuleMappings.StudentToStudentDTO(student);
            }
            else
            {
                return null;
            }
            
        }
        public StudentSummaryDTO FetchStudentSummary(string studentId)
        {
            if (!string.IsNullOrEmpty(studentId))
            {
                var student = _unitOfWork.StudentRepository.GetFiltered(s => s.PersonId == studentId || s.MatricNumber == studentId).SingleOrDefault();
                if (student != null)
                {
                    return new StudentSummaryDTO
                    {
                        Award = student.Programme.Award,
                        DateAdmitted = student.AdmissionDate,
                        YearAdmitted = student.YearAddmitted,
                        Department = student.Department.Title,
                        Status = student.Status,
                        Duration = student.Duration.Value,
                        Faculty = student.Department.Faculty.Title,
                        Foto = student.Photo.Foto,
                        FullName = student.Name,
                        
                        MatricNumber = student.MatricNumber,
                        Phone = student.Phone,
                        Programme = student.Programme.Title,
                        ProgrammeType = student.ProgrammeType,
                        StudentId = student.PersonId,
                        AdmissionStatus=student.AdmissionStatus,
                        ProgrammeCode=student.ProgrammeCode
                    };
                }
                else return null;
            }
            else
            {
                return null;
            }

        }
        public StudentSummaryDTO FetchStudentByPhone(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                var st = _unitOfWork.StudentRepository.GetFiltered(s => s.Phone == phone).ToList();
                if(st.Count()>1)
                {
                    return new StudentSummaryDTO { Status = "This phone number is registered with more than one student." 
                        +"Operation terminated" };

                }
                if(st.Count()==1)

                {
                    var student = st.SingleOrDefault();
                    return new StudentSummaryDTO
                    {
                        
                        
                        FullName = student.Name,
                        MatricNumber = student.MatricNumber,
                        Phone = student.Phone,
                        Programme = student.Programme.Title,
                        ProgrammeType = student.ProgrammeType,
                        StudentId = student.PersonId,
                        AdmissionStatus = student.AdmissionStatus,
                        ProgrammeCode = student.ProgrammeCode
                    };
                }
                else
                {
                    return null;
                }
 
            }
            else
            {
                return null;
            }

        }

        public string AddmitWithoutAcceptanceFee(string studentId)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            student.AddmissionCompleteStage = (byte)(student.AddmissionCompleteStage + 1);
            student.AdmissionStatus = "Accepted";
            student.Status = "Active";
            //Generate MatricNo
            var prog = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);
            if (prog.AutoGenerateMatricNo)
            { student.MatricNumber = GenerateMatricNo(student.ProgrammeCode, student.YearAddmitted);
                _unitOfWork.Commit(studentId);
                return student.MatricNumber;
            }

            return null;

        }
        public void GrantStudentAccessToUtilitesBasedOnPayments(string paymentType,string studentId,int? payOptionId,int sessionId,bool late=false)
        {
            var student = _unitOfWork.StudentRepository.Get(studentId);
            
            switch(paymentType)
            {
                case "Screening Fee":
                    student.AddmissionCompleteStage = 1;
                    break;
                case "Application Fee":
                    student.AddmissionCompleteStage = 1;
                    break;
                case "Admission Fee":
                    student.AddmissionCompleteStage = 1;
                    break;
                case "Form Fee":
                    student.AddmissionCompleteStage = 1;
                    break;
                case "Acceptance Fee":
                    student.AdmissionStatus = "Accepted";
                    student.Status = "Active";
                    //Generate MatricNo
                    var prog = _unitOfWork.ProgrammeTypeRepository.Get(student.ProgrammeType);
                    if (prog.AutoGenerateMatricNo==true) {student.MatricNumber = GenerateMatricNo(student.ProgrammeCode, student.YearAddmitted); }
                     
                    //Debit student for fee payment
                    
                    break;
                
                case "School Fee":
                    if (payOptionId > 0)
                    {
                        
                        var options = _unitOfWork.FeeOptionsRepository.Get((int)payOptionId);
                        //Check if detail contains lateReg
                        
                        GrantStudentAcademicAccess((int)payOptionId, sessionId, studentId, late);
                                        
                    }
                

                    break;
            }
            _unitOfWork.Commit("System");
        }

        
        void GrantStudentAcademicAccess(int optionId, int sessionId, string studentId,bool lateRegPayCleared)
        {
            var log = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(a => a.SessionId == sessionId && a.StudentId == studentId).FirstOrDefault();
            if(log!=null)
            {
                var ops = _unitOfWork.FeeOptionsRepository.Get(optionId);
                if (log.Register1==false)
                { log.Register1 = ops.Register1;
                    if(log.Late1Clear==false)
                    { log.Late1Clear = lateRegPayCleared; }
                }
                if (log.Register2 == false)
                { log.Register2 = ops.Register2;
                    if (log.Late2Clear == false)
                    { log.Late2Clear = lateRegPayCleared; }
                }
                if (log.Register3 == false)
                { log.Register3 = ops.Register3;
                    if (log.Late3Clear == false)
                    { log.Late3Clear = lateRegPayCleared; }
                }
                if (log.Write1 == false)
                { log.Write1 = ops.Write1;
                    
                }
                if (log.Write2 == false)
                { log.Write2 = ops.Write2; }
                if (log.Write3 == false)
                { log.Write3 = ops.Write3; }
                //Update lateregs
                if(ops.Installment.Contains("1st Semester")&&lateRegPayCleared==true)
                { log.Late1Clear = true; }
                if (ops.Installment.Contains("2nd Semester") && lateRegPayCleared == true)
                { log.Late2Clear = true; }
                if (ops.Installment.Contains("3rd Semester") && lateRegPayCleared == true)
                { log.Late3Clear = true; }
                if (ops.Installment.Contains("Entire Session") && lateRegPayCleared == true)
                { log.Late1Clear = true; log.Late2Clear = true; log.Late3Clear= true; }
                _unitOfWork.Commit(studentId);
            }

            
        }
        string GenerateMatricNo(string programmeCode,string admitYear)
        {

            
            string matric ="";
            string yr = "";
            int seriallength=0;
            yr = admitYear.Substring(2, 2);
            var prog = _unitOfWork.ProgrammeRepository.Get(programmeCode);
            seriallength = prog.SerialLength;
            var matformat = prog.MatricNoFormats.OrderBy(a => a.Position).ToList();
            matric = string.Join(prog.MatricNoSeparator, matformat.Select(a => a.BankValue));
            matric = matric.Replace("YY", yr);
            List<Student> students;
            if(prog.MatricNoGeneratorType=="Cumulative")
            {
                students = _unitOfWork.StudentRepository.GetFiltered(a =>a.MatricNumber.Contains(matric))
                .OrderBy(a => a.MatricNumber).ToList();
            }
            else
            {
                students = _unitOfWork.StudentRepository.GetFiltered(a => a.ProgrammeCode == programmeCode && a.YearAddmitted == admitYear
                && a.MatricNumber != null)
                .OrderBy(a => a.MatricNumber).ToList();
            }
             
            if (students.Count>0)

            {
                //Get the last student
                var lastStudent = students.LastOrDefault().MatricNumber;
                var wrk = lastStudent.Replace(matric+prog.MatricNoSeparator, "");
                int lastno = Convert.ToInt32(wrk);
                
                string newString = (lastno+1).ToString();
                int newLength = newString.Length;

                
                switch (newLength)
                {
                    case 1:
                        if (seriallength == 4)
                        {
                            matric = matric + prog.MatricNoSeparator + "000" + newString;
                        }
                        else if (seriallength==3)
                        {
                            matric = matric + prog.MatricNoSeparator + "00" + newString;
                        }
                        else
                        {
                            matric = matric + prog.MatricNoSeparator + "0" + newString;
                        }
                        break;
                    case 2:
                        if (seriallength == 4)
                        {
                            matric = matric + prog.MatricNoSeparator + "00" + newString;
                        }
                        else if (seriallength == 3)
                        {
                            matric = matric + prog.MatricNoSeparator + "0" + newString;
                        }
                        else
                        {
                            matric = matric + prog.MatricNoSeparator + newString;
                        }

                        break;
                    case 3:
                        if (seriallength == 4)
                        {
                            matric = matric + prog.MatricNoSeparator + "0" + newString;
                        }
                        else if (seriallength == 3)
                        {
                            matric = matric + prog.MatricNoSeparator + newString;
                        }
                        break;
                    case 4:
                        if (seriallength == 4)
                        {
                            matric = matric + prog.MatricNoSeparator + newString;
                        }
                        
                        break;

                }
            }
            else
            {
                switch(prog.SerialLength)
                {
                    case 4:
                        matric = matric + prog.MatricNoSeparator + "0001";
                        break;
                    case 3:
                        matric = matric + prog.MatricNoSeparator + "001";
                        break;
                    case 2:
                        matric=matric + prog.MatricNoSeparator + "01";
                        break;
                }
                
            }

            return matric;
        }
        #endregion

        public bool CanVote(string studentId,int sessionId)
        {
            var regs = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(a => a.StudentId == studentId
              && a.SessionId == sessionId).SingleOrDefault();
            if(regs==null)
            {
                return false;
            }
            else if(regs.Register1=true||regs.Register2==true||regs.Register3==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region REPORTS

        public MatricRegisterDTO MatricnRegister(string admitYr, string deptCode,string progType)
        {
            var students= _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == admitYr && a.DepartmentCode == deptCode
            && a.ProgrammeType==progType&& a.AdmissionStatus=="Accepted").ToList();

            if (students.Count == 0)
                return null;
            List<string> ids = new List<string>();
            

            foreach(var st in students)
            {
                ids.Add(st.PersonId);
            }
            var jambScores = _unitOfWork.JambResultRepository.GetFiltered(a => ids.Contains(a.StudentId)).ToList();

            MatricRegisterDTO dto = new MatricRegisterDTO();
            var first = students.First();
            dto.Department = first.Department.Title;
            dto.ProgrammeType = progType;
            dto.Faculty = first.Department.Faculty.Title;
            dto.Session = admitYr;
            //Get Headingg
            List<MatricRegHeadings> hdings = new List<MatricRegHeadings>();
            var hds = (from h in students
                       group h by new { h.ProgrammeCode, h.Programme.Title } into nh
                       select new { progCode = nh.Key.ProgrammeCode, prog = nh.Key.Title })
                    .OrderBy(s => s.prog).ToList();
            foreach(var h in hds)
            {
                MatricRegHeadings regH = new MatricRegHeadings();
                regH.Heading = h.prog;
                List<MatricRegDetailsDTO> hdetails = new List<MatricRegDetailsDTO>();
                var stInProg = students.Where(s => s.ProgrammeCode == h.progCode).OrderBy(a => a.MatricNumber).ToList();
                foreach(var s in stInProg)
                {
                    var hdingDetail = new MatricRegDetailsDTO();
                    hdingDetail.StudentId = s.PersonId;
                    hdingDetail.MatricNo = s.MatricNumber;
                    hdingDetail.Lg = s.Lg;
                    hdingDetail.State = s.State;
                    hdingDetail.Address = s.ResidentialAddress;
                    hdingDetail.Sex = s.Sex;
                    hdingDetail.BirthDate = s.BirthDate;
                    hdingDetail.Email = s.Email;
                    hdingDetail.Phone = s.Phone;
                    hdingDetail.Name = s.Name;
                    hdingDetail.Programme = s.Programme.Title;
                    hdingDetail.EntryMode = s.EntryMode;

                    var jas = s.JambResults.Where(a => a.StudentId == s.PersonId).FirstOrDefault();
                    if(jas!=null)
                    {
                        hdingDetail.JambScore = jas.Score;

                        hdingDetail.JambRegNo = jas.JambRegNumber;
                    }
                    
                    hdetails.Add(hdingDetail);

                }
                regH.Details = hdetails;
                hdings.Add(regH);
            }

            dto.Headings = hdings;
            return dto;

        }
        public int TotalSessionApplicants(string session)
        {
            return _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session&&a.Status=="Prospective").ToList().Count();
        }
        public List<ProgrammeDTO> SessionAdmissionsSummaryProgType(string session)
        {
            var students=_unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session&&a.AdmissionStatus=="Accepted").ToList();
            List<ProgrammeDTO> dto = new List<ProgrammeDTO>();
            if (students.Count>0)
            {
                var enr = from s in students
                          group s by s.ProgrammeType into sp
                          select new ProgrammeDTO
                          {
                              ProgrammeType = sp.Key,
                              TotalEnrollment = sp.Count()
                          };
                return enr.OrderBy(a => a.ProgrammeType).ToList();
            }
            return new List<ProgrammeDTO>();
        }
        public List<StudentEnrolmentDTO> StudentEnrolment(string session,string deptCode)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.AdmissionStatus == "Accepted"
            && a.DepartmentCode == deptCode).OrderBy(a => a.ProgrammeType).ThenBy(a=>a.Name).ToList();
            List<StudentEnrolmentDTO> dto = new List<StudentEnrolmentDTO>();
            if (students.Count > 0)
            {
                foreach (var s in students)
                {
                    dto.Add(new StudentEnrolmentDTO
                    {
                        StudentId=s.PersonId,FullName=s.Name,Gender=s.Sex,Phone=s.Phone,
                        Programme=s.Programme.Title,ProgrammeType=s.ProgrammeType,Department=s.Department.Title,
                        Status=s.Status,YearAdmitted=s.YearAddmitted
                    });
                }
                return dto;
            }

            return new List<StudentEnrolmentDTO>();
        }

        public List<StudentEnrolmentDTO> StudentEnrolmentByProgrammeType(string session, string progType)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.AdmissionStatus == "Accepted"
            && a.ProgrammeType == progType).OrderBy(a => a.Programme.Title).ThenBy(a => a.Name).ToList();
            List<StudentEnrolmentDTO> dto = new List<StudentEnrolmentDTO>();
            if (students.Count > 0)
            {
                foreach (var s in students)
                {
                    dto.Add(new StudentEnrolmentDTO
                    {
                        StudentId = s.PersonId,
                        FullName = s.Name,
                        Gender = s.Sex,
                        Phone = s.Phone,
                        Programme = s.Programme.Title,
                        ProgrammeType = s.ProgrammeType,
                        Department = s.Department.Title,
                        Status = s.Status,
                        YearAdmitted = s.YearAddmitted
                    });
                }
                return dto;
            }

            return new List<StudentEnrolmentDTO>();
        }
        public List<StudentEnrolmentDTO> StudentEnrolment(string session, string deptCode,string sex)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.AdmissionStatus == "Accepted"
            &&a.DepartmentCode==deptCode&&a.Sex==sex).OrderBy(a=>a.Surname).ToList();
            List<StudentEnrolmentDTO> dto = new List<StudentEnrolmentDTO>();
            if (students.Count > 0)
            {
                foreach(var s in students)
                {
                    dto.Add(new StudentEnrolmentDTO
                    {
                        StudentId = s.PersonId,
                        FullName = s.Name,
                        Gender = s.Sex,
                        Phone = s.Phone,
                        Programme = s.Programme.Title,
                        ProgrammeType = s.ProgrammeType,
                        Department = s.Department.Title,
                        Status = s.Status,
                        YearAdmitted = s.YearAddmitted
                    });
                }
                return dto;
            }

            return new List<StudentEnrolmentDTO>();
        }
        public List<ProgrammeDTO> StudentEnrolmentSummary(string session,string deptCode)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.AdmissionStatus == "Accepted"
            && a.DepartmentCode==deptCode).ToList();
            List<ProgrammeDTO> dto = new List<ProgrammeDTO>();
            if (students.Count > 0)
            {
                var enr = from s in students
                          group s by new { s.Department.Title, s.Sex } into sp
                          select new ProgrammeDTO
                          {
                              Department = sp.Key.Title,
                              Description=sp.Key.Sex,
                              TotalEnrollment = sp.Count()
                          };
                return enr.OrderBy(a => a.ProgrammeType).ToList();
            }
            return new List<ProgrammeDTO>();
        }
        public List<ProgrammeDTO> StudentEnrolmentSummary(string session,string deptCode,string gender)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.AdmissionStatus == "Accepted"
            && a.DepartmentCode==deptCode && a.Sex==gender).ToList();
            List<ProgrammeDTO> dto = new List<ProgrammeDTO>();
            if (students.Count > 0)
            {
                var enr = from s in students
                          group s by new { s.Department.Title, s.Sex } into sp
                          select new ProgrammeDTO
                          {
                              Department = sp.Key.Title,
                              Description=sp.Key.Sex,
                              TotalEnrollment = sp.Count()
                          };
                return enr.OrderBy(a => a.ProgrammeType).ToList();
            }
            return new List<ProgrammeDTO>();
        }
        public int CurrentTotalStudents()
        {
            return   _unitOfWork.StudentRepository.GetFiltered(a => a.Status == "Active"&& a.Status == "About to Graduate").Count();
        }
        public List<StudentInProgDTO> TotalActiveStudentsByProgramme()
        {
            var students= _unitOfWork.StudentRepository.GetFiltered(a => a.Status == "Active"||a.Status=="About to Graduate").ToList();
            var st = from s in students
                     group s by s.ProgrammeType into ns
                     select new StudentInProgDTO
                     {
                         ProgType = ns.Key,
                         Level = ns.Count()
                     };
            return st.OrderBy(a => a.ProgType).ToList();
        }
        public List<ProgrammeDTO> CurrentStudentEnrollmentsByProgType()
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(a => (a.Status == "Active" || a.Status == "About to Graduate")&&a.ProgrammeType!=null).ToList();
             
            var progs = from s in students
                        group s by s.ProgrammeType
                      into stProgs
                        select new ProgrammeDTO
                        {
                            ProgrammeType = stProgs.Key,
                            TotalEnrollment = stProgs.Count()
                        };

            return progs.OrderBy(a => a.Title).ToList();
        }

        public List<ApplicantDTO> ApplicantsByProgrammeType(string session, string progType)
        {
            var app = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.ProgrammeType
              == progType && a.AddmissionCompleteStage >= 2).ToList();
            if(app.Count==0)
            {
                return new List<ApplicantDTO>();
            }
            List<ApplicantDTO> dto = new List<ApplicantDTO>();
            foreach(var a in app)
            {

                ApplicantDTO apl = new ApplicantDTO();

                apl.Name = a.Name;
                apl.RegNo = a.PersonId;
                apl.Programme = a.Programme.Title;
                apl.Phone = a.Phone;
                apl.Email = a.Email;
                apl.State = a.State;
                apl.Lga = a.Lg;
                apl.Status = a.Status;
                apl.AddmissionCompleteStage = a.AddmissionCompleteStage;
                apl.Session = a.YearAddmitted;
                apl.ProgrammeType = a.ProgrammeType;

                if(a.JambResults.Count()>0)
                {
                    var frst = a.JambResults.First();
                    apl.JambNo = frst.JambRegNumber;
                    apl.JambScore = frst.Scores.Count() == 0 ? frst.Scores.Sum(s => s.Score) : 0;
                }
                
                
                dto.Add(apl);
            }

            return dto.OrderBy(a => a.Programme).ThenBy(a => a.Name).ToList();
            
        }
        public List<ApplicantDTO> ApplicantsByDepartment(string session, string deptCode)
        {
            var app = _unitOfWork.StudentRepository.GetFiltered(a => a.YearAddmitted == session && a.DepartmentCode
              == deptCode && a.AddmissionCompleteStage >= 2).ToList();
            if (app.Count == 0)
            {
                return new List<ApplicantDTO>();
            }
            List<ApplicantDTO> dto = new List<ApplicantDTO>();
            foreach (var a in app)
            {
                dto.Add(new ApplicantDTO
                {
                    Name = a.Name,
                    RegNo = a.PersonId,
                    Programme = a.Programme.Title,
                    Phone = a.Phone,
                    Email = a.Email,
                    State = a.State,
                    Lga = a.Lg,
                    Status = a.Status,
                    AddmissionCompleteStage = a.AddmissionCompleteStage
                });
            }

            return dto.OrderBy(a => a.Programme).ThenBy(a => a.Name).ToList();

        }
        #endregion

        #region DOCUMENT OPERATION
        public void SaveDocument(StudentDocuments doc, string userId)
        {
            var student = _unitOfWork.StudentRepository.GetSingle(a => a.PersonId == doc.PersonId);
            
            student.AddmissionCompleteStage += 1;
            
            _unitOfWork.DocumentRepository.Add(doc);
            _unitOfWork.Commit(userId);
        }
        public StudentDocuments ViewDocument(string studentId)
        {
            var docs = _unitOfWork.DocumentRepository.GetFiltered(a => a.PersonId == studentId).FirstOrDefault();
            return docs;
        }
        #endregion

        #region  PRIVATE HELPERS
         
        #endregion
    }
}
