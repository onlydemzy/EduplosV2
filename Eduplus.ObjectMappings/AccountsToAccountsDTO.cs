using KS.Domain.AccountsModule;
using KS.DTO.AccountsModule;

namespace KS.ObjectMappings
{
    public static class AccountsMappings
    {
        public static AccountsDTO AccountsToAccountsDTO(Accounts accounts)
        {
            return new AccountsDTO
            {
                AccountCode = accounts.AccountCode,
                Active = accounts.Active,
                Description = accounts.Description,
                //GroupId = accounts.GroupId,
                //Group = accounts.AccountGroup.Title,
                Title = accounts.Title,
                OpeningBalance = accounts.OpeningBalance,
                CurrentBalance = accounts.CurrentBalance
            };
        } 

    }
}
