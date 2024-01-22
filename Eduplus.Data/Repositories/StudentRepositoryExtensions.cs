using Eduplos.Domain.CoreModule;
using Eduplos.DTO.CoreModule;
using KS.Core;
using System.Collections.Generic;
using System.Linq;

namespace Eduplos.Data.Repositories
{
    public static class StudentRepositoryExtensions
    {
        public static IEnumerable<StudentDTO> GetEveryStudent(this IRepository<Student> rep,string deptcode)
        {
            var student = rep.GetFiltered(a => a.DepartmentCode == deptcode).Select(a=>new StudentDTO {
            FullName=a.Email});
            return student;

        }

       
    }
}
