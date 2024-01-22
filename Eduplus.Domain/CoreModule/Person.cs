using Eduplos.Domain.AcademicModule;
using System;
using System.Collections.Generic;

namespace Eduplos.Domain.CoreModule
{
    public abstract class Person
    {
        public Person()
        {
            OtherQualifications = new HashSet<OtherAcademicQualifications>();
        }
        public string PersonId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string MIddlename { get; set; }
        public string ResidentialAddress { get; set; }
        public string HomeTown { get; set; }
        public string PermanentHomeAdd { get; set; }
        public string SpouseName { get; set; }
        public string SpouseAddress { get; set; }
        public string State { get; set; }
        public string Lg { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string MaritalStatus { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string MailingAddress { get; set; }
        public string HighestQualification { get;set;}
        public int? BDay { get; set; }
        public string BMonth { get; set; }
        public int? BYear { get; set; }
        public string DepartmentCode { get; set; }
        public string NextKin { get; set; }
        public string kinAddress { get; set; }
        public string Relationship { get; set; }
        public string KinPhone  { get; set; }
        public string KinMail { get; set; }
        public string ProgrammeCode { get; set; }
        public string RefereeAddress { get; set; }
        public string Referee { get; set; }
        public string RefereePhone { get; set; }
        public string RefereeMail { get; set; }
       
        public string PhotoId { get; set; }
        public virtual AppImages Photo { get; set; }
        public string Status { get; set; }
        public string IDNumber { get; set; }
        public string IDType { get; set; }
        public virtual Programme Programme { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<OtherAcademicQualifications> OtherQualifications { get; set; }
        public string Name
        {
            get {
                if(!string.IsNullOrEmpty(MIddlename))
                {
                    return Title + " " + Surname.Trim() + ", " + Firstname.Trim() + " "+MIddlename.Trim();
                }
                else
                {
                    return Title + " " + Surname.Trim() + ", " + Firstname.Trim();
                }

                }
        }

        public string BirthDate
        {

            get
            {
                if (this.BDay > 0 && this.BYear > 0)
                {
                    return BDay.ToString() + "-" + BMonth + "-" + BYear.ToString();
                }
                else
                    return null;
            }
        }
    }
}
