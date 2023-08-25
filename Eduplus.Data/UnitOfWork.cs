using Eduplus.Data.EntityConfiguration.CoreModule;
using Eduplus.Data.EntityConfiguration.HRMModule;
using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.Data.EntityConfiguration.AcademicModule;
using KS.Core.UserManagement;
using KS.Data.EntityConfiguration.UserManagement;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using UniversitySolution.Data.CoreBC;
using Eduplus.Domain.ArticleModule;
using Eduplus.Data.EntityConfiguration.ArticleModule;
using KS.Core;
using KS.Data.Repositories;
using KS.Domain.HRModule;
using Eduplus.Data.EntityConfiguration.BurseryModule;
using Eduplus.Domain.BurseryModule;
using KS.Domain.AccountsModule;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Eduplus.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using Eduplus.Domain;
using Eduplus.Data.Repositories;

namespace KS.Data
{
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        #region Constructor
        #region Private IDbSet fields

        //User Manager
        private readonly Repository<User> _userRepo;
        private readonly Repository<MenuItem> _menuItemRepo;
        private readonly Repository<Permission> _permissionRepo;
        private readonly Repository<Role> _roleRepo;
        private readonly Repository<AuditLog> _auditLogRepo;
        private readonly Repository<Token> _tokenRepo;
        private readonly Repository<ApiLog> _apiLogRepo;

        private readonly Repository<StudentDocuments> _documentRepo;
        private readonly Repository<Country> _countryRepo;
        private readonly Repository<State> _stateRepo;
        private readonly Repository<LGA> _lgaRepo;
        private readonly Repository<Student> _studentRepo;
        private readonly Repository<Session> _sessionRepo;
        private readonly Repository<Semester> _semesterRepo;
        private readonly Repository<Department> _departmentRepo;
        private readonly Repository<UserData> _userDataRepo;
        private readonly Repository<AppImages> _appImagesRepo;
        private readonly Repository<Staff> _staffRepo;
        private readonly Repository<OtherAcademicQualifications> _otherAcadaQualificationsRepo;
        private readonly Repository<PaymentGateways> _paymentGatewaysRepo;

        private readonly Repository<Faculty> _facultyRepo;
        private readonly Repository<Programme> _programmeRepo;
        private readonly Repository<Course> _courseRepo;
        private readonly Repository<CourseCategory> _courseCategoryRepo;
        private readonly Repository<CourseRegistration> _courseRegistrationRepo;
        private readonly Repository<CourseRegRecover> _courseRegRecoverRepo;
        private readonly Repository<OutStandingCourse> _outStandingCourseRepo;
        private readonly Repository<CourseSchedule> _courseScheduleRepo;
        private readonly Repository<ResultComplain> _resultComplainRepo;
        private readonly Repository<ScoresEntryLog> _scoresEntryLogRepo;
        private readonly ISemesterRegistrationsRepository _semesterRegistrationsRepo;
        private readonly Repository<Calender> _calenderRepo;
        private readonly Repository<CalenderDetail> _calenderDetailRepo;
        private readonly Repository<Applicants> _applicantsRepo;
        private readonly Repository<Article> _articleRepo;
        private readonly Repository<ObongPublications> _obongPublicationsRepo;
        private readonly Repository<ProgrammeTypes> _programmeTypeRepo;
        private readonly Repository<JambResult> _jambResultRepo;
        private readonly Repository<JambScores> _jambScoresRepo;
        private readonly Repository<Grading> _gradingRepo;
        private readonly Repository<GraduatingClass> _gradClassRepo;
        private readonly Repository<RegistrationsPermissionsLog> _lateRegLogRepo;
        private readonly Repository<TranscriptApplication> _transcriptRepo;
        private readonly Repository<ExamsOfficer> _examsOfficerRepo;
        private readonly Repository<Accounts> _accountsRepo;
        private readonly Repository<FeeSchedule> _feeScheduleRepo;
        private readonly Repository<FeesExceptions> _feeExemptionRepo;
        private readonly Repository<StudentPayments> _studentPaymentsRepo;
        private readonly Repository<OtherCharges> _otherChargesRepo;
        private readonly Repository<TransMaster> _transMasterRepo;
        private readonly Repository<AccountsGroup> _accountsGroupRepo;
        private readonly Repository<FeeOptions> _feeOptionsRepo;
        private readonly Repository<PaymentInvoice> _paymentInvoiceRepo;
        private readonly Repository<FeeScheduleDetail> _feeScheduleDetailRepo;
        private readonly IRepository<GateWaylogs> _gatewayRepo;
        private readonly IInvoiceDetailRepository _invoiceDetailsRepo;
       // private readonly IRepository<InvoiceDetails> _invoiceDetailsRepo;
        private readonly Repository<OLevelSubject> _olevelSubjectRepo;
        private readonly Repository<OLevelResult> _olevelResultRepo;
        private readonly Repository<OlevelResultDetail> _olevelResultDetailRepo;


