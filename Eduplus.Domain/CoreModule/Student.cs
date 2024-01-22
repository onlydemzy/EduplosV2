using Eduplos.Domain.AcademicModule;
using Eduplos.Domain.BurseryModule;
using System;
using System.Collections.Generic;

namespace Eduplos.Domain.CoreModule
{
    public class Student:Person
    {
        public Student()
        {
            JambResults = new HashSet<JambResult>();
            OlevelResults = new HashSet<OLevelResult>();
            
        }
        public string MatricNumber { get; set; }
        public string YearAddmitted { get; set; }
        public string ProgrammeType { get; set; }
        public string StudyMode { get; set; }
        public string EntryMode { get; set; }
        public byte? Duration { get; set; }
        
        public double? BaseCGPA { get; set; }
        public string ReasonForTransfer { get; set; }
        public string IsHandicapped { get; set; }
        public int? CurrentLevel { get; set; }
       
        public string GradYear { get; set; }
        public string GradBatch { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public byte AddmissionCompleteStage { get; set; }
         
        public string WhyUs { get; set; }
        public string AdmissionStatus { get; set; }
        public virtual ICollection<StudentPayments> Payments { get; set; }
        public virtual ICollection<JambResult> JambResults { get; set; }
        public virtual ICollection<OLevelResult> OlevelResults { get; set; }

    }
}
