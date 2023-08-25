using Eduplus.Domain.AcademicModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using Eduplus.ObjectMappings;
using Eduplus.Services.Contracts;
using Eduplus.Services.UtilityServices;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Eduplus.Services.Implementations
{


    public class AcademicAffairsService : IAcademicAffairsService
    {
        private IUnitOfWork _unitOfWork;
        public AcademicAffairsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region COURSESCHEDULE OPERATIONS
        public List<StaffDTO> FetchLecturersForCourseAllocation(string deptCode)
        {

            
            var lecturers = _unitOfWork.StaffRepository.GetFiltered(a => a.DepartmentCode==deptCode && 
            a.Status=="Active" &&a.Category=="Academic Staff").ToList();
            if (lecturers == null)
                return new List<StaffDTO>();

            List<StaffDTO> dto = new List<StaffDTO>();
            foreach (var l in lecturers)
            {
                dto.Add(CoreModuleMappings.StaffToStaffDTO(l));
            }

            return dto;
        }
        
        public List<CourseSchedule> FetchCourseLecturers(string courseId, int semesterId)
        {
            var schedule = _unitOfWork.CourseScheduleRepository.GetFiltered(a => a.CourseId == courseId && a.SemesterId == semesterId).ToList();
            return schedule;
        }

        public string AddCourseSchedules(CourseScheduleDTO schedule,string userId)
        {
            //Check if already added
            var courses = _unitOfWork.CourseRepository.GetFiltered(a => a.CourseCode == schedule.CourseCode
            && a.IsActive==true).ToList();
            var exist = _unitOfWork.CourseScheduleRepository.GetFiltered(a => a.LecturerId == schedule.LecturerId
            && a.Course.CourseCode == schedule.CourseCode && a.SemesterId == schedule.SemesterId);
            if (exist.Count() > 0)
                return "Lecturer already added to this course schedule for the semester";
                foreach(var sc in courses)
                {
                    
                    var cs= new CourseSchedule
                    {
                        CourseId = sc.CourseId,
                        ProgrammeCode = sc.ProgrammeCode,
                        SemesterId = schedule.SemesterId,
                        LecturerId = schedule.LecturerId
                    };
                _unitOfWork.CourseScheduleRepository.Add(cs);
               
                }    
                 _unitOfWork.Commit(userId);

               
                return "00";
          
            
        }

        public string RemoveLecturerFromSchedule(string courseId, int semesterId, StaffDTO lecturer, string userId)
        {
            var courseschedule = _unitOfWork.CourseScheduleRepository.GetFiltered(
                s => s.CourseId == courseId && s.SemesterId == semesterId).SingleOrDefault();
            if (courseschedule == null)
                return "Cannot remove a null entry from schedule";
            
            _unitOfWork.CourseScheduleRepository.Remove(courseschedule);
            _unitOfWork.Commit(userId);
            return "Operation completed successfully";
        }

        public List<CourseScheduleDTO> CourseLecturers(int semesterId,string courseId)
        {
            var courseschedule = _unitOfWork.CourseScheduleRepository.GetFiltered(
                s => s.CourseId == courseId && s.SemesterId == semesterId).ToList();
            if (courseschedule.Count == 0)
                return new List<CourseScheduleDTO>();

            List<CourseScheduleDTO> dto = new List<CourseScheduleDTO>();
            foreach(var sc in courseschedule)
            {
                CourseScheduleDTO csd = new CourseScheduleDTO();
                csd.ScheduleId = sc.ScheduleId;
                csd.CourseId = sc.CourseId;
                csd.CourseCode = sc.Course.CourseCode;
                csd.Title = sc.Course.Title;

                csd.Lecturers = sc.Lecturer.Name;// string.Join(",", sc.CourseScheduleDetails.ToList().Select(a => a.LecturerName).ToList());
               
                dto.Add(csd);
            }
            return dto.OrderBy(a => a.CourseCode).ToList();
        }
        #endregion
        #region LECTURER RESULTS OPERATIONS

        public List<CourseDTO> LecturerCourses(string lecturerId,int semesterId)
        {
            var courses = _unitOfWork.CourseScheduleRepository.GetFiltered(a => a.LecturerId == lecturerId && 
            a.SemesterId == semesterId)
                .ToList();

            if (courses.Count == 0)
                return new List<CourseDTO>();
            List<CourseDTO> dto = new List<CourseDTO>();
            foreach(var c in courses)
            {
                var cdto = new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseCode = c.Course.CourseCode,
                    Title = c.Course.Title
                };
                dto.Add(cdto);
            }

            return dto.OrderBy(a => a.CourseCode).ToList();

        }
        public ExamsAttendanceDTO LecturerScoreSheet(int semesterId, string courseId)
        {


            List<string> courseIds = new List<string>();
            var students = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.SemesterId == semesterId && a.CourseId == courseId
            && !string.IsNullOrEmpty(a.Grade))
                .OrderBy(s => s.Student.MatricNumber).ToList();
            if (students.Count == 0)
            { return null; }
            //Fetch Single Entity

            var single = students.First();
            ExamsAttendanceDTO deptExamAttendance = new ExamsAttendanceDTO();
            deptExamAttendance.CourseCode = single.Course.CourseCode;
            deptExamAttendance.Course = single.Course.Title;
            deptExamAttendance.CreditHour = single.CourseCreditHour;
            deptExamAttendance.Department = single.Student.Department.Title;
            deptExamAttendance.Programme = single.Student.Programme.Title;
            deptExamAttendance.Semester = single.Semester.SemesterTitle;
            deptExamAttendance.Session = single.Session.Title;

            List<ExamsAttendanceDetailDTO> scoresDetails = new List<ExamsAttendanceDetailDTO>();
            List<Grading> grd = _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == single.Student.ProgrammeType).ToList();
            int count = 0;
            foreach (var d in students)
            {


                ExamsAttendanceDetailDTO details = new ExamsAttendanceDetailDTO();
                details.RegNo = d.Student.MatricNumber;
                details.Student = StandardGeneralOps.ToTitleCase(d.Student.Name);
                details.CA1 = d.CA1;
                details.CA2 = d.CA2;
                details.Exam = d.Exam;
                details.Total =d.CalculateTScore(d.Exam,d.CA1,d.CA2);
                if(!string.IsNullOrEmpty(d.Grade))
                {
                    details.Grade = d.Grade;
                    details.Remark = grd.Where(gr => gr.Grade == details.Grade).First().Remark;
                }
               
                 
                details.Count = count + 1;
                scoresDetails.Add(details);
                count++;

            }

            deptExamAttendance.Students = scoresDetails;
            return deptExamAttendance;
        }


        #endregion

        #region PRE EXAM OPERATIONS
        public string AllowStudentRegistration(int semesterId,string matNo,string userId)
        {
            matNo = matNo.Trim().ToUpper();
            var student = _unitOfWork.StudentRepository.GetFiltered(a => a.MatricNumber == matNo || a.PersonId==matNo).SingleOrDefault();
            if (student == null)
                return "Invalid matric number";
            var semester = _unitOfWork.SemesterRepository.Get(semesterId);
            var coursereg = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(s => s.SessionId == semester.SessionId
                 && s.StudentId == student.PersonId).FirstOrDefault();
            if(coursereg!=null)
            {
                switch (semester.SemesterTitle) {
                    case "1st Semester":
                        coursereg.Register1 = true;
                        coursereg.Write1 = true;
                        break;
                    case "2nd Semester":
                        coursereg.Register2 = true;
                        coursereg.Write2 = true;
                        break;
                    case "3rd Semester":
                        coursereg.Register3 = true;
                        coursereg.Write3 = true;
                        break;
                }
                
            }
            else
            {
                RegistrationsPermissionsLog log = new RegistrationsPermissionsLog();
                log.StudentId = student.PersonId;
                log.SessionId = semester.SessionId;
                switch (semester.SemesterTitle)
                {
                    case "1st Semester":
                        log.Register1 = true;
                        log.Write1 = true;
                        break;
                    case "2nd Semester":
                        log.Register2 = true;
                        log.Write2 = true;
                        break;
                    case "3rd Semester":
                        log.Register3 = true;
                        log.Write3 = true;
                        break;
                }
                _unitOfWork.RegistrationsPermissionsLogRepository.Add(log);
               
            }
            _unitOfWork.Commit(userId);
            return "Permission granted. Student may proceed for course registration";
        }
        public List<ExamsAttendanceDTO> ExamAttendance(int semesterId, string programCode)
        {

            List<ExamsAttendanceDTO> deptExamAttendance = new List<ExamsAttendanceDTO>();
            List<string> courseIds = new List<string>();
            var semester = _unitOfWork.SemesterRepository.Get(semesterId);
            var semRegLog = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(a => a.SessionId == semester.SessionId).ToList();
            if (semRegLog.Count == 0)
                return new List<ExamsAttendanceDTO>();
            switch(semester.SemesterTitle)
            {
                case "1st Semester":
                    semRegLog = semRegLog.Where(a => a.Write1 == true).ToList();
                    break;
                case "2nd Semester":
                    semRegLog = semRegLog.Where(a => a.Write1 == true).ToList();
                    break;
                case "3rd Semester":
                    semRegLog = semRegLog.Where(a => a.Write1 == true).ToList();
                    break;
            }
            if (semRegLog.Count == 0)
                return new List<ExamsAttendanceDTO>();

            List<string> studentIds = new List<string>();
            foreach(var r in semRegLog)
            {
                studentIds.Add(r.StudentId);
            }
             
                var students = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => studentIds.Contains(a.StudentId)
                && a.Student.ProgrammeCode == programCode &&a.SemesterId==semesterId)
                                .OrderBy(s => s.Student.MatricNumber).ToList();
                //Fetch courses from chosen session

                courseIds = (from c in students
                             group c by c.CourseId into nco
                             select (

                                 nco.Key
                             )).ToList();

                foreach (string c in courseIds)
                {
                    List<ExamsAttendanceDetailDTO> attendanceDetails = new List<ExamsAttendanceDetailDTO>();

                    var attHeadings = students.Where(ci => ci.CourseId == c).FirstOrDefault();
                    ExamsAttendanceDTO de = new ExamsAttendanceDTO();
                    de.Course = attHeadings.Course.Title;
                    de.CourseCode = attHeadings.Course.CourseCode;
                    de.CreditHour = attHeadings.CourseCreditHour;
                    de.Department = attHeadings.Student.Department.Title;
                    de.Semester = attHeadings.Semester.SemesterTitle;
                    de.Session = attHeadings.Session.Title;
                    de.Programme = attHeadings.Student.Programme.Title;
                    //Add students

                    var atdetails = students.Where(ci => ci.CourseId == c);
                     
                    foreach (var d in atdetails)
                    {

                        ExamsAttendanceDetailDTO details = new ExamsAttendanceDetailDTO();
                        details.RegNo = d.Student.MatricNumber;
                        details.Student = StandardGeneralOps.ToTitleCase(d.Student.Name);
                        details.CA1 = d.CA1;
                        details.CA2 = d.CA2;
                        details.Exam = d.Exam;
                        details.Total = details.CA1+details.CA2 + details.Exam;
                        details.Grade = d.Grade;
                        
                        attendanceDetails.Add(details);
                       
                    }
                    de.Students = attendanceDetails;
                    deptExamAttendance.Add(de);
                }


                return deptExamAttendance;
         
        }
        public ExamsAttendanceDTO ExamAttendanceByCourse(int semesterId, string courseId)
        {

            List<ExamsAttendanceDTO> deptExamAttendance = new List<ExamsAttendanceDTO>();
            List<string> courseIds = new List<string>();
            var semester = _unitOfWork.SemesterRepository.Get(semesterId);
            var semRegLog = _unitOfWork.RegistrationsPermissionsLogRepository.GetFiltered(a => a.SessionId == semester.SessionId).ToList();
            if (semRegLog.Count == 0)
                return null;
            switch (semester.SemesterTitle)
            {
                case "1st Semester":
                    semRegLog = semRegLog.Where(a => a.Write1 == true).ToList();
                    break;
                case "2nd Semester":
                    semRegLog = semRegLog.Where(a => a.Write1 == true).ToList();
                    break;
                case "3rd Semester":
                    semRegLog = semRegLog.Where(a => a.Write1 == true).ToList();
                    break;
            }
            if (semRegLog.Count == 0)
                return null;

            List<string> studentIds = new List<string>();
            foreach (var r in semRegLog)
            {
                studentIds.Add(r.StudentId);
            }

            var students = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => studentIds.Contains(a.StudentId)
            && a.CourseId == courseId && a.SemesterId == semesterId)
                            .OrderBy(s => s.Student.MatricNumber).ToList();
            //Fetch courses from chosen session

            courseIds = (from c in students
                         group c by c.CourseId into nco
                         select (

                             nco.Key
                         )).ToList();

            
                List<ExamsAttendanceDetailDTO> attendanceDetails = new List<ExamsAttendanceDetailDTO>();

                var attHeadings = students.FirstOrDefault();
                ExamsAttendanceDTO de = new ExamsAttendanceDTO();
                de.Course = attHeadings.Course.Title;
                de.CourseCode = attHeadings.Course.CourseCode;
                de.CreditHour = attHeadings.CourseCreditHour;
                de.Department = attHeadings.Student.Department.Title;
                de.Semester = attHeadings.Semester.SemesterTitle;
                de.Session = attHeadings.Session.Title;
                de.Programme = attHeadings.Student.Programme.Title;
                //Add students
                 
                foreach (var d in students)
                {

                    ExamsAttendanceDetailDTO details = new ExamsAttendanceDetailDTO();
                    details.RegNo = d.Student.MatricNumber;
                    details.Student = StandardGeneralOps.ToTitleCase(d.Student.Name);
                    details.CA1 = d.CA1;
                    details.CA2 = d.CA2;
                    details.Exam = d.Exam;
                    details.Total = details.CA1 + details.CA2 + details.Exam;
                    details.Grade = d.Grade;
                      
                    attendanceDetails.Add(details);
                     
            }
            de.Students = attendanceDetails;

            return de;
        }


        #endregion

        #region ACADEMIC CALENDER

        public CurrentCalenderDTO FetchCurrentCalender()
        {
            
            var calender = _unitOfWork.CalenderRepository.GetSingle(s => s.Session.IsCurrent==true);
            CurrentCalenderDTO current = new CurrentCalenderDTO();
            List<SemesterCalender> semesters = new List<SemesterCalender>();
            
            current.Title = calender.Title;
            //get first semester activities

            var sems = calender.Details.GroupBy(a => a.Semester)
                .Select(a => a.Key).ToList();
            var details = calender.Details.OrderBy(a => a.StartDate.Year)
                .ThenBy(a=>a.StartDate.Month)
                .ThenBy(a=>a.StartDate.Date).ToList();
            //Get Each semester Activites
            foreach(string item in sems)
            {
                SemesterCalender sc = new SemesterCalender();
                List<CalenderActivities> activites = new List<CalenderActivities>();
                sc.Semester = item;
                foreach (var c in details.Where(a=>a.Semester==item))
                {
                    var activity = new CalenderActivities
                    {
                        Activity = c.Activity,
                        StartDate = c.StartDate.ToString("dd-MMM-yyy"),
                        EndDate = c.EndDate.ToString("dd-MMM-yyyy"),
                        Duration = CalculateDuration(c.StartDate, c.EndDate)
                    };
                    activites.Add(activity);
                }
                sc.Activities = activites;
                semesters.Add(sc);
                current.Semesters=semesters;
            }
            
            return current;
        }
        string CalculateDuration(DateTime sd, DateTime ed)
        {
            var span = ed.Subtract(sd);

            int days;

            days = span.Days+1;
            if(days>7)
            {
                
                int wks = (days / 7);
                int dys = (days % 7);
                if(dys>0)
                {
                    return wks.ToString() + " Week(s) & " + dys.ToString() + " Day(s)";
                }
                
                else
                {
                    return wks.ToString() + " Week(s)";
                }
            }

            return days.ToString()+"day(s)";
        }
        public List<CalenderDTO> FetchCalenders()
        {
            var cals = _unitOfWork.CalenderRepository.GetAll().ToList();
            List<CalenderDTO> details = new List<CalenderDTO>();
            foreach (var c in cals)
            {
                var cal = new CalenderDTO
                {

                    Session = c.Session.Title,
                    Title = c.Title,
                    CalenderId = c.CalenderId
                };
                details.Add(cal);
            }
            return details.OrderBy(a => a.Session).ToList();
        }
        public CalenderDTO FetchCalenderDetails(int calenderId)
        {
            var cals = _unitOfWork.CalenderRepository.Get(calenderId);
            if (cals == null)
                return new CalenderDTO();
            CalenderDTO dto = new CalenderDTO();
            dto.CalenderId = cals.CalenderId;
            dto.SessionId = cals.SessionId;
            dto.Session = cals.Session.Title;
            dto.Title = cals.Title;

            List<CalenderDetailsDTO> details = new List<CalenderDetailsDTO>();
            foreach (var c in cals.Details)
            {
                var detail = new CalenderDetailsDTO
                {
                    DetailsId = c.DetailsId,
                    Activity = c.Activity,
                    StartDate = c.StartDate,
                    CalenderId = c.CalenderId,
                    Semester = c.Semester

                };
                details.Add(detail);
            }
            dto.Details = details.OrderBy(a => a.StartDate).ToList();

            return dto;
        }
        public CalenderDTO SaveCalender(CalenderDTO calender, string userId)
        {
            if (calender.CalenderId > 0)
            {
                var dbcal = _unitOfWork.CalenderRepository.Get(calender.CalenderId);
                dbcal.Title = calender.Title;
                dbcal.SessionId = calender.SessionId;

                _unitOfWork.Commit(userId);
                return calender;
            }
            else
            {
                var cal = new Calender
                {

                    SessionId = calender.SessionId,
                    Title = calender.Title,

                };
                _unitOfWork.CalenderRepository.Add(cal);
                _unitOfWork.Commit(userId);
                return calender;
            }
        }
        public CalenderDetailsDTO SaveCalenderDetail(CalenderDetailsDTO calender, string userId)
        {
            if (calender.DetailsId > 0)
            {
                var dbcal = _unitOfWork.CalenderDetailRepository.Get(calender.DetailsId);

                dbcal.Activity = calender.Activity;
                dbcal.StartDate = calender.StartDate;
                dbcal.EndDate = calender.EndDate;
                dbcal.LastUpDate = DateTime.UtcNow;

                _unitOfWork.Commit(userId);
                return calender;
            }
            else
            {

                var details = new CalenderDetail
                {
                    Activity = calender.Activity,
                    StartDate = calender.StartDate,
                    EndDate=calender.EndDate,
                    LastUpDate = DateTime.UtcNow,
                    CalenderId = calender.CalenderId,
                    Semester = calender.Semester
                };
                _unitOfWork.CalenderDetailRepository.Add(details);
                _unitOfWork.Commit(userId);
                return calender;
            }
        }
        public void RemoveCalenderDetail(CalenderDetailsDTO calender, string userId)
        {
            if (calender.DetailsId > 0)
            {
                var dbcal = _unitOfWork.CalenderDetailRepository.Get(calender.DetailsId);


                //dbCalenderDetail.CalenderId = 0;
                _unitOfWork.CalenderDetailRepository.Remove(dbcal);

                _unitOfWork.Commit(userId);

            }

        }

        #endregion

        public string DeleteOutstandingCourse(int outstandingId,string userId)
        {
            var outs = _unitOfWork.OutStandingCourseRepository.Get(outstandingId);
            if (outs == null)
                return "Invalid Outstanding Course ID";
            _unitOfWork.OutStandingCourseRepository.Remove(outs);
            _unitOfWork.Commit(userId);
            return "Outstanding successfully removed";
        }
        public List<StudentSemesterProfileDTO> GetREgisteredStudents(string progCode,int semesterId,int lvl)
        {
            List<StudentSemesterProfileDTO> dto = new List<StudentSemesterProfileDTO>();
            var students = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.Lvl == lvl && a.SemesterId == semesterId
              && a.Student.ProgrammeCode == progCode).ToList();
            foreach(var s in students)
            {
                var d = new StudentSemesterProfileDTO
                {
                    Session = s.Session,
                    Semester = s.Semester,
                    Level = s.Lvl,
                    Name = s.Student.Name,
                    RegNo = s.Student.MatricNumber,
                };

                dto.Add(d);
            }
            
            return dto;
        }

        public List<StudentSemesterProfileDTO> GetREgisteredStudents(int semesterid)
        {
            List<StudentSemesterProfileDTO> dto = new List<StudentSemesterProfileDTO>();
            var students = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a=>a.SemesterId==semesterid).ToList();
            foreach (var s in students)
            {
                var d = new StudentSemesterProfileDTO
                {
                    Session = s.Session,
                    Semester = s.Semester,
                    Level = s.Lvl,
                    Name = s.Student.Name,
                    RegNo = s.Student.MatricNumber,
                    Programme=s.Student.Programme.Title
                };

                dto.Add(d);
            }

            return dto;
        }

        public List<StudentSemesterProfileDTO> GetREgisteredStudents(int semesterid, string progCode)
        {
            List<StudentSemesterProfileDTO> dto = new List<StudentSemesterProfileDTO>();
            var students = _unitOfWork.SemesterRegistrationsRepository.GetFiltered(a => a.SemesterId == semesterid&& a.Student.ProgrammeCode==progCode);
            foreach (var s in students)
            {
                var d = new StudentSemesterProfileDTO
                {
                    Session = s.Session,
                    Semester = s.Semester,
                    Level = s.Lvl,
                    Name = s.Student.Name,
                    RegNo = s.Student.MatricNumber,
                    Programme = s.Student.Programme.Title
                };

                dto.Add(d);
            }

            return dto;
        }
        public List<ProgTypeSemesterRegistrationsDTO> TotalSemesterRegistrationsByProgType(int semesterId)
        {
            return _unitOfWork.SemesterRegistrationsRepository.TotalSemesterRegistrationsByProgType(semesterId);
        }

        #region GRADING SYSTEM
        public Grading SaveGrade(Grading grade, string userID)
        {
            if (grade.GradeId > 0)
            {
                var dbgrade = _unitOfWork.GradingRepository.Get(grade.GradeId);
                dbgrade.Low = grade.Low;
                dbgrade.High = grade.High;
                dbgrade.GradePoint = grade.GradePoint;
                dbgrade.Grade = grade.Grade;
                dbgrade.Remark = grade.Remark;
                _unitOfWork.Commit(userID);
            }
            else
            {
                _unitOfWork.GradingRepository.Add(grade);
                _unitOfWork.Commit(userID);
            }
            return grade;
        }
        public void DeleteGrade(int gradeId, string userId)
        {
            var dbgrade = _unitOfWork.GradingRepository.Get(gradeId);
            if (dbgrade != null)
            {
                _unitOfWork.GradingRepository.Remove(dbgrade);
                _unitOfWork.Commit(userId);
            }
        }

        public List<Grading> AllGrades()
        {
            return _unitOfWork.GradingRepository.GetAll()
                .OrderBy(a => a.ProgrammeType).ToList();
        }
        public List<Grading> GradesByProgrammeType(string progType)
        {
            return _unitOfWork.GradingRepository.GetFiltered(a => a.ProgrammeType == progType)
                .ToList();
        }
        public GraduatingClass SaveGraduatingClass(GraduatingClass grade, string userID)
        {
            if (grade.ClassId > 0)
            {
                var dbgrade = _unitOfWork.GraduatingClassRepository.Get(grade.ClassId);
                dbgrade.Low = grade.Low;
                dbgrade.High = grade.High;
                dbgrade.IsProbation = grade.IsProbation;

                dbgrade.Remark = grade.Remark;
                _unitOfWork.Commit(userID);
            }
            else
            {
                _unitOfWork.GraduatingClassRepository.Add(grade);
                _unitOfWork.Commit(userID);
            }
            return grade;
        }

        public List<GraduatingClass> AllGradClasses()
        {
            return _unitOfWork.GraduatingClassRepository.GetAll()
                .OrderBy(a => a.ProgrammeType).ToList();
        }
        public List<GraduatingClass> GraduatingClassByProgrammeType(string progType)
        {
            return _unitOfWork.GraduatingClassRepository.GetFiltered(a => a.ProgrammeType == progType)
                .ToList();
        }
        public void DeleteGradClass(int gradeId, string userId)
        {
            var dbgrade = _unitOfWork.GraduatingClassRepository.Get(gradeId);
            if (dbgrade != null)
            {
                _unitOfWork.GraduatingClassRepository.Remove(dbgrade);
                _unitOfWork.Commit(userId);
            }
        }
        #endregion

        #region COURSE CATEGORY
        public List<CourseCategory> AllCourseCategories(string progType,string progCode)
        {
            if(string.IsNullOrEmpty(progType)&&!string.IsNullOrEmpty(progCode))
            {
                var prog = _unitOfWork.ProgrammeRepository.Get(progCode);
                return _unitOfWork.CourseCategoryRepository.GetFiltered(a=>a.ProgrammeType==prog.ProgrammeType).OrderBy(a => a.ProgrammeType).ToList();
            }
            else if (!string.IsNullOrEmpty(progType) && string.IsNullOrEmpty(progCode))
            {
                return _unitOfWork.CourseCategoryRepository.GetFiltered(a => a.ProgrammeType == progType).OrderBy(a => a.ProgrammeType).ToList();
            }
            else
            return _unitOfWork.CourseCategoryRepository.GetAll().OrderBy(a => a.ProgrammeType).ToList();
        }
        
        public string EditCategory(CourseCategory category, string userId)
        {
            var cat = _unitOfWork.CourseCategoryRepository.GetFiltered(a => a.Category == category.Category).SingleOrDefault();
            if (cat != null)
                return "Course category already exist in the database";
            if(category.CategoryId==0)
            _unitOfWork.CourseCategoryRepository.Add(category);
            _unitOfWork.Commit(userId);
            return "Course category successfully added";
        }
        #endregion

        #region TRANSCRIPT OPERATIONS
        public string  SubmitTranscriptApplication(TranscriptApplication transc)
        {
            if (string.IsNullOrEmpty(transc.StudentId))
                return "null";
            var currentSes = _unitOfWork.SessionRepository.GetFiltered(a => a.IsCurrent).FirstOrDefault();
            transc.DateApplied = DateTime.UtcNow;
            var studnet = _unitOfWork.StudentRepository.Get(transc.StudentId);
            transc.Student = StandardGeneralOps.ToTitleCase(studnet.Name);
            transc.DeliveryAddress = StandardGeneralOps.ToTitleCase(transc.DeliveryAddress);
            
            transc.Matricnumber = studnet.MatricNumber;
            transc.Status = "New Application";
            transc.TranscriptNo = StandardGeneralOps.GeneratePersonId(currentSes.SessionId);
            _unitOfWork.TranscriptRepository.Add(transc);
            _unitOfWork.Commit(transc.StudentId);

            return transc.TranscriptNo;
        }

        /// <summary>
        /// Update Transcript payments to paid status, payment=Payment particulars
        /// </summary>
        /// <param name="payment"></param>
        public void UpdateTranscriptToPaidStatus(string payment)
        {
            string[] getNo = payment.Split('_');
            string transcriptNo = getNo[1];
            var userData = _unitOfWork.UserDataRepository.GetAll().FirstOrDefault();
            //Update Transcript
            var transcript = _unitOfWork.TranscriptRepository.GetFiltered(a=>a.TranscriptNo==transcriptNo).FirstOrDefault();
            transcript.Status = "PAID";
            _unitOfWork.Commit("System");
            //Send Email to Registry
             
        }

        public List<TranscriptApplication> CurrentlyPaidTranscriptRequest20DaysOld()
        {
            DateTime _20daysBack = DateTime.UtcNow.AddDays(-25);
            return _unitOfWork.TranscriptRepository.GetFiltered(
                a => a.Status != "Old").ToList();
                // return _unitOfWork.TranscriptRepository.GetFiltered(
                //a => a.Status == "PAID" && DbFunctions.TruncateTime(a.DateApplied) >= _20daysBack).ToList();
        }
        public TranscriptApplication FetchTranscriptApplication(string transcriptNo)
        {
            var trans=_unitOfWork.TranscriptRepository.GetFiltered(a => a.TranscriptNo == transcriptNo).SingleOrDefault();
            if(trans!=null)
            {
                var student = _unitOfWork.StudentRepository.Get(trans.StudentId);
                trans.Phone = student.Phone;
                return trans;
            }
            return null;
        }
        public List<TranscriptApplication> FetchTranscriptApplications(string studentId)
        {
            return _unitOfWork.TranscriptRepository.GetFiltered(a => a.StudentId == studentId).ToList();
        }
        #endregion

        public string EliminateIncompleteResult()
        {
            var result = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.Grade == "I").ToList();
            var grads = _unitOfWork.GradingRepository.GetAll().ToList();
            if (result.Count > 0)
            {
                foreach (var r in result)
                {
                    int tot = r.TScore;
                    Grading grade = r.CalculateGrade(tot, grads);
                    r.Grade = grade.Grade;
                    r.GradePoint = grade.GradePoint;
                    if (r.Grade == "F")
                    {
                        _unitOfWork.OutStandingCourseRepository.Add(new OutStandingCourse
                        {
                            CourseId = r.CourseId,
                            SemesterId = r.SemesterId,
                            SessionId = r.SessionId,
                            StudentId = r.StudentId,
                            Owing = true,
                            OwingType = "Repeat"
                        });
                    }
                }
                _unitOfWork.Commit("System");
                return "Good";
            }
            else return "No incomplete result";
            
        }
        #region SPECIAL RECOVERY
        public void UpdateCourseRecovery()
        {
            var coursereg = _unitOfWork.CourseRegistrationRepository.GetFiltered(a => a.CourseId == "BIO002-BIO212").ToList();
            var recova= _unitOfWork.CourseRegRecoverRepository.GetAll().ToList();
            foreach(var c in coursereg)
            {
                var toWork = recova.Where(a => a.RegistrationId == c.RegistrationId).SingleOrDefault();
                if (toWork != null)
                {
                    c.CourseId = toWork.CourseId;
                    _unitOfWork.Commit("SytemRegCourse");
                }
                
            }
        }
        #endregion

        #region EXAMS OFFICERS
        public List<ExamsOfficer> GetCurrentExamsOfficers(string progCode)
        {
            return _unitOfWork.ExamsOfficerRepository
                .GetFiltered(a => a.ProgrammeCode == progCode&& a.Iscurrent==true)
                .OrderBy(a=>a.Fullname).ToList();
        }
        #endregion

    }
}