        //Hostel Module



        #endregion

        #region PUBLIC PROPERTIES
        public IDbSet<User> User { get; set; }
        public IDbSet<Role> Role { get; set; }
        public IDbSet<Permission> Permission { get; set; }
        public IDbSet<MenuItem> MenuItem { get; set; }
        public IDbSet<AuditLog> AuditLog { get; set; }
        public IDbSet<Token> Token { get; set; }
        public IDbSet<ApiLog> ApiLog { get; set; }

        //Core Module
        public IDbSet<StudentDocuments> Document { get; set; }
        public IDbSet<Student> Student { get; set; }
        public IDbSet<Department> Department { get; set; }
        public IDbSet<State> State { get; set; }
        public IDbSet<Country> Country { get; set; }
        public IDbSet<LGA> Lga { get; set; }
        public IDbSet<Semester> Semester { get; set; }
        public IDbSet<Session> Session { get; set; }
        public IDbSet<UserData> UserData { get; set; }
        public IDbSet<OtherAcademicQualifications> OtherQualifications { get; set; }
        public IDbSet<Staff> Staff { get; set; }
        public IDbSet<AppImages> AppImages { get; set; }
        public IDbSet<PaymentGateways> PaymentGateways { get; set; }
        
        //Academic Module
        public IDbSet<Course> Course { get; set; }
        public IDbSet<CourseCategory> CourseCategory { get; set; }
        public IDbSet<CourseRegistration> CourseRegistration { get; set; }
        public IDbSet<CourseRegRecover> CourseRegRecover { get; set; }
        public IDbSet<CourseSchedule> CourseSchedule { get; set; }
        
        public IDbSet<Faculty> Faculty { get; set; }
        public IDbSet<JambResult> JambResult { get; set; }
        public IDbSet<JambScores> JambScores { get; set; }
        public IDbSet<OLevelResult> OlevelResults { get; set; }
        public IDbSet<OLevelSubject> OLevelSubject { get; set; }
        public IDbSet<OutStandingCourse> OutStandingCourse { get; set; }
        public IDbSet<Programme> Programme { get; set; }
        public IDbSet<ResultComplain> ResultComplain { get; set; }
        public IDbSet<ResultComplainDetail> ResultComplainDetail { get; set; }
        public IDbSet<ScoresEntryLog> ScoresEntry { get; set; }
        public DbSet<SemesterRegistrations> SemesterRegistrations { get; set; }
        public IDbSet<Calender> Calender { get; set; }
        public IDbSet<CalenderDetail> CalenderDetail { get; set; }
        public IDbSet<Applicants> Applicants { get; set; }
        public IDbSet<ProgrammeTypes> ProgrammeType { get; set; }
        public IDbSet<Grading> Grading { get; set; }
        public IDbSet<GraduatingClass> GraduatingClass { get; set; }
        public IDbSet<RegistrationsPermissionsLog> RegistrationsPermissionsLog { get; set; }
        public IDbSet<MatricNoFormat> MatricNoFormat { get; set; }
        public IDbSet<TranscriptApplication> TranscriptApplication { get; set; }
        public IDbSet<ExamsOfficer> ExamsOfficer { get; set; }
        public IDbSet<OlevelResultDetail> OlevelResultDetail { get; set; }
        
