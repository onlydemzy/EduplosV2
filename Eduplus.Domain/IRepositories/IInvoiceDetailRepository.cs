using Eduplos.Domain.BurseryModule;
using Eduplos.DTO.BursaryModule;
using KS.Core;
using System;
using System.Collections.Generic;

namespace Eduplos.Domain
{
    public interface IInvoiceDetailRepository:IRepository<InvoiceDetails>
    {
        List<FeesCollectionDetailsDTO> FeesCollectionSummary(DateTime fromf, DateTime to, string progType);
        List<AccountsDTO> RevenueAccountsSummaryCollectionsBySession(string sessionPaid);
    }
}
