using Eduplus.Domain.BurseryModule;

namespace KS.Domain.AccountsModule
{
    public class Accounts
    {
        public string AccountCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double OpeningBalance { get; set; }
        public double CurrentBalance { get; set; }
        public bool Active { get; set; }
        public string AccountType { get; set; }
        public bool IsCollectionAccount { get; set; }
        public string CollectionType { get; set; }//All,Tuition,sundry
        //public virtual InvoiceDetails InvoiceDetails { get; set; }
        //public int GroupId { get; set; }
        //public virtual AccountsGroup AccountGroup { get; set; }
    }
}