        //Article Module
        public IDbSet<Article> Article { get; set; }
        public IDbSet<ObongPublications> ObongPublications { get; set; }

        //Bursery
        public IDbSet<Accounts> Accounts { get; set; }
        public IDbSet<FeeSchedule> FeeSchedule { get; set; }
        public IDbSet<FeeScheduleDetail> FeeScheduleDetail { get; set; }
        public IDbSet<StudentPayments> StudentPayments { get; set; }
        public IDbSet<OtherCharges> OtherCharges { get; set; }
        public IDbSet<FeesExceptions> FeesExceptions { get; set; }
        public IDbSet<TransMaster> TransMaster { get; set; }
        public IDbSet<FeeOptions> FeeOptions { get; set; }
        public IDbSet<AccountsGroup> AccountsGroup { get; set; }
        public IDbSet<PaymentInvoice> PaymentInvoice { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public IDbSet<GateWaylogs> GateWaylogs { get; set; }
        #endregion

        #region DEFAULT CONSTRUCTOR
        public UnitOfWork()
            : base("name=Conn")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;

            //Initialising db with default seed data
            Database.SetInitializer<UnitOfWork>(new DBInitializer());

            _userRepo = new Repository<User>(this.User);
            _menuItemRepo= new Repository<MenuItem>(this.MenuItem);
            _roleRepo = new Repository<Role>(this.Role);
            _permissionRepo= new Repository<Permission>(this.Permission);
            _auditLogRepo= new Repository<AuditLog>(this.AuditLog);
            _userDataRepo= new Repository<UserData>(this.UserData);
            _tokenRepo = new Repository<Token>(this.Token);
            _documentRepo = new Repository<StudentDocuments>(this.Document);
            _apiLogRepo = new Repository<ApiLog>(this.ApiLog);
            _countryRepo= new Repository<Country>(this.Country);
            _stateRepo= new Repository<State>(this.State);
            _lgaRepo= new Repository<LGA>(this.Lga);
            _studentRepo= new Repository<Student>(this.Student);
            _staffRepo= new Repository<Staff>(this.Staff);
            _departmentRepo= new Repository<Department>(this.Department);
            _sessionRepo= new Repository<Session>(this.Session);
            _semesterRepo= new Repository<Semester>(this.Semester);
            _appImagesRepo= new Repository<AppImages>(this.AppImages);
            _paymentGatewaysRepo = new Repository<PaymentGateways>(this.PaymentGateways);

            _courseRepo= new Repository<Course>(this.Course);
            _courseRegistrationRepo= new Repository<CourseRegistration>(this.CourseRegistration);
            _courseRegRecoverRepo = new Repository<CourseRegRecover>(this.CourseRegRecover);
            _semesterRegistrationsRepo = new SemesterRegistrationsRepository(this.SemesterRegistrations);
            _outStandingCourseRepo= new Repository<OutStandingCourse>(this.OutStandingCourse);
            _courseScheduleRepo= new Repository<CourseSchedule>(this.CourseSchedule);
            _scoresEntryLogRepo = new Repository<ScoresEntryLog>(this.ScoresEntry);
            _facultyRepo= new Repository<Faculty>(this.Faculty);
            _programmeRepo= new Repository<Programme>(this.Programme);
            _resultComplainRepo= new Repository<ResultComplain>(this.ResultComplain);
            _otherAcadaQualificationsRepo = new Repository<OtherAcademicQualifications>(this.OtherQualifications);
            _calenderRepo = new Repository<Calender>(this.Calender);
            _calenderDetailRepo = new Repository<CalenderDetail>(this.CalenderDetail);
            _applicantsRepo = new Repository<Applicants>(this.Applicants);
            _articleRepo = new Repository<Article>(this.Article);
            _obongPublicationsRepo = new Repository<ObongPublications>(this.ObongPublications);
            _programmeTypeRepo = new Repository<ProgrammeTypes>(this.ProgrammeType);
            _jambResultRepo = new Repository<JambResult>(this.JambResult);
            _jambScoresRepo = new Repository<JambScores>(this.JambScores);
            _gradingRepo = new Repository<Grading>(this.Grading);
            _gradClassRepo = new Repository<GraduatingClass>(this.GraduatingClass);
            _courseCategoryRepo = new Repository<CourseCategory>(this.CourseCategory);
            _lateRegLogRepo=new Repository<RegistrationsPermissionsLog>(this.RegistrationsPermissionsLog);
            
            _transcriptRepo = new Repository<TranscriptApplication>(this.TranscriptApplication);
            _examsOfficerRepo = new Repository<ExamsOfficer>(this.ExamsOfficer);
            _olevelSubjectRepo = new Repository<OLevelSubject>(this.OLevelSubject);
            _olevelResultRepo = new Repository<OLevelResult>(this.OlevelResults);
            _olevelResultDetailRepo = new Repository<OlevelResultDetail>(this.OlevelResultDetail);
            _accountsRepo = new Repository<Accounts>(this.Accounts);
            _feeScheduleRepo = new Repository<FeeSchedule>(this.FeeSchedule);
            _otherChargesRepo = new Repository<OtherCharges>(this.OtherCharges);
            _transMasterRepo = new Repository<TransMaster>(this.TransMaster);
            _studentPaymentsRepo = new Repository<StudentPayments>(this.StudentPayments);
            _feeExemptionRepo = new Repository<FeesExceptions>(this.FeesExceptions);
            _feeOptionsRepo = new Repository<FeeOptions>(this.FeeOptions);
            _paymentInvoiceRepo = new Repository<PaymentInvoice>(this.PaymentInvoice);
            _gatewayRepo = new Repository<GateWaylogs>(this.GateWaylogs);
            _accountsGroupRepo = new Repository<AccountsGroup>(this.AccountsGroup);
            _feeScheduleDetailRepo = new Repository<FeeScheduleDetail>(this.FeeScheduleDetail);
            _invoiceDetailsRepo = new InvoiceDetailRepository(this.InvoiceDetails);
           
        }
        #endregion


