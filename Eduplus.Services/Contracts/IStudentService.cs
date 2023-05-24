using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using KS.Core.UserManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eduplus.Services.Contracts
{
    public interface IStudentService
    {
        //Task<Student> Step1SubmissionAsync(Student student);
        string Step1Submission(StudentDTO student, string userId,out  string flag);
        StudentDTO AdmissionStatus(string studentID);
        string SubmitPassport(AppImages passport, string studentId);
        OlevelResultDetailDTO AddOlevelResult(OlevelResultDetailDTO dto, string userId,out int flag);
         
        OlevelResultDTO FetchOlevelResults(string studentId, byte sit);
        string DeleteOlevelResult(OlevelResultDetailDTO item,string userid);
        StudentDTO StudentApplicationSummary(string studentId);
        StudentDTO StudentApplicationDetail(string studentId);
        List<ApplicantDTO> FetchApplicants(string session, string programmeType, string rpt);
        List<ApplicantDTO> FetchApplicantsByDept(string session, string deptCode, string progType, string rpt);


        string AdmitStudent(string studentId, string userId);
        string AddmitWithoutAcceptanceFee(string studentId);
        int CreateNewStudentProfile(ProspectiveStudentDTO st, out string studentId);
        List<StudentSummaryDTO> FetchStudents(string programmeCode, string sessionAddmitted);
        StudentDTO FetchStudent(string studentId);
        StudentSummaryDTO FetchStudentSummary(string studentId);
        StudentInProgDTO FetchStudentsInProgram(string programmeCode, string sessionAddmitted);
        Applicants FetchApplicant(string jambNo);
        List<StudentSummaryDTO> SearchStudents(string searchText);
        string UpdateStudentProgramme(string studentId, string newProgCode, string reason, string userId);
        string CheckCompletedProfile(string studentId);

        void SaveDocument(StudentDocuments doc, string userId);
        StudentDocuments ViewDocument(string studentId);
        int CheckAdmissionCompletionStep(string studentID);
        List<StudentDTO> FetchStudents(string programmeCode);
        string NewStudent(Student student, int sessionId, string userId);
        int CreateNewAlumus(StudentDTO st, out string studentId);
        void GrantStudentAccessToUtilitesBasedOnPayments(string paymentType, string studentId, int? payOptionId,int sessionId, bool late = false);
        int TotalSessionApplicants(string session);
        List<ProgrammeDTO> SessionAdmissionsSummaryProgType(string session);
        List<StudentInProgDTO> TotalActiveStudentsByProgramme();
        List<ProgrammeDTO> CurrentStudentEnrollmentsByProgType();
        int CurrentTotalStudents();
        List<ApplicantDTO> ApplicantsByDepartment(string session, string deptCode);
        List<ApplicantDTO> ApplicantsByProgrammeType(string session, string progType);
        JambDTO GetStudentJambReg(string studentId);
        JambScoresDTO SaveJambScore(JambScoresDTO dto, string userId,out int flag);
        string DeleteJambScore(JambScoresDTO dto, string userId);
        string DeleteAlevel(OtherQualificationDTO dto, string userId);
        string SaveAlevel(OtherQualificationDTO dto, string userId);
        List<OtherQualificationDTO> GetStudentAlevel(string studentId);
        MatricRegisterDTO MatricnRegister(string admitYr, string deptCode, string progType);


        bool CanVote(string studentId, int sessionId);
        StudentSummaryDTO FetchStudentByPhone(string phone);


    }
}