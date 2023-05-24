using Eduplus.Domain.CoreModule;
using Eduplus.DTO.CoreModule;
using KS.Core;
using System.Collections.Generic;
using System.Linq;

namespace Eduplus.Data.Repositories
{
    public static class CoreRepositoryExtensions
    {
        public static List<DepartmentDTO> DepartmentList(this IRepository<Department> rep)
        {
            var depts=rep.GetAll()
                .Select(a => new DepartmentDTO
                {
                    DepartmentCode = a.DepartmentCode,
                    Title = a.Title,
                    Location = a.Location,
                    FacultyCode = a.FacultyCode,
                    IsAcademic = a.IsAcademic,
                    Faculty = a.Faculty.Title
                })
                .OrderBy(a => a.Title);
            return depts.ToList();
        }
    }
}
