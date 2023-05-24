using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using KS.Domain.HRModule;
using System.Collections.Generic;
using System.Linq;

namespace Eduplus.ObjectMappings
{
    public static class CoreModuleMappings
    {
        public static StudentDTO StudentToStudentDTO(Student student)
        {
            
            StudentDTO dto = new StudentDTO();
             
            JambDTO jams = new JambDTO();

            dto.FullName = student.Name;
            if(!string.IsNullOrEmpty(student.PhotoId))
            {dto.Foto = student.Photo.Foto;
                dto.PhotoId = student.PhotoId;
            }
            dto.AddmissionCompleteStage = student.AddmissionCompleteStage;
            dto.Email = student.Email;
            dto.StudentId = student.PersonId;
            dto.Phone = student.Phone;
            dto.Sex = student.Sex;
            dto.MatricNumber = student.MatricNumber;
            dto.Status = student.Status;
            dto.MaritalStatus = student.MaritalStatus;
            dto.MatricNumber = student.MatricNumber;
            dto.StudyMode = student.StudyMode;
            dto.GradBatch = student.GradBatch;
            
            
            if (!string.IsNullOrEmpty(student.ProgrammeCode))
                {
                dto.Programme = student.Programme.Title;
                dto.ProgrammeCode = student.ProgrammeCode;
                
            }
            if (!string.IsNullOrEmpty(student.DepartmentCode))
            {
                dto.FacultyCode = student.Department.FacultyCode;
                dto.DepartmentCode = student.DepartmentCode;
                dto.Department = student.Department.Title;
                dto.Faculty = student.Department.Faculty.Title;
            }
            
            dto.YearAdmitted = student.YearAddmitted;
            
            dto.ProgrammeType = student.ProgrammeType;
            dto.ReasonForTransfer = student.ReasonForTransfer;
            dto.Referee = student.Referee;
            dto.RefereeAddress = student.RefereeAddress;
            dto.RefereeMail = student.RefereeMail;
            dto.RefereePhone = student.RefereePhone;
            dto.Relationship = student.Relationship;
            dto.ResidentialAddress = student.ResidentialAddress;
            dto.HomeTown = student.HomeTown;
            dto.PermanentHomeAdd = student.PermanentHomeAdd;
            dto.SpouseName = student.SpouseName;
            dto.SpouseAddress = student.SpouseAddress;
            dto.State = student.State;
            dto.AdmissionDate = student.AdmissionDate;
            dto.StudyMode = student.StudyMode;
            dto.Title = student.Title;
            dto.Country = student.Country;
            
            dto.CurrentLevel = student.CurrentLevel;
            dto.BDay = student.BDay;
            dto.BMonth = student.BMonth;
            dto.BYear = student.BYear;
            dto.Duration = student.Duration;
            dto.EntryMode = student.EntryMode;
            dto.Firstname = student.Firstname;
            dto.Middlename = student.MIddlename;
            dto.Surname = student.Surname;
            dto.GradYear = student.GradYear;
            dto.BirthDate = student.BirthDate;
            dto.IsHandicapped = student.IsHandicapped;
            dto.NextKin = student.NextKin;
            
             
            dto.KinAddress = student.kinAddress;
            dto.KinMail = student.KinMail;
            dto.KinPhone = student.KinPhone;
            dto.KinMail = student.KinMail;
            dto.FullName = student.Name;
            dto.Lg = student.Lg;
            dto.AddmissionCompleteStage = student.AddmissionCompleteStage;
            dto.AdmissionStatus = student.AdmissionStatus;


            if (student.JambResults.Count()==1)
            {
                var res = student.JambResults.First();
                dto.JambRegNumber = res.JambRegNumber;
                dto.JambYear = res.JambYear;

                jams.JambYear = res.JambYear;
                jams.Total = res.Score;
                jams.JambRegNumber = res.JambRegNumber;

                if(res.Scores.Count()>0)
                {
                    List<JambScoresDTO> fg = new List<JambScoresDTO>();
                    foreach (var s in res.Scores)
                    {
                        fg.Add(new JambScoresDTO { Subject = s.Subject, Score = s.Score });
                    }
                    jams.Scores = fg;

                }
                dto.Jamb = jams;
            }

           
            //Add Olevl Results========1st sitting
            List<OlevelResultDTO> oldtoList = new List<OlevelResultDTO>();
            if (student.OlevelResults.Count() > 0)
            {
                foreach (var o in student.OlevelResults.ToList())
                {
                    OlevelResultDTO oldto = new OlevelResultDTO();
                    oldto.ExamType = o.ExamType;
                    oldto.ExamNumber = o.ExamNumber;
                    oldto.SitAttempt = o.SitAttempt;
                    oldto.Venue = o.Venue;
                    oldto.Year = o.ExamYear;
                    oldto.StudentId = o.StudentId;
                    List<OlevelResultDetailDTO> details = new List<OlevelResultDetailDTO>();

                    if (o.OlevelResultDetail.Count() > 0)
                    {
                        foreach (var os in o.OlevelResultDetail.ToList())
                        {
                            details.Add(new OlevelResultDetailDTO
                            {
                                Subject = os.Subject,
                                Grade = os.Grade,
                                 
                            });
                        }
                    }
                    oldto.Details = details;
                    oldtoList.Add(oldto);
                }
            }

            dto.Olevels = oldtoList;    
            //Add Alevel programmes
            if(student.OtherQualifications.Count()>0)
            {
                List<OtherQualificationDTO> qu = new List<OtherQualificationDTO>();
                foreach(var o in student.OtherQualifications)
                {
                    qu.Add(new OtherQualificationDTO
                    {
                        EndMonth = o.EndMonth,
                        StartMonth = o.StartMonth,
                        Institution = o.Institution,
                        Qualification = o.Qualification
                    });
                }
                dto.Alevels = qu;
            }
            else { dto.Alevels = new List<OtherQualificationDTO>(); }
            
            return dto;
        }
        public static ApplicantDTO StudentToApplicantDTO(Student student)
        {
            ApplicantDTO dto = new ApplicantDTO();

            dto.Name = student.Name;
            
            dto.AddmissionCompleteStage = student.AddmissionCompleteStage;
            dto.Email = student.Email;
            dto.RegNo = student.PersonId;
            dto.Phone = student.Phone;
            dto.Name = student.Name;
            dto.Status = student.Status;
            dto.EntryMode = student.EntryMode;
            dto.Programme = student.Programme.Title;
            dto.ProgrammeType = student.ProgrammeType;
            dto.Faculty = student.Department.Faculty.Title;
            dto.Department = student.Department.Title;
            dto.StudyMode = student.StudyMode;
            dto.Session = student.YearAddmitted;
            dto.JambNo = student.JambResults.Count()==1?student.JambResults.FirstOrDefault().JambRegNumber:"";
            dto.State = student.State;
            dto.Lga = student.Lg;
            return dto;
        }
        public static DepartmentDTO DepartmentToDepartmentDTO(Department dept)
        {
            DepartmentDTO dto = new DepartmentDTO();
            dto.DepartmentCode = dept.DepartmentCode;
            if(!string.IsNullOrEmpty(dept.FacultyCode))
            { dto.FacultyCode = dept.FacultyCode;
              dto.Faculty = dept.Faculty.Title;
            }
            dto.IsAcademic = dept.IsAcademic;
            dto.Location = dept.Location;
            dto.Title = dept.Title;
            return dto;
        }

        public static StaffDTO StaffToStaffDTO(Staff staff)
        {
            StaffDTO dto = new StaffDTO();
            dto.PersonId = staff.PersonId;
            dto.Name = staff.Name;
            dto.DepartmentCode = staff.DepartmentCode;
            dto.Category = staff.Category;
            dto.Designation = staff.Designation;
            dto.IDNumber = staff.IDNumber;
            dto.IDType = staff.IDType;
            return dto;
        }
        public static SessionDTO SessionToSessionList(Session session)
        {
            var dto = new SessionDTO
            {
                SessionId = session.SessionId,
                Title = session.Title
        };
            return dto;
        }

        public static SemesterDTO SemesterToSemesterDTO(Semester semester)
        {
            var _semester = new SemesterDTO
            {
                SemesterId = semester.SemesterId,
                SessionId = semester.SessionId,
                Title = semester.SemesterTitle,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                LateRegistrationEndtDate = semester.LateRegistrationEndDate,
                LateRegistrationStartDate = semester.LateRegistrationStartDate,
                IsCurrent = semester.IsCurrent,
                ApplyLate = semester.ApplyLate,
                
            };
            return _semester;
        }
    }
}
