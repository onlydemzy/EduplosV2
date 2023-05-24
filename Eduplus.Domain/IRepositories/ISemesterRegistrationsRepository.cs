using Eduplus.Domain.AcademicModule;
using Eduplus.DTO.AcademicModule;
using KS.Core;
using System.Collections.Generic;

namespace Eduplus.Domain
{
    public interface ISemesterRegistrationsRepository:IRepository<SemesterRegistrations>
    {
        List<ProgTypeSemesterRegistrationsDTO> TotalSemesterRegistrationsByProgType(int semesterId);
    }
}
