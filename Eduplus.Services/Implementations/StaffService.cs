using Eduplos.DTO.CoreModule;
using Eduplos.ObjectMappings;
using Eduplos.Services.Contracts;
using Eduplos.Services.UtilityServices;
using KS.Core;
using KS.Domain.HRModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eduplos.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }   
        public List<StaffDTO> FetchStaff(string departmentCode)
        {
            
            var staff = _unitOfWork.StaffRepository.GetFiltered(s=>s.DepartmentCode==departmentCode).ToList();
            List<StaffDTO> dto = new List<StaffDTO>();
            if(staff!=null)
            {
                foreach(var s in staff)
                {
                    dto.Add(CoreModuleMappings.StaffToStaffDTO(s));
                }
            }
            return dto.OrderBy(s=>s.Name).ToList();
        }

        public void CreateStaff(Staff staff,string inputtedBy)
        {

            staff.Surname = StandardGeneralOps.ToTitleCase(staff.Surname);
            staff.Firstname = StandardGeneralOps.ToTitleCase(staff.Firstname);
            staff.MIddlename = StandardGeneralOps.ToTitleCase(staff.MIddlename);
            staff.ResidentialAddress = StandardGeneralOps.ToTitleCase(staff.ResidentialAddress);
            staff.Email = staff.Email;
            staff.NextKin = StandardGeneralOps.ToTitleCase(staff.NextKin);
            staff.kinAddress = StandardGeneralOps.ToTitleCase(staff.kinAddress);
            staff.KinMail = staff.KinMail;
            staff.Referee = StandardGeneralOps.ToTitleCase(staff.Referee);
            staff.RefereeAddress = StandardGeneralOps.ToTitleCase(staff.RefereeAddress);
            staff.RefereeMail = staff.RefereeMail;
            staff.Status = "Active";
            staff.PersonId = StandardGeneralOps.GeneratePersonId(0);
           

            _unitOfWork.StaffRepository.Add(staff);
            _unitOfWork.Commit(inputtedBy);
        }

        #region  PRIVATE HELPERS
       
       
        #endregion
    }
}
