using System;

namespace Eduplus.DTO
{
    public class StudentProfileDTO
    {
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string ResidentialAddress { get; set; }
        public string State { get; set; }
        public string Lg { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Sex { get; set; }
        public string PhotoPath { get;set;}
        public string Email { get; set; }
        public string HighestQualification { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DepartmentCode { get; set; }
        public string SponsorAddress { get; set; }
        public string SponsorPhone { get; set; }
        public string SponsorMail { get; set; }
        public DateTime? CreateDate { get; set; }
        public string InputtedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpDatedBy { get; set; }
        public byte[] Photo { get; set; }
        public string Status { get; set; }
        public string MatricNumber { get; set; }
        public string YearAddmitted { get; set; }
        public int? ProgrammeId { get; set; }
        public string EntryMode { get; set; }
        public string Sponsor { get; set; }
        public string CourseStudy { get; set; }
        public byte? Duration { get; set; }
        public string StudyLvl { get; set; }
        public string CGPA { get; set; }
        public string ReasonForTransfer { get; set; }
        public int CurrentLevel { get; set; }
        public string JambRegNumber { get; set; }
    }
}