        #endregion Constructor

        #region Public REPOSITORY PROPERTIES
        //===================================User Managment
        public IRepository<MenuItem> MenuItemRepository
        {
            get
            {
                return _menuItemRepo;
            }
        }
        
        public IRepository<Permission> PermissionRepository
        {
            get
            {
                

                return _permissionRepo;
            }
        }

        public IRepository<Role> RoleRepository
        {
            get
            {
                
                return _roleRepo;
            }
        }
        public IRepository<User> UserRepository
        {
            get
            {
                return _userRepo;
            }
        }

        public IRepository<AuditLog> AuditLogRepository
        {
            get
            {
                return _auditLogRepo;
            }
         }
        public IRepository<ApiLog> ApiLogRepository
        {
            get
            {
                return _apiLogRepo;
            }
        }
        public IRepository<Token> TokenRepository
        {
            get
            {
                return _tokenRepo;
            }
        }

        //==========================CoreModule=============================================
        public IRepository<StudentDocuments> DocumentRepository
        {
            get
            {
                return _documentRepo;
            }
        }
        public IRepository<Country> CountryRepository
        {
            get
            {
               
                return _countryRepo;
            }
        }
        public IRepository<Department> DepartmentRepository
        {
            get
            {
                return _departmentRepo;
            }
        }
        public IRepository<LGA> LgaRepository
        {
            get { 
                return _lgaRepo;
            }
        }

       
        public IRepository<Semester> SemesterRepository
        {
            get
            {
                return _semesterRepo;
            }
        }
        public IRepository<Session> SessionRepository
        {
            get
            {
                
                return _sessionRepo;
            }
        }
        public IRepository<State> StateRepository
        {
            get
            {
                return _stateRepo;
            }
        }
        public IRepository<Student> StudentRepository
        {
            get
            {
                return _studentRepo;
            }
        }
        public IRepository<UserData> UserDataRepository
        {
            get
            {
                return _userDataRepo;
            }
        }
        public IRepository<Staff> StaffRepository
        {
            get
            {
                return _staffRepo;
            }
        }

