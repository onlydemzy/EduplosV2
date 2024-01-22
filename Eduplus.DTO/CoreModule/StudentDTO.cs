using Eduplos.DTO.AcademicModule;
using System;
using System.Collections.Generic;

namespace Eduplos.DTO.CoreModule
{
    public class StudentDTO
    {
        public string Password { get; set; }
        public string StudentId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string FullName { get; set; }
        public string Middlename { get; set; }
        public string ResidentialAddress { get; set; }
        public string State { get; set; }
        public string Lg { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string MaritalStatus { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string MailingAddress { get; set; }
        public string HighestQualification { get; set; }
        public string IsHandicapped { get; set; }
        public int? BDay { get; set; }
        public string BMonth { get; set; }
        public int? BYear { get; set; }
        public string DepartmentCode { get; set; }
        public string NextKin { get; set; }
        public string KinAddress { get; set; }
        public string Relationship { get; set; }
        public string KinPhone { get; set; }
        public string KinMail { get; set; }
        public string ProgrammeCode { get; set; }
        public string FacultyCode { get; set; }
        public string RefereeAddress { get; set; }
        public string Referee { get; set; }
        public string RefereePhone { get; set; }
        public string RefereeMail { get; set; }
        public DateTime? CreateDate { get; set; }
        public string InputtedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpDatedBy { get; set; }
        public string PhotoId { get; set; }
        public byte[] Foto { get; set; }
        public string Status { get; set; }
        public string WhyUs { get; set; }
        public string MatricNumber { get; set; }
        public string YearAdmitted { get; set; }
        public string GradBatch { get; set; }
        public string AdmissionStatus { get; set; }
        public string ProgrammeType { get; set; }
        public string StudyMode { get; set; }
        public string EntryMode { get; set; }
        public byte? Duration { get; set; }
        public string StudyLvl { get; set; }
        
        public string CGPA { get; set; }
        public string ReasonForTransfer { get; set; }

        public int? CurrentLevel { get; set; }
        public string JambRegNumber { get; set; }
        public int JambYear { get; set; }
        public string GradYear { get; set; }
        public byte AddmissionCompleteStage { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Qualification { get; set; }
        public string BirthDate { get; set; }
        public string HomeTown { get; set; }
        public string PermanentHomeAdd { get; set; }
        public string SpouseName { get; set; }
        public string SpouseAddress { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public List<OlevelResultDTO> Olevels { get; set; }
        public JambDTO Jamb { get; set; }
        public List<OtherQualificationDTO> Alevels { get; set; }


        //User Info
        
    }
       
}
