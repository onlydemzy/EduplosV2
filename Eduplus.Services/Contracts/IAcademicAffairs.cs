using System.Collections.Generic;
using Eduplus.DTO.CoreModule;
using Eduplus.Domain.AcademicModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO;

namespace Eduplus.Services.Contracts
{
    public interface IAcademicAffairsService
    {
        string EliminateIncompleteResult();
        string AddCourseSchedules(CourseScheduleDTO schedule, string userId);
        List<CourseSchedule> FetchCourseLecturers(string courseId, int semesterId);
        List<StaffDTO> FetchLecturersForCourseAllocation(string departmentCode);
         
        string RemoveLecturerFromSchedule(string courseId, int semesterId, StaffDTO lecturer, string userId);
        List<CourseDTO> LecturerCourses(string lecturerId, int semesterId);
        ExamsAttendanceDTO LecturerScoreSheet(int semesterId, string courseId);
        List<ExamsAttendanceDTO> ExamAttendance(int semesterId, string programCode);
        ExamsAttendanceDTO ExamAttendanceByCourse(int semesterId, string courseId);
        List<CalenderDTO> FetchCalenders();
        CalenderDTO FetchCalenderDetails(int calenderId);
        CalenderDTO SaveCalender(CalenderDTO calender, string userId);
        CalenderDetailsDTO SaveCalenderDetail(CalenderDetailsDTO calender, string userId);
        CurrentCalenderDTO FetchCurrentCalender();
        void RemoveCalenderDetail(CalenderDetailsDTO calender, string userId);
        List<StudentSemesterProfileDTO> GetREgisteredStudents(string progCode, int semesterId, int lvl);
        List<ProgTypeSemesterRegistrationsDTO> TotalSemesterRegistrationsByProgType(int semesterid);
        List<StudentSemesterProfileDTO> GetREgisteredStudents(int semesterid, string progCode);
        string AllowStudentRegistration(int semesterId, string matNo, string userId);
        

        //===================================Grading operations=========================================
        Grading SaveGrade(Grading grade, string userID);
        void DeleteGrade(int gradeId, string userId);
        List<Grading> AllGrades();
        List<Grading> GradesByProgrammeType(string progType);
        GraduatingClass SaveGraduatingClass(GraduatingClass grade, string userID);
        List<GraduatingClass> AllGradClasses();
        List<GraduatingClass> GraduatingClassByProgrammeType(string progType);
        List<CourseCategory> AllCourseCategories(string progType,string program);
        void DeleteGradClass(int gradeId, string userId);
        string EditCategory(CourseCategory category, string userId);
        //====================================================Transcript Operations============================
        List<TranscriptApplication> CurrentlyPaidTranscriptRequest20DaysOld();
        string SubmitTranscriptApplication(TranscriptApplication transc);
        void UpdateTranscriptToPaidStatus(string payment);
        TranscriptApplication FetchTranscriptApplication(string transcriptNo);
        List<TranscriptApplication> FetchTranscriptApplications(string studentId);
        string DeleteOutstandingCourse(int outstandingId, string userId);
        void UpdateCourseRecovery();
        List<ExamsOfficer> GetCurrentExamsOfficers(string progCode);
    }
}