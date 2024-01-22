using Eduplos.Domain.AcademicModule;
using Eduplos.DTO.AcademicModule;
using KS.Core;
using System.Collections.Generic;

namespace Eduplos.Domain
{
    public interface ISemesterRegistrationsRepository:IRepository<SemesterRegistrations>
    {
        List<ProgTypeSemesterRegistrationsDTO> TotalSemesterRegistrationsByProgType(int semesterId);
    }
}