        public IRepository<AppImages> AppImagesRepository
        {
            get
            {
                return _appImagesRepo;
            }
        }
        public IRepository<PaymentGateways> PaymentGatewaysRepository
        {
            get
            {
                return _paymentGatewaysRepo;
            }
        }
        //=====================================Academics Module========================================
        public IRepository<Course> CourseRepository
        {
            get
            {
                return _courseRepo;
            }
        }
        public IRepository<CourseCategory> CourseCategoryRepository
        {
            get
            {
                return _courseCategoryRepo;
            }
        }
        public IRepository<OtherAcademicQualifications> OtherAcademicQualificationsRepository
        {
            get
            {
                return _otherAcadaQualificationsRepo;
            }
        }
        public IRepository<CourseRegistration> CourseRegistrationRepository
        {
            get
            {
                return _courseRegistrationRepo;
            }
        }

        public IRepository<CourseRegRecover> CourseRegRecoverRepository
        {
            get
            {
                return _courseRegRecoverRepo;
            }
        }
        public IRepository<CourseSchedule> CourseScheduleRepository
        {
            get
            {
                return _courseScheduleRepo;
            }
        }
        public IRepository<Faculty> FacultyRepository
        {
            get
            {
                return _facultyRepo;
            }
        }
        
        public IRepository<OutStandingCourse> OutStandingCourseRepository
        {
            get
            {
                return _outStandingCourseRepo;
            }
        }
        public IRepository<Programme> ProgrammeRepository
        {
            get
            {
                return _programmeRepo;
            }
        }
        public IRepository<ResultComplain> ResultComplainRepository
        {
            get
            {
                return _resultComplainRepo;
            }
        }
        public IRepository<ScoresEntryLog> ScoresEntryLogRepository
        {
            get
            {
                return _scoresEntryLogRepo;
            }
        }
        public ISemesterRegistrationsRepository SemesterRegistrationsRepository
        {
            get
            {
                return _semesterRegistrationsRepo;
            }
        }
        public IRepository<Calender> CalenderRepository
        {
            get
            {
                return _calenderRepo;
            }
        }
        public IRepository<CalenderDetail> CalenderDetailRepository
        {
            get
            {
                return _calenderDetailRepo;
            }
        }
        public IRepository<Applicants> ApplicantsRepository
        {
            get
            {
                return _applicantsRepo;
            }
        }
        public IRepository<ProgrammeTypes> ProgrammeTypeRepository
        {
            get
            {
                return _programmeTypeRepo;
            }
        }
        public IRepository<JambResult> JambResultRepository
        {
            get
            {
                return _jambResultRepo;
            }
        }
        public IRepository<JambScores> JambScoresRepository
        {
            get
            {
                return _jambScoresRepo;
            }
        }
        public IRepository<Grading> GradingRepository
        {
            get
            {
                return _gradingRepo;
            }
        }
        public IRepository<GraduatingClass> GraduatingClassRepository
        {
            get
            {
                return _gradClassRepo;
            }
        }
        public IRepository<RegistrationsPermissionsLog> RegistrationsPermissionsLogRepository
        {
            get
            {
                return _lateRegLogRepo;
            }
        }
        
        public IRepository<TranscriptApplication> TranscriptRepository
        {
            get
            {
                return _transcriptRepo;
            }
        }
        public IRepository<ExamsOfficer> ExamsOfficerRepository
        {
            get
            {
                return _examsOfficerRepo;
            }
        }
        public IRepository<OLevelSubject> OlevelSubjectRepository
        {
            get
            {
                return _olevelSubjectRepo;
            }
        }
        public IRepository<OLevelResult> OLevelResultRepository
        {
            get
            {
                return _olevelResultRepo;
            }
        }
        public IRepository<OlevelResultDetail> OLevelResultDetailRepository
        {
            get { return _olevelResultDetailRepo; }
        }
        //=============================================Articles Modules========================
        public IRepository<Article> ArticleRepository
        {
            get
            {
                
                return _articleRepo;
            }
        }
        public IRepository<ObongPublications> ObongPublicationsRepository
        {
            get
            {

                return _obongPublicationsRepo;
            }
        }

