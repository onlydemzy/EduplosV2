using Eduplus.Domain.AcademicModule;
using Eduplus.DTO.AcademicModule;
using System.Collections.Generic;
using System.Linq;

namespace Eduplus.ObjectMappings
{
    public static class AcademicModuleMappings
    {
        public static CourseRegistrationDTO CourseToCourseRegistrationDTO(Course course)
        {
            var dto = new CourseRegistrationDTO
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CreditHour = course.CreditHours,
                Title = course.Title,
                Level = course.Level,
                Semester = course.Semester,
                ProgrammeCode = course.ProgrammeCode,
                Type = course.CourseType
            };
            return dto;
        }

        public static ScoresEntryDTO CourseRegToScoresEntryDTO(CourseRegistration registration)
        {
            var dto = new ScoresEntryDTO
            {
                CA1 = registration.CA1,
                CA2=registration.CA2,
                Exam = registration.Exam,
                Grade = registration.Grade,
                CourseCode = registration.Course.CourseCode,
                CourseLevel = registration.Course.Level,
                StudentLevel = registration.Lvl,
                RegNo = registration.Student.MatricNumber,
                RegistrationId = registration.RegistrationId,
                CreditHour = registration.CourseCreditHour,
                Title = registration.Course.Title,
                SemesterId = registration.SemesterId,
                SessionId=registration.SessionId,
                StudentId=registration.StudentId,
                CourseId=registration.CourseId

            };
            return dto;
        }

        public static CourseDTO CoursesToCourseDTOs(Course c)
        {
            List<CourseDTO> dto = new List<CourseDTO>();
            
                return new CourseDTO
                {
                    CourseCode = c.CourseCode,
                    Title = c.Title,
                    CourseId = c.CourseId,
                    Semester=c.Semester,
                    Level=c.Level,
                    Type=c.CourseType,
                    Active=c.IsActive,
                    ProgrammeCode=c.ProgrammeCode,
                    CreditHours=c.CreditHours,
                    Category=c.Category
                };
             
        }
        public static List<StudentAcademicProfileDetailstDTO> CourseRegToAcadaDetailsDTO(List<CourseRegistration> regs)
        {
            List<StudentAcademicProfileDetailstDTO> dto = new List<StudentAcademicProfileDetailstDTO>();
            foreach (var s in regs)
            {
                StudentAcademicProfileDetailstDTO sd = new StudentAcademicProfileDetailstDTO();

                sd.SessionId = s.SessionId;
                
                sd.SemesterId = s.SemesterId;
                sd.Level = s.Lvl;
                sd.RegNo = s.Student.MatricNumber;
                sd.StudentId = s.StudentId;
                sd.CourseCode = s.Course.CourseCode;
                sd.CreditHour = s.CourseCreditHour;
                sd.CA1 = s.CA1;
                sd.CA2 = s.CA2;
                sd.CourseTitle = s.Course.Title;
                sd.Exam = s.Exam;
                sd.Score = s.CalculateTScore(sd.Exam, sd.CA1, sd.CA2); //sd.ScoreM(s.Exam, s.CA);
                sd.Grade = s.Grade;
                sd.CreditHourHeading = s.CreditHourHeading(sd.CourseCode, sd.CreditHour);
                sd.GP = s.GradePoint;
                sd.QP = s.QualityPointM(sd.CreditHour, sd.GP);
                sd.RecordValue = s.RecordValueM(sd.Score, sd.Grade, sd.QP);
                

                dto.Add(sd);
            }
            return dto;
        }

        public static InMemoryScoresDTO CourseRegToInMemoryScoresDTO(CourseRegistration s)
        {
            
                InMemoryScoresDTO sd = new InMemoryScoresDTO();

                sd.SessionId = s.SessionId;
                sd.Semester = s.Semester.SemesterTitle;
                sd.Session = s.Session.Title;
                sd.SemesterId = s.SemesterId;
                sd.Level = s.Lvl;
                sd.Name = s.Student.Name;
                sd.MatricNumber = s.Student.MatricNumber;
                sd.StudentId = s.StudentId;
                sd.CourseCode = s.Course.CourseCode;
                sd.CreditHour = s.CourseCreditHour;
                sd.CA1 = s.CA1;
                sd.CA2 = s.CA2;
                sd.CourseTitle = s.Course.Title;
                sd.Exam = s.Exam;
                sd.Score = s.CalculateTScore(sd.Exam, sd.CA1,sd.CA2); //sd.ScoreM(s.Exam, s.CA);
                sd.Grade = s.Grade;
                sd.CreditHourHeading = s.CreditHourHeading(sd.CourseCode, sd.CreditHour);
                sd.GP = s.GradePoint;
                sd.QP = s.QualityPointM(sd.CreditHour, sd.GP);
                sd.RecordValue = s.RecordValueM(sd.Score, sd.Grade, sd.QP);
                sd.Programme = s.Student.Programme.Title;
                sd.Department = s.Student.Department.Title;
                sd.Faculty = s.Student.Department.Faculty.Title;
            sd.CourseCategory = s.Course.Category;

                if(s.Student.BaseCGPA>0)
                {
                    sd.BaseCGPA = (double)s.Student.BaseCGPA;
                }

               
            return sd;
        }
    }
}
