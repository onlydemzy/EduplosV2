﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.AcademicModule
{
    public class InMemoryScoresDTO
    {
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public string Session { get; set; }
        public string Semester { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public int CreditHour { get; set; }
        public int CA1 { get; set; }
        public int CA2 { get; set; }
        public int Exam { get; set; }
        public int Score { get; set; }
        public string Grade { get; set; }
        public string CourseId { get; set; }
        public int Level { get; set; }
        public int SemesterId { get; set; }
        public int SessionId { get; set; }
        public string StudentId { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Gender { get; set; }
        public string YearAdmitted { get; set; }
        public double BaseCGPA { get; set; }
        public string CreditHourHeading { get; set; }
        public string RecordValue { get; set; }
        public double GP { get; set; }
        public double QP { get; set; }
        public string CourseCategory { get; set; }
        public long RegistrationId { get; set; }

    }
}
