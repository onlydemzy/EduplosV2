using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.BurseryModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.ArticleModule;
using Eduplus.DTO.CoreModule;
using Eduplus.ObjectMappings;
using Eduplus.Services.Contracts;
using Eduplus.Services.UtilityServices;
using KS.Core;
using KS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Eduplus.Services.Implementations
{
    public class GeneralDutiesService : IGeneralDutiesService
    {
        #region PRIVATE FIELDS
        private readonly IUnitOfWork _unitOfWork;
        
        #endregion

        #region PUBLIC CONSTRUCTOR
        public GeneralDutiesService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
        }
        
        public GeneralDutiesService()
        {
            
        }
        #endregion



        public List<CountryDTO> FetchCountries()
        {
            
            var cts=_unitOfWork.CountryRepository.GetAll()
                .Select(c=>new CountryDTO {
                    Country= c.CountryName
                });


            return cts.ToList();
        }
        public  List<StateDTO> FetchStates(string country)
        {

            var cts = _unitOfWork.StateRepository.GetFiltered(a=>a.Country.CountryName==country)
                .Select(c => new StateDTO
                {
                    State = c.StateName
                });


            return cts.ToList();
        }

        public List<LgaDTO> FetchLgs(string state)
        {

            var cts = _unitOfWork.LgaRepository.GetFiltered(a=>a.State.StateName==state)
                .Select(c => new LgaDTO
                {
                    Lga = c.LgTitle
                });


            return cts.ToList();
        }

        public List<Semester> FetchSemesterBySession(int sessionId)
        {
            //return await _unitOfWork.SemesterRepository.GetFiltered(s => s.SessionId == sessionId, a => a.SemesterTitle);
            return _unitOfWork.SemesterRepository.GetFiltered(s => s.SessionId == sessionId).ToList();
            
        }

        #region PROGRAMME OPERATIONS
        public List<ProgrammeDTO> FetchProgrammes(string departmentCode=null)
        {

            List<Programme> progs;
            List<ProgrammeDTO> dto = new List<ProgrammeDTO>();
            if (string.IsNullOrEmpty(departmentCode))
            {
                progs= _unitOfWork.ProgrammeRepository.GetAll().ToList();
            }
            else
            {
                progs=_unitOfWork.ProgrammeRepository.GetFiltered(f => f.DepartmentCode == departmentCode).ToList();
            }
            if(progs.Count>0)
            {
                foreach(var p in progs)
                {
                    dto.Add(new ProgrammeDTO
                    {
                        ProgrammeCode = p.ProgrammeCode,
                        Title = p.Title,
                        DepartmentCode = p.DepartmentCode,
                        ProgrammeType = p.ProgrammeType,
                        Department = p.Department.Title,
                        Award= p.Award,
                        IsActive = p.IsActive
                    });
                }
                return dto.OrderBy(a => a.Title).ToList();
            }
            return dto;
        }
        public List<ProgrammeTypesDTO> GetProgrammeTypes()
        {
            var types = _unitOfWork.ProgrammeTypeRepository.GetAll().ToList();
            List<ProgrammeTypesDTO> dto = new List<ProgrammeTypesDTO>();
            foreach(var t in types)
            {
                dto.Add(new ProgrammeTypesDTO
                {
                    Type = t.Type,
                    AdmissionPause = t.AdmissionPause,
                    ApplyGatewayCharge = t.ApplyGatewayCharge,
                    ApplyMajorCharge = t.ApplyMajorCharge,
                    ApplyMinorCharge = t.ApplyMinorCharge,
                    AutoGenerateMatricNo = t.AutoGenerateMatricNo,
                    IsActive = t.IsActive,
                    MaxCreditUnit = t.MaxCreditUnit,
                    CollectAcceptanceFee=t.CollectAcceptanceFee,
                    MaxCA1=t.MaxCA1,
                    MaxCA2=t.MaxCA2,
                    MaxExam=t.MaxExam

                });
            }
            return dto;

        }
        
        public List<ProgrammeDTO> ProgrammesByType(string type)
        {
            var programmes = _unitOfWork.ProgrammeRepository.GetFiltered(a=>a.ProgrammeType==type)
                .Select(p => new ProgrammeDTO
                {
                    ProgrammeCode = p.ProgrammeCode,
                    Title = p.Title,
                    DepartmentCode = p.DepartmentCode,
                    ProgrammeType = p.ProgrammeType,
                    Department = p.Department.Title,
                    Award=p.Award,
                    Faculty=p.Department.Faculty.Title,
                    IsActive = p.IsActive
                }).OrderBy(a => a.Title)
            .ToList();
            return programmes;
        }
        public List<ProgrammeDTO> ProgrammesByTypeDept(string type,string deptcode)
        {
             
                var programmes = _unitOfWork.ProgrammeRepository.GetFiltered(a => a.ProgrammeType == type && a.DepartmentCode == deptcode)
                .Select(p => new ProgrammeDTO
                {
                    ProgrammeCode = p.ProgrammeCode,
                    Title = p.Title,
                    DepartmentCode = p.DepartmentCode,
                    ProgrammeType = p.ProgrammeType,
                    Department = p.Department.Title,
                    Faculty = p.Department.Faculty.Title,
                    Award=p.Award,
                    IsActive = p.IsActive
                }).OrderBy(a => a.Title)
            .ToList();
                return programmes;
         }
        public ProgrammeDTO AddUpdateProgramme(ProgrammeDTO programme, string userId)
        {

            var dbProgs = _unitOfWork.ProgrammeRepository.GetSingle(p => p.ProgrammeCode == programme.ProgrammeCode);
            if (dbProgs == null)//fresh man, add him
            {
                var nProgs = new Programme
                {
                    ProgrammeCode =programme.ProgrammeCode.ToUpper().Trim(),
                    ProgrammeType = programme.ProgrammeType,
                    DepartmentCode = programme.DepartmentCode,
                    IsActive = programme.IsActive,
                    
                    Title = StandardGeneralOps.ToTitleCase(programme.Title),

                };
                _unitOfWork.ProgrammeRepository.Add(nProgs);
                _unitOfWork.Commit(userId);
                return programme;
            }
            dbProgs.DepartmentCode = programme.DepartmentCode;

            dbProgs.IsActive = programme.IsActive;
            dbProgs.Title = programme.Title;
            dbProgs.ProgrammeType = programme.ProgrammeType;
            dbProgs.Award = programme.Award;
            _unitOfWork.Commit(userId);
            return programme;

        }

        public List<FacultyProgrammesDTO> AllProgrammesByFaculty()
        {
            var facs = _unitOfWork.FacultyRepository.GetAll().OrderBy(f => f.Title);
            var progs = _unitOfWork.ProgrammeRepository.GetAll().OrderBy(f => f.Title);
            List<FacultyProgrammesDTO> dto = new List<FacultyProgrammesDTO>();
            List<FacultyProgs> depts = new List<FacultyProgs>();

            foreach (var f in facs)
            {

                FacultyProgrammesDTO fp = new FacultyProgrammesDTO();
                fp.FacultyCode = f.FacultyCode;
                fp.Faculty = f.Title;
                dto.Add(fp);
            }

            foreach (var p in progs)
            {
                FacultyProgs fd = new FacultyProgs();
                fd.FacultyCode = p.Department.FacultyCode;
                fd.Programme = p.Title;
                fd.ProgrammeCode = p.ProgrammeCode;
                
                depts.Add(fd);
            }

            foreach (var f in dto)
            {
                var fprogs = depts.Where(fa => fa.FacultyCode == f.FacultyCode)
                    .OrderBy(a => a.Programme).ToList();
                f.Programmes = fprogs;
            }
            return dto;
        }
        #endregion


        #region DEPARTMENT/FACULTY OPERATIONS
        public List<DepartmentDTO> FetchDepartments(string facultyCode=null)
        {

            List<Department> depts;
            List<DepartmentDTO> dto=new List<DepartmentDTO>();
            if (string.IsNullOrEmpty(facultyCode))
            {
                depts= _unitOfWork.DepartmentRepository.GetAll().OrderBy(s => s.Title).ToList();
            }
            else
            {
                depts= _unitOfWork.DepartmentRepository.GetFiltered(f => f.FacultyCode == facultyCode||f.Faculty.Title==facultyCode).OrderBy(d => d.Title).ToList();
            }
            if(depts.Count>0)
            {
                foreach (var d in depts)
                {
                    dto.Add(CoreModuleMappings.DepartmentToDepartmentDTO(d));
                }
                return dto.OrderBy(d => d.Title).ToList();
            }
            return dto;
        }
        
        public List<DepartmentDTO> AllAcademicDepartments()
        {

            var depts = _unitOfWork.DepartmentRepository.GetFiltered(a => a.IsAcademic == true).ToList();
            List<DepartmentDTO> dtos = new List<DepartmentDTO>();
            foreach (var d in depts)
            {
                dtos.Add(CoreModuleMappings.DepartmentToDepartmentDTO(d));

            }
            return dtos.OrderBy(d => d.Title).ToList();
               
        }
        public DepartmentDTO AddUpdateDepartment(DepartmentDTO dept, string userId)
        {


            var dbdept = _unitOfWork.DepartmentRepository.GetSingle(a => a.DepartmentCode == dept.DepartmentCode);
            if (dbdept == null)//fresh dept, add new
            {
                Department ndept = new Department();

                ndept.DepartmentCode = dept.DepartmentCode;
                ndept.Title = StandardGeneralOps.ToTitleCase(dept.Title);
                ndept.Location = StandardGeneralOps.ToTitleCase(dept.Location);
                if (dept.FacultyCode != "undefined")
                { ndept.FacultyCode = dept.FacultyCode; }
                ndept.IsAcademic = dept.IsAcademic;


                _unitOfWork.DepartmentRepository.Add(ndept);
                _unitOfWork.Commit(userId);
                return dept;
            }
            if (dbdept.Title != dept.Title)//name changed update article table reference
            {
                var article = _unitOfWork.ArticleRepository.GetSingle(a => a.Title == dbdept.Title);
                article.Title = dept.Title;
            }
            dbdept.Title = dept.Title;

            if (dept.FacultyCode != "undefined")
            { dbdept.FacultyCode = dept.FacultyCode; }

            dbdept.IsAcademic = dept.IsAcademic;
            dbdept.Location = dept.Location;

            _unitOfWork.Commit(userId);
            return dept;
        }
        public IEnumerable<Faculty> FetchFaculties()
        {

            return _unitOfWork.FacultyRepository.GetAll().OrderBy(f => f.Title);

        }
        public Faculty AddUpdateFaculty(Faculty faculty, string userId)
        {
            var dbfaculty = _unitOfWork.FacultyRepository.GetSingle(a => a.FacultyCode == faculty.FacultyCode);

            if (dbfaculty == null)//new faculty, 
            {
                faculty.Location = StandardGeneralOps.ToTitleCase(faculty.Location);
                faculty.Title = StandardGeneralOps.ToTitleCase(faculty.Title);
                _unitOfWork.FacultyRepository.Add(faculty);
                _unitOfWork.Commit(userId);
                return faculty;
            }
            dbfaculty.Title = StandardGeneralOps.ToTitleCase(faculty.Title);
            dbfaculty.Location = StandardGeneralOps.ToTitleCase(faculty.Location);
            dbfaculty.IsActive = faculty.IsActive;
            _unitOfWork.Commit(userId);
            return faculty;
        }
        public List<FacultyListDTO> FacultyList()
        {
            var faculties = _unitOfWork.FacultyRepository.GetAll()
                            .Select(a => new FacultyListDTO { FacultyCode = a.FacultyCode, Faculty = a.Title })
                            .OrderBy(a => a.Faculty)
                            .ToList();
            return faculties;
        }

        
        #endregion
        
        #region SEMESTER/SESSION OPERATIONS
        public List<SessionDTO> FetchSessions()
        {
            var sess = _unitOfWork.SessionRepository.GetAll()
                .Select(s => new SessionDTO
                {
                    SessionId = s.SessionId,
                    Title = s.Title,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsCurrent = s.IsCurrent
                })
                .OrderByDescending(a => a.Title).ToList();
            return sess;
        }
        public List<SessionDTO> FetchAdmissionSession()
        {
            var sess = _unitOfWork.SessionRepository.GetFiltered(a=>a.IsAdmissionSession==true)
                .Select(s => new SessionDTO
                {
                    SessionId = s.SessionId,
                    Title = s.Title,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsCurrent = s.IsCurrent
                })
                .OrderByDescending(a => a.Title).ToList();
            return sess;
        }

        public SessionDTO FetchSingleSession(int sessionId)
        {
            var session = _unitOfWork.SessionRepository.Get(sessionId);
            if (session == null)
                return new SessionDTO();
            SessionDTO dto = new SessionDTO();
            dto.EndDate = session.EndDate;
            dto.IsCurrent = session.IsCurrent;
            dto.SessionId = session.SessionId;
            dto.Title = session.Title;
            dto.StartDate = session.StartDate;

            List<SemesterDTO> semesters = new List<SemesterDTO>();
            foreach(var s in session.Semesters)
            {
                semesters.Add(CoreModuleMappings.SemesterToSemesterDTO(s));
            }
            dto.Semesters = semesters;
            return dto;

        }
        public SessionDTO SaveSession(SessionDTO session, string userId)
        {
           
                var dbSession = _unitOfWork.SessionRepository.Get(session.SessionId);
            if(session.SessionId==0)//New session, Add
            {
                Session ses = new Session();
                List<Semester> sems = new List<Semester>();
                ses.SessionId = StandardGeneralOps.GenereteSessionId(session.Title);
                ses.Title = session.Title;
                ses.StartDate = session.StartDate;
                ses.EndDate = session.EndDate;
                ses.IsCurrent = false;
               
                _unitOfWork.SessionRepository.Add(ses);
                _unitOfWork.Commit(userId);
                return session;
            }
            else
            {
                dbSession.StartDate = session.StartDate;
                dbSession.EndDate = session.EndDate;

                if(session.IsCurrent==true)
                {
                    var currentsessions = _unitOfWork.SessionRepository.GetFiltered(s => s.IsCurrent == true).ToList();
                    foreach(var cs in currentsessions)
                    {
                        cs.IsCurrent = false;
                    }
                    dbSession.IsCurrent = true;
                
                }
                
                _unitOfWork.Commit(userId);
                return session;
            }
  
        }

        public List<Semester> FetchSemester(int sessionId)
        {
            var sems = _unitOfWork.SemesterRepository.GetFiltered(a => a.SessionId == sessionId);
            return sems.ToList();
        }
        public Semester FetchCurrentSemester()
        {
            return _unitOfWork.SemesterRepository.GetSingle(f => f.IsCurrent == true);
        }
        public List<SemesterDTO> FetchSessionSemester(int sessionId)
        {
            var semesters = _unitOfWork.SemesterRepository.GetFiltered(a => a.SessionId == sessionId).ToList();
            List<SemesterDTO> dto = new List<SemesterDTO>();
            foreach (var s in semesters)
            {
                var d = new SemesterDTO
                {
                    SemesterId = s.SemesterId,
                    Title = s.SemesterTitle,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsCurrent = s.IsCurrent,
                    SessionId=s.SessionId,
                    ApplyLate=s.ApplyLate,
                     
                    LateRegistrationEndtDate=s.LateRegistrationEndDate,
                    LateRegistrationStartDate=s.LateRegistrationStartDate
                };
                dto.Add(d);
            }
            return dto;
        }

        public string SaveSemester(SemesterDTO dto, string userId)
        {
            var dbSemester = _unitOfWork.SemesterRepository.Get(dto.SemesterId);
            if(dbSemester==null)// New semester Add
            {
               
                Semester sem = new Semester();
                sem.SemesterId = StandardGeneralOps.GenerateSemesterId(dto.Title, dto.SessionId);
                sem.SemesterTitle = dto.Title;
                sem.SessionId = dto.SessionId;
                sem.ApplyLate = false;
                 
                sem.StartDate = dto.StartDate;
                sem.EndDate = dto.EndDate;
                sem.LateRegistrationEndDate = dto.LateRegistrationEndtDate;
                sem.LateRegistrationStartDate = dto.LateRegistrationStartDate;
                if (dto.IsCurrent == true)
                {
                    var dbCurrent = _unitOfWork.SemesterRepository.GetFiltered(a => a.IsCurrent == true).ToList();
                    foreach (var s in dbCurrent)
                    {
                        s.IsCurrent = false;
                    }

                    sem.IsCurrent = true;
                }
                sem.IsCurrent = false;

                _unitOfWork.SemesterRepository.Add(sem);
                _unitOfWork.Commit(userId);
                return "Semester successfully added.";
            }
            else
            {
                dbSemester.ApplyLate = dto.ApplyLate;
                
                dbSemester.StartDate = dto.StartDate;
                dbSemester.EndDate = dto.EndDate;
                if(dto.LateRegistrationEndtDate>DateTime.MinValue)
                {
                    dbSemester.LateRegistrationEndDate = dto.LateRegistrationEndtDate;
                }
                
                

                if (dto.IsCurrent == true)
                {
                    var dbCurrent = _unitOfWork.SemesterRepository.GetFiltered(a => a.IsCurrent == true).ToList();
                    foreach (var s in dbCurrent)
                    {
                        s.IsCurrent = false;
                    }

                    dbSemester.IsCurrent = true;
                }
                _unitOfWork.Commit(userId);

                return "Semester Successfully updated";
            }
                
        }

        #endregion

        

        #region COURSE OPERATIONS

        public List<CourseDTO> PopulateCourse(string programmeCode)
        {
            
            var courses = _unitOfWork.CourseRepository.GetFiltered(a => a.ProgrammeCode == programmeCode).ToList();
            List<CourseDTO> dto = new List<CourseDTO>();
            if (courses.Count > 0)
            {
                foreach (var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CoursesToCourseDTOs(c));
                }
                return dto.OrderBy(c => c.CourseCode).ToList();
            }
            return dto;
        }
        public List<CourseDTO> PopulateCourseByCategory(string programmeCode,string cat)
        {

            
            var courses = _unitOfWork.CourseRepository.GetFiltered(a => a.ProgrammeCode == programmeCode && a.Category == cat).ToList();
            List<CourseDTO> dto = new List<CourseDTO>();
            if (courses.Count > 0)
            {
                foreach (var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CoursesToCourseDTOs(c));
                }
                return dto.OrderBy(c => c.CourseCode).ToList();
            }
            return dto;
        }
        public List<CourseDTO> PopulateCourseByCategory(string programmeCode, string[] cat)
        {


            var courses = _unitOfWork.CourseRepository.GetFiltered(a => cat.Contains(a.Category) && a.ProgrammeCode==programmeCode).ToList();
            List<CourseDTO> dto = new List<CourseDTO>();
            if (courses.Count > 0)
            {
                foreach (var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CoursesToCourseDTOs(c));
                }
                return dto.OrderBy(c => c.CourseCode).ToList();
            }
            return dto;
        }
        public List<CourseDTO> PopulateActiveCourses(string programmeCode)
        {

            var courses = _unitOfWork.CourseRepository.GetFiltered(a => a.ProgrammeCode == programmeCode && a.IsActive==true).ToList();
            List<CourseDTO> dto = new List<CourseDTO>();
            if (courses.Count>0)
            {
                foreach(var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CoursesToCourseDTOs(c));
                }
                return dto.OrderBy(c => c.CourseCode).ToList();
            }
            return dto;
        }
        public List<CourseDTO> PopulateCourse(string programmeCode,int lvl,string semester)
        {

            var courses = _unitOfWork.CourseRepository.GetFiltered(a => a.ProgrammeCode == programmeCode&& a.Level<=lvl 
            && a.Semester==semester && a.IsActive==true).ToList();
            List<CourseDTO> dto = new List<CourseDTO>();
            if (courses.Count > 0)
            {
                foreach (var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CoursesToCourseDTOs(c));
                }
                return dto.OrderBy(c => c.CourseCode).ToList();
            }
            return dto;
        }
        public List<CourseDTO> PopulateCourse()
        {

            var courses = _unitOfWork.CourseRepository.GetAll().OrderBy(a=>a.Level).ToList();
            List<CourseDTO> dto = new List<CourseDTO>();
            if (courses.Count > 0)
            {
                foreach (var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CoursesToCourseDTOs(c));
                }
                return dto.OrderBy(c => c.CourseCode).ToList();
            }
            return dto;
        }
        
        public CourseDTO SaveCourse(CourseDTO course,string progCode, string userId)
        {
            string courseCode = course.CourseCode.ToUpper();
            
            if (string.IsNullOrEmpty(course.CourseId))//fresh course, Add
            {
                var dept = _unitOfWork.ProgrammeRepository.Get(course.ProgrammeCode);
                var currentSession = _unitOfWork.SessionRepository.GetFiltered(s => s.IsCurrent == true).FirstOrDefault();
                Course _cos = new Course();
                _cos.CourseCode = courseCode;
                _cos.Title =StandardGeneralOps.ToTitleCase(course.Title);
                _cos.ProgrammeCode = course.ProgrammeCode;
                _cos.Level = course.Level;
                _cos.Semester = course.Semester;
                _cos.CourseType = course.Type;
                _cos.CreditHours = course.CreditHours;
                _cos.DepartmentCode = dept.DepartmentCode;
                _cos.CourseId=currentSession.SessionId.ToString()+"-"+_cos.ProgrammeCode+"-" + courseCode.Replace(" ", "");
                _cos.IsActive = course.Active;
                _cos.Category = course.Category;

                //Check if courseid already exist;
                var existcourse = _unitOfWork.CourseRepository.Get(_cos.CourseId);
                if(existcourse!=null)
                { return new CourseDTO(); }
                _cos.InsertDate = DateTime.UtcNow;
                
                _unitOfWork.CourseRepository.Add(_cos);
                _unitOfWork.Commit(userId);
                return course;
            }
            else
            {
                var dbCourse = _unitOfWork.CourseRepository.Get(course.CourseId);
                dbCourse.CreditHours = course.CreditHours;
                dbCourse.IsActive = course.Active;
                dbCourse.Level = course.Level;
                dbCourse.Semester = course.Semester;
                dbCourse.Category = course.Category;
                
                _unitOfWork.Commit(userId);

                return course;
            }
        }

        #endregion

        #region USER DATA Services
        public UserData GetUserData()
        {
            var user = _unitOfWork.UserDataRepository.GetAll().FirstOrDefault();
            return user;
        }
        public UserData GetUserData1()
        {
            UnitOfWork db = new UnitOfWork();
            return db.UserDataRepository.GetAll().FirstOrDefault();
        }
        public string SaveUserData(UserData data,string userId)
        {
                _unitOfWork.UserDataRepository.Add(data);
                _unitOfWork.Commit(userId);
           
            return "School data saved";
        }
        public void AddImages2User(UserData data,string userid)
        {
            var dbU = _unitOfWork.UserDataRepository.GetAll().FirstOrDefault();
            if(dbU!=null)
            {
                if (data.Logo != null)
                {
                    dbU.Logo = data.Logo;
                }
                if (data.Regbanner != null)
                {
                    dbU.Regbanner = data.Regbanner;
                }
                if(data.RegFooter!=null)
                { dbU.RegFooter = data.RegFooter; }
                if (data.RegSign != null) { dbU.RegSign = data.RegSign; }

                _unitOfWork.Commit(userid);
            }
        }
        public ProgrammeTypes GetStudentProgrameType(string studentId)
        {
            var student = _unitOfWork.StudentRepository.GetFiltered(a=>a.PersonId==studentId||a.MatricNumber==studentId).FirstOrDefault();
            return _unitOfWork.ProgrammeTypeRepository.GetFiltered(a => a.Type == student.ProgrammeType).SingleOrDefault();
        }
        #endregion

        #region DATA ENCRIPTION/GATEWAY TRANSACTION LOGGING
        public string Sha512Hasher(string text)
        {
            string hash = "";
            using (SHA512 sha =SHA512.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                hash = builder.ToString();
            }
            return hash;
        }
        public string Sha256Hasher(string text)
        {
            string hash = "";
            using (SHA512 sha = SHA512.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                hash = builder.ToString();
            }
            return hash;
        }

        public PaymentGateways GetDefaultPaymentGateway(string progType)
        {
            return _unitOfWork.PaymentGatewaysRepository.GetFiltered(a=>a.ProgrammeTypeCode==progType
            && a.IsDefault==true).SingleOrDefault();
           
        }
        public List<PaymentGateways> GetAllPaymentGateway()
        {
            return _unitOfWork.PaymentGatewaysRepository.GetAll().ToList();
        }

        public void SaveGatewayTransactionLogs(GateWaylogs log, string serv)
        {

            _unitOfWork.GateWaylogsRepository.Add(log);
            _unitOfWork.Commit(serv);

        }
        #endregion

        #region OLEVEL SUBJECT
        public List<OLevelSubject> AllSubjects()
        {
            return _unitOfWork.OlevelSubjectRepository.GetAll()
                .OrderBy(s => s.Title).ToList();
        }
        public string AddSubject(string title)
        {
            string ntitle = StandardGeneralOps.ToTitleCase(title.Trim());
            var db = _unitOfWork.OlevelSubjectRepository.GetFiltered(a => a.Title == ntitle).FirstOrDefault();
            try
            {
                if (db == null)
                {
                    _unitOfWork.OlevelSubjectRepository.Add(new OLevelSubject
                    {
                        Title = ntitle
                    });
                }

                return "Subject added successfully";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

         
        #endregion

        #region LOG API
        public void SaveApiLog(ApiLog log)
        {
            UnitOfWork uow = new UnitOfWork();
            uow.ApiLogRepository.Add(log);
             
            uow.Commit("System");
        }
        #endregion
        #region Exams Officers
        public ExamsOfficer FetchActiveExamOfficer(string officerCode)
        {
            return _unitOfWork.ExamsOfficerRepository.GetFiltered(a => a.OfficerCode == officerCode).SingleOrDefault();
        }
        #endregion
    }
}
