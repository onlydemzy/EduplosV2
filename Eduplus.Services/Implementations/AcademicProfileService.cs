using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using Eduplus.ObjectMappings;
using Eduplus.Services.Contracts;
using Eduplus.Services.UtilityServices;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Eduplus.Services.Implementations
{
    public class AcademicProfileService:IAcademicProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public AcademicProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
             
        }
        #region COURSE REGISTRATION PROCESS
        public byte CheckIfStudentIsClearedToRegisterForCourse(string studentId,int semesterId)
        {
            
            var student = _unitOfWork.StudentRepository.Get(studentId);
            var semester = _unitOfWork.SemesterRepository.Get(semesterId);
            var reglog = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(a => a.SessionId == semester.SessionId && (a.StudentId == studentId
            ||a.Student.MatricNumber==studentId)).FirstOrDefault();
            //0 Can register
            //1 Fees payment not complete
            //2 Already registered
            //3 Portal Closed for registration
            //4 Student Should pay Late registration Fee
            //5 Something went wrong
            //Check student is qualified
            if (reglog == null)
                return 1;
            
            DateTime lt = (DateTime)semester.LateRegistrationEndDate;
            DateTime current = DateTime.UtcNow;

            if (lt.Date<current.Date)
            {
                return 3;
            }

            var alreadyreg = CheckIfStudentAlreadyRegisteredCourses(studentId, semester.SemesterId);
            if (alreadyreg == true)
            {
                return 2;
            }

            if(reglog.Register1==true && semester.SemesterTitle== "1st Semester")
            {
                /*if (DateTime.UtcNow.Date>=semester.LateRegistration1StartDate.Value.Date&&reglog.Late1Clear==false)
                {
                    return 4;
                }*/
                //else  
                return 0;
            }
            if (reglog.Register2 == true && semester.SemesterTitle == "2nd Semester")
            {
                /*if (DateTime.UtcNow.Date >= semester.LateRegistration1StartDate.Value.Date && reglog.Late2Clear == false)
                {
                    return 4;
                }*/
                    return 0;
            }
            if (reglog.Register3 == true && semester.SemesterTitle == "3rd Semester")
            {
                if (DateTime.UtcNow.Date >= semester.LateRegistrationStartDate.Date && reglog.Late3Clear == false)
                {
                    return 4;
                }
                else return 0;
            }

            else return 1;
        }
        
        bool CheckIfStudentAlreadyRegisteredCourses(string studentId,int semesterId)
        {


            var chck = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(s => s.StudentId == studentId && s.SemesterId == semesterId).FirstOrDefault();
            if (chck == null)
            { return false; }
            else return true;

        }
        
        public void AddRegistrationPermissionsLog(string studentId,int sessionId)
        {
            var log = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(a => a.StudentId == studentId && a.SessionId == sessionId).FirstOrDefault();
            if (log == null)
            {
                
                RegistrationsPermissionsLog nlog = new RegistrationsPermissionsLog();
                nlog.SessionId = sessionId;
                nlog.StudentId = studentId;
                nlog.Register1 = false;
                nlog.Register2 = false;
                nlog.Register3 = false;
                nlog.Write1= false;
                nlog.Write2 = false;
                nlog.Write3 = false;
                nlog.Late1Clear = false;
                nlog.Late2Clear = false;
                nlog.Late3Clear = false;
                _unitOfWork.RegistrationsPermissionsLogRepository.Add(nlog);
                _unitOfWork.Commit(studentId);
            }
                
            
            
        }
        public List<CourseRegistrationDTO> FetchCoursesToRegister(string studentId, int lvl,int? semesterId)
        {


            var courses = new List<CourseRegistrationDTO>();
            List<OutStandingCourse> ou = new List<OutStandingCourse>();
            Semester semester;
            if (semesterId > 0)
            {
                semester = _unitOfWork.SemesterRepository.Get((int)semesterId);
            }
            else
            {
                semester = _unitOfWork.SemesterRepository.GetFiltered(a=>a.IsCurrent==true).FirstOrDefault();
            }

            //Fetch outstanding courses if any
            //var osc = _unitOfWork.OutStandingCourseRepository.GetFiltered(o => o.Owing == true && o.StudentId == studentId &&
            // o.Course.Semester == semester.SemesterTitle).ToList();

            var osc = _unitOfWork.OutStandingCourseRepository.GetFiltered(o => o.Owing == true && (o.StudentId == studentId || o.Student.MatricNumber==studentId) &&
                        o.Course.Semester == semester.SemesterTitle)
                        .GroupBy(a=>new { a.StudentId,a.CourseId,a.Course })
                        .Select(a=>new OutStandingCourse { StudentId = a.Key.StudentId, CourseId = a.Key.CourseId,Course=a.Key.Course }).ToList();


            //Fetch Current Courses
            var student = _unitOfWork.StudentRepository.GetSingle(a=>a.PersonId==studentId || a.MatricNumber==studentId);
            var currentCourse = _unitOfWork.CourseRepository.GetFiltered(c => c.Semester == semester.SemesterTitle && c.Level == lvl && c.ProgrammeCode == student.ProgrammeCode
                     && c.IsActive == true).ToList();


            //Adding courses to RegList
            if (osc != null || osc.Count() > 0)
            {



                foreach (var o in osc)
                {
                    var dto = new CourseRegistrationDTO
                    {
                        CourseId = o.CourseId,
                        CourseCode = o.Course.CourseCode,
                        CreditHour = o.Course.CreditHours,
                        Title = o.Course.Title,
                        Level = o.Course.Level,
                        IsOutStanding = true,
                        SemesterId=semester.SemesterId,
                        SessionId=semester.SessionId,
                        ProgrammeCode=student.ProgrammeCode,
                        Type=o.Course.CourseType,
                        StudentId=studentId
                    };
                    courses.Add(dto);
                }
            }

            //ADDING CURRENT COURSES
            foreach (var c in currentCourse)
            {
                var cr = new CourseRegistrationDTO
                {
                    CourseId = c.CourseId,
                    CourseCode = c.CourseCode,
                    Title = c.Title,
                    CreditHour = c.CreditHours,
                    IsOutStanding = false,
                    Type = c.CourseType,
                    SemesterId = semester.SemesterId,
                    SessionId = semester.SessionId,
                    Level=c.Level,
                    ProgrammeCode = c.ProgrammeCode,
                    StudentId=studentId

                };
                courses.Add(cr);
            }

            return courses;
        }

        public List<CourseRegistrationDTO> FetchStudentRegisteredCourses(int semesterId, string matricNo)
        {
            var st = matricNo.ToUpper();
            var regs = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && (a.Student.MatricNumber == matricNo||a.StudentId==matricNo)).ToList();
            if (regs.Count == 0)
                return new List<CourseRegistrationDTO>();
            List<CourseRegistrationDTO> dto = new List<CourseRegistrationDTO>();
            foreach (var r in regs)
            {
                dto.Add(new CourseRegistrationDTO
                {
                    CourseCode = r.Course.CourseCode,
                    CourseId = r.CourseId,
                    Title = r.Course.Title,
                    RegistrationId = r.RegistrationId,
                    Level = r.Lvl,
                    CreditHour = r.CourseCreditHour,
                    SemesterId = r.SemesterId,
                    SessionId = r.SessionId,
                    Type = r.Course.CourseType,
                    ProgrammeCode = r.Course.ProgrammeCode,
                    StudentId = regs[0].StudentId
                });
            }
            return dto.OrderBy(a => a.CourseCode).ToList();
        }
        public List<CourseRegistrationDTO> AdditionalCoursesToRegister(string programmeCode, int lvl, string studentId,int? semesterId)
        {


            var courses = new List<CourseRegistrationDTO>();
            Semester semester;
            
            if(semesterId>0)
            {
                semester = _unitOfWork.SemesterRepository.Get((int)semesterId);
            }
            else
            {
                semester = _unitOfWork.SemesterRepository.GetFiltered(a=>a.IsCurrent==true).FirstOrDefault();
            }
            var student = _unitOfWork.StudentRepository.Get(studentId);
            //Fetch Current Courses
             

            var currentCourse = _unitOfWork.CourseRepository.GetFiltered(c => c.Semester == semester.SemesterTitle && c.Level <= lvl
                    && c.ProgrammeCode == student.ProgrammeCode).ToList();

            
            //ADDING CURRENT COURSES
            foreach (var c in currentCourse)
            {
                var cr = new CourseRegistrationDTO
                {
                    CourseId = c.CourseId,
                    CourseCode = c.CourseCode,
                    Title = c.Title,
                    CreditHour = c.CreditHours,
                    IsOutStanding = false,
                    Type = c.CourseType,
                    SemesterId = semester.SemesterId,
                    SessionId = semester.SessionId,
                    Level = lvl,
                    ProgrammeCode = c.ProgrammeCode,
                    StudentId = studentId

                };
                courses.Add(cr);
            }

            return courses.OrderBy(a=>a.CourseCode).ToList();
        }

        public string SubmitCourseRegistration(List<CourseRegistrationDTO> regCourses,List<CourseRegistrationDTO> outstandingCourses, string userId)
        {
            //Check if courses are ok
            var studentId = regCourses.FirstOrDefault().StudentId;
            var student = _unitOfWork.StudentRepository.GetSingle(a => a.PersonId == studentId
              || a.MatricNumber == studentId);
            if(outstandingCourses!=null)
            {
                //Get courses to add to Outstanding Courses 
                foreach (var c in outstandingCourses)
                {
                    if (c.Type!= "Elective")
                    {
                        
                            var ou = new OutStandingCourse
                            {
                                CourseId = c.CourseId,
                                SemesterId = c.SemesterId,
                                SessionId = c.SessionId,
                                StudentId = student.PersonId,
                                OwingType = "OutStanding",
                                Owing = true
                            };

                        _unitOfWork.OutStandingCourseRepository.Add(ou);
                      }
                    }
                }
            foreach(var c in regCourses)
            {
                var course = new CourseRegistration
                {
                    CourseId = c.CourseId,
                    CourseCreditHour = c.CreditHour,
                    Lvl = c.Level,
                    CA1 = 0,
                    CA2 = 0,
                    Exam = 0,
                    IsApproved = false,
                    StudentId = student.PersonId,
                    SemesterId=c.SemesterId,
                    SessionId=c.SessionId
                };
                _unitOfWork.CourseRegistrationRepository.Add(course);
            }
            //update studentlvl
            var co = regCourses.FirstOrDefault();
            
            student.CurrentLevel = co.Level;
            //Finally logging the entry
            var semester = _unitOfWork.SemesterRepository.Get(co.SemesterId);
            var semreg = new SemesterRegistrations
            {
                StudentId = student.PersonId,
                RegisteredDate = DateTime.UtcNow,
                Lvl = co.Level,
                SemesterId = co.SemesterId,
                Semester = semester.SemesterTitle,
                Session = semester.Session.Title,
                
            };
            _unitOfWork.SemesterRegistrationsRepository.Add(semreg);
            _unitOfWork.Commit(userId);

            return "Course Registration was succesful";
        }

        public StudentAcademicProfileDTO StudentSemesterCourseRegistration(int semesterId,string studentId)
        {
            var student = _unitOfWork.StudentRepository.GetSingle(a=>a.PersonId==studentId||a.MatricNumber==studentId);
            var course = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.StudentId == studentId && a.SemesterId == semesterId).ToList();
            var singleProf = course.FirstOrDefault();
            StudentAcademicProfileDTO profile = new StudentAcademicProfileDTO();
            profile.Department = student.Department.Title;
            profile.Faculty = student.Department.Faculty.Title;
            //profile.Gender = student.Sex;
            profile.Level = singleProf.Lvl;
            profile.Semester = singleProf.Semester.SemesterTitle;
            profile.Session = singleProf.Session.Title;
            profile.Programme = student.Programme.Title;
            profile.Name = student.Name;
            profile.ProgrammeType = student.ProgrammeType;
            profile.MatricNumber = student.MatricNumber;
            profile.TotalCreditUnit = course.Sum(s => s.CourseCreditHour);
            if(!string.IsNullOrEmpty(student.PhotoId))
            {
                profile.Photo = student.Photo.Foto;
            }
            else
            {
                profile.Photo = null;
            }

            //Add Details
            List<StudentAcademicProfileDetailstDTO> details = new List<StudentAcademicProfileDetailstDTO>();
            foreach (var d in course)
            {
                var detail = new StudentAcademicProfileDetailstDTO
                {
                    CourseCode = d.Course.CourseCode,
                    CourseTitle = d.Course.Title,
                    CreditHour = d.CourseCreditHour,
                    Type=d.Course.CourseType
                };
                details.Add(detail);
            }
            details.OrderBy(a => a.CourseCode).ToList();
            profile.Results = details;
            return profile;

        }
        public List<SemesterRegistrations> FetchStudentRegistrations(string studentId)
        {
            var regs = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.StudentId == studentId).ToList();
            return regs;
        }

        
        public string DeleteCourseFromCourseRegistration(CourseRegistrationDTO course,string userId)
        {
            var reg = _unitOfWork.CourseRegistrationRepository.Get(course.RegistrationId);
            if (reg != null)
            {
                _unitOfWork.CourseRegistrationRepository.Remove(reg);
                _unitOfWork.Commit(userId);
                return "Ok";
            }
            else
            {
               return "Error removing course";
            }
        }

        public string AddCourseToCourseRegistration(CourseRegistrationDTO course, string userId)
        {
            var reg = _unitOfWork.CourseRegistrationRepository.GetFiltered(c=>c.CourseId==course.CourseId&&c.SemesterId==course.SemesterId&&c.StudentId==course.StudentId)
                .FirstOrDefault();
            
            if (reg == null)
            {
                _unitOfWork.CourseRegistrationRepository.Add(new CourseRegistration
                {
                    StudentId=course.StudentId,SemesterId=course.SemesterId,SessionId=course.SessionId,
                    Lvl=course.Level,CA1=0,CA2=0,GradePoint=0,IsApproved=false,
                    CourseId=course.CourseId,CourseCreditHour=course.CreditHour,Exam=0
                });
                //Check if course was added to outstanding
                var outs = _unitOfWork.OutStandingCourseRepository.GetFiltered(c => c.CourseId == course.CourseId
                && c.SemesterId == course.SemesterId && c.StudentId == course.StudentId
                && c.OwingType=="OutStanding").FirstOrDefault();
                if (outs != null)
                {
                    _unitOfWork.OutStandingCourseRepository.Remove(outs);
                }
                _unitOfWork.Commit(userId);
                return "Ok";
            }
            else
            {
                return "Course already registered";
            }
        }
        public string DeleteCourseRegistration(string studentid,int semesterId,string userId)
        {
            var sem = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.StudentId == studentid && a.SemesterId == semesterId)
                .SingleOrDefault();
            var courseregs = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.StudentId == studentid && a.SemesterId == semesterId)
                .ToList();
            foreach(var i in courseregs)
            {
                _unitOfWork.CourseRegistrationRepository.Remove(i);
            }
            _unitOfWork.SemesterRegistrationsRepository.Remove(sem);
            //check if there was any dropped course
            var oust = _unitOfWork.OutStandingCourseRepository.GetFiltered(a => a.StudentId == studentid && a.SemesterId == semesterId).ToList();
            if (oust.Count > 0)
            {
                foreach(var o in oust)
                {
                    _unitOfWork.OutStandingCourseRepository.Remove(o);
                }
            }
            _unitOfWork.Commit(userId);
            return "Courseregistration successfully deleted";
        }
        #endregion


        #region RESULT COMPUTATIONS
        //Fetch Registered course for result scores entry
        public List<ScoresEntryDTO> FetchCoursesForScoreEntry(int semesterId, string courseId, int flag, out byte msg)
        {
            //check if scores already entered
            //1=Scores already entered
            //2=Fresh course to enter
            //0=No student registered for that course
            //3=Edit;
            //4=No scores to edit;
             
            var courses = _unitOfWork.CourseRegistrationRepository.GetFiltered(s => s.SemesterId == semesterId && s.CourseId == courseId).ToList();
            if (courses.Count == 0) {
                msg = 0;
                return new List<ScoresEntryDTO>();
            }
                List<ScoresEntryDTO> dto = new List<ScoresEntryDTO>();
                foreach (var c in courses)
                {
                    dto.Add(AcademicModuleMappings.CourseRegToScoresEntryDTO(c));
                }
            

            switch(flag)
            {
                case 1:
                    var fresh = dto.Where(a => string.IsNullOrEmpty(a.Grade)).ToList();
                    if (fresh.Count == 0)
                    {
                        msg = 1;
                        return new List<ScoresEntryDTO>();
                    }
                    else { msg = 2;
                        return fresh.OrderBy(a=>a.RegNo).ToList(); }
                    
                case 2:
                    var edits = dto.Where(a => !string.IsNullOrEmpty(a.Grade)).ToList();
                    if(edits.Count==0)
                    {
                        msg = 4;
                        return new List<ScoresEntryDTO>();
                    }
                    else
                    {
                        dto = edits;
                        msg = 3;
                        return edits.OrderBy(a => a.RegNo).ToList();
                    }
                     
            }

            msg = 0; return new List<ScoresEntryDTO>(0);
        }

        public List<ScoresEntryDTO> FetchBackLogScoresEntry(int sessionId,int semesterId, string courseId, string admitSessin,string progCode,int lvl,out byte msg)
        {
            
            //1=Scores already entered
            //2=Fresh course to enter
            //0=No student registered for that course
            

            var students = _unitOfWork.StudentRepository.GetFiltered(s => s.YearAddmitted == admitSessin&& s.ProgrammeCode==progCode && s.Status!="Prospective").ToList();
            if (students == null || students.Count == 0)
            {
                msg = 0;
                return new List<ScoresEntryDTO>();
            }
            //Fillter out students with already inputted scores
            List<string> stIds = new List<string>();
            foreach(var s in students)
            {
                stIds.Add(s.PersonId);
            }

            var exists = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.CourseId == courseId
              && a.SemesterId == semesterId && stIds.Contains(a.StudentId)).ToList();
            var course = _unitOfWork.CourseRepository.Get(courseId);
            List<ScoresEntryDTO> dto = new List<ScoresEntryDTO>();
            foreach (var s in students)
            {
                var exist = exists.Where(e => e.StudentId == s.PersonId).FirstOrDefault();
                if (exist == null) {
                    ScoresEntryDTO details = new ScoresEntryDTO();
                    details.StudentId = s.PersonId;
                    details.RegNo = s.MatricNumber;
                    details.SemesterId = semesterId;
                    details.SessionId = sessionId;
                    details.StudentLevel = lvl;
                    details.CourseId = courseId;
                    details.CourseCode = course.CourseCode;
                    details.CreditHour = course.CreditHours;
                    details.Title = course.Title;
                    dto.Add(details);
                }                
            }
            msg = 2;
            return dto.OrderBy(s => s.RegNo).ToList();
        }


        public bool CheckIfScoresEntryExist(int semesterId, string courseId)
        {
            var entry = _unitOfWork.ScoresEntryLogRepository.GetFiltered(e => e.SemesterId == semesterId && e.CourseId == courseId).FirstOrDefault();
            if (entry == null)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Submit Students semester scores
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="inputedBy"></param>
       
        public string SubmitScores(ScoresEntryDTO score, string inputedBy)
        {



            var reg = _unitOfWork.CourseRegistrationRepository.Get(score.RegistrationId);
            if (reg == null)
                return "Error inputting score. Course registrationID cannot be 0";
            if (!string.IsNullOrEmpty(reg.Grade))
            { return "Scores already inputted for  this student in the selected Semester and Course"; }
            

            var  prog = _unitOfWork.StudentRepository.Get(score.StudentId);
            var grades = _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == prog.ProgrammeType).ToList();
            var outstandings = _unitOfWork.OutStandingCourseRepository.GetFiltered(s =>s.StudentId==score.StudentId
            && s.CourseId == score.CourseId && s.Owing == true).ToList();
            //Updating the dbcourses with scores
            
            reg.CA1 = score.CA1;
            reg.CA2 = score.CA2;
            reg.Exam = score.Exam;
            if (score.IsIR == true)
            {
                reg.Grade = "I";
            }
            else
            {
                var grade = reg.CalculateGrade((reg.CA1 + reg.CA2 + reg.Exam), grades);
                reg.Grade = grade.Grade;
                reg.GradePoint = grade.GradePoint;
            }

            reg.IsApproved = false;

            //Add score to outstanding if any
            if (reg.Grade == "F")
            {
                OutStandingCourse ou = new OutStandingCourse();
                ou.CourseId = score.CourseId;
                ou.SemesterId = score.SemesterId;
                ou.SessionId = score.SessionId;
                ou.StudentId = score.StudentId;
                ou.OwingType = "Repeat";
                ou.Owing = true;

                _unitOfWork.OutStandingCourseRepository.Add(ou);
            }
            else //Check if student is writing as carryover.
                 //If yes, update outstanding course from owing to not owing
            {
                if (outstandings.Count > 0)
                {
                    
                        foreach (var o in outstandings)
                        {
                            if (reg.Grade != "I")
                            {
                                o.Owing = false;
                            }

                        }
                    
                }

            }

           
            _unitOfWork.Commit(inputedBy);
            return "Scores successfully saved";

        }

        public string SubmitScoresEdit(ScoresEntryDTO score, string inputedBy)
        {

            //try
            //{
            
            var prog = _unitOfWork.CourseRepository.Get(score.CourseId);
            var grades = _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == prog.Programme.ProgrammeType).ToList();
            var dbScore = _unitOfWork.CourseRegistrationRepository.Get(score.RegistrationId);
            if (dbScore == null)
                return "Invalid registrationId, operation terminated";
            dbScore.CA1 = score.CA1;
            dbScore.CA2 = score.CA2;
            dbScore.Exam = score.Exam;
            var grade = dbScore.CalculateGrade((score.CA1 + score.CA2 + score.Exam), grades);
            dbScore.Grade = grade.Grade;
            dbScore.GradePoint = grade.GradePoint;
            bool addCurent = false;
            if (grade.Grade == "F")
            {

                OutStandingCourse ou = new OutStandingCourse();
                ou.CourseId = score.CourseId;
                ou.SemesterId = score.SemesterId;
                ou.SessionId = score.SessionId;
                ou.StudentId = score.StudentId;
                ou.OwingType = "Repeat";
                ou.Owing = true;
                _unitOfWork.OutStandingCourseRepository.Add(ou);

            }
            var allOutstandings = _unitOfWork.OutStandingCourseRepository.GetFiltered(a => a.CourseId == score.CourseId && a.StudentId == score.StudentId
            && a.Owing == true).ToList();
            if (allOutstandings.Count>0)
            {
                // fetch for semester
                var semestaOut = allOutstandings.Where(a => a.SemesterId == score.SemesterId).ToList();//.SingleOrDefault();
                
                if (grade.Grade != "I" || grade.Grade != "Error")
                {

                    foreach (var o in allOutstandings)//convert all others to false
                    { o.Owing = false; }
                    if (semestaOut != null)//remove
                        { //_unitOfWork.OutStandingCourseRepository.Remove(semestaOut); 
                    }
                        
                
                }
            }
                
                _unitOfWork.Commit(inputedBy);
                return "Scores successfully updated";
 
        }
        public string SubmitBacklogScores(List<ScoresEntryDTO> scores,string inputedBy,bool isCarryOver)
        {
            //Check if scores already entered
            string msg;
             
            if (scores.Count > 0)
            {
                try
                {


                    var first = scores.FirstOrDefault();
                    var entry = _unitOfWork.ScoresEntryLogRepository.GetFiltered(e => e.SemesterId == first.SemesterId && e.CourseId == first.CourseId).FirstOrDefault();

                    if (isCarryOver != true && entry != null)//check if already inputted
                    {
                        return "Scores already entered for selected Course and Semester";
                    }
                    var prog = _unitOfWork.CourseRepository.Get(first.CourseId);
                    var grades = _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == prog.Programme.ProgrammeType).ToList();

                    //Updating the dbcourses with scores
                    foreach (ScoresEntryDTO r in scores)
                    {
                        CourseRegistration cr = new CourseRegistration();

                        cr.CA1 = r.CA1;
                        cr.CA2 = r.CA2;
                        cr.Exam = r.Exam;
                        cr.StudentId = r.StudentId;
                        cr.Lvl = r.StudentLevel;
                        cr.SessionId = r.SessionId;
                        cr.SemesterId = r.SemesterId;
                        cr.CourseCreditHour = r.CreditHour;
                        cr.CourseId = r.CourseId;
                        cr.IsApproved = false;
                        var grade = cr.CalculateGrade((r.CA1 + r.CA2 + r.Exam), grades);
                        if (grade == null)
                        {
                            throw new Exception("Invalid inputted score detected");
                        }
                        cr.Grade = grade.Grade;
                        cr.GradePoint = grade.GradePoint;


                        _unitOfWork.CourseRegistrationRepository.Add(cr);

                        //Add score to outstanding if any
                        if (cr.Grade == "F")
                        {
                            OutStandingCourse ou = new OutStandingCourse();
                            ou.CourseId = r.CourseId;
                            ou.SemesterId = r.SemesterId;
                            ou.SessionId = r.SessionId;
                            ou.StudentId = r.StudentId;
                            ou.OwingType = "Repeat";
                            ou.Owing = true;

                            _unitOfWork.OutStandingCourseRepository.Add(ou);
                        }

                        else
                        {
                            var oustanding = _unitOfWork.OutStandingCourseRepository.GetFiltered(o => o.CourseId == r.CourseId
                                    && o.StudentId == r.StudentId).ToList();
                            if (oustanding.Count > 0)
                            {
                                foreach (var o in oustanding)
                                {
                                    o.Owing = false;
                                }
                            }
                        }//Check if student is writing as carryover.
                         //If yes, update outstanding course from owing to not owing

                    }
                    //log entry
                    if (entry == null)
                    {
                        _unitOfWork.ScoresEntryLogRepository.Add(new ScoresEntryLog
                        {
                            CourseId = first.CourseId,
                            EnteredBy = inputedBy,
                            EntryDate = DateTime.UtcNow,
                            SemesterId = first.SemesterId,
                            SessionId = first.SessionId,
                            Status = "Inputted"
                        });

                    }
                    _unitOfWork.Commit(inputedBy);
                    return msg = "Scores successfully submitted";
                }
                catch(Exception ex)
                {
                    return ex.Message;

                }
            }
            else
            {
                return "Cannot insert blank scores";
            }
        }
        public string SubmitBacklogScores(ScoresEntryDTO score, string inputedBy)
        {
            //Check if scores already entered
            string msg;

            if (score!=null)
            {
                try
                {


                   
                    var prog = _unitOfWork.StudentRepository.Get(score.StudentId);
                    var grades = _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == prog.ProgrammeType).ToList();

                    CourseRegistration cr = new CourseRegistration();

                    cr.CA1 = score.CA1;
                    cr.CA2 = score.CA2;
                    cr.Exam = score.Exam;
                    cr.StudentId = score.StudentId;
                    cr.Lvl = score.StudentLevel;
                    cr.SessionId = score.SessionId;
                    cr.SemesterId = score.SemesterId;
                    cr.CourseCreditHour = score.CreditHour;
                    cr.CourseId = score.CourseId;
                    cr.IsApproved = false;
                    var grade = cr.CalculateGrade((score.CA1 + score.CA2 + score.Exam), grades);
                    if (grade == null)
                    {
                        throw new Exception("Invalid inputted score detected");
                    }
                    cr.Grade = grade.Grade;
                    cr.GradePoint = grade.GradePoint;


                    _unitOfWork.CourseRegistrationRepository.Add(cr);

                    //Add score to outstanding if any
                    if (cr.Grade == "F")
                    {
                        OutStandingCourse ou = new OutStandingCourse();
                        ou.CourseId = score.CourseId;
                        ou.SemesterId = score.SemesterId;
                        ou.SessionId = score.SessionId;
                        ou.StudentId = score.StudentId;
                        ou.OwingType = "Repeat";
                        ou.Owing = true;

                        _unitOfWork.OutStandingCourseRepository.Add(ou);
                    }

                    else
                    {
                        var outstandings = _unitOfWork.OutStandingCourseRepository.GetFiltered(o => o.CourseId == score.CourseId
                                && o.StudentId == score.StudentId&&o.Owing==true).ToList();
                        if(outstandings.Count > 0)
                          {

                            foreach (var o in outstandings)
                            {
                                if (score.Grade != "I")
                                {
                                    o.Owing = false;
                                }

                            }

                        }
                    }
                _unitOfWork.Commit(inputedBy);
                    return msg = "Scores successfully submitted";
                }
                catch (Exception ex)
                {
                    return ex.Message;

                }
            }
            else
            {
                return "Cannot insert blank scores";
            }
        }
        #endregion

        #region RESULT APPROVAL
        public string ApproveResults(int semesterId,int lvl,string prog,string userId)
        {
            if(lvl>0)
            {
                var res = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && a.Student.ProgrammeCode == prog
                && a.Lvl==lvl && a.IsApproved==false)
                    .ToList();
                if (res.Count > 0)
                {
                    foreach (var r in res)
                    {
                        r.IsApproved = true;
                    }
                    _unitOfWork.Commit(userId);
                    return "Result successfully approved";
                }
                else return "No result to approve for choosen semester, Level and programme";
            }
            else
            {
                var res = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && a.Student.ProgrammeCode == prog
                && a.IsApproved==false)
                    .ToList();
                if (res.Count > 0)
                {
                    foreach (var r in res)
                    {
                        r.IsApproved = true;
                    }
                    _unitOfWork.Commit(userId);
                    return "Result successfully approved";
                }
                else return "No result to approve for choosen semester and programme";
            }
        }
        #endregion

        #region RESULT COMPLAINS
        public string AddResultComplain(ResultComplainDTO dto, string userid,string username)
        {
            var sem = _unitOfWork.SemesterRepository.Get(dto.SemesterId);
            ResultComplain comp = new ResultComplain();
            List<ResultComplainDetail> dtails = new List<ResultComplainDetail>();
            var course = dto.Details[0].CourseId;
            var prg = _unitOfWork.CourseRepository.Get(course);

            comp.Complain = dto.Complain;
            comp.ProgrammeCode = prg.ProgrammeCode;
            comp.DepartmentCode = prg.DepartmentCode;
            comp.CourseLecturerFlag = false;
            comp.ExamsOfficer = dto.ExamsOfficer;
            comp.HODFlag = false;
            comp.InputtedDate = DateTime.UtcNow;
            comp.RaisedDate = dto.RaisedDate;
            comp.Semester = sem.SemesterTitle;
            comp.SemesterId = sem.SemesterId;
            comp.Session = sem.Session.Title;
            comp.SessionId = sem.SessionId;
            comp.Treated = false;
            comp.VCFlag = false;
             
            foreach(var d in dto.Details)
            {

                dtails.Add(new ResultComplainDetail
                {
                    CourseCode = d.CourseCode,
                    CourseId = d.CourseId,
                    MatricNumber = d.MatricNumber,
                    NewCA1 = d.NewCA1,
                    NewCA2 = d.NewCA2,
                    NewExam = d.NewExam,
                    OldCA1 = d.OldCA1,
                    OldCA2 = d.OldCA2,
                    OldExam = d.OldExam,
                    StudentId = d.StudentId

                });
            }
            comp.Details = dtails;
            _unitOfWork.ResultComplainRepository.Add(comp);
            _unitOfWork.Commit(userid);
            return "Complain lodged successfully. Contact necessary authorities for further action";
        }

        public List<CourseDTO> StudentOutstandingCourses(string StudentId)
        {
            return null;
        }
        public ResultComplainDTO FetchComplain(int complainId)
        {
            ResultComplainDTO comp = new ResultComplainDTO();
            List<ResultComplainDetailDTO> dtails = new List<ResultComplainDetailDTO>();
            var dto = _unitOfWork.ResultComplainRepository.Get(complainId);
            if (dto != null)
            {
                comp.Complain = dto.Complain;
                comp.ProgrammeCode = dto.ProgrammeCode;
                
                comp.CourseLecturerFlag = dto.CourseLecturerFlag;
                comp.CourseLecturerComment = dto.CourseLecturerComment;
                comp.ExamsOfficer = dto.ExamsOfficer;
                comp.VCComment = dto.VCComment;
                comp.VCFlag = dto.VCFlag;
                comp.VC = dto.VC;
                comp.Treated = dto.Treated;
                comp.HODFlag = dto.HODFlag;
                comp.HODComment = dto.HODComment;
                comp.InputtedDate = dto.InputtedDate;
                comp.RaisedDate = dto.RaisedDate;
                comp.Semester = dto.Semester;
                comp.SemesterId = dto.SemesterId;
                comp.Session = dto.Session;
                comp.SessionId = dto.SessionId;
                comp.HODFlagDate = dto.HODFlagDate;
                comp.VCFlagDate = dto.VCFlagDate;

                comp.VCFlag = false;

                foreach (var d in dto.Details)
                {

                    dtails.Add(new ResultComplainDetailDTO
                    {
                        CourseCode = d.CourseCode,
                        CourseId = d.CourseId,
                        MatricNumber = d.MatricNumber,
                        NewCA1 = d.NewCA1,
                        NewCA2 = d.NewCA2,
                        NewExam = d.NewExam,
                        OldCA1 = d.OldCA1,
                        OldCA2 = d.OldCA2,
                        OldExam = d.OldExam,
                        StudentId = d.StudentId

                    });
                }
                comp.Details = dtails;
                return comp;
            }
            else return null;
        }
        public List<ResultComplainDTO> ResultComplains(string staffId, string departmentCode)
        {
            if (!string.IsNullOrEmpty(staffId))
            {
                var complains = _unitOfWork.ResultComplainRepository.GetFiltered(a => a.LecturerId == staffId).ToList();
                if (complains.Count > 0)
                {
                    List<ResultComplainDTO> dl = new List<ResultComplainDTO>();
                    foreach (var c in complains)
                    {
                        dl.Add(new ResultComplainDTO
                        {
                            ComplainId = c.ComplainId,
                            Complain = c.Complain,
                            RaisedDate = c.RaisedDate,
                            Treated = c.Treated
                        });
                    }
                    return dl.OrderBy(a => a.RaisedDate).ToList();
                }
                else
                    return new List<ResultComplainDTO>();
            }
            else if (!string.IsNullOrEmpty(departmentCode))
            {
                var complains = _unitOfWork.ResultComplainRepository.GetFiltered(a => a.DepartmentCode == departmentCode).ToList();
                if (complains.Count > 0)
                {
                    List<ResultComplainDTO> dl = new List<ResultComplainDTO>();
                    foreach (var c in complains)
                    {
                        dl.Add(new ResultComplainDTO
                        {
                            ComplainId = c.ComplainId,
                            Complain = c.Complain,
                            RaisedDate = c.RaisedDate,
                            Treated = c.Treated
                        });
                    }
                    return dl.OrderBy(a => a.RaisedDate).ToList();
                }
                else
                    return new List<ResultComplainDTO>();
            }
            else return new List<ResultComplainDTO>();
        }
        public string LecturerCourseComplainFlag(string flaggingAuthority,string username,int complainId,string comment,bool flag,string userId)
        {
            var comp = _unitOfWork.ResultComplainRepository.Get(complainId);
            if (comp != null)
            {
                switch (flaggingAuthority)
                {
                    case "VC":
                        comp.VCComment = comment;
                        comp.VCFlag = flag;
                        comp.VC = username;
                        comp.VCFlagDate = DateTime.UtcNow;
                        break;
                    case "HOD":
                        comp.HODComment = comment;
                        comp.HODFlag = flag;
                        comp.HOD = username;
                        comp.HODFlagDate = DateTime.UtcNow;
                        if(comp.HODFlag==true)
                        {
                            RecomputeCorrectedResult(complainId, userId);
                        }
                        break;
                    case "Lecturer":
                        comp.CourseLecturerComment = comment;
                        comp.CourseLecturerFlag = flag;
                        comp.CourseLecturerFlagDate = DateTime.UtcNow;
                        break;
                }
                
                _unitOfWork.Commit(userId);
                return "Operation was successful";
            }
            else
                return "Error completing operation";
            
        }

        private void RecomputeCorrectedResult(int complainId,string userId)
        {
            var complain = _unitOfWork.ResultComplainRepository.Get(complainId);
            var progType = _unitOfWork.ProgrammeRepository.GetFiltered(a => a.ProgrammeCode == complain.ProgrammeCode).SingleOrDefault().ProgrammeType;
            var grades = _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == progType).ToList();
            foreach(var c in complain.Details.ToList())
            {
                var courseReg = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.StudentId == c.StudentId && a.CourseId == c.CourseId
                  && a.SemesterId == complain.SemesterId).First();

                courseReg.CA1 = c.NewCA1;
                courseReg.CA2 = c.NewCA2;
                courseReg.Exam = c.NewExam;
                var cgrades= courseReg.CalculateGrade((c.NewCA1 + c.NewCA2 + c.NewExam), grades);
                courseReg.Grade = cgrades.Grade;
                courseReg.GradePoint = cgrades.GradePoint;

                if(courseReg.Grade=="F")//Add to outstandingcourse
                {
                    _unitOfWork.OutStandingCourseRepository.Add(new OutStandingCourse
                    {
                        StudentId = c.StudentId,
                        SemesterId = complain.SemesterId,
                        SessionId = complain.SessionId,
                        CourseId = c.CourseId,
                        OwingType = "Repeat",
                        Owing = true
                    });
                }
                else //Check and remove from outstanding course if exist
                {
                    var outs = _unitOfWork.OutStandingCourseRepository.GetFiltered(a => a.StudentId == c.StudentId && a.CourseId == c.CourseId
                    &&a.SemesterId<=complain.SemesterId).ToList();
                    if(outs.Count>0)
                    {
                        foreach(var o in outs)
                        {
                            if(o.SemesterId==complain.SemesterId)//remove
                            {
                                _unitOfWork.OutStandingCourseRepository.Remove(o);
                            }
                            else
                            {
                                o.Owing = false;
                            }
                        }
                    }
                }
            }
            _unitOfWork.Commit(userId);
        }
        public InMemoryScoresDTO SingleStudentScore(int semesterId, string matricNumber, string courseId)
        {
            var courseRegistration = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && courseId == a.CourseId
              && a.Student.MatricNumber == matricNumber.ToUpper()).FirstOrDefault();
            if (courseRegistration == null)
                return null;
            return new InMemoryScoresDTO
            {
                CourseCode = courseRegistration.Course.CourseCode,
                CourseId = courseRegistration.CourseId,
                CA1 = courseRegistration.CA1,
                CA2 = courseRegistration.CA2,
                Exam = courseRegistration.Exam,
                Grade = courseRegistration.Grade,
                GP = courseRegistration.GradePoint,
                RegistrationId=courseRegistration.RegistrationId
            };
            
        }
         
        #endregion

            #region REPORT GENERATIONS

        public List<OutstandingCoursesDTO> FetchStudentOutstandings(string studentId)
        {
            var courses = _unitOfWork.OutStandingCourseRepository.GetFiltered(a => a.Owing == true && a.StudentId == studentId || a.Student.MatricNumber == studentId).ToList();
            List<OutstandingCoursesDTO> dto = new List<OutstandingCoursesDTO>();
            if(courses.Count==0)
            {
                return null;
            }
            List<string> coursIds = new List<string>();
            
            foreach(var c in courses)
            {
                OutstandingCoursesDTO ot = new OutstandingCoursesDTO();
                ot.CourseCode = c.Course.CourseCode;
                ot.Title = c.Course.Title;
                ot.OutStandingCourseId = c.OutStandingCourseId;
                ot.OwingType = c.OwingType;
                ot.Semester = c.Semester.SemesterTitle + ", " + c.Semester.Session.Title;
                coursIds.Add(c.CourseId);
                dto.Add(ot);
            }

            return dto.OrderBy(a => a.CourseCode).ToList();
        }
        public BroadSheetDTO FetchBroadSheet(string programmeCode, int sessionId, int semesterId, int level)
        {

            var singleComponents = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && a.Lvl == level && a.Student.ProgrammeCode == programmeCode).FirstOrDefault();
            if (singleComponents == null)
                return null;

            BroadSheetDTO dto = new BroadSheetDTO();
            dto.Department = singleComponents.Student.Department.Title;
            dto.Faculty = singleComponents.Student.Department.Faculty.Title;
            dto.Programme = singleComponents.Student.Programme.Title;
            dto.ProgrammeType = singleComponents.Student.ProgrammeType;
            dto.Semester = singleComponents.Semester.SemesterTitle;
            dto.Session = singleComponents.Session.Title;
            dto.Level = singleComponents.Lvl;
            
            List<InMemoryScoresDTO> scores = FetchRawScoresForInMemoryUse(programmeCode, level, semesterId).ToList();
            if(scores.Count==0)
            {
                return null;
            }
            

            List<string> studentIds = new List<string>();
            foreach(var i in scores)
            {
                studentIds.Add(i.StudentId);
            }
            //Peak students with basecgpa
            var baseCGPAStudents = _unitOfWork.StudentRepository.GetFiltered(a => studentIds.Contains(a.PersonId) && a.BaseCGPA >0).ToList();
            

            var semestergpa = CalculateGPA(scores, semesterId, 1);
            var semestercgpa = CalculateGPA(scores, semesterId, 2);
            if(baseCGPAStudents.Count>0)
            {
                foreach(var g in semestercgpa)
                {
                    var basegp = baseCGPAStudents.Where(s => s.MatricNumber == g.RegNo).FirstOrDefault();
                    if (basegp != null)
                    { g.GPA = String.Format("{0:0.00}", ((Convert.ToDouble(g.GPA) + (double)basegp.BaseCGPA) / 2)); }
                }
            }
            var outstandingsR = GetOutStanding_Repeat(studentIds, sessionId, semesterId, level);

            //Joining the 3 list above//////////////////////////////////////////

            var res = from gpa in semestergpa
                      join cgpa in semestercgpa
                      on gpa.RegNo equals cgpa.RegNo
                      join os in outstandingsR
                      on cgpa.RegNo equals os.RegNo into sub
                      from d in sub.DefaultIfEmpty()
                      select new
                      {
                          RegNo = gpa.RegNo,
                          CH = gpa.TotCreditHour,
                          GP = gpa.TotQualitPoints,
                          GPA = gpa.GPA,
                          CCH = cgpa.TotCreditHour,
                          CGP = cgpa.TotQualitPoints,
                          CGPA = cgpa.GPA,
                          OutStandings = (d == null ? String.Empty : d.Outstanding),
                          Repeat = (d == null ? String.Empty : d.Repeat)
                      };


            //Converting results to datatable
            DataTable cgpaOutstanding = DataTableManipulations.ConvertToDataTable(res.ToList());//Datatable 1
            List<StudentAcademicProfileDetailstDTO> bs = new List<StudentAcademicProfileDetailstDTO>(); 
            foreach(var s in scores)
            {
                StudentAcademicProfileDetailstDTO ap = new StudentAcademicProfileDetailstDTO();
                ap.Score = s.Score;
                ap.CreditHour = s.CreditHour;
                ap.Grade = s.Grade;
                ap.RegNo = s.MatricNumber;
                ap.RecordValue = s.RecordValue;
                ap.CreditHourHeading = s.CreditHourHeading;
                //ap.CourseCode = s.CourseCode;
                ap.SemesterId = s.SemesterId;
                ap.Level = s.Level;
                ap.SessionId = s.SessionId;
                bs.Add(ap);
            }

            DataTable scoresTable = DataTableManipulations.ScoresCreditHourSheet(bs.OrderBy(s=>s.CreditHourHeading).ToList(), sessionId, semesterId, level);//Datatable 2

            //Joining the two data tables (1&2)
            DataTable result = DataTableManipulations.JoinResultDataTables(scoresTable, cgpaOutstanding, (key1, key2) => key1.Field<string>("RegNo") == key2.Field<string>("RegNo"));
            dto.Results = result;
            return dto;
        }

        

        /// <summary>
        /// Fetch Individual student results
        /// </summary>
        /// <param name="programmeCode"></param>
        /// <param name="level"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public StudentAcademicProfileDTO FetchSingleSemesterResultForStudent(string studentId,int semesterId,string progCode, int flag)
        {
             
            string regNo = studentId.ToUpper();
            List<CourseRegistration> scores = new List<CourseRegistration>();
            if(!string.IsNullOrEmpty(progCode))
            {
                scores = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => (a.Student.MatricNumber == regNo || a.StudentId == regNo)
                    &&a.Student.ProgrammeCode==progCode && a.SemesterId <= semesterId).ToList();
            }
            else
            {
                scores = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => (a.Student.MatricNumber == regNo || a.StudentId == regNo)
                    && a.SemesterId <= semesterId).ToList();
            }
            List<CourseRegistration> currentScores = new List<CourseRegistration>();
            if (flag == 2)
            {
                currentScores = scores.Where(s => s.SemesterId == semesterId && s.IsApproved == true).ToList();
            }
            else
            { currentScores = scores.Where(s => s.SemesterId == semesterId).ToList(); }
            if (currentScores.Count == 0)

                return null;
           
          StudentAcademicProfileDTO dto = new StudentAcademicProfileDTO();
            List<StudentAcademicProfileDetailstDTO> details = new List<StudentAcademicProfileDetailstDTO>();
            var single = currentScores.First();
            dto.Department = single.Student.Department.Title;
            dto.Faculty = single.Student.Department.Faculty.Title;
            dto.Programme = single.Student.Programme.Title;
            dto.ProgrammeType = single.Student.ProgrammeType;
            dto.Name = single.Student.Name;
            dto.Semester = single.Semester.SemesterTitle;
            dto.Session = single.Session.Title;
            dto.Level = single.Lvl;
            dto.MatricNumber = single.Student.MatricNumber;
            dto.SessionAddmitted = single.Student.YearAddmitted;
            double baseCGPA = (double)single.Student.BaseCGPA;
            List<InMemoryScoresDTO> res = new List<InMemoryScoresDTO>();
            
            foreach(var s in scores)
            {
                res.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(s));
            }

                foreach(var r in res.Where(s=>s.SemesterId==semesterId))
                {
                    StudentAcademicProfileDetailstDTO acad = new StudentAcademicProfileDetailstDTO();
                acad.CourseCode = r.CourseCode;
                acad.CourseTitle = r.CourseTitle;
                acad.CreditHour = r.CreditHour;
                acad.CA1 = r.CA1;
                acad.CA2 = r.CA2;
                acad.Exam = r.Exam;
                acad.Score = r.Score;
                acad.Grade = r.Grade;

                details.Add(acad);
                }
                
                
                dto.TotalCreditUnit = details.Sum(s => s.CreditHour);
            dto.Results = details.OrderBy(a=>a.CourseCode).ToList();

                dto.GPA = SingleGPA(res, semesterId, single.StudentId, 1);
            if(baseCGPA>0)
            {
                dto.CGPA =String.Format("{0:0.00}", (Convert.ToDouble(SingleGPA(res, semesterId, single.StudentId, 2)) + baseCGPA) / 2);
            }
            else {dto.CGPA = SingleGPA(res, semesterId, single.StudentId, 2); }
              
                return dto;
          
        }

        public List<StudentAcademicProfileDTO> SemesterResultForDepartment(int semesterId,int lvl,string progCode)
        {

            var prog = _unitOfWork.ProgrammeRepository.Get(progCode);
            var scores = FetchRawScoresForInMemoryUse(progCode, lvl, semesterId);
            List <string> studentIds= scores.GroupBy(a => a.StudentId).Select(r => r.Key).ToList();
            List<StudentAcademicProfileDTO> results = new List<StudentAcademicProfileDTO>();
               
            if (scores.Count == 0)
            {

                return new List<StudentAcademicProfileDTO>();
            }
            else
            {
                foreach(string id in studentIds)
                {
                    var single =scores.Where(s=>s.StudentId==id&&s.SemesterId==semesterId).FirstOrDefault();
                    
                    StudentAcademicProfileDTO sap = new StudentAcademicProfileDTO();
                    sap.Department = single.Department;
                    sap.Faculty = single.Faculty;
                        sap.Programme = single.Programme;
                        sap.Name = single.Name;
                        sap.Semester = single.Semester;
                        sap.Session = single.Session;
                        sap.Level = single.Level;
                        sap.MatricNumber = single.MatricNumber;
                        sap.SessionAddmitted = single.YearAdmitted;
                        sap.StudentId = single.StudentId;
                     
                    List<StudentAcademicProfileDetailstDTO> details = new List<StudentAcademicProfileDetailstDTO>();
                    //Fetch result for this student
                    var studentScores = scores.Where(s => s.StudentId == id && s.SemesterId == semesterId).ToList();
                        foreach (var r in studentScores)
                        {
                            StudentAcademicProfileDetailstDTO res = new StudentAcademicProfileDetailstDTO();
                            res.CourseCode = r.CourseCode;
                            res.CourseTitle = r.CourseTitle;
                            res.CreditHour = r.CreditHour;
                            res.CA1 = r.CA1;
                        res.CA2 = r.CA2;
                            res.Exam = r.Exam;
                            res.Score = r.Score;
                            res.Grade = r.Grade;
                            details.Add(res);
                        }

                        sap.TotalCreditUnit = details.Sum(s => s.CreditHour);

                        sap.GPA = SingleGPA(scores, single.SemesterId, sap.StudentId, 1);

                    if (single.BaseCGPA > 0)
                    {
                        sap.CGPA = String.Format("{0:0.00}", (Convert.ToDouble(SingleGPA(scores, single.SemesterId, sap.StudentId, 2)) + single.BaseCGPA) / 2);
                    }
                    else { sap.CGPA = SingleGPA(scores, single.SemesterId, sap.StudentId, 2); }
                    
                        sap.Results = details.OrderBy(a=>a.CourseCode).ToList();

                        results.Add(sap);
                    }
                }
                
               
                return results;
         
        }

        //===========================Lagacy Transcript Generation==============================
        public TranscriptDTO FetchStudentTranscript(string matricNumber)
        {
            //Fetch student results

            string regNo = matricNumber.ToUpper();
            var scores = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.Student.MatricNumber == regNo || a.StudentId == regNo).ToList();

            if (scores.Count == 0)
            {
                return null;
            }
            //Transcript Titles
            var single = scores.First();
            TranscriptDTO transcript = new TranscriptDTO();
            transcript.Name = single.Student.Name;
            transcript.Department = single.Student.Department.Title;
            transcript.Faculty = single.Student.Department.Faculty.Title;
            transcript.Programme = single.Student.Programme.Title;
            transcript.StudentId = single.StudentId;
            transcript.RegNo = single.Student.MatricNumber;
            transcript.Gender = single.Student.Sex;
            transcript.YearAdmitted = single.Student.YearAddmitted;
            transcript.Duration = single.Student.Duration;
            transcript.EntryMode = single.Student.EntryMode;
            transcript.ProgrammeType = single.Student.ProgrammeType;
            //transcript.Photo = single.Student.Phone;
            var baseCGPA = single.Student.BaseCGPA;

            var gradClasses = _unitOfWork.GraduatingClassRepository.GetFiltered(a => a.ProgrammeType == transcript.ProgrammeType).ToList();
            List<InMemoryScoresDTO> results = new List<InMemoryScoresDTO>();
            foreach (var s in scores)
            {
                results.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(s));
            }

            results = results.OrderBy(s => s.SemesterId).ToList();

            double degclass;
            //Obtain Student's overall cgpa
            double cgpa = (double)(results.Where(c => c.Grade != null || c.Grade != "I").Sum(q => q.QP)) / results.Where(c => c.Grade != null).Sum(c => c.CreditHour);
            if (baseCGPA > 0)
            {
                degclass = (cgpa + (double)baseCGPA) / 2;
                transcript.CGPA = String.Format("{0:0.00}", degclass);
            }
            else
            {
                degclass = cgpa;
                transcript.CGPA = String.Format("{0:0.00}", degclass);
            }


            transcript.DegreeClass = GetQualification(degclass, gradClasses);
            // Fetch All Semesters the Student had Registered for Course(s)
            List<int> semesterIds = results.GroupBy(r => r.SemesterId).Select(s => s.Key).ToList();
            List<TranscriptSemesterHeading> semHeadings = new List<TranscriptSemesterHeading>();
            //Populate all semester Headings

            foreach (var id in semesterIds)
            {

                var semesterRes = results.Where(s => s.SemesterId == id).ToList();
                //SemesterHeadings
                var singleSemRes = semesterRes.First();
                //Assign values to transcript Headings
                TranscriptSemesterHeading th = new TranscriptSemesterHeading();
                th.SemesterId = singleSemRes.SemesterId;
                th.CreditUnit = semesterRes.Sum(a => a.CreditHour);
                th.Title = singleSemRes.Semester + ", " + singleSemRes.Session + " (" + singleSemRes.Level.ToString() + ")";
                th.GPA = SingleGPA(semesterRes, singleSemRes.SemesterId, singleSemRes.StudentId, 1);

                if (baseCGPA > 0)
                {
                    var currentCGPA = Convert.ToDouble(SingleGPA(results.ToList(), singleSemRes.SemesterId, singleSemRes.StudentId, 2));
                    th.CGPA = String.Format("{0:0.00}", (baseCGPA + currentCGPA) / 2);
                }
                else
                {
                    th.CGPA = SingleGPA(results.ToList(), singleSemRes.SemesterId, singleSemRes.StudentId, 2);
                }


                //Populate Courses for the semester
                List<TranscriptScoresDTO> tsd = new List<TranscriptScoresDTO>();
                foreach (var s in semesterRes)
                {
                    TranscriptScoresDTO ts = new TranscriptScoresDTO();
                    ts.CourseCode = s.CourseCode;
                    ts.CourseTitle = s.CourseTitle;
                    ts.CHr = s.CreditHour;
                    ts.CA1 = s.CA1;
                    ts.CA2 = s.CA2;
                    ts.Exam = s.Exam;
                    ts.Score = s.Score;
                    ts.Grade = s.Grade;
                    ts.GradePoint = s.GP;
                    ts.QualityPoint = s.QP;

                    tsd.Add(ts);
                }
                th.SemesterResults = tsd;
                semHeadings.Add(th);
            }
            transcript.SemesterSummaries = semHeadings;

            return transcript;
        }
        //===========================End Legacy============================

        //Transcript Generation
        public TranscriptDTO FetchStudentAcademicProfile(string recordNo,bool isTranscript)
        {
            //Fetch student results
            string stId;
             
            if (isTranscript == true)
            {
                var trans=_unitOfWork.TranscriptRepository.GetFiltered(f => f.TranscriptNo == recordNo).SingleOrDefault();
                if (trans == null)
                    return null;
                else stId = trans.StudentId;

            }
            else
            {
                var student = _unitOfWork.StudentRepository.GetFiltered(a => a.MatricNumber == recordNo).FirstOrDefault();
                if (student == null)
                    return null;
                else stId = student.PersonId;
            }
            
             var scores = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.StudentId == stId).ToList();
            
            if (scores.Count==0)
            {
                return null;
            }
            //Transcript Titles
            var single = scores.First();
            TranscriptDTO transcript = new TranscriptDTO();
            transcript.Name = single.Student.Name;
            transcript.Department = single.Student.Department.Title;
            transcript.Faculty = single.Student.Department.Faculty.Title;
            transcript.Programme = single.Student.Programme.Title;
            transcript.StudentId = single.StudentId;
            transcript.RegNo = single.Student.MatricNumber;
            transcript.Gender = single.Student.Sex;
            transcript.YearAdmitted = single.Student.YearAddmitted;
            transcript.Duration = single.Student.Duration;
            transcript.EntryMode = single.Student.EntryMode;
            transcript.ProgrammeType = single.Student.ProgrammeType;
            if (!string.IsNullOrEmpty(single.Student.PhotoId))
            {
                transcript.Photo = single.Student.Photo.Foto;
            }
            
            var baseCGPA = single.Student.BaseCGPA;
            

            var gradClasses = _unitOfWork.GraduatingClassRepository.GetFiltered(a => a.ProgrammeType == transcript.ProgrammeType).ToList();
            List<InMemoryScoresDTO> results = new List<InMemoryScoresDTO>();
            foreach(var s in scores)
            {
                results.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(s));
            }

            results = results.OrderBy(s => s.SemesterId).ToList();

            var chkGP = (results.Where(c => c.Grade != null || c.Grade != "I").Sum(q => q.QP));
            var chkCH= results.Where(c => c.Grade != null || c.Grade != "I").Sum(c => c.CreditHour);
            double degclass;
            //Obtain Student's overall cgpa
            double cgpa= (double)(results.Where(c => c.Grade != null || c.Grade!="I").Sum(q => q.QP)) / results.Where(c => c.Grade != null).Sum(c => c.CreditHour);
            if(baseCGPA>0)
            {
                degclass = (cgpa + (double)baseCGPA) / 2;
                transcript.CGPA= String.Format("{0:0.00}",degclass );
            }
            else
            {
                degclass = cgpa;
                transcript.CGPA = String.Format("{0:0.00}",degclass );
            }
            

            transcript.DegreeClass = GetQualification(degclass,gradClasses);
            // Fetch All Semesters the Student had Registered for Course(s)
            List<int> semesterIds = results.GroupBy(r => r.SemesterId).Select(s => s.Key).ToList();
            List<TranscriptSemesterHeading> semHeadings = new List<TranscriptSemesterHeading>();
            //Populate all semester Headings

            foreach (var id in semesterIds)
            {

                var semesterRes = results.Where(s => s.SemesterId == id).ToList();
                //SemesterHeadings
                var singleSemRes = semesterRes.First();
                //Assign values to transcript Headings
                TranscriptSemesterHeading th = new TranscriptSemesterHeading();
                th.SemesterId = singleSemRes.SemesterId;
                th.CreditUnit = semesterRes.Sum(a => a.CreditHour);
                th.Title = singleSemRes.Semester + ", " + singleSemRes.Session + " (" + singleSemRes.Level.ToString() + ")";
                th.GPA = SingleGPA(semesterRes, singleSemRes.SemesterId, singleSemRes.StudentId, 1);

                if(baseCGPA>0)
                {
                    var currentCGPA=Convert.ToDouble(SingleGPA(results.ToList(), singleSemRes.SemesterId, singleSemRes.StudentId, 2));
                    th.CGPA=String.Format("{0:0.00}",(baseCGPA+currentCGPA)/2);
                }
                else
                {
                    th.CGPA = SingleGPA(results.ToList(), singleSemRes.SemesterId, singleSemRes.StudentId, 2);
                }
                

                //Populate Courses for the semester
                List<TranscriptScoresDTO> tsd = new List<TranscriptScoresDTO>();
                foreach(var s in semesterRes)
                {
                    TranscriptScoresDTO ts = new TranscriptScoresDTO();
                    ts.CourseCode = s.CourseCode;
                    ts.CourseTitle = s.CourseTitle;
                    ts.CHr = s.CreditHour;
                    ts.CA1 = s.CA1;
                    ts.CA2 = s.CA2;
                    ts.Exam = s.Exam;
                    ts.Score = s.Score;
                    ts.Grade = s.Grade;
                    ts.GradePoint = s.GP;
                    ts.QualityPoint = s.QP;

                    tsd.Add(ts);
                }
                th.SemesterResults = tsd.OrderBy(a=>a.CourseCode).ToList();
                semHeadings.Add(th);
            }
            transcript.SemesterSummaries = semHeadings;

            List<DegreeClassDTO> degs = new List<DegreeClassDTO>();
            foreach(var d in gradClasses.OrderBy(a=>a.Low))
            {
                degs.Add(new DegreeClassDTO
                {
                    Low = d.Low,
                    High = d.High,
                    Remark = d.Remark
                });
            }
            transcript.GradClass = degs;
            return transcript;
        }

        #endregion

        #region ACADEMIC PERFORMANCES
        /// <summary>
        /// Generate Students under Probations for 
        /// All students within a programe,lvl and semester
        /// </summary>
        /// <param name="cgpa"></param>
        /// <returns></returns>
        public ProbationDTO GenerateStudentsOnProbation(int sessionId, string progCode, byte flag)
        {
            //Fetch Students Offering the programe
            var students = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.Student.ProgrammeCode == progCode && a.SessionId == sessionId)
                .ToList();
            if (students.Count == 0)
                return new ProbationDTO();
            //Fetch Student Details
            ProbationDTO dto = new ProbationDTO();
            var probs = students.First();
            dto.Department = probs.Student.Department.Title;
            dto.Programme = probs.Student.Programme.Title;
            dto.ProgrammeType = probs.Student.ProgrammeType;
            dto.Session = probs.Session.Title;

            //Fetch studentIds
            List<string> ids = students.GroupBy(a => a.StudentId).Select(r => r.Key).ToList();

            var results = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => ids.Contains(a.StudentId)).ToList();

            //Convert Result to Usable Item
            List<InMemoryScoresDTO> usable = new List<InMemoryScoresDTO>();
            foreach (var r in results)
            {
                usable.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(r));
            }
            List<ProbationDetailsDTO> details = new List<ProbationDetailsDTO>();

            foreach (var c in ids)
            {
                var indvResult = usable.Where(a => a.StudentId == c);
                var studentInfo = indvResult.First();
                ProbationDetailsDTO detail = new ProbationDetailsDTO();
                detail.Name = StandardGeneralOps.ToTitleCase(studentInfo.Name);
                detail.MatricNumber = studentInfo.MatricNumber;
                detail.StudentId = studentInfo.StudentId;

                var currentCGPA = ((double)indvResult.Where(r => r.Grade != null || r.Grade != "I").Sum(q => q.QP) / indvResult.Where(r => r.Grade != null).Sum(r => r.CreditHour));
                if (studentInfo.BaseCGPA > 0)
                {
                    detail.CGPA = String.Format("{0:0.00}", (studentInfo.BaseCGPA + currentCGPA) / 2);
                }
                else { detail.CGPA = String.Format("{0:0.00}", currentCGPA); }
                detail.Level = indvResult.Where(a => a.SessionId == sessionId).First().Level;

                details.Add(detail);
            }
            int count = 0;
            switch (flag)
            {
                case 1://General Performance
                    var transform = details.OrderByDescending(b => b.CGPA).ToList();

                    foreach (var t in transform)
                    {
                        t.Count = count + 1;
                        count++;
                    }
                    dto.Details = transform;
                    return dto;
                case 2://Students on Probation
                    var war = details.Where(d => Convert.ToDouble(d.CGPA) < 1.5).OrderBy(a => a.Level).ToList();

                    foreach (var t in war)
                    {
                        t.Count = count + 1;
                        count++;
                    }
                    dto.Details = war;
                    return dto;
                default:
                    return new ProbationDTO();
            }

        }


        /// <summary>
        /// Get Students that are qualified to graduate
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public ProbationDTO GraduatedStudent(string session, string progCode)
        {
            var students = _unitOfWork.StudentRepository.GetFiltered(s => s.GradYear == session && s.ProgrammeCode == progCode)
                .ToList();
            if (students.Count == 0)
                return new ProbationDTO();
            //Fetch Student Details
            ProbationDTO dto = new ProbationDTO();
            var probs = students.First();
            dto.Department = probs.Department.Title;
            dto.Programme = probs.Programme.Title;
            dto.ProgrammeType = probs.ProgrammeType;
            dto.Session = probs.GradYear;
            //Fetch studentIds
            List<string> ids = students.Select(r => r.PersonId).ToList();

            var results = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => ids.Contains(a.StudentId)).ToList();
            var gradClass = _unitOfWork.GraduatingClassRepository.GetFiltered(a => a.ProgrammeType == dto.ProgrammeType).ToList();
            //Convert Result to Usable Item
            List<InMemoryScoresDTO> usable = new List<InMemoryScoresDTO>();
            foreach (var s in results)
            {
                usable.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(s));
            }
            List<ProbationDetailsDTO> details = new List<ProbationDetailsDTO>();

            foreach (var c in ids)
            {
                var indvResult = usable.Where(a => a.StudentId == c);

                var studentInfo = indvResult.First();
                ProbationDetailsDTO detail = new ProbationDetailsDTO();
                detail.Name = StandardGeneralOps.ToTitleCase(studentInfo.Name);
                detail.MatricNumber = studentInfo.MatricNumber;
                detail.StudentId = studentInfo.StudentId;
                double baseCGPA = studentInfo.BaseCGPA;
                double cgpa = (double)indvResult.Sum(q => q.QP) / (double)indvResult.Sum(r => r.CreditHour);
                if (baseCGPA > 0)
                {
                    detail.CGPA = String.Format("{0:0.00}", (cgpa + baseCGPA) / 2);
                }
                else { detail.CGPA = String.Format("{0:0.00}", cgpa); }
                detail.Qualification = GetQualification(cgpa, gradClass);
                details.Add(detail);

            }
            var transform = details.OrderByDescending(b => b.CGPA).ToList();
            int count = 0;
            foreach (var t in transform)
            {
                t.Count = count + 1;
                count++;
            }
            dto.Details = transform;
            return dto;

        }

        public List<ProbationDetailsDTO> StudentsDueForGraduation(string progCode)
        {
             
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.ProgrammeCode == progCode &&
                a.CurrentLevel >= a.Programme.GradLevel && a.Status=="Active")
                .ToList();
            if (students.Count == 0)
                return new List<ProbationDetailsDTO>();
            var frst = students.First();
            var gradClass = _unitOfWork.GraduatingClassRepository.GetFiltered(a => a.ProgrammeType == frst.ProgrammeType).ToList();
            //Fetch Student Details
            List<ProbationDetailsDTO> grads = new List<ProbationDetailsDTO>();
            //Fetch studentIds
            List<string> ids = students.Select(r => r.PersonId).ToList();
            //Fetch Outstanding Courses
            List<string> outstandingIds = _unitOfWork.OutStandingCourseRepository.GetFiltered(o => ids.Contains(o.StudentId) && o.Owing == true)
               .GroupBy(a => a.StudentId).Select(b => b.Key).ToList();
            //Fetch their Results
            var results = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => ids.Contains(a.StudentId)).ToList();

            //Convert Result to Usable Item
            List<InMemoryScoresDTO> usable = new List<InMemoryScoresDTO>();
            foreach (var s in results)
            {
                usable.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(s));
            }

            foreach (var c in ids)
            {
                //Return result of Student in qeue
                var indvResult = usable.Where(a => a.StudentId == c).ToList();
                double cgpa = 0;
                //Add only students with complete results
                var studentInfo = indvResult.First();
                ProbationDetailsDTO detail = new ProbationDetailsDTO();
                detail.Name = StandardGeneralOps.ToTitleCase(studentInfo.Name);
                detail.MatricNumber = studentInfo.MatricNumber;
                bool clearedBursary = StudentClearedBursary(c);
                detail.StudentId = studentInfo.StudentId;
                double baseCGPA = studentInfo.BaseCGPA;
                cgpa = (double)indvResult.Where(i=>i.Grade!=null ||i.Grade!="I").Sum(q => q.QP) / indvResult.Where(i => i.Grade != null || i.Grade != "I").Sum(r => r.CreditHour);

                if (baseCGPA > 0)
                {
                    detail.CGPA = String.Format("{0:0.00}", (cgpa + baseCGPA) / 2);
                }
                else { detail.CGPA = String.Format("{0:0.00}", cgpa); }

                detail.Qualification = GetQualification(cgpa, gradClass);

                if (indvResult.Any(a => string.IsNullOrEmpty(a.Grade)) || outstandingIds.Contains(c))
                {
                    detail.Status = "Incomplete Result";
                    detail.Qualified = false;

                }
                else
                {
                    detail.Status = "Qualified";
                    detail.Qualified = true;

                }
                /* Including bursary clearance
                if (clearedBursary==false)
                {
                    detail.Status = "Still owing Fee(s)";
                    detail.Qualified = false;
                }
                else
                {
                    detail.Status = "Qualified";
                    detail.Qualified = true;
                }*/
                grads.Add(detail);
            }
            var transform = grads.OrderByDescending(b => b.CGPA).ToList();
            int count = 0;
            foreach (var t in transform)
            {
                t.Count = count + 1;
                count++;
            }

            return transform;
        }

        public BroadSheetDTO FetchGraduantsBroadSheet(string programmeCode, int sessionId, int semesterId, int level, string admittedSession, string gradYr, string batch)
        {


            List<InMemoryScoresDTO> scores = FetchRawScoresForInMemoryUseGraduated(programmeCode, level, semesterId, gradYr, batch);


            var singleComponents = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && a.Lvl == level && a.Student.ProgrammeCode == programmeCode && a.Student.Status == "Graduated" && a.Student.YearAddmitted == admittedSession && a.Student.GradYear == gradYr && a.Student.GradBatch == batch).FirstOrDefault();
            if (singleComponents == null)
                return new BroadSheetDTO();

            BroadSheetDTO dto = new BroadSheetDTO();
            dto.Department = singleComponents.Student.Department.Title;
            dto.Faculty = singleComponents.Student.Department.Faculty.Title;
            dto.Programme = singleComponents.Student.Programme.Title;
            dto.Semester = singleComponents.Semester.SemesterTitle;
            dto.Session = singleComponents.Session.Title;
            dto.Level = singleComponents.Lvl;
            dto.ProgrammeType = singleComponents.Student.ProgrammeType;
            List<string> studentIds = new List<string>();
            foreach (var i in scores)
            {
                studentIds.Add(i.StudentId);
            }
            //Peak students with basecgpa
            var baseCGPAStudents = _unitOfWork.StudentRepository.GetFiltered(a => studentIds.Contains(a.PersonId) && a.BaseCGPA > 0).ToList();


            var semestergpa = CalculateGPA(scores, semesterId, 1);
            var semestercgpa = CalculateGPA(scores, semesterId, 2);
            if (baseCGPAStudents.Count > 0)
            {
                foreach (var g in semestercgpa)
                {
                    var basegp = baseCGPAStudents.Where(s => s.MatricNumber == g.RegNo).FirstOrDefault();
                    if (basegp != null)
                    { g.GPA = String.Format("{0:0.00}", ((Convert.ToDouble(g.GPA) + (double)basegp.BaseCGPA) / 2)); }
                }
            }



            var outstandingsR = GetOutStanding_Repeat(studentIds, sessionId, semesterId, level);

            //Joining the 3 list above//////////////////////////////////////////

            var res = from gpa in semestergpa
                      join cgpa in semestercgpa
                      on gpa.RegNo equals cgpa.RegNo
                      join os in outstandingsR
                      on cgpa.RegNo equals os.RegNo 
                      into sub
                      from d in sub.DefaultIfEmpty()
                      select new
                      {
                          RegNo = gpa.RegNo,
                          CH = gpa.TotCreditHour,
                          GP = gpa.TotQualitPoints,
                          GPA = gpa.GPA,
                          CCH = cgpa.TotCreditHour,
                          CGP = cgpa.TotQualitPoints,
                          CGPA = cgpa.GPA,
                          OutStandings = (d == null ? String.Empty : d.Outstanding),
                          Repeat = (d == null ? String.Empty : d.Repeat)
                      };


            //Converting results to datatable
            DataTable cgpaOutstanding = DataTableManipulations.ConvertToDataTable(res.ToList());//Datatable 1
            List<StudentAcademicProfileDetailstDTO> bs = new List<StudentAcademicProfileDetailstDTO>();
            foreach (var s in scores)
            {
                StudentAcademicProfileDetailstDTO ap = new StudentAcademicProfileDetailstDTO();
                ap.Score = s.Score;
                ap.CreditHour = s.CreditHour;
                if (ap.Grade == "I")
                {

                }
                ap.Grade = s.Grade;
                ap.RegNo = s.MatricNumber;
                ap.RecordValue = s.RecordValue;
                ap.CreditHourHeading = s.CreditHourHeading;
                //ap.CourseCode = s.CourseCode;
                ap.SemesterId = s.SemesterId;
                ap.Level = s.Level;
                ap.SessionId = s.SessionId;
                bs.Add(ap);
            }

            DataTable scoresTable = DataTableManipulations.ScoresCreditHourSheet(bs.OrderBy(a => a.CreditHourHeading).ToList(), sessionId, semesterId, level);//Datatable 2

            //Joining the two data tables (1&2)
            DataTable result = DataTableManipulations.JoinResultDataTables(scoresTable, cgpaOutstanding, (key1, key2) => key1.Field<string>("RegNo") == key2.Field<string>("RegNo"));
            dto.Results = result;
            return dto;
        }
        public string GraduateAcademicStudent(List<ProbationDetailsDTO> grads, string sessionId, string batch,string userId)
        {
            ProbationDetailsDTO v = new ProbationDetailsDTO();

            //filter out 
            List<string> ids = grads.Where(g => g.Graduate == true).Select(r => r.StudentId).ToList();
            var dbStudents = _unitOfWork.StudentRepository.GetFiltered(g => ids.Contains(g.PersonId)).ToList();
            if (dbStudents.Count == 0)
                return "Error graduating students, try again later";

            foreach (Student s in dbStudents)
            {
                s.Status = "Graduated";
                s.GradYear = sessionId;
                s.GradBatch = batch;
            }
            _unitOfWork.Commit(userId);
            return "Students Successfuly Graduated";
        }

        public BroadSheetDTO FetchGraduantsSummary(string programmeCode,  string gradYr, string batch)
        {
           
            var students = _unitOfWork.StudentRepository.GetFiltered(a => a.ProgrammeCode == programmeCode
              && a.GradYear == gradYr && a.GradBatch == batch).ToList();
            if (students == null)
                return null;
            var singleComponents = students.First();
            var gradClasses = _unitOfWork.GraduatingClassRepository.GetFiltered(a => a.ProgrammeType == singleComponents.Programme.ProgrammeType).ToList();
            List<string> studentIds = new List<string>();
            foreach(var s in students)
            {
                studentIds.Add(s.PersonId);
            }
            List<InMemoryScoresDTO> scores = FetchRawScoresForInMemoryUseGraduated(programmeCode, gradYr, batch,studentIds);


            BroadSheetDTO dto = new BroadSheetDTO();
            dto.Department = singleComponents.Department.Title;
            dto.Faculty = singleComponents.Department.Faculty.Title;
            dto.Programme = singleComponents.Programme.Title;
            dto.Session = singleComponents.GradYear;
            dto.Name = singleComponents.GradBatch;
            dto.ProgrammeType = singleComponents.ProgrammeType;
             
            //Peak students with basecgpa
            var baseCGPAStudents = _unitOfWork.StudentRepository.GetFiltered(a => studentIds.Contains(a.PersonId) && a.BaseCGPA > 0).ToList();


            var catGpa = CalculateGPA(scores, 1);
            var finalCgpa = CalculateFinalCGPA(scores,gradClasses);
            if (baseCGPAStudents.Count > 0)
            {
                foreach (var g in finalCgpa)
                {
                    var basegp = baseCGPAStudents.Where(s => s.MatricNumber == g.RegNo).FirstOrDefault();
                    if (basegp != null)
                    { g.CGPA = String.Format("{0:0.00}", ((Convert.ToDouble(g.CGPA) + (double)basegp.BaseCGPA) / 2)); }
                }
            }

            //Joining the 3 list above//////////////////////////////////////////

            //Converting results to datatable

           DataTable gpaCategory = DataTableManipulations.GraduatingStudentsBroadsheetSummary(catGpa);//Datatable 2
            DataTable grandCgpa = DataTableManipulations.ConvertToDataTable(finalCgpa);
            //Joining the two data tables (1&2)
            DataTable result = DataTableManipulations.JoinResultDataTables(gpaCategory, grandCgpa, (key1, key2) => key1.Field<string>("RegNo") == key2.Field<string>("RegNo"));
            dto.Results = result;
            return dto;
        }

        string GetQualification(double cgpa, List<GraduatingClass> grads)
        {
            cgpa = Math.Round(cgpa, 2);
           return grads.Where(a => cgpa >= a.Low && cgpa <= a.High).SingleOrDefault().Remark;
        }
        #endregion
        #region PRIVATE HELPERS

        bool StudentClearedBursary(string studentId)
        {
            var stAct = _unitOfWork.StudentPaymentsRepository.GetFiltered(s => s.RegNo == studentId).ToList();
            if (stAct.Count == 0)
                return false;

            double balance = stAct.Where(a => a.TransType == "Debit").Sum(a => a.Amount) - stAct.Where(a => a.TransType == "Credit").Sum(a => a.Amount);
            if (balance < 0)
                return false;
            else
                return true;

        }

        private List<InMemoryScoresDTO> FetchRawScoresForInMemoryUse(string programmeCode, int level, int semesterId)
        {
            List<string> studentIds = FetchStudentIds(programmeCode, level, semesterId);

            var rawList = _unitOfWork.CourseRegistrationRepository.GetFiltered(r => r.Student.ProgrammeCode == programmeCode && r.SemesterId <= semesterId && r.Lvl <= level
                            && studentIds.Contains(r.StudentId)).ToList();

            //Fetch Headers

            List<InMemoryScoresDTO> dto = new List<InMemoryScoresDTO>();
            foreach (var r in rawList)
            {
                dto.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(r));
            }

            var c = dto.OrderBy(a => a.SessionId).OrderBy(a => a.SemesterId);
            return c.ToList();
        }

        private List<InMemoryScoresDTO> FetchRawScoresForInMemoryUseGraduated(string programmeCode, int level, int semesterId, string gradyr, string gradbatch)
        {
            List<string> studentIds = FetchStudentIds(programmeCode, level, semesterId);

            var rawList = _unitOfWork.CourseRegistrationRepository.GetFiltered(r => r.Student.ProgrammeCode == programmeCode && r.Student.Status == "Graduated"
            && r.Student.GradBatch == gradbatch && r.Student.GradYear == gradyr && r.SemesterId <= semesterId && r.Lvl <= level
                            && studentIds.Contains(r.StudentId)).ToList();
            var single = rawList.First();

            //Fetch Headers

            List<InMemoryScoresDTO> dto = new List<InMemoryScoresDTO>();
            foreach (var r in rawList) { dto.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(r)); }

            var c = dto.OrderBy(a => a.SessionId).OrderBy(a => a.SemesterId);
            return c.ToList();
        }
        private List<InMemoryScoresDTO> FetchRawScoresForInMemoryUseGraduated(string programmeCode, string gradyr, string gradbatch,List<string> studentIds)
        {
            var rawList = _unitOfWork.CourseRegistrationRepository.GetFiltered(r => studentIds.Contains(r.StudentId)).ToList();
            var single = rawList.First();

            //Fetch Headers

            List<InMemoryScoresDTO> dto = new List<InMemoryScoresDTO>();
            foreach (var r in rawList) { dto.Add(AcademicModuleMappings.CourseRegToInMemoryScoresDTO(r)); }

            return dto.OrderBy(a => a.CourseCategory).ToList();
        }
        List<string> FetchStudentIds(string progCode, int? level, int? sessionId, int? semesterId)
        {
            List<string> studentIds = _unitOfWork.CourseRegistrationRepository.GetFiltered(r => r.Student.ProgrammeCode == progCode && r.Lvl == level && r.SemesterId == semesterId)
                 .GroupBy(s => s.StudentId).Select(a => a.Key).ToList();
            return studentIds;
        }
        List<string> FetchStudentIds(string progCode, int sessionId)
        {
            List<string> studentIds = _unitOfWork.CourseRegistrationRepository.GetFiltered(r => r.Student.ProgrammeCode == progCode && r.SessionId == sessionId)
                 .GroupBy(s => s.StudentId).Select(a => a.Key).ToList();
            return studentIds;
        }
        List<string> FetchStudentIds(string progCode, int level, int semesterId)
        {
            List<string> studentIds = _unitOfWork.CourseRegistrationRepository.GetFiltered(r => r.Student.ProgrammeCode == progCode && r.Lvl == level && r.SemesterId == semesterId)
                 .GroupBy(s => s.StudentId).Select(a => a.Key).ToList();
            return studentIds;
        }
        private List<GPADTO> CalculateGPA(List<InMemoryScoresDTO> broadsheet, int semesterId, byte flag)
        {
            if (flag == 1)
            {
                var gps = from g in broadsheet
                          where g.SemesterId == semesterId
                          group g by g.MatricNumber into gs

                          select new GPADTO
                          {
                              RegNo = gs.Key,
                              TotCreditHour = gs.Sum(c => c.CreditHour),
                              TotQualitPoints = gs.Sum(q => q.QP),
                              GPA = String.Format("{0:0.00}", ((double)(gs.Where(c => c.Grade != null).Sum(q => q.QP)) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)))
                          };
                return gps.ToList();
            }
            else
            {
                var cgps = from g in broadsheet
                           where g.SemesterId <= semesterId
                           group g by g.MatricNumber into gs

                           select new GPADTO
                           {
                               RegNo = gs.Key,
                               TotCreditHour = gs.Sum(c => c.CreditHour),
                               TotQualitPoints = gs.Sum(q => q.QP),
                               GPA = String.Format("{0:0.00}", ((double)gs.Where(c => c.Grade != null).Sum(q => q.QP) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)))
                           };
                return cgps.ToList();
            }

        }
        private List<GPADTO> CalculateGPA(List<InMemoryScoresDTO> broadsheet, byte flag)
        {
            if (flag == 1)
            {
                var gps = from g in broadsheet
                          group g by new { g.MatricNumber, g.CourseCategory } into gs

                          select new GPADTO
                          {
                              RegNo = gs.Key.MatricNumber,
                              CourseCategory=gs.Key.CourseCategory,
                              TotCreditHour = gs.Sum(c => c.CreditHour),
                              TotQualitPoints = gs.Sum(q => q.QP),
                              GPA = String.Format("{0:0.00}", ((double)(gs.Where(c => c.Grade != null).Sum(q => q.QP)) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)))
                          };
                return gps.ToList();
            }
            else
            {
                var cgps = from g in broadsheet
                           group g by g.MatricNumber into gs

                           select new GPADTO
                           {
                               RegNo = gs.Key,
                               TotCreditHour = gs.Sum(c => c.CreditHour),
                               TotQualitPoints = gs.Sum(q => q.QP),
                               GPA = String.Format("{0:0.00}", ((double)gs.Where(c => c.Grade != null).Sum(q => q.QP) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)))
                           };
                return cgps.ToList();
            }

        }

        private List<FinalGPADTO> CalculateFinalCGPA(List<InMemoryScoresDTO> broadsheet,List<GraduatingClass> gradClass)
        {
            var gps = from g in broadsheet
                      group g by g.MatricNumber into gs

                      select new FinalGPADTO
                      {
                          RegNo = gs.Key,
                          TotCreditHour = gs.Sum(c => c.CreditHour),
                          TotQualitPoints = gs.Sum(q => q.QP),
                          CGPA = String.Format("{0:0.00}", ((double)(gs.Where(c => c.Grade != null).Sum(q => q.QP)) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour))),
                          Class=GetQualification(((double)(gs.Where(c => c.Grade != null).Sum(q => q.QP)) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)),gradClass)
                          };
                return gps.ToList();
           
        }
        private string SingleGPA(List<InMemoryScoresDTO> res, int semesterId, string studentId, byte flag)
        {
            if (flag == 1)
            {
                var st = (from r in res
                          where r.StudentId == studentId && r.SemesterId == semesterId
                          group r by r.MatricNumber into gs

                          select new
                          {
                              //RegNo = re.Key,
                              GPA = String.Format("{0:0.00}", ((double)gs.Where(c => c.Grade != null || c.Grade != "I").Sum(q => q.QP) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)))
                          }).SingleOrDefault();
                if (st == null)
                    return null;
                else
                    return st.GPA;
            }
            else
            {
                var gps = (from g in res
                           where g.StudentId == studentId && g.SemesterId <= semesterId
                           group g by g.MatricNumber into gs

                           select new
                           {
                               RegNo = gs.Key,

                               GPA = String.Format("{0:0.00}", ((double)gs.Where(c => c.Grade != null || c.Grade != "I").Sum(q => q.QP) / gs.Where(c => c.Grade != null).Sum(c => c.CreditHour)))
                           }).SingleOrDefault();
                if (gps == null)
                    return null;
                else
                    return gps.GPA;
            }
        }
        private List<OutstandingDTO> GetOutStanding_Repeat(List<string> studentIds, int sessionId, int semesterId, int level)
        {
            //Get outstandings
            var outStandings = _unitOfWork.OutStandingCourseRepository.GetFiltered(r => studentIds.Contains(r.StudentId) && r.SemesterId == semesterId)
                .Select(o => new OutstandingDTO
                {

                    RegNo = o.Student.MatricNumber,
                    OwingType = o.OwingType,
                    CourseCode = o.Course.CourseCode
                }).ToList();

            //Transforming All outstandings and CarryOVers into coma separated Lists
            string rpt = "Repeat";
            string ous = "OutStanding";

            var btc = from f in outStandings
                      group f by f.RegNo into bt
                      select new OutstandingDTO
                      {
                          RegNo = bt.Key,
                          Outstanding = String.Join(", ", bt.Where(i => ous.Contains(i.OwingType)).Select(se => se.CourseCode)),
                          Repeat = String.Join(", ", bt.Where(i => rpt.Contains(i.OwingType)).Select(se => se.CourseCode))
                      };

            return btc.ToList();
        }
        #endregion

        #region SUMMARY REPORTS
        public int TotalSemesterRegistrations(int semesterId, string progCode)
        {
            if (string.IsNullOrEmpty(progCode))
            {
                return _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.SemesterId == semesterId )
                    .ToList().Count();
            }

            return _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.SemesterId == semesterId && a.Student.ProgrammeCode == progCode)
                    .ToList().Count();
        }
        
        public List<SemesterRegistrations> SemesterRegistrations(int semesterId, string progCode)
        {
            return _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.SemesterId == semesterId && a.Student.ProgrammeCode == progCode)
                .ToList();
        }
        public List<ProgTypeSemesterRegistrationsDTO> SemesterRegistrations(int semesterId)
        {
            var st = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.SemesterId == semesterId);
            if (st.Count() == 0)
                return new List<ProgTypeSemesterRegistrationsDTO>();
             
            var fins = (from s in st
                        group s by s.Student.ProgrammeType into nst
                        select new ProgTypeSemesterRegistrationsDTO
                        {
                            ProgramTpe = nst.Key,
                            Total = nst.Count()
                        });
            return fins.ToList();

        }
        

        public List<StudentDTO> RegisteredStudents(int semesterId, string departCode)
        {
            var students = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.SemesterId == semesterId && a.Student.DepartmentCode == departCode).ToList();
            if (students.Count == 0)
                return new List<StudentDTO>();

            List<StudentDTO> dto = new List<StudentDTO>();
            foreach (var s in students)
            {
                var student = new StudentDTO
                {
                    FullName = s.Student.Name,
                    MatricNumber = s.Student.MatricNumber,
                    StudentId = s.Student.PersonId,
                    CurrentLevel = s.Lvl,
                    Sex=s.Student.Sex,

                };
                dto.Add(student);
            }

            return dto;
        }

        #endregion
        
    }
}
