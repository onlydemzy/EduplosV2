using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.BurseryModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.ArticleModule;
using Eduplus.DTO.CoreModule;
using System.Collections.Generic;

namespace Eduplus.Services.Contracts
{
    public interface IGeneralDutiesService
    {
        List<DepartmentDTO> FetchDepartments(string facultyCode = null);
        IEnumerable<Faculty> FetchFaculties();
        
        List<StateDTO> FetchStates(string country);

        List<LgaDTO> FetchLgs(string state);
        List<CountryDTO> FetchCountries();
        
        List<ProgrammeDTO> FetchProgrammes(string departmentCode=null);
        List<ProgrammeDTO> ProgrammesByTypeDept(string type, string deptcode);
        Faculty AddUpdateFaculty(Faculty faculty, string userId);
        DepartmentDTO AddUpdateDepartment(DepartmentDTO dept, string userId);
        List<FacultyListDTO> FacultyList();
        SessionDTO FetchSingleSession(int sessionId);
        List<SessionDTO> FetchSessions();
        SessionDTO SaveSession(SessionDTO session, string userId);
        string SaveSemester(SemesterDTO dto, string userId);
        List<Semester> FetchSemester(int sessionId);
        ProgrammeDTO AddUpdateProgramme(ProgrammeDTO programme, string userId);
        List<DepartmentDTO> AllAcademicDepartments();
        List<FacultyProgrammesDTO> AllProgrammesByFaculty();
        List<ProgrammeTypesDTO> GetProgrammeTypes();
        List<ProgrammeDTO> ProgrammesByType(string type);
        List<SessionDTO> FetchAdmissionSession();
        ProgrammeTypes GetStudentProgrameType(string studentId);
        Semester FetchCurrentSemester();
        List<SemesterDTO> FetchSessionSemester(int sessionId);
        CourseDTO SaveCourse(CourseDTO course, string deptCode, string userId);
        List<CourseDTO> PopulateCourse(string programmeCode);
        List<CourseDTO> PopulateCourse(string programmeCode,  string semester);
        List<CourseDTO> PopulateCourse();
        List<CourseDTO> PopulateCourseByCategory(string programmeCode, string category);
        List<CourseDTO> PopulateCourseByCategory(string programmeCode, string[] cat);
        List<CourseDTO> PopulateActiveCourses(string programmeCode);
        string SaveUserData(UserData data, string userId);
        void AddImages2User(UserData data, string userid);
        string Sha256Hasher(string text);
        string Sha512Hasher(string text);      
        PaymentGateways GetDefaultPaymentGateway(string progType);
        List<PaymentGateways> GetAllPaymentGateway();
        void SaveGatewayTransactionLogs(GateWaylogs log, string serv);
        UserData GetUserData();
        void SaveApiLog(ApiLog log);
        ExamsOfficer FetchActiveExamOfficer(string officerCode);
        List<OLevelSubject> AllSubjects();
        string AddSubject(string title);

    }
}