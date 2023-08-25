using Eduplus.Domain;
using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.ArticleModule;
using Eduplus.Domain.BurseryModule;
using Eduplus.Domain.CoreModule;
using Eduplus.Domain.HostelModule;
using KS.Core.UserManagement;
using KS.Domain.AccountsModule;
using KS.Domain.HRModule;
using System;

namespace KS.Core
{
    /// <summary>
    /// Base interface to implement UnitOfWork Pattern.
    /// </summary>
    public interface IUnitOfWork
        : IDisposable
    {
        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,  
        /// then an exception is thrown
        ///</remarks>
        void Commit(string userId);
        void SetModified<T>(T item) where T : class;
        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then 'client changes' are refreshed - Client wins
        ///</remarks>
        void CommitAndRefreshChanges();
         

        /// <summary>
        /// Rollback tracked changes. See references of UnitOfWork pattern
        /// </summary>
        void RollbackChanges();

        //IQuerable repository members
        IRepository<User> UserRepository { get;}
        IRepository<MenuItem> MenuItemRepository { get;}
        IRepository<Permission> PermissionRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Token> TokenRepository { get; }
        IRepository<StudentDocuments> DocumentRepository { get; }
        IRepository<AppImages> AppImagesRepository { get; }
        IRepository<Country> CountryRepository { get; }
        IRepository<State> StateRepository { get; }
        IRepository<LGA> LgaRepository { get; }
        IRepository<Department> DepartmentRepository { get; }
        IRepository<OtherAcademicQualifications> OtherAcademicQualificationsRepository { get; }
        IRepository<Semester> SemesterRepository { get; }
        IRepository<Session> SessionRepository { get; }
        IRepository<Staff> StaffRepository { get; }
        IRepository<Student> StudentRepository { get; }
        IRepository<UserData> UserDataRepository { get; }
        IRepository<PaymentGateways> PaymentGatewaysRepository { get;}
        IRepository<ApiLog> ApiLogRepository { get;}

        //AcademicModule
        IRepository<CourseRegistration> CourseRegistrationRepository { get; }
        IRepository<CourseRegRecover> CourseRegRecoverRepository { get; }
        IRepository<Course> CourseRepository { get; }
        IRepository<CourseCategory> CourseCategoryRepository { get; }
        ISemesterRegistrationsRepository SemesterRegistrationsRepository { get; }
        IRepository<CourseSchedule> CourseScheduleRepository { get; }
        IRepository<Programme> ProgrammeRepository { get; }
        IRepository<Faculty> FacultyRepository { get; }
        IRepository<ResultComplain> ResultComplainRepository { get; }
        IRepository<OutStandingCourse> OutStandingCourseRepository { get; }
        IRepository<Calender> CalenderRepository { get;}
        IRepository<CalenderDetail> CalenderDetailRepository { get; }
        IRepository<ScoresEntryLog> ScoresEntryLogRepository { get;}
        IRepository<Applicants> ApplicantsRepository { get;}
        IRepository<ProgrammeTypes> ProgrammeTypeRepository { get; }
        IRepository<JambResult> JambResultRepository { get;}
        IRepository<JambScores> JambScoresRepository { get; }
        IRepository<Grading> GradingRepository { get; }
        IRepository<GraduatingClass> GraduatingClassRepository { get; }
        IRepository<RegistrationsPermissionsLog> RegistrationsPermissionsLogRepository { get; }
         
        IRepository<TranscriptApplication> TranscriptRepository { get; }
        IRepository<ExamsOfficer> ExamsOfficerRepository { get; }
        IRepository<OLevelSubject> OlevelSubjectRepository { get; }
        IRepository<OLevelResult> OLevelResultRepository { get; }
        IRepository<OlevelResultDetail> OLevelResultDetailRepository { get; }

        //BursaryModule
        IRepository<Accounts> AccountsRepository { get; }
        IRepository<FeeSchedule> FeeScheduleRepository { get; }
        IRepository<OtherCharges> OtherChargesRepository { get;}
        IRepository<TransMaster> TransMasterRepository { get; }
        IRepository<StudentPayments> StudentPaymentsRepository { get;}
        IRepository<FeesExceptions> FeesExceptionsRepository { get; }
        IRepository<FeeOptions> FeeOptionsRepository { get;}
        IRepository<AccountsGroup> AccountsGroupRepository { get; }
        IRepository<PaymentInvoice> PaymentInvoiceRepository { get; }
        IRepository<FeeScheduleDetail> FeeScheduleDetailRepository { get;}
        IRepository<GateWaylogs> GateWaylogsRepository { get; }
         
        IInvoiceDetailRepository InvoiceDetailsRepository { get; }

        //Articles
        IRepository<Article> ArticleRepository { get;}
        IRepository<ObongPublications> ObongPublicationsRepository { get;}

        //Hostel
       // IRepository<BedSpace> BedSpaceRepository { get; }
        //IRepository<Hostel> HostelRepository { get; }
        //IRepository<HostelAllocations> HostelAllocationRepository { get; }



    }
}
