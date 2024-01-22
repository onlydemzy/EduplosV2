using Eduplos.DTO.CoreModule;
using KS.Domain.HRModule;
using System.Collections.Generic;

namespace Eduplos.Services.Contracts
{
    public interface IStaffService
    {
        void CreateStaff(Staff staff, string inputtedBy);
        List<StaffDTO> FetchStaff(string departmentCode);
    }
}