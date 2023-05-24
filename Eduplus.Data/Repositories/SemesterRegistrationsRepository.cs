﻿using Eduplus.Domain;
using Eduplus.Domain.AcademicModule;
using Eduplus.DTO.AcademicModule;
using KS.Data.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Eduplus.Data.Repositories
{
    public class SemesterRegistrationsRepository : Repository<SemesterRegistrations>, ISemesterRegistrationsRepository
    {
        IQueryable<SemesterRegistrations> qry;
        public SemesterRegistrationsRepository(DbSet<SemesterRegistrations> dbSet) : base(dbSet)
        {
            qry = _dbSet;
        }

        public List<ProgTypeSemesterRegistrationsDTO> TotalSemesterRegistrationsByProgType(int semesterId)
        {
            return (from q in qry
                        where q.SemesterId == semesterId
                        group q by q.Student.ProgrammeType into nr
                        select new ProgTypeSemesterRegistrationsDTO
                        {
                            ProgramTpe = nr.Key,
                            Total = nr.Count()
                        }).ToList();
            
        }
    }
}