using Eduplus.Domain;
using Eduplus.Domain.BurseryModule;
using Eduplus.DTO.BursaryModule;
using KS.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Data.Repositories
{
    public class InvoiceDetailRepository : Repository<InvoiceDetails>, IInvoiceDetailRepository
    {
        IQueryable<InvoiceDetails> qry;
        public InvoiceDetailRepository(DbSet<InvoiceDetails> dbSet) : base(dbSet)
        {
            qry = _dbSet;
        }
        public List<FeesCollectionDetailsDTO> FeesCollectionSummary(DateTime fromf, DateTime to, string progType)
        {


            return (from q in qry
                    where (DbFunctions.TruncateTime(q.PaymentInvoice.CompletedDate) >= fromf && DbFunctions.TruncateTime(q.PaymentInvoice.CompletedDate) <= to
                    && q.PaymentInvoice.Status == "PAID" && q.PaymentInvoice.ProgrammeType == progType)
                    group q by new { q.ItemCode, q.Item } into nq
                    select new FeesCollectionDetailsDTO
                    {
                        Name = nq.Key.ItemCode,
                        Particulars = nq.Key.Item,
                        Amount = nq.Sum(a => a.Amount)
                    }).ToList();

        }

        public List<AccountsDTO> RevenueAccountsSummaryCollectionsBySession(string sessionPaid)
        {
            return (from q in qry
                      where (q.PaymentInvoice.Session == sessionPaid && q.PaymentInvoice.Status == "PAID")
                      group q by new { q.ItemCode, q.Item, q.PaymentInvoice.Session } into nq
                      orderby nq.Key.Item
                      select new AccountsDTO
                    {
                        AccountCode = nq.Key.ItemCode,
                        Title = nq.Key.Item,
                         
                        Amount = nq.Sum(a => a.Amount)
                    }).ToList();
        }
    }
}