        //========================================Bursery Module==================================
        public IRepository<Accounts> AccountsRepository
        {
            get
            {

                return _accountsRepo;
            }
        }
        public IRepository<FeeSchedule> FeeScheduleRepository
        {
            get
            {

                return _feeScheduleRepo;
            }
        }
        public IRepository<OtherCharges> OtherChargesRepository
        {
            get
            {

                return _otherChargesRepo;
            }
        }
        public IRepository<StudentPayments> StudentPaymentsRepository
        {
            get
            {

                return _studentPaymentsRepo;
            }
        }
        public IRepository<TransMaster> TransMasterRepository
        {
            get
            {

                return _transMasterRepo;
            }
        }
        public IRepository<FeesExceptions> FeesExceptionsRepository
        {
            get
            {

                return _feeExemptionRepo;
            }
        }
        public IRepository<AccountsGroup> AccountsGroupRepository
        {
            get
            {
                return _accountsGroupRepo;
            }
        }
        public IRepository<FeeOptions> FeeOptionsRepository
        {
            get
            {
                return _feeOptionsRepo;
            }
        }
        public IRepository<PaymentInvoice> PaymentInvoiceRepository
        {
            get
            {
                return _paymentInvoiceRepo;
            }
        }
        public IRepository<FeeScheduleDetail> FeeScheduleDetailRepository
        {
            get
            {
                return _feeScheduleDetailRepo;
            }
        }
        public IRepository<GateWaylogs> GateWaylogsRepository
        {
            get
            {
                return _gatewayRepo;
           }
        }
        public IInvoiceDetailRepository InvoiceDetailsRepository
        {
            get { return _invoiceDetailsRepo; }
        }

        
        #endregion

        #region IQueryableUnitOfWork Members

        public DbSet<T> CreateSet<T>()
            where T : class
        {
            return base.Set<T>();
        }

        public void Attach<T>(T item)
            where T : class
        {
            //attach and set as unchanged
            base.Entry<T>(item).State =  EntityState.Unchanged;
        }

        public void SetModified<T>(T item)
            where T : class
        {
            //this operation also attach item in object state manager
            base.Entry<T>(item).State = EntityState.Modified;
        }
        

