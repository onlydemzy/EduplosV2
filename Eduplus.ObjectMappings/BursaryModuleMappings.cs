using Eduplus.Domain.BurseryModule;
using Eduplus.DTO.BursaryModule;
using System.Collections.Generic;
using System.Linq;

namespace Eduplus.ObjectMappings
{
    public static class BursaryModuleMappings
    {
        public static FeeScheduleDTO FeeScheduleToFeeScheduleDTOSummary(FeeSchedule schedule)
        {
            FeeScheduleDTO dto = new FeeScheduleDTO();
            dto.Faculty = schedule.Faculty.Title;
            dto.FacultyCode = schedule.FacultyCode;
            dto.Session = schedule.Session.Title;
            dto.ProgrammeType = schedule.ProgrammeType;
            dto.ScheduleId = schedule.ScheduleId;
            dto.SessionId = schedule.SessionId;
            dto.Total = schedule.Details.Sum(a=>a.Amount);
            dto.ScheduleId = schedule.ScheduleId;
            dto.Status = schedule.Status;
            return dto;
        }

        public static FeeScheduleDTO FeeScheduleToScheduleDTO(FeeSchedule schedule)
        {
            FeeScheduleDTO dto = new FeeScheduleDTO();
            List<FeeScheduleDetailsDTO> detailsDTO = new List<FeeScheduleDetailsDTO>();
            
            dto.Faculty = schedule.Faculty.Title;
            dto.ProgrammeType = schedule.ProgrammeType;
            dto.SessionId = schedule.SessionId;
            dto.Session = schedule.Session.Title;
            dto.Total = schedule.Total;
            dto.FacultyCode = schedule.FacultyCode;
            dto.Status = schedule.Status;
            dto.ScheduleId = schedule.ScheduleId;
            foreach (var s in schedule.Details)
            {
                var fs = new FeeScheduleDetailsDTO
                {
                    
                    AccountCode = s.AccountCode,
                    Amount = s.Amount,
                    AppliesTo=s.AppliesTo,
                    Type=s.Type,
                    ScheduleDetailId=s.ScheduleDetailId,
                    Account=s.Accounts.Title,
                    FacultyCode=dto.FacultyCode,
                    ProgrammeType=dto.ProgrammeType,
                    SessionId=dto.SessionId,
                    ScheduleId=dto.ScheduleId
                    
                };
                detailsDTO.Add(fs);
            }

            dto.Details = detailsDTO;
            dto.Total = dto.Details.Sum(a => a.Amount);
            return dto;
        }

        public static PaymentInvoiceDTO PaymentInvoiceToInvoiceDTO(PaymentInvoice invoice,string email,string phone,byte[] foto)
        {
            PaymentInvoiceDTO dto = new PaymentInvoiceDTO();
            List<InvoiceDetailsDTO> detals = new List<InvoiceDetailsDTO>();
            dto.Amount = invoice.Amount;
            dto.ApprovalChannel = invoice.ApprovalChannel;
            dto.CompletedDate = invoice.CompletedDate;
            dto.Department = invoice.Department;
            dto.GeneratedBy = invoice.GeneratedBy;
            dto.GeneratedDate = invoice.GeneratedDate;
            dto.Installment = invoice.Installment;
            dto.Name = invoice.Name;
            dto.Particulars = invoice.Particulars;
            dto.PaymentType = invoice.PaymentType;
            dto.PayOptionId = invoice.PayOptionId;
            dto.Programme = invoice.Programme;
            dto.ProgrammeType = invoice.ProgrammeType;
            dto.Regno = invoice.Regno;
            dto.ServiceCharge = invoice.ServiceCharge;
            dto.Session = invoice.Session;
            dto.LevelToPay = invoice.LevelToPay;
            dto.Status = invoice.Status;
            dto.StudentId = invoice.StudentId;
            dto.TransactionId = invoice.TransactionId;
            dto.TransRef = invoice.TransRef;
            dto.Semeseter = invoice.Semester;
            dto.Email = email;
            dto.Phone = phone;
            dto.Photo = foto;
            if(invoice.Details.Count()>0)
            {
                foreach(var d in invoice.Details.ToList())
                {
                    detals.Add(new InvoiceDetailsDTO
                    {
                        Item = d.Item,
                        ItemCode = d.ItemCode,
                        Amount = d.Amount,
                        InvoiceNo = d.InvoiceNo
                    });
                }
            }
            dto.Details = detals;
            return dto;

        }
    }
}
