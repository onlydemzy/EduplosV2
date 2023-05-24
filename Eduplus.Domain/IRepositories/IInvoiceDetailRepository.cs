using Eduplus.Domain.BurseryModule;
using Eduplus.DTO.BursaryModule;
using KS.Core;
using System;
using System.Collections.Generic;

namespace Eduplus.Domain
{
    public interface IInvoiceDetailRepository:IRepository<InvoiceDetails>
    {
        List<FeesCollectionDetailsDTO> FeesCollectionSummary(DateTime fromf, DateTime to, string progType);
        List<AccountsDTO> RevenueAccountsSummaryCollectionsBySession(string sessionPaid);
    }
}
