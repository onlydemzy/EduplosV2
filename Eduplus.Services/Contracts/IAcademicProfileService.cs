using Eduplos.Domain.AcademicModule;
using Eduplos.DTO.AcademicModule;
using Eduplos.DTO.CoreModule;
using System.Collections.Generic;

namespace Eduplos.Services.Contracts
{
    public interface IAcademicProfileService
    {
        List<CourseRegistrationDTO> FetchCoursesToRegister(string studentId, int lvl, int?semesterId);
        List<CourseRegistrationDTO> AdditionalCoursesToRegister(string programmeCode, int lvl, string studentId, int? semesterId);
        byte CheckIfStudentIsClearedToRegisterForCourse(string studentId, int semesterId);
        string SubmitCourseRegistration(List<CourseRegistrationDTO> regCourses, List<CourseRegistrationDTO> outstandingCourses, string username);
        StudentAcademicProfileDTO StudentSemesterCourseRegistration(int semesterId, string studentId);
        List<SemesterRegistrations> FetchStudentRegistrations(string studentId);
        List<ScoresEntryDTO> FetchCoursesForScoreEntry(int semesterId, string courseId,int flag, out byte msg);
        List<CourseRegistrationDTO> FetchStudentRegisteredCourses(int semesterId, string matricNo);
        List<ScoresEntryDTO> FetchBackLogScoresEntry(int sessionId, int semesterId, string courseId, string admitSessin, string progCode, int lvl, out byte msg);
        
        string SubmitScores(ScoresEntryDTO score, string inputedBy);
        string SubmitScoresEdit(ScoresEntryDTO score, string inputedBy);
        string SubmitTemplateScores(List<ScoresEntryDTO> scores, string inputedBy);
        string SubmitBacklogScores(ScoresEntryDTO score, string inputedBy);
        BroadSheetDTO FetchBroadSheet(string programmeCode, int sessionId, int semesterId, int level);
        BroadSheetDTO FetchGraduantsBroadSheet(string programmeCode, int sessionId, int semesterId, int level, string admittedSession, string gradYr,string batch);
        BroadSheetDTO FetchGraduantsSummary(string programmeCode, string gradYr, string batch);
        StudentAcademicProfileDTO FetchSingleSemesterResultForStudent(string studentId, int semesterId,string progCode, int flag);
        List<StudentAcademicProfileDTO> SemesterResultForDepartment(int semesterId, int lvl, string progCode);
        TranscriptDTO FetchStudentAcademicProfile(string recordNo, bool isTranscript);
        List<ProbationDetailsDTO> StudentsDueForGraduation(string progCode);
        ProbationDTO GraduatedStudent(string session, string progCode);
        string GraduateAcademicStudent(List<ProbationDetailsDTO> grads, string session,string batch,string userId);
        
        ProbationDTO GenerateStudentsOnProbation(int sessionId, string progCode,byte flag);
        int TotalSemesterRegistrations(int semesterId, string progCode);
        List<SemesterRegistrations> SemesterRegistrations(int semesterId, string progCode);
        
        List<StudentDTO> RegisteredStudents(int semesterId, string departCode);
        
        void AddRegistrationPermissionsLog(string studentId,int sessionId);
        string DeleteCourseFromCourseRegistration(CourseRegistrationDTO course, string userId);
        string AddCourseToCourseRegistration(CourseRegistrationDTO course, string userId);
        string DeleteCourseRegistration(string studentid, int semesterId, string userId);
        TranscriptDTO FetchStudentTranscript(string matricNumber);
        List<OutstandingCoursesDTO> FetchStudentOutstandings(string studentId);

        #region ResultComplain
        string AddResultComplain(ResultComplainDTO dto, string userid, string username);
        ResultComplainDTO FetchComplain(int complainId);
        List<ResultComplainDTO> ResultComplains(string staffId, string departmentCode);
        InMemoryScoresDTO SingleStudentScore(int semesterId, string matricNumber, string courseId);
        #endregion


    }
}
