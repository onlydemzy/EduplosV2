using Eduplos.DTO.AcademicModule;
using System;
using System.Collections.Generic;

namespace Eduplos.DTO.CoreModule
{
    public class MatricRegisterDTO
    {
        public string Session { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string ProgrammeType { get; set; }
        public string YearAdmitted { get; set; }
        public List<MatricRegHeadings> Headings{get;set;}

        
        
    }
       
   public class MatricRegHeadings
    {
        
        public string Heading { get; set; }
        public List<MatricRegDetailsDTO> Details { get; set; }
    }
    public class MatricRegDetailsDTO
    {
        public string State { get; set; }
        public string Lg { get; set; }
        public string Programme { get; set; }
        public string Phone { get; set; }
        public string Email{ get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string Address { get; set; }
        public int JambScore { get; set; }
        public string Name { get; set; }
        public string JambRegNo { get; set; }
        public string MatricNo { get; set; }
        public string StudentId { get; set; }
        public string EntryMode { get; set; }
    }
     
}