        public void Commit(string userId)
        {
            // Get all Added/Deleted/Modified entities
           try
           {
                foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == System.Data.Entity.EntityState.Added
            || p.State == System.Data.Entity.EntityState.Modified || p.State == System.Data.Entity.EntityState.Deleted))
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (AuditLog x in GetAuditRecordsForChange(ent, userId))
                    {
                        this.AuditLog.Add(x);
                    }
                }
                base.SaveChanges();
          }
            catch(DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(a => a.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErorMessages = string.Join(";", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErorMessages);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            
        }

        public async void CommitAsync()
        {
            await base.SaveChangesAsync();
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed = false;
            

            do
            {
                try
                {
                    base.SaveChanges();

                    saveFailed = false;

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });

                }
            } while (saveFailed);

        }

        public void RollbackChanges()
        {
            // set all entities in change tracker 
            // as 'unchanged state'
            base.ChangeTracker.Entries()
                              .ToList()
                              .ForEach(entry => entry.State = EntityState.Unchanged);
        }

        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters)
        {
            return base.Database.SqlQuery<T>(sqlQuery, parameters);
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return base.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

        private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, string userId)
        {
            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.UtcNow;
           

            // Get the Table() attribute, if one exists
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            if (dbEntry.State == System.Data.Entity.EntityState.Added)
            {
                // For Inserts, just add the whole record
                // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
                AuditLog al = new AuditLog();
                List<string> keys = new List<string>();
                al.UserId = userId;
                al.EventDate = changeTime;
                al.Action = "A"; // Added
                al.TableName = tableName;
                al.PrimaryKey = GetPrimaryKeyValue(dbEntry);
                  // Again, adjust this if you have a multi-column key
                al.ColumnName = "*ALL";    // Or make it nullable, whatever you want
                                           //NewValue = dbEntry.CurrentValues.ToObject().ToString(),
                List<string> sb = new List<string>();//Using String.Join can still achieve this;
                foreach (string propertyName in dbEntry.CurrentValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually chan
                    string cg = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
                    string pn = propertyName + ":" + cg;
                    sb.Add(pn);
                    
                }
                al.NewValue = string.Join("_",sb);
                result.Add(al);
            }
            else if (dbEntry.State == System.Data.Entity.EntityState.Deleted)
            {
                AuditLog al = new AuditLog();
                List<string> keys = new List<string>();
                al.UserId = userId;
                al.EventDate = changeTime;
                al.Action = "D"; // Added
                al.TableName = tableName;
                al.PrimaryKey = GetPrimaryKeyValue(dbEntry);
                // Again, adjust this if you have a multi-column key
                al.ColumnName = "*ALL";    // Or make it nullable, whatever you want
                                           //NewValue = dbEntry.CurrentValues.ToObject().ToString(),
                List<string> sb = new List<string>();//Using String.Join can still achieve this;
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually chan
                    string cg = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                    string pn = propertyName + ":" + cg;
                    sb.Add(pn);

                }
                al.OriginalValue = string.Join("_", sb);
                result.Add(al);
            }
            else if (dbEntry.State == System.Data.Entity.EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        result.Add(new AuditLog()
                        {
                            
                            UserId = userId,
                            EventDate = changeTime,
                            Action = "M",    // Modified
                            TableName = tableName,
                            PrimaryKey = GetPrimaryKeyValue(dbEntry),
                            ColumnName = propertyName,
                            OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName)==null?null: dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NewValue =dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                        }
                            );
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

             return result;
        }

        string GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var entity = ObjectContext.GetObjectType(entry.Entity.GetType());
            //Get entity keys of entry
            var keyname = DbContextExtensions.GetKeyNames(this,entity);
            List<string> keys = new List<string>();
            string primaryKey;
            if (keyname.Count() > 1)
            {
                for (int i = 0; i < keyname.Count(); i++)
                {
                    string ptyn = keyname[i];
                    string ptyv = entry.CurrentValues.GetValue<object>(ptyn).ToString();

                    if (entry.State == System.Data.Entity.EntityState.Added)
                    { ptyv = entry.CurrentValues.GetValue<object>(ptyn).ToString(); }
                    else { ptyv = entry.OriginalValues.GetValue<object>(ptyn).ToString(); }

                    keys.Add(ptyn + ":" + ptyv);
                }

                primaryKey = string.Join("_", keys);
            }
            else
            {
                if(entry.State== System.Data.Entity.EntityState.Added)
                { primaryKey = entry.CurrentValues.GetValue<object>(keyname[0]).ToString(); }
                else { primaryKey = entry.OriginalValues.GetValue<object>(keyname[0]).ToString();}
            }

            return primaryKey;
        }
        #endregion

        #region DbContext Overrides

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Remove unused conventions
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //UserManagement
            modelBuilder.Configurations.Add(new UserConfiguration());
             
            modelBuilder.Configurations.Add(new UserRoleConfiguration());
            modelBuilder.Configurations.Add(new PermissionConfiguration());
            modelBuilder.Configurations.Add(new MenuItemConfiguration());

            //CoreModule
            modelBuilder.Configurations.Add(new DepartmentConfiguration());
            modelBuilder.Configurations.Add(new LgConfiguration());
            modelBuilder.Configurations.Add(new OtherQualificationsConfiguration());
            modelBuilder.Configurations.Add(new PersonConfiguration());
            modelBuilder.Configurations.Add(new SemesterConfiguration());
            modelBuilder.Configurations.Add(new SessionConfiguration());
            modelBuilder.Configurations.Add(new StaffConfiguration());
            modelBuilder.Configurations.Add(new StateConfiguration());
            modelBuilder.Configurations.Add(new StudentConfiguration());
            modelBuilder.Configurations.Add(new ImagesConfiguration());
            modelBuilder.Configurations.Add(new DocumentConfiguration());
            modelBuilder.Configurations.Add(new UserDataConfiguration());
            modelBuilder.Configurations.Add(new AuditLogConfiguration());
            modelBuilder.Configurations.Add(new GatewayConfiguration());
            modelBuilder.Configurations.Add(new ApiLogConfiguration());
            //AcademicModule
            modelBuilder.Configurations.Add(new CourseRegistrationConfiguration());
            modelBuilder.Configurations.Add(new CourseRegRecoverConfiguration());
            modelBuilder.Configurations.Add(new CourseScheduleConfiguration());
            
            modelBuilder.Configurations.Add(new FacultyConfiguration());
            modelBuilder.Configurations.Add(new JambResultConfiguration());
            modelBuilder.Configurations.Add(new JambScoresConfiguration());
            modelBuilder.Configurations.Add(new OutStandingCourseConfiguration());
            modelBuilder.Configurations.Add(new ProgrammeConfiguration());
            modelBuilder.Configurations.Add(new ScoresEntryLogConfiguration());
            modelBuilder.Configurations.Add(new SemesterRegistrationsConfiguration());
            modelBuilder.Configurations.Add(new ResultComplainConfiguration());
            modelBuilder.Configurations.Add(new ResultComplainDetailConfiguration());
            modelBuilder.Configurations.Add(new OLevelResultConfiguration());
            modelBuilder.Configurations.Add(new OlevelResultDetailConfiguration());
            modelBuilder.Configurations.Add(new CalenderConfiguration());
            modelBuilder.Configurations.Add(new CalenderDetailConfiguration());
            modelBuilder.Configurations.Add(new ApplicantsConfiguration());
            modelBuilder.Configurations.Add(new ProgrammeTypeConfiguration());
            modelBuilder.Configurations.Add(new CourseConfiguration());
            modelBuilder.Configurations.Add(new CourseCategoryConfiguration());
            modelBuilder.Configurations.Add(new GradingConfiguration());
            modelBuilder.Configurations.Add(new GraduatingClassConfiguration());
            modelBuilder.Configurations.Add(new LategRegLogConfiguration());
            modelBuilder.Configurations.Add(new MatricNoFormatConfiguration());
            modelBuilder.Configurations.Add(new TranscriptConfiguration());
            modelBuilder.Configurations.Add(new ExamsOfficerConfiguration());
            modelBuilder.Entity<RegistrationsPermissionsLog>().HasRequired(a => a.Student)
                .WithMany().HasForeignKey(aa => aa.StudentId);
            //ArticleModule
            modelBuilder.Configurations.Add(new ArticleConfiguration());
            modelBuilder.Configurations.Add(new ObongPublicationsConfiguration());

            //Bursery Module
            modelBuilder.Configurations.Add(new FeeScheduleConfiguration());
            modelBuilder.Configurations.Add(new FeeScheduleDetailConfiguration());
            modelBuilder.Configurations.Add(new StudentPaymentsConfiguration());
            modelBuilder.Configurations.Add(new FeesExceptionsConfiguration());
            modelBuilder.Configurations.Add(new TransMasterConfiguration());
            modelBuilder.Configurations.Add(new AccountsConfiguration());
            modelBuilder.Configurations.Add(new OtherChargesConfiguration());
            modelBuilder.Configurations.Add(new AccountsGroupConfiguration());
            modelBuilder.Configurations.Add(new FeeOptionsConfiguration());
            modelBuilder.Configurations.Add(new InvoiceConfiguration());
            modelBuilder.Configurations.Add(new InvoiceDetailConfiguration());
            modelBuilder.Configurations.Add(new GatewaylogsConfiguration());

            modelBuilder.Entity<OLevelSubject>()
                .HasKey(a => a.SubjectId);
            modelBuilder.Entity<OLevelSubject>()
                .Property(a => a.SubjectId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<OLevelSubject>()
                .Property(a => a.Title).HasMaxLength(150);

            //modelBuilder.HasDefaultSchema("eduplosDemo");


        }

        
        

         void ApplyCurrentValues<T>(T original, T current)
            where T : class
        {
            //if it is not attached, attach original and set current values
            base.Entry<T>(original).CurrentValues.SetValues(current);
        }

        #endregion

    }
}
