using KS.Common;

namespace KS.Domain.AccountsModule
{
    public class AccountsGroup:EntityBase
    {
        public int AccountsGroupId { get; set; }
        public string Title { get; set; }
    }
}
